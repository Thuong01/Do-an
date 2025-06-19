using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog.Events;
using Serilog;
using Datas.Data;
using Models.Models;
using System.Text;
using System.Reflection;
using Configurations.Middlewares;
using Datas.ViewModels.Payment.Momo;
using Datas.ViewModels.Payment.Vnpay;
using Services.Services;
using AutoMapper;
using Services.ObjectMapping;
using BookStore.Bussiness.Services;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using WebApi.Configurations;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Services.Repositories;

namespace WebApi
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

                builder.Services.AddSerilog();
                builder.Services.AddLogging();

                builder.Services.AddMemoryCache();

                builder.Services.AddCors(p => p.AddPolicy($@"{Commons.Commons.CommonConstant.AppName}.WebApi.CorsPolicy",
                    build =>
                    {
                        build.WithOrigins("http://localhost:3005", "https://localhost:7110")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                    })
                );

                // Add services to the container.
                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();

                #region Swagger
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // Fix for CS0121: Specify the namespace explicitly to resolve ambiguity  
                builder.Services.AddSwaggerService(xmlFilename);
                #endregion

                builder.Services.AddDbContext<CameraNowContext>(option =>
                {
                    option.UseNpgsql(builder.Configuration.GetConnectionString("HuyenThuongStore"));
                });

                // Đăng ký các dịch vụ của Identity
                builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
                {
                    o.Stores.MaxLengthForKeys = 128;
                    o.SignIn.RequireConfirmedAccount = false;
                })
                    .AddEntityFrameworkStores<CameraNowContext>()
                    .AddDefaultTokenProviders();

                builder.Services.Configure<IdentityOptions>(options =>
                {
                    // Thiết lập về Password
                    options.Password.RequireDigit = true; // bắt phải có số
                    options.Password.RequireLowercase = true; // bắt phải có chữ thường
                    options.Password.RequireNonAlphanumeric = true; // bắt ký tự đặc biệt
                    options.Password.RequireUppercase = true; // bắt buộc chữ in
                    options.Password.RequiredLength = 6; // Số ký tự tối thiểu của password
                    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                    // Cấu hình Lockout - khóa user
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // Khóa 5 phút
                    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                    options.Lockout.AllowedForNewUsers = true;

                    // Cấu hình về User.
                    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true; // Email là duy nhất

                    // Cấu hình đăng nhập.
                    options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
                });

                builder.Services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
                    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
                    options.ValidationInterval = TimeSpan.FromSeconds(5);
                });

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
                            ValidateIssuer = true,
                            ValidIssuer = builder.Configuration["JWT:Issuer"],
                            ValidateAudience = false,
                            ValidAudience = builder.Configuration["JWT:Audience"]
                        };
                    });

                builder.Services.Configure<VnpayConfig>(builder.Configuration.GetSection(VnpayConfig.ConfigName));
                builder.Services.Configure<MomoConfig>(builder.Configuration.GetSection(MomoConfig.ConfigName));

                builder.Services.AddHttpContextAccessor();

                var mapperConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MyShopAutoMapperProfile());
                });
                IMapper mapper = mapperConfig.CreateMapper();
                builder.Services.AddSingleton(mapper);
                //services.AddAutoMapper(typeof(ShopThoiTrangNuAutoMapperProfile));

                builder.Services.AddScoped<IDbFactory, DbFactory>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

                builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
                builder.Services.AddScoped<ICategoryService, CategoryService>();

                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IProductService, ProductService>();

                builder.Services.AddScoped<IImageRepository, ImageRepository>();
                builder.Services.AddScoped<IImageService, ImageService>();

                builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
                builder.Services.AddScoped<IAppUserService, AppUserService>();

                builder.Services.AddScoped<ICouponRepository, CouponRepository>();
                builder.Services.AddScoped<ICouponService, CouponService>();

                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderService, OrderService>();

                builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

                builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
                builder.Services.AddScoped<IFeedbackService, FeedbackService>();

                builder.Services.AddScoped<ICartRepository, CartRepository>();
                builder.Services.AddScoped<ICartService, CartService>();

                builder.Services.AddScoped<ICartItemRepository, CartItemrepository>();

                builder.Services.AddScoped<IAuthService, AuthService>();

                builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
                builder.Services.AddScoped<IPermissionService, PermissionService>();

                builder.Services.AddScoped<IPaymentService, PaymentService>();

                builder.Services.AddScoped<IStatsService, StatsService>();

                builder.Services.AddScoped<AprioriService>();

                var app = builder.Build();

                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseCors($@"{Commons.Commons.CommonConstant.AppName}.WebApi.CorsPolicy");
                app.UseSwaggerConfigre();
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                Log.Information($@"Starting {Commons.Commons.CommonConstant.AppName}.WebApi");
                app.Run();
            }
            catch (Exception ex)
            {
                if (ex is HostAbortedException) { throw; }
                Log.Fatal(ex, $@"{Commons.Commons.CommonConstant.AppName}.WebApi terminated unexpectedly!"); 
            }
            finally
            {
                Log.Information($@"Closing {Commons.Commons.CommonConstant.AppName}.WebApi.");
                Log.Information($@"Closed {Commons.Commons.CommonConstant.AppName}.WebApi.");
                Log.CloseAndFlush();
            }
        }
    }
}
