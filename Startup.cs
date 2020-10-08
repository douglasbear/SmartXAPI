using System;
using AutoMapper;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SmartxAPI.Profiles;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartxAPI.GeneralFunctions;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace SmartxAPI
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
            services.AddDbContext<SmartxContext>(opt => opt.UseSqlServer
                (Configuration.GetConnectionString("SmartxConnection")));

        services.AddCors();

            services.AddControllers().AddNewtonsoftJson(s => {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //JWT Auth
            var appSettings=appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(au=>{
                au.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                au.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt=>{
                jwt.RequireHttpsMetadata=false;
                jwt.SaveToken=true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey =true,
                    IssuerSigningKey=new SymmetricSecurityKey(key),
                    ValidateIssuer=false,
                    ValidateAudience=false

                };
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ISec_UserRepo,Sec_UserRepo>();
            services.AddScoped<ILanguageRepo,LanguageRepo>();
            services.AddScoped<ICommenServiceRepo,CommenServiceRepo>();
            services.AddScoped<IDataAccessLayer,DataAccessLayer>();
            services.AddScoped<IApiFunctions,ApiFunctions>();
            services.AddScoped<IMyFunctions,MyFunctions>();
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            app.UseCors(options=>
            //options.WithOrigins("http://localhost:3000")
            options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/error"); 
            }
            // else
            // {
            //     app.UseExceptionHandler("/error"); 
            // }

            app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

        
            var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("en-US"),
                    new CultureInfo("es"),
                    new CultureInfo("es-ES")
                };
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
