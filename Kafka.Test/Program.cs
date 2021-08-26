using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kafka.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) =>
                {
                    var cString = "User Id=root;Password=123456;Host=localhost;Database=my_other_store;AllowZeroDateTime=True;convert zero datetime=True;";
                    var ver = new MySqlServerVersion("5.7-mysql");
                    collection.AddHostedService<KafkaConsumerHostedService>();
                    collection.AddDbContext<ProductContext>(options => options
                        .UseMySql(cString, ver)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                    );
                });
    }


    
}
