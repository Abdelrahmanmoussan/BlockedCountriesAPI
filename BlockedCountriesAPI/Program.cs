using BlockedCountriesAPI.BackgroundServices;
using BlockedCountriesAPI.Services;
using BlockedCountriesAPI.Services.IServices;

namespace BlockedCountriesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddSingleton<IBlockedCountryRepository, BlockedCountryRepository>();

            builder.Services.AddSingleton<IBlockedAttemptsRepository, BlockedAttemptsRepository>();

            builder.Services.AddHttpClient<IpLookupService>();
            builder.Services.AddScoped<IIpLookupService, IpLookupService>();


            builder.Services.AddHostedService<BlockCleanupService>();



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapGet("/", () => Results.Redirect("/swagger"));

            }


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();




            app.Run();
        }
    }
}
