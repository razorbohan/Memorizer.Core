using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Memorizer.Data;
using Memorizer.Data.Models;
using Memorizer.Logic;
using Memorizer.Web.Areas.Identity.Services;
using Memorizer.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Memo = Memorizer.Data.Models.Memo;

namespace Memorizer.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddUserSecrets<Startup>()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (!env.IsDevelopment())
            {
                var config = builder.Build();

                builder.AddAzureKeyVault(
                    config["KeyVault:SecretUri"],
                    config["KeyVault:ClientId"],
                    config["KeyVault:ClientSecret"]);
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MemoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MemoContextConnection")));
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MemoContextConnection")));

            services.AddTransient<IRepository<Memo>, Repository>();
            services.AddTransient<IMemorizerLogic, MemorizerLogic>();
            services.AddTransient<IEmailSender, EmailSender>(emailSender => new EmailSender(Configuration["SendGridKey"]));

            services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = "https://localhost:44367",
                        ValidAudience = "https://localhost:44367",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
                    };
                })
                .AddVKontakte(vkOptions =>
                {
                    vkOptions.ClientId = Configuration["VkClientId"];
                    vkOptions.ClientSecret = Configuration["VkClientSecret"];
                    vkOptions.CallbackPath = "/signin-vkontakte";
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["FacebookAppId"];
                    facebookOptions.AppSecret = Configuration["FacebookAppSecret"];
                    facebookOptions.CallbackPath = "/signin-facebook";
                })
                .AddInstagram(instagramOptions =>
                {
                    instagramOptions.ClientId = Configuration["InstagramClientId"];
                    instagramOptions.ClientSecret = Configuration["InstagramClientSecret"];
                    instagramOptions.CallbackPath = "/signin-instagram";
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["GoogleClientId"];
                    googleOptions.ClientSecret = Configuration["GoogleClientSecret"];
                    googleOptions.CallbackPath = "/signin-google";
                })
                .AddYahoo(yahooOptions =>
                {
                    yahooOptions.ClientId = Configuration["YahooClientId"];
                    yahooOptions.ClientSecret = Configuration["YahooClientSecret"];
                })
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = Configuration["MicrosoftClientId"];
                    microsoftOptions.ClientSecret = Configuration["MicrosoftClientSecret"];
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = Configuration["RecaptchaSiteKey"],
                SecretKey = Configuration["RecaptchaSecretKey"],
                ValidationMessage = "Please, do not forget to prove your humanity!"
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = "MemosQueue";
                options.IdleTimeout = TimeSpan.FromDays(1);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();
                app.UseWhen(x => x.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
                    builder =>
                    {
                        builder.UseMiddleware<BasicAuthMiddleware>();
                    });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Mapper.Initialize(cfg => { });

            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roleExist = await roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var admin = new IdentityUser
            {
                UserName = "admin@memorizer.ga",
                Email = "admin@memorizer.ga",
                EmailConfirmed = true
            };

            var user = await userManager.FindByNameAsync("admin");
            if (user == null)
            {
                var createAdmin = await userManager.CreateAsync(admin, "admin");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
