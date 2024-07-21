using RecommendationEngineClient.Models;
using System.Globalization;
using System.Text.Json;

namespace RecommendationEngineClient
{
    public class ServerRequestBuilder
    {
        private static readonly string[] roles = {"admin", "chef", "employee"};
        
        public string BuildRequest(string command)
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

        private string BuildRoleBasedRequest(string command)
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

        private string BuildAdminRequest(string command)
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

                case "get feedbacks":
                case "5":
                    return BuildGetFeedbacksRequest();

                case "get discard menu list":
                case "6":
                    return BuildGetDiscardMenuRequest();

                case "discard food item":
                case "7":
                    return BuildDiscardMenuRequest();

                case "get detailed feedback for the item":
                case "8":
                    return BuildGetDetailedFeedbackRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private string BuildEmployeeRequest(string command)
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

                case "Provide detailed feedback for the item":
                case "4":
                    return BuildProvideDetailedFeedbackRequest();

                case "update profile":
                case "5":
                    return BuildUpdateProfileRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private string BuildChefRequest(string command)
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

                case "get discard menu list":
                case "8":
                    return BuildGetDiscardMenuRequest();

                case "discard food item":
                case "9":
                    return BuildDiscardMenuRequest();

                case "get detailed feedback for the item":
                case "10":
                    return BuildGetDetailedFeedbackRequest();

                case "logout":
                    return BuildLogoutrequest();

                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private string BuildLoginRequest()
        {
            string role = null;
            Console.WriteLine("Which role would you like to log in as? (Employee, Admin, Chef)");

            while (true)
            {
                Console.Write("Enter your role: ");
                role = Console.ReadLine()?.Trim().ToLower();

                if (roles.Contains(role.ToLower()))
                {
                    break; 
                }
                else
                {
                    Console.WriteLine("Invalid role. Please enter either 'Employee', 'Admin', or 'Chef'.");
                }
            }

            UserLoginDTO user = new();
            user.Role = role;

            switch (role.ToLower())
            {
                case "employee":
                    while(string.IsNullOrWhiteSpace(user.UserName))
                    {
                        Console.WriteLine("Enter your Employee Code:");
                        user.UserName = Console.ReadLine();

                        ValidateInputString(user.UserName,"employee code");
                    }
                    break;

                default:
                    while (string.IsNullOrWhiteSpace(user.UserName))
                    {
                        Console.WriteLine("Enter your UserName:");
                        user.UserName = Console.ReadLine();

                        ValidateInputString(user.UserName, "username");
                    }
                    break;
            }

            while (string.IsNullOrWhiteSpace(user.Password))
            {
                Console.WriteLine("Enter your Password:");
                user.Password = Console.ReadLine();

                ValidateInputString(user.Password, "password");
            }

            string userJson = JsonSerializer.Serialize(user);
            return $"login#{userJson}";
        }

        private static string BuildLogoutrequest()
        {
            return $"logout#{UserData.UserId}";
        }

        private string BuildRegisterRequest()
        {
            string role = null;

            Console.WriteLine("Which role would you like to log in as? (Employee, Admin, Chef)");
            while (true)
            {
                while (string.IsNullOrWhiteSpace(role))
                {
                    Console.Write("Enter your role: ");
                    role = Console.ReadLine()?.Trim().ToLower();
                }

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

            while (string.IsNullOrWhiteSpace(user.UserName))
            {
                Console.WriteLine("Enter your UserName:");
                user.UserName = Console.ReadLine();

                ValidateInputString(user.UserName, "username");
            }

            while (string.IsNullOrWhiteSpace(user.Password))
            {
                Console.WriteLine("Enter your Password:");
                user.Password = Console.ReadLine();

                ValidateInputString(user.Password, "password");
            }

            if (role == "employee")
            {
                while (string.IsNullOrWhiteSpace(user.EmployeeCode))
                {
                    Console.Write("Enter your Employee Code: ");
                    user.EmployeeCode = Console.ReadLine();

                    ValidateInputString(user.EmployeeCode, "employee code");
                }
            }

            string userJson = JsonSerializer.Serialize(user);
            return $"register#{userJson}";
        }

        private string BuildAddItemRequest()
        {
            FoodItemDTO item = new FoodItemDTO();

            while (string.IsNullOrWhiteSpace(item.ItemName))
            {
                Console.Write("Enter the Food item name: ");
                item.ItemName = Console.ReadLine();

                ValidateInputString(item.ItemName, "item name");
            }

            while (true)
            {
                Console.Write("Enter the price for it: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal price) && price > 0)
                {
                    item.Price = price;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid price. Please enter a valid price.");
                }
            }

            while (item.FoodCategory == FoodCategory.None)
            {
                Console.Write("Enter the category for it [Breakfast, Lunch, Dinner, Beverage]: ");
                item.FoodCategory = ParseFoodCategory(Console.ReadLine());
            }

            Console.Write("Is the item available? [Y/N]: ");
            item.IsAvailable = Console.ReadLine().ToLower() == "y" ? true : false;

            Console.WriteLine("(Leave blank for the below if you dont have preference)");
            Console.Write("Enter the cuisine for it [NorthIndian, SouthIndian, Chinese, Other]: ");
            string cuisineInput = Console.ReadLine();
            item.Cuisine = string.IsNullOrWhiteSpace(cuisineInput) ? Cuisine.NoPreference : (Cuisine)Enum.Parse(typeof(Cuisine), cuisineInput.Trim(), true);
            
            Console.WriteLine("(Leave blank for the below if you dont have preference)");
            Console.Write("Enter food diet for it[Veg, Non-Veg, Egg]: ");
            string dietInput = Console.ReadLine();
            item.FoodDiet = string.IsNullOrWhiteSpace(dietInput) ? FoodDiet.NoPreference : SetFoodDiet(dietInput.Trim());

            item.SpiceLevel = GetEnumInput<SpiceLevel>("Enter spice level for it [Low, Medium, High]: ");

            string menuJson = JsonSerializer.Serialize(item);
            return $"additem#{menuJson}";
        }

        private string BuildGetFoodItemsRequest()
        {
            return "getitems";
        }

        private string BuildUpdateItemRequest()
        {
            string oldItemName = null;

            while(string.IsNullOrWhiteSpace(oldItemName))
            {
                Console.Write("Enter the name of the item you want to update: ");
                oldItemName = Console.ReadLine();

                ValidateInputString(oldItemName, "item name");
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
                        while (string.IsNullOrWhiteSpace(item.ItemName))
                        {
                            Console.Write("Enter the new Food item name: ");
                            item.ItemName = Console.ReadLine();

                            ValidateInputString(item.ItemName, "item name");
                        }
                        break;

                    case "price":
                        while (true)
                        {
                            Console.Write("Enter the price for it: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal price) && price > 0)
                            {
                                item.Price = price;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid price. Please enter a valid positive number.");
                            }
                        }
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

        private string BuildDeleteItemRequest()
        {
            string itemName = null;

            while (string.IsNullOrWhiteSpace(itemName))
            {
                Console.Write("Enter the name of the item you want to delete: ");
                itemName = Console.ReadLine();
            }

            ValidateInputString(itemName, "item name");

            return $"deleteitem#{itemName}";
        }

        private string BuildFoodReportRequestRequest()
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
            while (year > DateTime.Now.Year)
            {
                Console.WriteLine("Invalid year. Please enter a year which is less than or equal to current year");
                year = Convert.ToInt32(Console.ReadLine());
            }

            return $"getfoodreport#{month}#{year}";
        }

        private string BuildAddFeedbackRequest()
        {
            FeedbackDTO feedbackDTO = new FeedbackDTO();
            feedbackDTO.UserId = UserData.UserId;
            feedbackDTO.FeedbackDate = DateTime.Now;
            int rating = 0;

            while (string.IsNullOrWhiteSpace(feedbackDTO.ItemName))
            {
                Console.Write("Enter the menu item Name for which you want to add feedback: ");
                feedbackDTO.ItemName = Console.ReadLine();

                ValidateInputString(feedbackDTO.ItemName, "item name");
            }

            Console.Write("Enter your feedback Comment: ");
            feedbackDTO.Comment = Console.ReadLine();

            Console.Write("Enter your rating for the item[1-5]: ");
            //feedbackDTO.Rating = Convert.ToInt32(Console.ReadLine());
            //while (feedbackDTO.Rating < 1 || feedbackDTO.Rating > 5)
            //{
            //    Console.WriteLine("Invalid rating. Please enter a rating between 1 and 5");
            //    feedbackDTO.Rating = Convert.ToInt32(Console.ReadLine());
            //}
            while (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
            {
                Console.WriteLine("Invalid rating. Please enter a rating between 1 and 5:");
            }
            feedbackDTO.Rating = rating;

            string feedbackJson = JsonSerializer.Serialize(feedbackDTO);
            return $"addfeedback#{feedbackJson}";
        }

        private string BuildGetFeedbacksRequest()
        {
            string itemName = null;

            while (string.IsNullOrWhiteSpace(itemName))
            {
                Console.Write("Enter the menu item Name for which you want to see feedbacks: ");
                itemName = Console.ReadLine();

                ValidateInputString(itemName, "item name");
            }

            return $"getfeedbacks#{itemName}";
        }

        private string BuildAddRecommendationRequest()
        {
            List<RecommendedMenuDTO> recommendations = new List<RecommendedMenuDTO>();

            while (true)
            {
                string category;
                int numberOfItems;

                do
                {
                    Console.WriteLine("Enter the food category for which you want to recommend items [Breakfast, Lunch, Dinner, Beverage]: ");
                    category = Console.ReadLine()?.Trim();
                }
                while (category.ToLower() != "breakfast" && category.ToLower() != "lunch" && category.ToLower() != "dinner" && category.ToLower() != "beverage");

                do
                {
                    Console.Write("Enter the number of items to be recommended: ");
                }
                while (!int.TryParse(Console.ReadLine(), out numberOfItems) || numberOfItems <= 0);

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

                        ValidateInputString(recommendationDTO.ItemName, "item name");
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

        private string BuildGetRecommendationForDateRequest()
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

        private string BuildUpdateRecommendationRequest()
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

                ValidateInputString(recommendedMenu.OldItemName, "item name");
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
                        while (string.IsNullOrWhiteSpace(recommendedMenu.ItemName))
                        {
                            Console.Write("Enter the new Food item name: ");
                            recommendedMenu.ItemName = Console.ReadLine()?.Trim();

                            ValidateInputString(recommendedMenu.ItemName, "item name");
                        }
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

        private string BuildAddOrderRequest()
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
                        Console.WriteLine("Invalid number of items. Please enter a valid number.");
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

        private string BuildGetOrdersRequest() 
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

        private string BuildGetDiscardMenuRequest()
        {
            return "getdiscardmenulist";
        }

        private string BuildDiscardMenuRequest()
        {
            string itemName = null;

            while (string.IsNullOrWhiteSpace(itemName))
            {
                Console.Write("Enter the name of the item you want to remove from the menu list: ");
                itemName = Console.ReadLine();

                ValidateInputString(itemName, "item name");
            }
   
            return $"discardmenu#{itemName}";
        }

        private string BuildGetDetailedFeedbackRequest()
        {
            string itemName = null;

            while(true)
            {
                Console.WriteLine("1. Enter '1' to notify the employees to provide feedback");
                Console.WriteLine("2. Enter '2' to Get detailed feedback.");
                string option = Console.ReadLine().Trim();

                while(string.IsNullOrWhiteSpace(itemName))
                {
                    Console.Write("Enter the name of the item: ");
                    itemName = Console.ReadLine().Trim();

                    ValidateInputString(itemName, "item name");
                }
                

                if (option == "1")
                {
                    return $"notifyemployees#{itemName}";
                }
                else if (option == "2")
                {
                    return $"getdetailedfeedback#{itemName}";
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private string BuildProvideDetailedFeedbackRequest()
        {
            FeedbackDTO feedbackDTO = new FeedbackDTO();
            feedbackDTO.UserId = UserData.UserId;
            feedbackDTO.FeedbackDate = DateTime.Now;
            feedbackDTO.Rating = null;
            feedbackDTO.Comment = "detailedfb";
            string comment = null;

            while (string.IsNullOrWhiteSpace(feedbackDTO.ItemName))
            {
                Console.Write("Enter the menu item Name for which you want to add feedback: ");
                feedbackDTO.ItemName = Console.ReadLine();

                ValidateInputString(feedbackDTO.ItemName, "item name");
            }
            
            while (string.IsNullOrWhiteSpace(comment))
            {
                Console.WriteLine($"What didn’t you like about {feedbackDTO.ItemName}?: ");
                comment += "  1." + Console.ReadLine();

                Console.WriteLine($"How would you like {feedbackDTO.ItemName} to taste? ");
                comment += "  2." + Console.ReadLine();

                Console.WriteLine($"What would you like to be improved in the dish? ");
                comment += "  3." + Console.ReadLine();

                Console.WriteLine($"Share your mom’s recipe");
                comment += "  4." + Console.ReadLine();
            }

            feedbackDTO.Comment += comment;

            string feedbackJson = JsonSerializer.Serialize(feedbackDTO);
            return $"providedetailedfeedback#{feedbackJson}";   
        }

        private string BuildUpdateProfileRequest()
        {
            EmployeeProfileDTO profile = new EmployeeProfileDTO
            {
                UserId = UserData.UserId,
                Cuisine = GetEnumInput<Cuisine>("What is your most preferred cuisine among these? [NorthIndian, SouthIndian, Chinese, Other, NoPreference]: "),
                FoodDiet = GetEnumInput<FoodDiet>("What is your preferred Diet? [Vegetarian, NonVegetarian, Eggetarian, NoPreference]: "),
                SpiceLevel = GetEnumInput<SpiceLevel>("What is your preferred Spice Level? [Low, Medium, High]: ")
            };

            string profileJson = JsonSerializer.Serialize(profile);
            return $"updateprofile#{profileJson}";
        }

        private FoodCategory ParseFoodCategory(string input)
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

        private T GetEnumInput<T>(string prompt) where T : struct, Enum
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine().Trim();
                if (Enum.TryParse(input, true, out T result) && Enum.IsDefined(typeof(T), result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
        }

        private FoodDiet SetFoodDiet(string input)
        {
            FoodDiet foodDiet;

            switch(input.ToLower())
            {
                case "non":
                    foodDiet = FoodDiet.NonVegetarian;
                    break;
                case "veg":
                    foodDiet = FoodDiet.Vegetarian;
                    break;
                case "egg":
                    foodDiet = FoodDiet.Eggetarian;
                    break;  
                default:
                    foodDiet = FoodDiet.NoPreference;
                    break;
            }

            return foodDiet;
        }

        private void ValidateInputString(string input, string displayParam)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"Please enter valid {displayParam}.");
            }
        }
    }
}