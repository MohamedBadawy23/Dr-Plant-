using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core;

namespace ASP.Authentication.Data
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    Name = "MohamedBadawy",
                    Email = "MohamedBadawy447@Gmail.com",
                    UserName = "MohamedBadawy447",


                };
                await userManager.CreateAsync(User, "Pa$$w0rd");
            }

        }
    }
}
