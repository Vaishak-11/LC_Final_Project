namespace RecommendationEngineServer.Models.Entities
{
    public class Vote
    {
        public int VoteId { get; set; }

        public int UserId { get; set; }

        public int RecommendationId { get; set; }

        public DateTime VoteDate { get; set; }

        public virtual User User { get; set; }

        public virtual Recommendation Recommendation { get; set; }
    }
}
