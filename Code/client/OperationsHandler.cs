using RecommendationEngineClient.Models;
using System;

namespace RecommendationEngineClient
{
    public class OperationsHandler
    {
        private bool _isLogoutRequested = false;
        private bool _isUserLoggedIn = false;
        private bool _welcomeMessageShown = false;
        private bool _isDiscardMenuRequested = false;

        private readonly AuthenticationHandler _authenticationHandler = new();
        private readonly DiscardMenuHandler _menuHandler = new();

        public async Task DisplayOperations()
        {
            while (true)
            {
                _isUserLoggedIn = await _authenticationHandler.HandleLoginOrRegister();

                if (!_isUserLoggedIn)
                    continue;

                ShowWelcomeMessage();
                _isLogoutRequested = false;

                if(_isDiscardMenuRequested)
                {
                    await _menuHandler.HandleDiscardMenuRequest();
                }

                while (!_isLogoutRequested && _isUserLoggedIn)
                {
                    await DisplayRoleBasedOperations();

                    await HandleUserCommand();
                }
            }
        }

        private async Task DisplayRoleBasedOperations()
        {
            switch (UserData.RoleId)
            {
                case 1:
                    await DisplayAdminOperations();
                    break;
                case 2:
                    await DisplayChefOperations();
                    break;
                case 3:
                    await DisplayEmployeeOperations();
                    break;
                default:
                    Console.WriteLine("Invalid User.");
                    break;
            }
        }

        private async Task DisplayAdminOperations()
        {
            Console.WriteLine("1. Add Menu Item");
            Console.WriteLine("2. Get available Menu Items");
            Console.WriteLine("3. Update Menu Item");
            Console.WriteLine("4. Delete Menu Item");
            Console.WriteLine("5. Get Feedbacks");
            Console.WriteLine("6. Get Discard Menu List");
            Console.WriteLine("7. Discard food item");
            Console.WriteLine("8. Get detailed feedback for the item");
            Console.WriteLine("Logout");
            Console.WriteLine("Enter the operation you want to perform:");
        }

        private async Task DisplayChefOperations()
        {
            Console.WriteLine("1. Add Recommended Items");
            Console.WriteLine("2. Get Feedback List");
            Console.WriteLine("3. Update Recommended Items");
            Console.WriteLine("4. Get recommended items for a particular date");
            Console.WriteLine("5. Get available Menu Items");
            Console.WriteLine("6. Get Orders for a particular date.");
            Console.WriteLine("7. Get Monthly Food report");
            Console.WriteLine("8. Get Discard Menu List");
            Console.WriteLine("9. Discard food item");
            Console.WriteLine("10. Get detailed feedback for the item");
            Console.WriteLine("Logout");
            Console.WriteLine("Enter the operation you want to perform:");
        }

        private async Task DisplayEmployeeOperations()
        {
            Console.WriteLine("1. Get Recommended Items");
            Console.WriteLine("2. Add Feedback");
            Console.WriteLine("3. Order Food");
            Console.WriteLine("4. Provide detailed feedback for the item");
            Console.WriteLine("5. Update Profile");
            Console.WriteLine("Logout");
            Console.WriteLine("Enter the operation you want to perform:");
        }

        private void ShowWelcomeMessage()
        {
            if (!_welcomeMessageShown)
            {
                switch (UserData.RoleId)
                {
                    case 1:
                        Console.WriteLine("\nWelcome, Admin!");
                        break;
                    case 2:
                        Console.WriteLine("\nWelcome, Chef!");
                        break;
                    case 3:
                        Console.WriteLine("\nWelcome, Employee!");
                        break;
                    default:
                        Console.WriteLine("Invalid User.");
                        break;
                }

                _welcomeMessageShown = true;
            }
        }

        private async Task HandleUserCommand()
        {
            string command = Console.ReadLine();

            string request = ServerRequestBuilder.BuildRequest(command);

            if (request != null)
            {
                ServerResponse response = ServerCommunicator.SendRequestToServer(request);
                if (command.ToLower() == "logout")
                {
                    _isLogoutRequested = true;
                    _isUserLoggedIn = false;
                    _welcomeMessageShown = false;
                }
                if (command.ToLower().Contains("discard"))
                {
                    _isDiscardMenuRequested = true;
                }

                ResponseHandler.HandleResponse(response);
            }
        }  
    }
}
