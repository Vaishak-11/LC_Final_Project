using RecommendationEngineClient.Models;
using System.Globalization;
using System.Text.Json;

namespace RecommendationEngineClient
{
    public class ServerRequestBuilder
    {
        public static string BuildRequest(string command)
        {
            switch (command.ToLower())
            {
                case "login":
                    return BuildLoginRequest();
                case "register":
                    return BuildRegisterRequest();
                default:
                    return BuildRoleBasedRequest(command);
            }
        }

        private static string BuildRoleBasedRequest(string command)
        {
            int currentRoleId = UserData.RoleId < 1 ? throw new Exception("Invalid role") : UserData.RoleId;

            switch (currentRoleId)
            {
                case 1:
                    return BuildAdminRequest(command);
                case 2:
                    return BuildChefRequest(command);
                case 3:
                    return BuildEmployeeRequest(command);
                default:
                    Console.WriteLine("Unknown role.");
                    return null;
            }
        }

        private static string BuildAdminRequest(string command)
        {
            switch (command.ToLower())
            {
                case "add menu item":
                case "1":
                    return BuildAddItemRequest();

                case "get menu items":
                case "2":
                    return BuildGetFoodItemsRequest();

                case "update menu item":
                case "3":
                    return BuildUpdateItemRequest();

                case "delete menu item":
                case "4":
                    return BuildDeleteItemRequest();

                case "get monthly food report":
                case "5":
                    return BuildFoodReportRequestRequest();

                case "get feedbacks":
                case "6":
                    return BuildGetFeedbacksRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private static string BuildEmployeeRequest(string command)
        {
            switch (command.ToLower())
            {
                case "add feedback":
                case "2":
                    return BuildAddFeedbackRequest();

                case "get recommended items":
                case "1":
                    return BuildGetRecommendationForDateRequest();

                case "add order":
                case "3":
                    return BuildAddOrderRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private static string BuildChefRequest(string command)
        {
            switch (command.ToLower())
            {
                case "get feedback list":
                case "2":
                    return BuildGetFeedbacksRequest();

                case "add recommended items":
                case "1":
                    return BuildAddRecommendationRequest();

                case "update recommended items":
                case "3":
                    return BuildUpdateRecommendationRequest();

                case "get recommended items":
                case "4":
                    return BuildGetRecommendationForDateRequest();

                case "get menu items":
                case "5":
                    return BuildGetFoodItemsRequest();

                case "get orders":
                case "6":
                    return BuildGetOrdersRequest();

                case "get monthly food report":
                case "7":
                    return BuildFoodReportRequestRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private static string BuildLoginRequest()
        {
            string role = null;
            Console.WriteLine("Which role would you like to log in as? (Employee, Admin, Chef)");

            while (true)
            {
                Console.Write("Enter your role: ");
                role = Console.ReadLine()?.Trim().ToLower();

                if (role == "employee" || role == "admin" || role == "chef")
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Invalid role. Please enter either 'Employee', 'Admin', or 'Chef'.");
                }
            }

            UserLoginDTO user = new UserLoginDTO();
            user.Role = role;

            switch (role.ToLower())
            {
                case "employee":
                    Console.WriteLine("Enter your Employee Code:");
                    user.UserName = Console.ReadLine();
                    break;

                default:
                    Console.WriteLine("Enter your UserName:");
                    user.UserName = Console.ReadLine();
                    break;
            }

            Console.WriteLine("Enter your Password:");
            user.Password = Console.ReadLine();

            string userJson = JsonSerializer.Serialize(user);
            return $"login#{userJson}";
        }

        private static string BuildLogoutrequest()
        {
            return $"logout#{UserData.UserId}";
        }

        private static string BuildRegisterRequest()
        {
            string role = null;

            Console.WriteLine("Which role would you like to log in as? (Employee, Admin, Chef)");
            while (true)
            {
                Console.Write("Enter your role: ");
                role = Console.ReadLine()?.Trim().ToLower();

                if (role == "employee" || role == "admin" || role == "chef")
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Invalid role. Please enter either 'Employee', 'Admin', or 'Chef'.");
                }
            }

            UserLoginDTO user = new UserLoginDTO();
            user.Role = role;

            Console.WriteLine("Enter your UserName:");
            user.UserName = Console.ReadLine();

            Console.WriteLine("Enter your Password:");
            user.Password = Console.ReadLine();

            if (role == "employee")
            {
                Console.Write("Enter your Employee Code: ");
                user.EmployeeCode = Console.ReadLine();
            }

            string userJson = JsonSerializer.Serialize(user);
            return $"register#{userJson}";
        }

        private static string BuildAddItemRequest()
        {
            FoodItemDTO item = new FoodItemDTO();

            while (string.IsNullOrWhiteSpace(item.ItemName))
            {
                Console.Write("Enter the Food item name: ");
                item.ItemName = Console.ReadLine();
            }
            
            while (item.Price <= 0)
            {
                Console.Write("Enter the price for it: ");
                item.Price = Convert.ToDecimal(Console.ReadLine());
            }

            while (item.FoodCategory == FoodCategory.None)
            {
                Console.Write("Enter the category for it [Breakfast, Lunch, Dinner, Beverage]: ");
                item.FoodCategory = ParseFoodCategory(Console.ReadLine());
            }

            Console.Write("Is the item available? [Y/N]: ");
            item.IsAvailable = Console.ReadLine().ToLower() == "y" ? true : false;

            string menuJson = JsonSerializer.Serialize(item);
            return $"additem#{menuJson}";
        }

        private static string BuildGetFoodItemsRequest()
        {
            return "getitems";
        }

        private static string BuildUpdateItemRequest()
        {
            string oldItemName = null;

            while(string.IsNullOrWhiteSpace(oldItemName))
            {
                Console.Write("Enter the name of the item you want to update: ");
                oldItemName = Console.ReadLine();
            }

            string availablity = null;
            FoodItemDTO item = new FoodItemDTO();

            while (true)
            {
                Console.WriteLine("Name, Price, Category, Availability. Enter 'e' to exit");
                Console.Write("Enter the parameter you want to update for the item: ");
                string param = Console.ReadLine()?.Trim().ToLower();

                if (param == "e")
                {
                    break;
                }

                switch (param)
                {
                    case "name":
                        Console.Write("Enter the new Food item name: ");
                        item.ItemName = Console.ReadLine();
                        break;

                    case "price":
                        Console.Write("Enter the price for it: ");
                        item.Price = Convert.ToDecimal(Console.ReadLine());
                        break;

                    case "category":
                        Console.Write("Enter the category for it [Breakfast, Lunch, Dinner, Beverage]: ");
                        item.FoodCategory = ParseFoodCategory(Console.ReadLine());
                        break;

                    case "availability":
                        Console.Write("Is the item available? [Y/N]: ");
                        availablity = Console.ReadLine()?.Trim().ToLower();
                        break;

                    default:
                        Console.WriteLine("Unknown parameter.");
                        break;
                }
            }

            string menuJson = JsonSerializer.Serialize(item);
            return $"updateitem#{oldItemName}#{menuJson}#{availablity}";
        }

        private static string BuildDeleteItemRequest()
        {
            Console.Write("Enter the name of the item you want to delete: ");
            string itemName = Console.ReadLine();

            return $"deleteitem#{itemName}";
        }

        private static string BuildFoodReportRequestRequest()
        {
            Console.Write("Enter the month for which you want to see the report [1-12]: ");
            int month = Convert.ToInt32(Console.ReadLine());
            while (month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month. Please enter a month between 1 and 12");
                month = Convert.ToInt32(Console.ReadLine());
            }

            Console.Write("Enter the year for which you want to see the report: ");
            int year = Convert.ToInt32(Console.ReadLine());
            while(year > DateTime.Now.Year)
            {
                Console.WriteLine("Invalid year. Please enter a year which is less than or equal to current year");
                year = Convert.ToInt32(Console.ReadLine());
            }

            return $"getfoodreport#{month}#{year}";
        }

        private static string BuildAddFeedbackRequest()
        {
            FeedbackDTO feedbackDTO = new FeedbackDTO();
            feedbackDTO.UserId = UserData.UserId;
            feedbackDTO.FeedbackDate = DateTime.Now;

            while (string.IsNullOrWhiteSpace(feedbackDTO.ItemName))
            {
                Console.Write("Enter the menu item Name for which you want to add feedback: ");
                feedbackDTO.ItemName = Console.ReadLine();
            }
            
            while (string.IsNullOrWhiteSpace(feedbackDTO.Comment))
            {
                Console.Write("Enter your feedback Comment: ");
                feedbackDTO.Comment = Console.ReadLine();
            }
            
            Console.Write("Enter your rating for the item[1-5]: ");
            feedbackDTO.Rating = Convert.ToInt32(Console.ReadLine());
            while(feedbackDTO.Rating < 1 || feedbackDTO.Rating > 5)
            {
                Console.WriteLine("Invalid rating. Please enter a rating between 1 and 5");
                feedbackDTO.Rating = Convert.ToInt32(Console.ReadLine());
            }

            string feedbackJson = JsonSerializer.Serialize(feedbackDTO);
            return $"addfeedback#{feedbackJson}";
        }

        private static string BuildGetFeedbacksRequest()
        {
            string itemName = null;

            while (string.IsNullOrEmpty(itemName))
            {
                Console.Write("Enter the menu item Name for which you want to see feedbacks: ");
                itemName = Console.ReadLine();
            }

            return $"getfeedbacks#{itemName}";
        }

        private static string BuildAddRecommendationRequest()
        {
            List<RecommendedMenuDTO> recommendations = new List<RecommendedMenuDTO>();

            while (true)
            {
                Console.WriteLine("Enter the food category for which you want to recommend items [Breakfast, Lunch, Dinner, Beverage]: ");
                string category = Console.ReadLine()?.Trim();

                Console.Write("Enter the number of items to be recommended: ");
                int numberOfItems = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < numberOfItems; i++)
                {
                    RecommendedMenuDTO recommendationDTO = new RecommendedMenuDTO();
                    recommendationDTO.UserId = UserData.UserId;
                    recommendationDTO.RecommendationDate = DateTime.Now.AddDays(1);
                    recommendationDTO.Category = ParseFoodCategory(category);

                    while(string.IsNullOrWhiteSpace(recommendationDTO.ItemName))
                    {
                        Console.Write("Enter the menu item Name you want to recommend: ");
                        recommendationDTO.ItemName = Console.ReadLine();
                    }

                    recommendations.Add(recommendationDTO);
                }

                Console.Write("Do you want to recommend more items? [Y/N]: ");
                if (Console.ReadLine()?.ToLower().Trim() != "y")
                {
                    break;
                }
            }

            string recommendationsJson = JsonSerializer.Serialize(recommendations);
            return $"addrecommendations#{recommendationsJson}";
        }

        private static string BuildGetRecommendationForDateRequest()
        {
            DateTime dateTime = DateTime.Now.Date;

            if(UserData.RoleId == 2)
            {
                Console.Write("Enter the Date in the format [yyyy-mm-dd] for which you want to see the recommendations: ");
                string userInput = Console.ReadLine();

                if (DateTime.TryParseExact(userInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime userDate))
                {
                    dateTime = userDate.Date;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Using current date instead.");
                }
            }
            else
            {
                dateTime = dateTime.AddDays(1);
            }
            
            return $"getrecommendations#{dateTime: yyyy-MM-dd}";
        }


        private static string BuildUpdateRecommendationRequest()
        {
            RecommendedMenuDTO recommendedMenu = new RecommendedMenuDTO();

            while(recommendedMenu.OldCategory == FoodCategory.None)
            {
                Console.WriteLine("Enter the food category for which you want to update the item [Breakfast, Lunch, Dinner, Beverage]: ");
                recommendedMenu.OldCategory = ParseFoodCategory(Console.ReadLine()?.Trim());
            }

            while (string.IsNullOrWhiteSpace(recommendedMenu.OldItemName))
            {
                Console.Write("Enter the name of the item you want to update: ");
                recommendedMenu.OldItemName = Console.ReadLine();
            }

            while (true)
            {
                Console.WriteLine("Enter the parameter you want to update for the item (Name, Category, RecommendationStatus). Enter 'e' to exit:");
                string param = Console.ReadLine()?.Trim().ToLower();

                if (param == "e")
                {
                    break;
                }

                switch (param)
                {
                    case "name":
                        Console.Write("Enter the new Food item name: ");
                        recommendedMenu.ItemName = Console.ReadLine()?.Trim();
                        break;

                    case "category":
                        Console.Write("Enter the new category for the item [Breakfast, Lunch, Dinner, Beverage]: ");
                        recommendedMenu.Category = ParseFoodCategory(Console.ReadLine()?.Trim());
                        break;

                    case "recommendationstatus":
                        Console.Write("Enter the recommendation status for the item [Y/N]: ");
                        recommendedMenu.IsRecommended  = Console.ReadLine()?.Trim().ToLower() == "y";
                        break;

                    default:
                        Console.WriteLine("Unknown parameter.");
                        break;
                }
            }

            recommendedMenu.RecommendationDate = DateTime.Now.AddDays(1);
            recommendedMenu.UserId = UserData.UserId;

            string recommendationsJson = JsonSerializer.Serialize(recommendedMenu);
            return $"updaterecommendation#{recommendationsJson}";
        }

        private static string BuildAddOrderRequest()
        {
            OrderDTO orderDTO = new()
            {
                UserId = UserData.UserId,
                OrderDate = DateTime.Now,
                ItemNames = new List<string>()
            };
            bool userOption = true;

            while (userOption)
            {
                FoodCategory category = FoodCategory.None;

                while (category == FoodCategory.None)
                {
                    Console.Write("Enter the category for which you want to order [Breakfast, Lunch, Dinner, Beverage]: ");
                    category = ParseFoodCategory(Console.ReadLine()?.Trim());

                    if (category == FoodCategory.None)
                    {
                        Console.WriteLine("Invalid category. Please try again.");
                    }
                }

                int numberOfItems;

                while (true)
                {
                    Console.Write("Enter the number of items you want to order: ");

                    if (!int.TryParse(Console.ReadLine(), out numberOfItems) || numberOfItems <= 0)
                    {
                        Console.WriteLine("Invalid number of items. Please enter a positive integer.");
                    }
                    else
                    {
                        break;
                    }
                }

                Console.Write("Enter the item names you want to order: ");
                for (int i = 0; i < numberOfItems; i++)
                {
                    Console.Write($"Food Item {i + 1}: ");
                    string item = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        orderDTO.ItemNames.Add(item);
                    }
                    else
                    {
                        Console.WriteLine("Item name cannot be empty. Please try again.");
                        i--; 
                    }
                }

                Console.Write("Do you want to order more items? [Y/N]: ");
                userOption = Console.ReadLine()?.ToLower().Trim() == "y";
            }

            string orderJson = JsonSerializer.Serialize(orderDTO);

            return $"addorder#{orderJson}";
        }

        private static string BuildGetOrdersRequest() 
        {
            DateTime dateTime = DateTime.Now.Date;

            Console.WriteLine("Note: You can leave the date field empty to see the orders for the upcoming day.");
            Console.Write("Enter the Date in the format [yyyy-mm-dd] for the recommended menu for which you want to see the orders: "); 
            string userInput = Console.ReadLine();

            if(userInput != string.Empty)
            {
                if (DateTime.TryParseExact(userInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime userDate))
                {
                    dateTime = userDate.Date.AddDays(-1);
                }
                else
                {
                    Console.WriteLine("Invalid date format. Using current order date instead.");
                }
            }

            return $"getorders#{dateTime: yyyy-MM-dd}";
        }

        private static FoodCategory ParseFoodCategory(string input)
        {
            if (Enum.TryParse(input?.Trim(), true, out FoodCategory category))
            {
                return category;
            }
            else
            {
                return FoodCategory.None;
            }
        }
    }
}