using ASP.Authentication.DTOs;
using ASP.Authentication.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core;
using PlanetDiseaaseDR.Controllers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ASP.Authentication.Controllers;

public class AccountController : BaseController
{
    private readonly UserManager<AppUser> _userManager;

    private readonly IMapper _mapper;
    private readonly JwtHandler _jwtHandler;
    private readonly static List<string> _allowedImageFormats = [".jpg", ".jpeg", ".png"];
    public AccountController(UserManager<AppUser> userManager,  IMapper mapper, JwtHandler jwtHandler)
    {
        _userManager = userManager;
        
        _mapper = mapper;
        _jwtHandler = jwtHandler;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterationResponseDto>> RegisterUser(
        [FromBody] UserForRegisterationDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new AppUser()
        {
            Name = model.Name,
            Email = model.Email,
            UserName = model.Email!.Split('@')[0],
            Gender = model.Gender
        };

        var result = await _userManager.CreateAsync(user, model.Password!);
       
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new RegisterationResponseDto(false, errors.ToList()));
        }

        await _userManager.AddToRoleAsync(user, "Visitor");

        return Ok(new RegisterationResponseDto(true));
    }

    
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponseDto>> AuthenticateUser(
         UserForAuthenticationDto model)
    {

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password!))
            return Unauthorized(new AuthenticationResponseDto(false, "Invalid Authentication"));
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHandler.CreateToken(user, roles);

       
        return Ok(new AuthenticationResponseDto(true, token));
      
    }
    
    

   

    [HttpPost("logout")]
   // Ensures only authenticated users can call this
    public ActionResult Logout()
    {
        Response.Cookies.Delete("jwt"); // If JWT is stored in cookies
        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("Profile")]
    public async Task<IActionResult> GetUserProfileAsync()
    {
        var userId = User.FindFirstValue("id");

        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        var response = new ProfileDto
        {
            Email = user?.Email!,
            Name = user?.Name!,
            UserName = user?.UserName!,
            ImageUrl = !string.IsNullOrEmpty(user?.ImageName) ?
                $"{Request.Scheme}://{Request.Host}/Uploads/{user?.ImageName}" : null!
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPut("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest("Image is required");

        var fileExtension = Path.GetExtension(image.FileName);

        if (!_allowedImageFormats.Contains(fileExtension))
            return BadRequest("Invalid image format");

        var userId = User.FindFirstValue("id");
        if (userId is null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var uniqueImage = Guid.NewGuid() + "_" + image.FileName;

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueImage);

        using var fileStream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(fileStream);

        user.ImageName = uniqueImage;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new { ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{user.ImageName}" });

        return BadRequest("Could not upload image");

    }
}
