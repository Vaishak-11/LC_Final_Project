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

        public async Task DisplayOperations()
        {
            while (true)
            {
                while (!_isUserLoggedIn)
                {
                    await HandleLoginorRegister();
                }

                ShowWelcomeMessage();
                _isLogoutRequested = false;

                if(_isDiscardMenuRequested)
                {
                    

                    await HandleDiscardMenuRequest();
                }

                while (!_isLogoutRequested && _isUserLoggedIn)
                {
                    await DisplayRoleBasedOperations();

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
                        if(command.ToLower().Contains("discard"))
                        {
                            _isDiscardMenuRequested = true;
                        }

                        ResponseHandler.HandleResponse(response);
                    }
                }
            }
        }

        private async Task HandleLoginorRegister()
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
                    return;
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
                        _isUserLoggedIn = true;
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
            Console.WriteLine("5. Get Monthly Food report");
            Console.WriteLine("6. Get Feedbacks");
            Console.WriteLine("7. Get Discard Menu List");
            Console.WriteLine("8. Discard food item");
            Console.WriteLine("9. Get detailed feedback for the item");
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

        private async Task HandleDiscardMenuRequest()
        {
            string? discardMenuOption = null;
            string? discardMenuRequest = null;

            while (!string.IsNullOrEmpty(discardMenuOption))
            {
                Console.WriteLine("1. Remove the Food Item from Menu List ");
                Console.WriteLine("2. Get detailed feedback.");
                discardMenuOption = Console.ReadLine()?.ToLower().Trim();
            }

            switch(discardMenuOption.ToLower())
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

                //if (!discardMenuResponse.Value.ToString().Contains("failed", StringComparison.OrdinalIgnoreCase))
                //{
                //    Console.WriteLine("Menu discarded successfully.");
                //}
                //else
                //{
                //    Console.WriteLine("Menu discard failed. Please try again.");
                //}
            }
        }
    }
}
