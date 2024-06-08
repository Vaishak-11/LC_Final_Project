using RecommendationEngineClient.Models;
using System;

namespace RecommendationEngineClient
{
    public class ResponseHandler
    {
        public static void HandleResponse(ServerResponse response)
        {
            if (response.Name.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"An error occurred: {response.Value}");
            }
            else if (response.Name.Equals("ServerResponse", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(response.Value);
            }
            else
            {
                Console.WriteLine($"Server response: {response}");
            }
        }
    }
}