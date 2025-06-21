using System.ComponentModel.DataAnnotations;

namespace PlanetDiseaaseDR.Dto
{
    public class UploadPlantImageDTO
    {
        //[Required]
        //public string UserId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
