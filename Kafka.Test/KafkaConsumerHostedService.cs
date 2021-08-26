using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.Test
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private IConsumer<string, string> _consumer;
        private Func<IServiceScope> _scope;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = "kafka:9092",
                GroupId = "kafka.test",
                AutoOffsetReset = AutoOffsetReset.Latest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe("mysql.mystore.products");
            _scope = () => scopeFactory.CreateScope();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scope())
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    var key = consumeResult?.Message?.Key;
                    var value = consumeResult?.Message?.Value;
                    _logger.LogInformation($"\n{key}\n{value}");

                    var keyProd = JsonConvert.DeserializeObject<Product>(key);
                    var product = value == null ? null : JsonConvert.DeserializeObject<Product>(value);

                    var storeContext = scope.ServiceProvider.GetService<ProductContext>();
                    var exists = storeContext.Products.AsNoTracking().Any(p => p.Id == keyProd.Id);

                    if (exists && product != null)
                    {
                        storeContext.Products.Update(product);
                    }
                    else if (!exists)
                    {
                        storeContext.Products.Add(product);
                    }
                    else
                    {
                        storeContext.Products.Remove(keyProd);
                    }
                    await storeContext.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
