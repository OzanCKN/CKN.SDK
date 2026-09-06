using Meilisearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Search.Meilisearch;

public static class MeilisearchBuilderExtensions
{
    public static IServiceCollection AddCknMeilisearch(this IServiceCollection services, Action<MeilisearchOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MeilisearchOptions>>().Value;
            return new MeilisearchClient(options.Url, options.ApiKey);
        });

        services.AddTransient(typeof(ISearchService<>), typeof(MeilisearchSearchService<>));

        return services;
    }
}
