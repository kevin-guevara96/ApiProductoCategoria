using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APIPRUEBAS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DBAPIContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConexion"))
            );

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            var RulesCors = "ReglasCors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: RulesCors, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Configuration.AddJsonFile("appsettings.json");
            var secretkey = builder.Configuration.GetSection("settings").GetSection("secretkey").ToString();
            var keyBytes = Encoding.UTF8.GetBytes(secretkey);

            builder.Services.AddAuthentication(config => { 
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience  = false
                };
            });

            var app = builder.Build();

            //using (var scope = app.Services.CreateScope()) 
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<DBAPIContext>();
            //    context.Database.Migrate();
            //}

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

            app.UseCors(RulesCors);

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
