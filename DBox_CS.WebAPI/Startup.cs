using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBox_CS.WebAPI.Authentication;
using DBox_CS.WebAPI.DAL;
using DBox_CS.WebAPI.Services;
using DBox_CS.Core.AppClass;
using DBox_CS.Core.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DBox_CS.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{

        //    services.AddControllers();
        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DBox_CS.WebAPI", Version = "v1" });
        //        //c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        //        //{
        //        //    Description="the API key to access the API",
        //        //    Type= SecuritySchemeType.ApiKey,
        //        //    Name="x-api-key",
        //        //    In=ParameterLocation.Header,
        //        //    Scheme="ApiKeyScheme"
        //        //});
        //        //var scheme = new OpenApiSecurityScheme
        //        //{
        //        //    Reference = new OpenApiReference
        //        //    {
        //        //        Type = ReferenceType.Schema,
        //        //        Id = "ApiKey"
        //        //    },
        //        //    In = ParameterLocation.Header
        //        //};
        //        //var requirement = new OpenApiSecurityRequirement
        //        //{
        //        //    {scheme,new List<string>() }
        //        //};
        //        //c.AddSecurityRequirement(requirement);

        //        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        //        {
        //            Description = "the API key to access the API",
        //            Type = SecuritySchemeType.ApiKey,
        //            Name = "x-api-key",
        //            In = ParameterLocation.Header,
        //            Scheme = "ApiKeyScheme"
        //        });


        //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                new OpenApiSecurityScheme
        //                {
        //                    Reference = new OpenApiReference
        //                    {
        //                        Id = "ApiKey",
        //                        Type = ReferenceType.SecurityScheme
        //                    }
        //                },
        //                Array.Empty<string>()
        //            }
        //        });

        //    });
        //    services.AddScoped<ApiKeyAuthFilter>();

        //}
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

# region OAUTH related
            var jwtSettings = Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // Set to true in production
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true
                };
            });
            #endregion

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Civilsoft DBI API",
                    Version = "v1"
                });

                //// Basic Auth for Swagger //09032026
                //c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                //{
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "basic",
                //    In = ParameterLocation.Header,
                //    Description = "Username: cpiuser, Password: Cpi@12345"
                //});

                // Change 'basic' to 'Bearer'
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1...\""
                });

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement //09032026
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "basic"
                //            }
                //        },
                //        new string[] { }
                //    }
                //});

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Bind the configuration settings to AppSettings
            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            appSettings.ConnectionString = Configuration.GetConnectionString("SQL");

            services.AddSingleton<IAppSettings>(appSettings);


            //services.AddScoped<DbHelper>();
            services.AddScoped<ILoggingService,LoggingService>();


           

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "DBox_CS.WebAPI v1"));
            }

            app.UseHttpsRedirection();

            //app.UseMiddleware<ApiKeyAuthMiddleware>();
            //app.UseMiddleware<BasicAuthMiddleware>();
            //app.UseBasicAuth(); //09032026
            app.UseRouting();
            app.UseAuthentication(); // 1. Checks the JWT token //09032026
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
