namespace ServerUnitTests.testData
{
    public class HelperTestData
    {
        public static class UserData
        {
            public static int RoleId { get; set; }
        }

        public static List<string> Comments()
        {
            return new()
            {
                "This is good",
                "Yummy",
                "Not good",
                "Not bad",
                "terrible",
                "Poor quality"
            };
        }

        public enum PositiveCommentWords
        {
            good, great, yummy
        }

        public enum NegativeCommentWords
        {
            bad, terrible, not, poor
        }
    }
}
