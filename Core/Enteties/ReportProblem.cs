using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enteties
{
    public class ReportProblem:BaseEntity
    {
       // public int Id {  get; set; }
        public string AppuserId {  get; set; }
        public AppUser AppUser { get; set; }
        public string Description { get; set; }
    }
}
