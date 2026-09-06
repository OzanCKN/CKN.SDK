using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Search.Elasticsearch;

public static class ElasticsearchBuilderExtensions
{
    public static IServiceCollection AddCknElasticsearch(this IServiceCollection services, Action<ElasticsearchOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            var settings = new ElasticsearchClientSettings(new Uri(options.Url));

            if (!string.IsNullOrEmpty(options.ApiKey))
            {
                settings.Authentication(new ApiKey(options.ApiKey));
            }
            else if (!string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password))
            {
                settings.Authentication(new BasicAuthentication(options.Username, options.Password));
            }

            return new ElasticsearchClient(settings);
        });

        services.AddTransient(typeof(ISearchService<>), typeof(ElasticsearchSearchService<>));

        return services;
    }
}
