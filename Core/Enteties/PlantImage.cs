using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enteties
{
   public class PlantImage:BaseEntity
    {
       // public int Id {  get; set; }
        public string PlanetImageUrl {  get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public DiagnosisResult DiagnosisResult { get; set; }
    }
}
