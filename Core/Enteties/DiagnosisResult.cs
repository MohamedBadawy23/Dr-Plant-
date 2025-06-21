using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enteties
{
   public class DiagnosisResult:BaseEntity
    {
        //public int Id {  get; set; }
        public string DiagnosisName {  get; set; }

        public int PlanetImageId { get; set; }
        public PlantImage PlanetImage { get; set; }
    }
}
