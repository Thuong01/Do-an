using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Datas.Data;
using Models.Models;
using System.Globalization;
using System.Reflection;
using Web.Admin.Resources;
using AutoMapper;
using Datas.Infrastructures.Cores;
using Datas.Infrastructures.Interfaces;
using DinkToPdf.Contracts;
using DinkToPdf;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Services.Repositories;
using Services.Services;
using Services.ObjectMapping;
using BookStore.Bussiness.Services;

namespace Web.Admin
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddSerilog();
            services.AddLogging();

            var mvcbuilder = services.AddControllersWithViews();
#if DEBUG
            mvcbuilder.AddRazorRuntimeCompilation();
#endif

            var cultures = new[]
                    {
                        new CultureInfo("en"),
                        new CultureInfo("vi"),
                    };
            // Add services to the container.
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(ApplicationResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("ApplicationResource", assemblyName.Name);
                    };
                });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo> {
                    new CultureInfo("en"),
                    new CultureInfo("vi")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            //services.AddSwaggerService();

            // Thêm SignalR
            //services.AddSignalR();
            //services.AddScoped<NotificationHub>();
            //services.AddScoped<SubscribeProductTableDependency>();

            #region Sử dụng package
            //services.AddControllersWithViews()
            //    .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>
            //    (ops =>
            //        {
            //            // When using all the culture providers, the localization process will
            //            // check all available culture providers in order to detect the request culture.
            //            // If the request culture is found it will stop checking and do localization accordingly.
            //            // If the request culture is not found it will check the next provider by order.
            //            // If no culture is detected the default culture will be used.

            //            // Checking order for request culture:
            //            // 1) RouteSegmentCultureProvider
            //            //      e.g. http://localhost:1234/tr
            //            // 2) QueryStringCultureProvider
            //            //      e.g. http://localhost:1234/?culture=tr
            //            // 3) CookieCultureProvider
            //            //      Determines the culture information for a request via the value of a cookie.
            //            // 4) AcceptedLanguageHeaderRequestCultureProvider
            //            //      Determines the culture information for a request via the value of the Accept-Language header.
            //            //      See the browsers language settings

            //            // Uncomment and set to true to use only route culture provider
            //            ops.UseAllCultureProviders = false;
            //            ops.ResourcesPath = "LocalizationResources";
            //            ops.RequestLocalizationOptions = o =>
            //            {
            //                o.SupportedCultures = cultures;
            //                o.SupportedUICultures = cultures;
            //                o.DefaultRequestCulture = new RequestCulture("vi");
            //            };
            //        });
            #endregion

            //services.AddRazorPages();
            services.AddEndpointsApiExplorer();

            #region Identity 
            services.AddDbContext<CameraNowContext>(option =>
            {
                option.UseNpgsql(config.GetConnectionString("ShopthoiTrangNu"));
            }, ServiceLifetime.Singleton);

            // Đăng ký các dịch vụ của Identity
            services.AddIdentity<AppUser, IdentityRole>(o =>
            {
                o.Stores.MaxLengthForKeys = 128;
                o.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<CameraNowContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
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

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
                // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
                options.ValidationInterval = TimeSpan.FromSeconds(5);
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LogoutPath = "/Login/Index";
                    // options.LogoutPath = "/Login/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    /// SlidingExpiration: thời gian hết hạn của cookie sẽ được cập nhật 
                    /// mỗi khi người dùng tương tác với ứng dụng, 
                    /// đảm bảo rằng cookie chỉ hết hạn nếu người dùng không hoạt động 
                    /// trong khoảng thời gian được chỉ định
                    options.SlidingExpiration = true;
                });
                //.AddJwtBearer(options =>
                //{
                //    options.SaveToken = true;
                //    options.RequireHttpsMetadata = false;
                //    options.TokenValidationParameters = new TokenValidationParameters()
                //    {
                //        ValidateIssuerSigningKey = true,
                //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"])),
                //        ValidateIssuer = true,
                //        ValidIssuer = config["JWT:Issuer"],
                //        ValidateAudience = false,
                //        ValidAudience = config["JWT:Audience"]
                //    };
                //});            

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Index";
                options.LogoutPath = "/Login/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            #endregion

            services.AddHttpContextAccessor();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MyShopAutoMapperProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            //services.AddAutoMapper(typeof(ShopThoiTrangNuAutoMapperProfile));

            services.AddEndpointsApiExplorer();

            services.AddScoped<IDbFactory, DbFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppUserService, AppUserService>();

            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICouponService, CouponService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<ICartItemRepository, CartItemrepository>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IStatsService, StatsService>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddSingleton<LocalizationService>();

            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            });
            services.AddHttpClient();

            return services;
        }
    }
}
