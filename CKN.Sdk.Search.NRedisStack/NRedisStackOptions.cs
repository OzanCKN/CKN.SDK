namespace CKN.Sdk.Search.NRedisStack;

public class NRedisStackOptions
{
    public const string SectionName = "Search:NRedisStack";

    public string Configuration { get; set; } = "localhost:6379";
}
