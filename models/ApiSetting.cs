public class ApiSetting {
    public string BaseUri { get; set; }
    public string ApiKey { get; set; }

    public ApiSetting(string baseUri, string apiKey) {
        BaseUri = baseUri;
        ApiKey = apiKey;
    }
}
