namespace CKN.Sdk.Search.Elasticsearch;

public class ElasticsearchOptions
{
    public const string SectionName = "Search:Elasticsearch";

    public string Url { get; set; } = "http://localhost:9200";
    public string? ApiKey { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
