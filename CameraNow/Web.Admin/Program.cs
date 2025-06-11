using Datas.Data;
using Serilog;
using Serilog.Events;
using AspNetCoreHero.ToastNotification.Extensions;
using System.Net;
using Microsoft.Extensions.Options;
using Configurations.Middlewares;

namespace Web.Admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt", restrictedToMinimumLevel: LogEventLevel.Error))
                .WriteTo.Async(c => c.Console());

            Log.Logger = loggerConfiguration.CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //builder.Host.AddAppSettingsSecretsJson()
                //    .UseAutofac()
                //    .UseSerilog();

                builder.Services.AddCors(p => p.AddPolicy($@"{Commons.Commons.CommonConstant.AppName}.WebApi.CorsPolicy",
                    build =>
                    {
                        build.WithOrigins("http://localhost:3005", "https://localhost:7110")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                    })
                );

                var connectionString = builder.Configuration.GetConnectionString("HuyenThuongStore");

                builder.Services.AddApplicationService(builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCors($@"{Commons.Commons.CommonConstant.AppName}.WebApi.CorsPolicy");
                app.UseStatusCodePages(appErr =>
                {
                    appErr.Run(async context =>
                    {
                        var response = context.Response;
                        var code = response.StatusCode;

                        var content = @$"<html>
                                        <header>
                                            <meta charset='UTF-8 />
                                            <title>{code}</title> 
                                        </header>
                                        <body> 
                                            <h1> Cos loi xay ra roi {code} - {(HttpStatusCode)code} </h1>

                                            <main>
                                                <div class=""container"">
                                                  <section class=""section error-404 min-vh-100 d-flex flex-column align-items-center justify-content-center"">
                                                    <h1> Cos loi xay ra roi {code} - {(HttpStatusCode)code} </h1>
                                                    <h2>The page you are looking for doesn't exist.</h2>
                                                    <a class=""btn"" href=""index.html"">Back to home</a>
                                                    <img src=""assets/img/not-found.svg"" class=""img-fluid py-5"" alt=""Page Not Found"">
                                                  </section>

                                                </div>
                                              </main><!-- End #main -->
                                        </body> 
                                    </html>";

                        await response.WriteAsync(content);
                    });
                });
                                

                app.UseAuthentication();

                app.UseRouting();

                // Localization
                //var cultures = new List<CultureInfo> {
                //    new CultureInfo("en"),
                //    new CultureInfo("vi")
                //};
                //app.UseRequestLocalization(options => {
                //    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                //    options.SupportedCultures = cultures;
                //    options.SupportedUICultures = cultures;
                //});
                app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

                app.UseAuthorization();

                app.UseNotyf();                

                app.UseMiddleware<ExceptionHandlingMiddleware>();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                //app.UseSqlTableDependency<SubscribeProductTableDependency>(connectionString);

                //app.MapRazorPages();
                Log.Information("Starting Web.Admin.");

                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                if (app.Environment.IsDevelopment())
                {
                    try
                    {
                        await DbContextSeed.SeedAsync(services);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occured during migration: {ex.Message}");
                    }
                }

                app.Run();
            }
            catch (Exception ex)
            {
                if (ex is HostAbortedException)
                {
                    throw;
                }

                Log.Fatal(ex, "Web.Admin terminated unexpectedly!");
            }
            finally
            {
                Log.Information("Closing Web.Admin.");
                Log.Information("Closed Web.Admin.");
                Log.CloseAndFlush();
            }
        }
    }
}
