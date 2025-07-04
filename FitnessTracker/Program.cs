using FitnessTracker.Data;
using FitnessTracker.Infrastructure;
using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FitnessTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

   //         builder.Services.AddScoped<IEmailService, EmailService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("RegistrationPortal")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Admin/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Configure FluentEmail
            //var smtpClient = new SmtpClient(builder.Configuration["SmtpSettings:Host"])
            //{
            //    Port = int.Parse(builder.Configuration["SmtpSettings:Port"]),
            //    Credentials = new NetworkCredential(
            //    builder.Configuration["SmtpSettings:Username"],
            //    builder.Configuration["SmtpSettings:Password"]
            //),
            //    EnableSsl = true                
            //};

            //builder.Services.AddFluentEmail(builder.Configuration["SmtpSettings:Username"], "kraken butcher")
            //    .AddRazorRenderer()
            //    .AddSmtpSender(smtpClient);

            //// Register EmailService for DI
            //builder.Services.AddTransient<EmailService>();

            builder.Services.AddSignalR();

            //    builder.Services.AddSingleton<TokenProvider>();
            //    //From Here ----
            //    builder.Services.AddAuthentication(options =>
            //    {
            //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
            //    };
            //    options.Events = new JwtBearerEvents
            //    {
            //        OnMessageReceived = context =>
            //        {
            //            var authCookie = context.Request.Cookies["AuthToken"];
            //            if (!string.IsNullOrEmpty(authCookie))
            //            {
            //                context.Token = authCookie;
            //            }
            //            return Task.CompletedTask;
            //        }
            //    };
            //});
            //    builder.Services.AddAuthorization();
            //To Here ----

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<UserStatusHub>("/userStatusHub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

			// Add the admin user and role seeding logic here
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
				SeedAdminRoleAndUserAsync(roleManager, userManager).Wait();
			}        

            app.Run();
        }
        private static async Task SeedAdminRoleAndUserAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync("fasii@gmail.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "fasii@gmail.com",
                    Email = "fasii@gmail.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "AdminPassword123!");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

}
