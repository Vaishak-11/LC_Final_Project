using RecommendationEngineClient.Models;

namespace RecommendationEngineClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exitRequested = false;
            bool loggedIn = false;

            while (!loggedIn)
            {
                Console.WriteLine("Please login first.");
                string loginCommand = "login";
                string loginRequest = ServerRequestBuilder.BuildRequest(loginCommand);
                if (loginRequest != null)
                {
                    ServerResponse loginResponse = ServerCommunicator.SendRequestToServer(loginRequest);
                    ResponseHandler.HandleResponse(loginResponse);

                    if (loginResponse.Value.ToString().ToLower().Contains("success"))
                    {
                        loggedIn = true;
                    }
                    else
                    {
                        Console.WriteLine("Login failed. Please try again.");
                    }
                }
            }

            while (!exitRequested)
            {
                Console.WriteLine("Enter command (login, addmenu, recommend, feedback, exit):");
                string command = Console.ReadLine();

                switch (command.ToLower())
                {
                    case "exit":
                        exitRequested = true;
                        ServerCommunicator.SendRequestToServer("exit");
                        break;
                    default:
                        string request = ServerRequestBuilder.BuildRequest(command);
                        if (request != null)
                        {
                            ServerResponse response = ServerCommunicator.SendRequestToServer(request);
                            ResponseHandler.HandleResponse(response);
                        }
                        break;
                }
            }
        }
    }
}