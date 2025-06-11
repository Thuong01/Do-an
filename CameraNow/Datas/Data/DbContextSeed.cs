using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Datas.SeedData;
using Models.Enums;
using Models.Models;
namespace Datas.Data
{
    public class DbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider service)
        {
            using (var context = new CameraNowContext(service.GetRequiredService<DbContextOptions<CameraNowContext>>()))
            {
                await RoleDataSeedContributor.RoleSeederAsync(context, service);
                await UserDataSeedContributor.UserSeederAsync(context, service);
                PermissionContributor.Define(context);

                // Add Seed data cart
                if (context.Users.Any() && !context.Carts.Any())
                {
                    var users = context.Users.ToList();
                    foreach (var user in users)
                    {
                        context.Carts.Add(new Cart { User_Id = user.Id });
                    }
                    await context.SaveChangesAsync();
                }
                // end

                //if (!context.Brands.Any())
                //{
                //    var brandsData = await File.ReadAllTextAsync("../Datas/SeedData/brands.json");
                //    var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData);
                //    await context.Brands.AddRangeAsync(brands);
                //}
                //if (!context.BlogCategories.Any())
                //{
                //    var blogcategoriesData = await File.ReadAllTextAsync("../Datas/SeedData/blog_categories.json");
                //    var blogcategories = JsonSerializer.Deserialize<List<BlogCategory>>(blogcategoriesData);
                //    await context.BlogCategories.AddRangeAsync(blogcategories);
                //}
                //if (!context.Blogs.Any())
                //{
                //    var blogData = await File.ReadAllTextAsync("../Datas/SeedData/Blog.json");
                //    var blogs = JsonSerializer.Deserialize<List<Blog>>(blogData);
                //    await context.Blogs.AddRangeAsync(blogs);
                //}

                if (context.ChangeTracker.HasChanges())
                {
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}