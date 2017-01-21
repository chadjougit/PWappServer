﻿
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PWappServer.Models;


namespace PWappServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

            /*
            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
            });
            */

            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                /*
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                */

                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            });

            //   services.AddCors();
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                          .AllowAnyMethod()
                                                                           .AllowAnyHeader().AllowCredentials()));

            services.AddAuthorization(options =>
            {
                // Policy for dashboard: only administrator role.
                options.AddPolicy("Manage Accounts", policy => policy.RequireClaim("role", "administrator"));
                // Policy for resources: user or administrator role.
                options.AddPolicy("Access Resources", policyBuilder => policyBuilder.RequireAssertion(
                        context => context.User.HasClaim(claim => (claim.Type == "role" && claim.Value == "user")
                           || (claim.Type == "role" && claim.Value == "administrator"))
                    )
                );
            });

            services.AddIdentityServer()
                 .AddTemporarySigningCredential()
                 .AddInMemoryIdentityResources(Config.GetIdentityResources())
                 .AddInMemoryApiResources(Config.GetApiResources())
                 .AddInMemoryClients(Config.GetClients())
                 .AddAspNetIdentity<ApplicationUser>(); // IdentityServer4.AspNetIdentity.

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext _context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("AllowAll");

            // IdentityServer4.AccessTokenValidation: authentication middleware for the API.
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000/",
                //Authority = "http://angular2spawebapi.azurewebsites.net",
                AllowedScopes = { "WebAPI" },

                RequireHttpsMetadata = false
            });

            app.UseMvc();

            // Adds Identity.
            app.UseIdentity();

            // Adds IdentityServer.
            app.UseIdentityServer();

            // Initializes the database.
         //   DbInitializer.Initialize(_context);
         //   DbInitializer.InitializeUser(_context);


        }
    }
}
