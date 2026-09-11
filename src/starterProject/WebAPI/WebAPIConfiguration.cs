namespace WebAPI;

public class WebAPIConfiguration
{
    public string APIDomain { get; set; }
    public string[] AllowedOrigins { get; set; }
    public string AppDomain { get; set; }
    public string AppName { get; set; }

    public WebAPIConfiguration()
    {
        APIDomain = string.Empty;
        AllowedOrigins = [];
        AppDomain = string.Empty;
        AppName = string.Empty;
    }

    public WebAPIConfiguration(
        string apiDomain,
        string[] allowedOrigins,
        string appDomain,
        string appName
    )
    {
        APIDomain = apiDomain;
        AllowedOrigins = allowedOrigins;
        AppDomain = appDomain;
        AppName = appName;
    }
}
