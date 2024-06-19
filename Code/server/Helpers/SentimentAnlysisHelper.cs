using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Helpers
{
    public static class SentimentAnlysisHelper
    {
        public static async Task<string> AnalyzeSentiments(List<string> comments, double avgRating = 0)
        {
            int sentimentScore = 0;
            string overallRating = string.Empty;

            foreach (var comment in comments)
            {
                string normalizedComment = comment.ToLower();

                if (normalizedComment.Contains("not"))
                {
                    sentimentScore = await HandleNotComment(comment, sentimentScore);
                }
                else
                {
                    if (ContainsPositiveWord(normalizedComment) && !ContainsNegativeWord(normalizedComment))
                    {
                        ++sentimentScore;
                    }
                    else if (ContainsNegativeWord(normalizedComment) && !ContainsPositiveWord(normalizedComment))
                    {
                        --sentimentScore;
                    }
                }
            }

            if(avgRating == 0 && !comments.Any())
            {
                sentimentScore = 0;
            }
            else
            {
                sentimentScore = avgRating > 3 && avgRating <= 3.5 ? sentimentScore : avgRating > 3.5 ? sentimentScore + 1 : sentimentScore - 1;
            }

            if (UserData.RoleId != 3)
            {
                overallRating = sentimentScore.ToString();
            }
            else
            {
                Sentiment sentiment = sentimentScore == 0 ? Sentiment.Neutral : sentimentScore > 0 ? Sentiment.Positive : Sentiment.Negative;
                overallRating = sentiment.ToString();
            }

            return overallRating;
        }

        private static async Task<int> HandleNotComment(string comment, int sentimentScore)
        {
            string[] words = comment.ToLower().Split(' ');
            string[] intensityWords = { "very", "too", "much" };

            bool isNegativeContext = false;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "not")
                {
                    if (i + 1 < words.Length && intensityWords.Contains(words[i + 1]))
                    {
                        i++;
                    }

                    isNegativeContext = true;
                    continue;
                }

                if (isNegativeContext)
                {
                    if (Enum.TryParse(words[i], true, out PositiveCommentWords positiveWord))
                    {
                        sentimentScore--;
                    }
                    else if (Enum.TryParse(words[i], true, out NegativeCommentWords negativeWord))
                    {
                        sentimentScore++;
                    }

                    isNegativeContext = false;
                }
                else
                {
                    if (Enum.TryParse(words[i], true, out PositiveCommentWords positiveWord))
                    {
                        sentimentScore++;
                    }
                    else if (Enum.TryParse(words[i], true, out NegativeCommentWords negativeWord))
                    {
                        sentimentScore--;
                    }
                }
            }

            return sentimentScore;
        }

        public static bool ContainsPositiveWord(string comment)
        {
            foreach (PositiveCommentWords word in Enum.GetValues(typeof(PositiveCommentWords)))
            {
                if (comment.Contains(word.ToString().ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsNegativeWord(string comment)
        {
            foreach (NegativeCommentWords word in Enum.GetValues(typeof(NegativeCommentWords)))
            {
                if (comment.Contains(word.ToString().ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
