using RecommendationEngineClient.Models;

namespace RecommendationEngineClient
{
    public class DiscardMenuHandler
    {
        public async Task HandleDiscardMenuRequest()
        {
            string? discardMenuOption = null;
            string? discardMenuRequest = null;

            while (!string.IsNullOrEmpty(discardMenuOption))
            {
                Console.WriteLine("1. Remove the Food Item from Menu List ");
                Console.WriteLine("2. Get detailed feedback.");
                discardMenuOption = Console.ReadLine()?.ToLower().Trim();
            }

            switch (discardMenuOption.ToLower())
            {
                case "1":
                case "remove the food item from menu list":
                    discardMenuRequest = ServerRequestBuilder.BuildRequest("discardmenurequest");
                    break;

                case "2":
                case "get detailed feedback":
                    discardMenuRequest = ServerRequestBuilder.BuildRequest("getdetailedfeedback");
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            if (!string.IsNullOrEmpty(discardMenuRequest))
            {
                ServerResponse discardMenuResponse = ServerCommunicator.SendRequestToServer(discardMenuRequest);
                ResponseHandler.HandleResponse(discardMenuResponse);
            }
        }
    }
}
