using System.Configuration;
using Rebus.Config;
using Serilog;
// ReSharper disable ArgumentsStyleLiteral

namespace Common
{
    public static class CommonRebusConfigurationExtensions
    {
        public static RebusConfigurer ConfigureEndpoint(this RebusConfigurer configurer, EndpointRole role)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RebusDatabase"]?.ConnectionString ?? throw new ConfigurationErrorsException("Could not find 'RebusDatabase' connection string");

            configurer
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t =>
                {
                    if (role == EndpointRole.Client)
                    {
                        t.UseMsmqAsOneWayClient();
                    }
                    else
                    {
                        t.UseMsmq(Config.AppSetting("QueueName"));
                    }
                })
                .Subscriptions(s =>
                {
                    var subscriptionsTableName = Config.AppSetting("SubscriptionsTableName");

                    s.StoreInSqlServer(connectionString, subscriptionsTableName, isCentralized: true);
                })
                .Sagas(s =>
                {
                    if (role != EndpointRole.SagaHost) return;

                    var dataTableName = Config.AppSetting("SagaDataTableName");
                    var indexTableName = Config.AppSetting("SagaIndexTableName");

                    // store sagas in SQL Server to make them persistent and survive restarts
                    s.StoreInSqlServer(connectionString, dataTableName, indexTableName);
                })
                .Timeouts(t =>
                {
                    if (role == EndpointRole.Client) return;

                    var tableName = Config.AppSetting("TimeoutsTableName");

                    // store timeouts in SQL Server to make them persistent and survive restarts
                    t.StoreInSqlServer(connectionString, tableName);
                });

            return configurer;
        }
    }
}
