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
    public class PlanetImageConfiguration : IEntityTypeConfiguration<PlantImage>
    {
        public void Configure(EntityTypeBuilder<PlantImage> builder)
        {
            builder.HasOne(P => P.User).WithMany().HasForeignKey(P => P.UserId)
                .OnDelete(DeleteBehavior.Cascade); ;
            builder.Property(P=>P.PlanetImageUrl).IsRequired();
        }
    }
}
