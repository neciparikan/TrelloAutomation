namespace TrelloAutomation.Config
{
   public class TrelloSettings
{
    public string Url { get; set; }
    public Credentials Credentials { get; set; }
}

public class Credentials
{
    public string ValidEmail { get; set; }
    public string ValidPassword { get; set; }
    public string InvalidPassword { get; set; }
    public string ApiKey { get; set; }
    public string ApiToken { get; set; }
}
}
