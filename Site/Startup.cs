using Core.Repositories;
using Core.Services;
using Site.Data;
using Site.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipeline.MongoDB;
using PipelineTask.MongoDB;
using Services;

namespace Site {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddTransient<IPipelineTaskRepository, IdentityPipelineTaskMongoDbService>();
            services.AddTransient<IPipelineRepository, IdentityPipelineMongoDbRepository>();
            services.AddTransient<IPipelineRunner, PipelineRunner>();
            services.AddTransient<IPipelineService, NotifierPipelineService>();
            services.AddTransient<IPipelineTaskService, PipelineTaskService>();
            services.AddTransient<IPipelineHubNotifier, PipelineHubNotifier>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddAuthorization(options => {
                options.AddPolicy("EditPolicy", policy =>
                    policy.Requirements.Add(new SameAuthorRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, TaskAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PipelineAuthorizationHandler>();

            services.AddMvc(options => {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<PipelineHub>("/pipelinehub");
                endpoints.MapRazorPages();
            });
        }
    }
}
