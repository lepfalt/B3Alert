class EmailOAuthSetting {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public EmailOAuthSetting(string clientId, string clientSecret) {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
}
