using RecommendationEngineClient.Models;

namespace RecommendationEngineClient
{
    public class AuthenticationHandler
    {
        public async Task<bool> HandleLoginOrRegister()
        {
            Console.WriteLine("Please register/ login first. Enter 'a' for login 'b' for creating account");
            string? loginOption = Console.ReadLine()?.ToLower().Trim();
            string? loginCommand;

            switch (loginOption)
            {
                case "a":
                    loginCommand = "login";
                    break;

                case "b":
                    loginCommand = "register";
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    return false;
            }

            string loginRequest = ServerRequestBuilder.BuildRequest(loginCommand);
            if (loginRequest != null)
            {
                ServerResponse loginResponse = ServerCommunicator.SendRequestToServer(loginRequest);
                ResponseHandler.HandleResponse(loginResponse);

                if (!loginResponse.Value.ToString().Contains("failed", StringComparison.OrdinalIgnoreCase))
                {
                    if (loginCommand == "login")
                    {
                        UserData.UserId = loginResponse.UserId;
                        UserData.RoleId = loginResponse.RoleId;

                        return true;
                    }
                    else if (loginCommand == "register")
                    {
                        Console.WriteLine("Registration successful. Please log in.");
                    }
                }
                else
                {
                    Console.WriteLine($"{loginCommand} failed. Please try again.");
                }
            }

            return false;
        }
    }
}
