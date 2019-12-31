using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapp.Areas.Identity;
using webapp.Data;
using System;
using webapp.Data.Entities;
using Blazored.Toast;
using Blazored.Modal;

namespace webapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == null) // Using no Docker i.e. IIS Express
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(
                            @"Server=(localdb)\mssqllocaldb;Database=auth;MultipleActiveResultSets=true"),
                    ServiceLifetime.Transient);
                services.AddDbContext<ZFContext>(options =>
                    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=zf;MultipleActiveResultSets=true"),
                    ServiceLifetime.Transient);
            }
            else if (Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == "Development")
            {
                //Server=(localdb)\\mssqllocaldb;Database=MvcMovieContext-2;Trusted_Connection=True;MultipleActiveResultSets=true
                var connectionAuthDb = @"Server=db;Database=auth;User=sa;Password=7zc7agecM6EmRmoiQmvYF5k3v;integrated security=false;MultipleActiveResultSets=true";
                var connectionZFDb = @"Server=db;Database=zf;User=sa;Password=7zc7agecM6EmRmoiQmvYF5k3v;integrated security=false;MultipleActiveResultSets=true";

                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseSqlServer(connectionAuthDb), ServiceLifetime.Transient);
                services.AddDbContext<ZFContext>(
                    options => options.UseSqlServer(connectionZFDb), ServiceLifetime.Transient);
            }
            else if ((Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == "Production"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("ApplicationDB")), ServiceLifetime.Transient);

                services.AddDbContext<ZFContext>(options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("ZFContext")), ServiceLifetime.Transient);
            }
            else
            {
                throw new InvalidOperationException("Please specify which env you want to use.");
            }

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredToast();
            services.AddBlazoredModal();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Automatically perform database migration
            MigrateDatabase(app);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static void MigrateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }

                using (var context = serviceScope.ServiceProvider.GetService<ZFContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}