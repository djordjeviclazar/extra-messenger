using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ExtraMessenger.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ExtraMessenger.Hubs;
using System.Threading.Tasks;
using ExtraMessenger.Services.Authentication.Interfaces;
using ExtraMessenger.Services.Authentication;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Neo4jClient;
using System;
using ExtraMessenger.Services.Github.Interfaces;
using ExtraMessenger.Services.Github;

namespace ExtraMessenger
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            builder
            .WithOrigins("http://localhost:4200", "http://localhost:4201")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            ));


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Secret").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                               // If the request is for our hub...
                               var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/chat")))
                                {
                                   // Read the token out of the query string
                                   context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });
            // Neo4j:
            string pass = Configuration.GetSection("Neo4JTestSettingsL").GetSection("Pass").Value;
            string user = Configuration.GetSection("Neo4JTestSettingsL").GetSection("Username").Value;

            var client = new GraphClient(new Uri(Configuration.GetConnectionString("Neo4JDB")), user, pass);
            Task task = Task.Run(async () => await client.ConnectAsync());
            task.Wait();
            services.AddSingleton<IGraphClient>(client);

            // MongoDB:
            services.Configure<MongoDBSettings>(Configuration.GetSection("MongoDBSettings"));

            services.AddSingleton<IMongoDBSettings>(sp => sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

            services.AddSingleton<MongoService>();

            // Github:
            services.AddSingleton<IGithubClientService, GithubClientService>();

            services.AddSignalR();

            // test:
            services.AddSingleton<ChatHub>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //Global exception handler
                // app.UseExceptionHandler(builder =>
                // {
                //     builder.Run(async context =>
                //     { //context refers to Http context
                //         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //         var error = context.Features.Get<IExceptionHandlerFeature>();
                //         if (error != null)
                //         {
                //             context.Response.AddApplicationError(error.Error.Message);
                //             await context.Response.WriteAsync(error.Error.Message);
                //         }
                //     });
                // });
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
