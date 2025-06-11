using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Datas.Data;
using Models.Models;
using System.Text.Json;

namespace Datas.SeedData
{
    public static class UserDataSeedContributor
    {
        public static async Task UserSeederAsync(CameraNowContext context, IServiceProvider service)
        {
            try
            {
                if (!context.Users.Any())
                {
                    var userManager = service.GetRequiredService<UserManager<AppUser>>();

                    var userAdmin = new List<AppUser>
                        {
                            new AppUser{ FullName = "Phạm Huyền Thương", UserName = "Admin", Birthday = DateTime.SpecifyKind(new DateTime(2003, 10, 01), DateTimeKind.Utc), Address = "Quản xương - Thanh Hóa", Email = "phamhuyenthuong0110@gmail.com", PhoneNumber = "0334237519", EmailConfirmed = true },
                            new AppUser{ FullName = "Nguyễn Văn A", UserName = "Admin1", Birthday = DateTime.SpecifyKind(new DateTime(2003, 12, 12), DateTimeKind.Utc), Address = "Định Tân - Yên Định - Thanh Hóa", Email = "hacnguyetcongtu12@gmail.com", PhoneNumber = "0334237519", EmailConfirmed = true }
                        };

                    foreach (var item in userAdmin)
                    {
                        var adminResult = await userManager.CreateAsync(item, "@Abc123456");

                        if (adminResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(item, "Admin");
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
