using ASP.Authentication;
using ASP.Authentication.Data;
using AutoMapper;
using Core;
using Core.Enteties;
using Core.Reository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanetDiseaaseDR.DTO;
using System.Security.Claims;

namespace PlanetDiseaaseDR.Controllers
{
     
    public class ReportProblemController :BaseController
    {
        private readonly IGenericRepository<ReportProblem> _repository;
  
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly JwtHandler _jwtHandler;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<AppUser> _userRepository;

        public ReportProblemController(IGenericRepository<ReportProblem> repository , 
            IMapper mapper, 
            ApplicationDbContext dbContext ,JwtHandler jwtHandler,UserManager<AppUser> userManager,IGenericRepository<AppUser> userRepository)
        {
            _repository = repository;
           
            _mapper = mapper;
            _dbContext = dbContext;
            _jwtHandler = jwtHandler;
            _userManager = userManager;
          _userRepository = userRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportProblemToReturnDTO>>>GetAll()
        {
            var Problems =await _repository.GetAllAsync();
             
            var Mapped = _mapper.Map<IEnumerable<ReportProblem>, IEnumerable<ReportProblemToReturnDTO>>(Problems);
            return Ok(Mapped);


        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateProblem([FromBody] CreateProblemDTO createProblemDTO)
        {
            // استخراج معرف المستخدم من التوكن
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return BadRequest("User ID not found or invalid.");
            }

            // البحث عن المستخدم
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // إنشاء المشكلة
            var problem = new ReportProblem
            {
                Description= createProblemDTO.Descripcion,  // تأكد من صحة اسم الخاصية
                AppuserId = userId.ToString()
            };

            // حفظ المشكلة في قاعدة البيانات
            await _repository.AddAsync(problem);
            await _dbContext.SaveChangesAsync();

            try
            {
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("Problem saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving problem: {ex.Message}");
                return StatusCode(500, "Error saving problem.");
            }

            // ** جلب أدوار المستخدم**
            var roles = await _userManager.GetRolesAsync(user);

            // ** إنشاء التوكن يحتوي على `problemId` فقط**
            string token = _jwtHandler.CreateToken(user, roles,null, problem.Id);

            // ** إرجاع الاستجابة مع التوكن فقط**
            return Ok(new
            {
                message = "Successful",
                token = token // التوكن يحتوي على `problemId`
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateProblem([FromBody] CreateProblemDTO createProblemDTO)
        {
            //  استخراج problemId من التوكن
            var problemIdClaim = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "problemId")?.Value;
            if (string.IsNullOrEmpty(problemIdClaim) || !int.TryParse(problemIdClaim, out int problemId))
            {
                return Unauthorized(new { message = "Problem ID not found in token or invalid." });
            }

            //  جلب المشكلة من قاعدة البيانات
            var problem = await _repository.GetByIdAsync(problemId);
            if (problem == null)
            {
                return NotFound(new { message = "Problem not found." });
            }

            //  تحديث الوصف
            problem.Description = createProblemDTO.Descripcion;
            await _repository.UpdateAsync(problem);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Problem updated successfully." });
        }


        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteProblem()
        {
            var problemIdClaim = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "problemId")?.Value;
            if (string.IsNullOrEmpty(problemIdClaim) || !int.TryParse(problemIdClaim, out int problemId))
            {
                return Unauthorized(new { message = "Problem ID not found in token or invalid." });
            }

            var problem = await _repository.GetByIdAsync(problemId);
            if (problem == null)
            {
                return NotFound(new { message = "Problem not found." });
            }

            await _repository.DeleteAsync(problem);
            return Ok(new { message = "Problem is deleted successfully." });
        }
        [Authorize]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchProblems(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query cannot be empty" });
            }

            try
            {
                var problems = await _dbContext.ReportProblems
                    .Where(p => p.Description.ToLower().Contains(query.ToLower()))
                    .Include(p => p.AppUser)  // Include the AppUser navigation property
                    .ToListAsync();

                if (!problems.Any())
                {
                    return Ok(new { 
                        message = "No reports found matching your search criteria",
                        results = new List<ReportProblemToReturnDTO>() 
                    });
                }

                var mappedProblems = _mapper.Map<IEnumerable<ReportProblemToReturnDTO>>(problems);

                return Ok(new {
                    message = "Search completed successfully",
                    count = mappedProblems.Count(),
                    results = mappedProblems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while searching reports",
                    error = ex.Message 
                });
            }
        }
    }
}
