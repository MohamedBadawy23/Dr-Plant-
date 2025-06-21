using ASP.Authentication.Data;
using Core.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
   public class ApplicationDbContextSeed
    {
        public static async Task SeedData(ApplicationDbContext context)
        {

            if (!context.ReportProblems.Any())
            {
                // مثال UserId من Identity
                var problems = new List<ReportProblem>
        {
          // new ReportProblem {  Description = "I noticed small yellow spots on my tomato leaves, and after a few days, they turned brown and dried out completely. I found out it was early blight and had to use a copper fungicide to save my crop.", AppuserId ="ec8f6b55-b678-4dd1-85b2-87082e3631c9" },
          // new ReportProblem {   Description = "My pepper plant was growing fine, but suddenly, the leaves started curling and losing their vibrant color. It turned out to be an aphid infestation, so I sprayed an organic soap solution and managed to control it.", AppuserId = "fd5786ef-58d5-416f-97a7-b1181f55dffd" }
            //new ReportProblem {   Description = "I faced a serious problem with my wheat crop, where some stalks turned black and died completely. After researching, I discovered it was black stem rust. I used a specialized fungicide and improved air circulation between plants.", AppuserId = defaultUserId },
            //new ReportProblem {  Description = "I noticed an unusual rot in my potato crop’s roots, with a white fungus that looked like cotton. After investigating, I realized it was white mold, and I had to remove the infected plants to prevent its spread.", AppuserId =defaultUserId},
            //new ReportProblem {   Description = "My apple trees started shedding leaves excessively, which was unusual. After inspection, I found out they were infected with powdery mildew. I sprayed a milk and water mixture every two weeks and saw significant improvement.", AppuserId = defaultUserId},
            //new ReportProblem {  Description = "The leaves of my rose plant are turning yellow.", AppuserId = defaultUserId }

        };
                await context.ReportProblems.AddRangeAsync(problems);
            }



            await context.SaveChangesAsync();
        }
    }
}
