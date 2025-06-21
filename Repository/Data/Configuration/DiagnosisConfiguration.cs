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
    public class DiagnosisConfiguration : IEntityTypeConfiguration<DiagnosisResult>
    {
        public void Configure(EntityTypeBuilder<DiagnosisResult> builder)
        {
            builder.HasOne(P => P.PlanetImage).WithOne(D=>D.DiagnosisResult).HasForeignKey<DiagnosisResult>(F => F.PlanetImageId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(P=>P.DiagnosisName).IsRequired();
        }
    }
}
