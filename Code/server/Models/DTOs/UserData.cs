namespace RecommendationEngineServer.Models.DTOs
{
    public static class UserData
    {
        public static int UserId { get; set; } 
        
        public static int RoleId { get; set; }

        public static Dictionary<int, List<NotificationStatus>> NotificationDeliverStatus = new Dictionary<int, List<NotificationStatus>>();
    }
}
