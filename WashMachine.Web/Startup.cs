using AutoMapper;
using BotDetect.Web;
using WashMachine.Business.Interfaces;
using WashMachine.Business.Services;
using WashMachine.Web.AppCode;
using Libraries.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using WashMachine.Repositories.Entities;

namespace WashMachine.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Administrator/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Administrator/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                                .AddSessionStateTempDataProvider()
                                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Errors/AccessDenied";
                options.Cookie.Name = "Application.Authenticate";
            });
            services.AddDistributedMemoryCache();
            // Add Session services.
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Application.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.IsEssential = true;
            });

            //Start Registering and Initializing AutoMapper
            //services.AddAutoMapper(typeof(MappingProfile));

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //Register Entity Framework  
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Repositories.Entities.WashMachineContext>(options => options.UseSqlServer(connection));

            RegisterDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();


            app.UseSession();

            // configure your application pipeline to use Captcha middleware
            // Important! UseCaptcha(...) must be called after the UseSession() call
            app.UseCaptcha(Configuration);

            // global error handler
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    ApiLogger.LogException(context);
                    //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "MyArea",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        public static void RegisterDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IBusiness, PosService>();
            services.AddScoped<IBusiness, OrderService>();
            services.AddScoped<IBusiness, AccessService>();
            services.AddScoped<IBusiness, ShopService>();
            services.AddScoped<IBusiness, SettingService>();
            services.AddScoped<IBusiness, ReportService>();
            services.AddScoped<IBusiness, FilesService>();
            services.AddScoped<IBusiness, CouponService>();
            services.AddScoped<IBusiness, LogService>();
            services.AddScoped<IBusiness, EmailService>();
            services.AddScoped<IBusiness, MachineCommadService>();
            services.AddScoped<IBusiness, RunCommandErrorService>();
            services.AddTransient<ExportExcelService>();
        }
    }
}
