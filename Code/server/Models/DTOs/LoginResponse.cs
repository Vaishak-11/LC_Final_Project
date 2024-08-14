namespace RecommendationEngineServer.Models.DTOs
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public List<string> Notifications { get; set; }
    }
}
