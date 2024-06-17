using RecommendationEngineClient.Models;
using System.Text.Json;

namespace RecommendationEngineClient
{
    public class ResponseHandler
    {
        public static void HandleResponse(ServerResponse response)
        {
            if (response.Name.ToLower() == "error")
            {
                Console.WriteLine($"An error occurred: {response.Value}");
            }
            else
            {
                if (response.Value is JsonElement jsonElement)
                {
                    HandleJsonElementResponse(jsonElement);
                }
                else
                {
                    Console.WriteLine(response.Value);
                }
            }
        }

        private static void HandleJsonElementResponse(JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Object:
                    Console.WriteLine(FormatJsonObject(jsonElement));
                    break;
                case JsonValueKind.Array:
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.Object)
                        {
                            Console.WriteLine(FormatJsonObject(element));
                        }
                        else
                        {
                            HandleJsonElementResponse(element);
                        }
                    }
                    break;
                default:
                    Console.WriteLine(jsonElement.ToString());
                    break;
            }
        }

        private static string FormatJsonObject(JsonElement jsonObject)
        {
            var formattedString = string.Empty;

            foreach (JsonProperty property in jsonObject.EnumerateObject())
            {
                formattedString += $"{property.Name}: {property.Value}   ";
            }

            return formattedString.Trim();
        }
    }
}
