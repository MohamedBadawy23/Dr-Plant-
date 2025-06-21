using ASP.Authentication;
using ASP.Authentication.Data;
using Core.Enteties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanetDiseaaseDR.Dto;
using Repository.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;


namespace PlanetDiseaaseDR.Controllers
{
  
    public class PlantDiagnosisController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public PlantDiagnosisController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        #region Upload

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadPlantImage([FromForm] UploadPlantImageDTO model, [FromServices] JwtHandler jwtHandler)
        {
            // استخراج معرف المستخدم من التوكن
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Unauthorized("User ID not found in token.");
            }

            string userId = userIdClaim.Value;
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            // التحقق من الصورة
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("Please upload an image.");
            }

            // إنشاء مسار التخزين
            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileExtension = Path.GetExtension(model.File.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // حفظ الصورة
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the file: {ex.Message}");
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/uploads/{uniqueFileName}";

            // حفظ بيانات الصورة في قاعدة البيانات
            var plantImage = new PlantImage
            {
                UserId = user.Id,
                PlanetImageUrl = imageUrl
            };

            _dbContext.PlanetImages.Add(plantImage);
            await _dbContext.SaveChangesAsync();

            //  بعد حفظ الصورة، توليد توكن جديد يحتوي على `plantImageId`
            var roles = new List<string>(); // ضع الأدوار هنا إذا كنت تستخدم الأدوار
            var token = jwtHandler.CreateToken(user, roles, plantImage.Id,null); // تمرير `plantImageId`

            return Ok(new { message = "Image uploaded successfully", imageUrl, token });
        }

        #endregion
        ///

        #region Dignosis


        [HttpPost("diagnose")]
        [Authorize]
        public async Task<ActionResult> DiagnosePlant()
        {
            Console.WriteLine(" DiagnosePlant endpoint called!");

            // استخراج `plantImageId` من التوكن
            var plantImageIdClaim = User.Claims.FirstOrDefault(c => c.Type == "plantImageId");
            if (plantImageIdClaim == null)
            {
                return BadRequest("Plant image ID not found in token.");
            }

            int plantImageId = int.Parse(plantImageIdClaim.Value);
            Console.WriteLine($" Extracted plantImageId: {plantImageId}");

            // التحقق من وجود الصورة
            var plantImage = await _dbContext.PlanetImages.FindAsync(plantImageId);
            if (plantImage == null)
            {
                return NotFound("Plant image not found.");
            }

            //  تحقق مما إذا كان هناك تشخيص موجود مسبقًا
            var existingDiagnosis = await _dbContext.DiagnosisResults
                .FirstOrDefaultAsync(d => d.PlanetImageId == plantImageId);

            if (existingDiagnosis != null)
            {
                return BadRequest("Diagnosis already exists for this image.");
            }

            // تحليل الصورة
            string diagnosisResult = await GetDiagnosisResult(plantImage.PlanetImageUrl);
            Console.WriteLine($" Diagnosis result: {diagnosisResult}");

            // حفظ النتيجة
            var diagnosis = new DiagnosisResult
            {
                PlanetImageId = plantImageId,
                DiagnosisName = diagnosisResult,
            };

            _dbContext.DiagnosisResults.Add(diagnosis);

            try
            {
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("Diagnosis saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error saving diagnosis: {ex.Message}");
                return StatusCode(500, "Error saving diagnosis.");
            }

            return Ok(new { message = "Diagnosis completed", result = diagnosisResult });
        }

        #endregion


        #region dignosis

        private async Task<string> GetDiagnosisResult(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // تحميل الصورة كـ ByteArray
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    var form = new MultipartFormDataContent();
                    var imageContent = new ByteArrayContent(imageBytes);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    form.Add(imageContent, "file", "image.jpg");

                    // إرسال الصورة إلى API
                    var response = await client.PostAsync("https://plant-health-tracker-plantdisease.replit.app/predict", form);

                    //  فحص حالة الاستجابة
                    if (!response.IsSuccessStatusCode)
                    {
                        if ((int)response.StatusCode >= 500) //  سيرفر إيرور (5xx)
                        {
                            Console.WriteLine($"[Server Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                            return "Server error, please try again later.";
                        }
                        else // 🟡 إيرور آخر (مثل 400 Bad Request)
                        {
                            Console.WriteLine($"[Client Error] {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                            return "Invalid request, please check the image format.";
                        }
                    }

                    // قراءة الاستجابة JSON
                    var resultJson = await response.Content.ReadAsStringAsync();

                    //  تحليل JSON للحصول على disease_name
                    using var doc = JsonDocument.Parse(resultJson);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("disease_name", out var diseaseName))
                    {
                        return diseaseName.GetString() ?? "Unknown";
                    }

                    return "Unknown";
                }
                catch (HttpRequestException httpEx) //  هندلة مشاكل الاتصال
                {
                    Console.WriteLine($"[Network Error] {httpEx.Message}");
                    return "Network error, please check your connection.";
                }
                catch (Exception ex) //  هندلة أي خطأ غير متوقع
                {
                    Console.WriteLine($"[Unexpected Error] {ex.Message}");
                    return "An unexpected error occurred, please try again.";
                }
            }
        }





        #endregion





    }
}
