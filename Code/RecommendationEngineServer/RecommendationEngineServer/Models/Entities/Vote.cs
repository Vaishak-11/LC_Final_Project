namespace RecommendationEngineServer.Models.Entities
{
    public class Vote
    {
        public int VoteId { get; set; }

        public int UserId { get; set; }

        public int RecommendationId { get; set; }

        public DateTime VoteDate { get; set; }

        public User User { get; set; }

        public Recommendation Recommendation { get; set; }
    }
}
