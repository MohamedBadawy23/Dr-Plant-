using Core.Enteties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data.Configuration
{
    public class ReportProblemConfiguration : IEntityTypeConfiguration<ReportProblem>
    {
        public void Configure(EntityTypeBuilder<ReportProblem> builder)
        {
             builder.HasOne(P=>P.AppUser).WithMany().HasForeignKey(F=>F.AppuserId);
            builder.Property(P=>P.Description).IsRequired();    
        }
    }
}
