using ASP.Authentication.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class AppUser :IdentityUser
    {
        public string Name { get; set; }
        public string? ImageName { get; set; }
        public Gender Gender { get; set; }
    }
}
