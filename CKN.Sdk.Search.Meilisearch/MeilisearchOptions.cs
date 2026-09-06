namespace CKN.Sdk.Search.Meilisearch;

public class MeilisearchOptions
{
    public const string SectionName = "Search:Meilisearch";

    public string Url { get; set; } = "http://localhost:7700";
    public string? ApiKey { get; set; }
}
