using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Pub.Application.Swagger;
using Api.Pub.Core.Managers.Authentication;
using Api.Pub.Core.Providers;
using Api.Pub.Core.Token;
using Api.Pub.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Api.Pub.Application
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Startup
    {
        private const string SecretKey = "webapitestjeyson@webapitestjeyson";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            #region Authentication

            services.AddAuthorization(options =>
            {
                options.AddPolicy("WebApiUser", policy => policy.RequireClaim("PanelAdministrator", "BackEnd"));
            });

            // Carrega as opções do Token
            var tokenOptions = Configuration.GetSection(nameof(AuthorizeOptions));
            var tokenValidationParameters = new TokenValidationParametersBuilder(tokenOptions, _signingKey)
                .Build();

            // Configura as opções do Token
            services.Configure<AuthorizeOptions>(options =>
            {
                options.Issuer = tokenOptions[nameof(AuthorizeOptions.Issuer)];
                options.Audience = tokenOptions[nameof(AuthorizeOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParameters;
            });

            #endregion

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "API de Autenticação",
                    Description = "API para criar e validar a autenticação",
                    TermsOfService = "None"
                });

                //var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "mysql.b2b.api.pub.authenticate.xml");
                //c.IncludeXmlComments(filePath);
                c.DescribeAllEnumsAsStrings();
            });
            #endregion

            services.AddTransient<IDataBaseProvider, DataBaseProvider>();
            services.AddTransient<IAuthenticateManager, AuthenticateManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = "swagger";
            });
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
