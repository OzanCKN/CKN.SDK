using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CKN.Sdk.Search.NRedisStack;

public static class NRedisStackBuilderExtensions
{
    public static IServiceCollection AddCknNRedisStack(this IServiceCollection services, Action<NRedisStackOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<NRedisStackOptions>>().Value;
            return ConnectionMultiplexer.Connect(options.Configuration);
        });

        services.AddTransient(typeof(ISearchService<>), typeof(NRedisStackSearchService<>));

        return services;
    }
}
