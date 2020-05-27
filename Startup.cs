using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SmartxAPI.Models;
using SmartxAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using SmartxAPI.Profiles;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartxAPI.GeneralFunctions;

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

            services.AddScoped<IInv_CustomerRepo, Inv_CustomerRepo>();
            services.AddScoped<ISec_UserRepo,Sec_UserRepo>();
            services.AddScoped<IAcc_CompanyRepo,Acc_CompanyRepo>();
            services.AddScoped<IInv_SalesQuotationRepo,Inv_SalesQuotationRepo>();
            services.AddScoped<IInvCustomerProjectsRepo,Inv_CustomerProjectsRepo>();
            services.AddScoped<ILanguageRepo,LanguageRepo>();
            services.AddScoped<IInvProductsListRepo,Inv_ProductsListRepo>();
            services.AddScoped<IAccTaxCategoryRepo,Acc_TaxCategoryRepo>();
            services.AddScoped<ICommenServiceRepo,CommenServiceRepo>();
            services.AddScoped<IDataAccessLayer,DataAccessLayer>();
            
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
            }

            

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
