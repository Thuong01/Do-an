using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Datas.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas.SeedData
{
    public static class RoleDataSeedContributor
    {
        public static async Task RoleSeederAsync(CameraNowContext context, IServiceProvider service)
        {
            List<IdentityRole> roles = new List<IdentityRole>()
                {
                    new IdentityRole("Admin"),
                    new IdentityRole("User"),
                };

            if (!context.Roles.Any())
            {
                try
                {
                    var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

                    foreach (var item in roles)
                    {
                        await roleManager.CreateAsync(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
