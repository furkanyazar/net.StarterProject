namespace WebAPI;

public class WebAPIConfiguration
{
    public string APIDomain { get; set; }
    public string[] AllowedOrigins { get; set; }

    public WebAPIConfiguration()
    {
        APIDomain = string.Empty;
        AllowedOrigins = [];
    }

    public WebAPIConfiguration(string apiDomain, string[] allowedOrigins)
    {
        APIDomain = apiDomain;
        AllowedOrigins = allowedOrigins;
    }
}
