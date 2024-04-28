using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Shangrila",
                    Address = "Manila",
                    CountryId = 1,
                    Rating = 2.8M
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Red Planet",
                    Address = "Cebu",
                    CountryId = 2,
                    Rating = 3.8M
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Ace",
                    Address = "Pasig",
                    CountryId = 3,
                    Rating = 4M
                }
            );
        }
    }
}
