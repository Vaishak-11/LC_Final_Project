using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Services.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace RecommendationEngineServer.Services

{
    public class RequestHandlerService : IRequestHandlerService
    {
        private readonly IUserService _userService;
        private readonly IFoodItemService _foodItemService;
        private readonly IRecommendedMenuService _recommendedMenuService;
        private readonly IFeedbackService _feedbackService;
        private readonly IOrderService _orderService;

        public RequestHandlerService(IUserService userService, IFoodItemService foodItemService, IRecommendedMenuService recommendedMenuService, IFeedbackService feedbackService, IOrderService orderService)
        {
            _userService = userService;
            _foodItemService = foodItemService;
            _recommendedMenuService = recommendedMenuService;
            _feedbackService = feedbackService;
            _orderService = orderService;
        }

        public async Task<ServerResponse> ProcessRequest(string request)
        {
            string[] parts = request.Split('#');
            string command = parts[0];

            switch (command.ToLower())
            {
                case "login":
                   return await HandleLoginRequest(request, parts);
                case "register":
                    return await HandleRegisterRequest(request, parts);
                case "additem":
                    return await HandleAddMenuRequest(request, parts);
                case "getitems":
                    return await HandleGetMenuRequest(request, parts);
                case "updateitem":
                    return await HandleUpdateMenuRequest(request, parts);   
                case "deleteitem":
                    return await HandleDeleteMenuRequest(request, parts);
                case "addfeedback":
                    return await HandleAddFeedbackRequest(request, parts);
                case "getfeedbacks":
                    return await HandleGetFeedbackRequest(request, parts);
                case "addrecommendations":
                    return await HandleAddRecommendationRequest(request, parts);
                case "getrecommendations":
                    return await HandleGetRecommendationRequest(request, parts);
                case "updaterecommendation":
                    return await HandleUpdateRecommendationRequest(request, parts);
                case "addorder":
                    return await HandleAddOrderRequest(request, parts);
                case "getorders":
                    return await HandleGetOrdersRequest(request, parts);
                case "getfoodreport":
                    return await HandleGetReportRequest(request, parts);
                default:
                    return new ServerResponse { Name = "Error", Value = "Unknown command." };
            }
        }

        private async Task<ServerResponse> HandleLoginRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid login command. Usage: login <userDetails>" };
            }

            UserLoginDTO user = JsonSerializer.Deserialize<UserLoginDTO>(parts[1]);

            Console.WriteLine($"Processing login command: username={user.UserName}, role={user.Role}");
            return await _userService.Login(user);
        }

        private async Task<ServerResponse> HandleRegisterRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid register command. Usage: login <username> <password> <role> <empCode>" };
            }

            UserLoginDTO user = JsonSerializer.Deserialize<UserLoginDTO>(parts[1]);

            Console.WriteLine($"Processing register command: username={user.UserName}, role={user.Role}");
            return await _userService.RegisterUser(user);
        }

        private async Task<ServerResponse> HandleAddMenuRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid Addmenu command. Usage: Addmenu <menu details>" };
            }

            string menu = parts[1];

            try
            {
                FoodItemDTO menuDTO = JsonSerializer.Deserialize<FoodItemDTO>(menu);
                Console.WriteLine($"Processing add menu command: ItemName={menuDTO.ItemName}, Price={menuDTO.Price}, Category={menuDTO.FoodCategory}, IsAvailable={menuDTO.IsAvailable}");

                return await _foodItemService.Add(menuDTO);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid menu details format" };
            }
        }

        private async Task<ServerResponse> HandleGetMenuRequest(string request, string[] parts)
        { 
            return await _foodItemService.GetList();
        }

        private async Task<ServerResponse> HandleUpdateMenuRequest(string request, string[] parts)
        {
            if (parts.Length < 4)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid UpdateMenu command. Usage: UpdateMenu <oldItemName> <new menu details> <avalability status>" };
            }

            string oldName = parts[1];
            string menu = parts[2];
            string availability = parts[3];

            try
            {
                FoodItemDTO menuDTO = JsonSerializer.Deserialize<FoodItemDTO>(menu);

                Console.WriteLine($"Processing update menu command: ItemName={menuDTO.ItemName}, Price={menuDTO.Price}, Category={menuDTO.FoodCategory}, IsAvailable={menuDTO.IsAvailable}");

                return await _foodItemService.Update(oldName,menuDTO, availability);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid menu details format" };
            }
        }

        private async Task<ServerResponse> HandleDeleteMenuRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid DeleteMenu command. Usage: DeleteMenu <ItemName>" };
            }

            string itemName = parts[1];
            Console.WriteLine($"Processing delete menu command: ItemName={itemName}");

            return await _foodItemService.Delete(itemName);
        }

        private async Task<ServerResponse> HandleAddFeedbackRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid AddFeedback command. Usage: AddFeedback <feedback details>" };
            }

            string feeedbackObject = parts[1];

            try
            {
                FeedbackDTO feedback = JsonSerializer.Deserialize<FeedbackDTO>(feeedbackObject);
                Console.WriteLine($"Processing add feedback command");

                return await _feedbackService.AddFeedback(feedback);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid feedback info" };
            }
        }

        private async Task<ServerResponse> HandleGetFeedbackRequest(string request, string[] parts)
        {
            string itemName = parts[1];

            return await _feedbackService.GetFeedbacks(itemName);
        }

        private async Task<ServerResponse> HandleAddRecommendationRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid AddRecommendation command. Usage: AddRecommendation <recommendation details>" };
            }

            string recommendationObject = parts[1];

            try
            {
                List<RecommendedMenuDTO> recommendations = JsonSerializer.Deserialize<List<RecommendedMenuDTO>>(recommendationObject);
                Console.WriteLine($"Processing add recommendation command");

                return await _recommendedMenuService.AddRecommendedMenu(recommendations);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid recommendation details format" };
            }
        }

        private async Task<ServerResponse> HandleGetRecommendationRequest(string request, string[] parts)
        {
            string dateString = parts[1].Trim();

            bool isParsed = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime selectedDate);

            if (isParsed)
            {
                DateTime selectedDateTime = selectedDate.Date;

                return await _recommendedMenuService.GetRecommendedMenu(selectedDateTime);
            }
            else
            {
                throw new ArgumentException($"Invalid date format: {dateString}. Expected format: yyyy-MM-dd.");
            }
        }

        private async Task<ServerResponse> HandleUpdateRecommendationRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid updateRecommendation command. Usage: updateRecommendation <itemName> <category>" };
            }

            string recommendationObject = parts[1];

            try
            {
                RecommendedMenuDTO recommendation = JsonSerializer.Deserialize<RecommendedMenuDTO>(recommendationObject);
                Console.WriteLine($"Processing add recommendation command");

                return await _recommendedMenuService.UpdateRecommendedMenu(recommendation);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid recommendation details format" };
            }
        }

        private async Task<ServerResponse> HandleAddOrderRequest(string request, string[] parts)
        {
            if (parts.Length < 2)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid AddOrder command. Usage: AddOrder <order details>" };
            }

            string orderObject = parts[1];

            try
            {
                OrderDTO order = JsonSerializer.Deserialize<OrderDTO>(orderObject);
                Console.WriteLine($"Processing add order command");

                return await _orderService.AddOrder(order);
            }
            catch (JsonException)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid order details format" };
            }
        }

        private async Task<ServerResponse> HandleGetOrdersRequest(string request, string[] parts)
        {
            return await _orderService.GetOrders();
        }

        private async Task<ServerResponse> HandleGetReportRequest(string request, string[] parts)
        {
            if (parts.Length < 3)
            {
                return new ServerResponse { Name = "Error", Value = "Invalid Get Report command. Usage: GetReport <month> <year>" };
            }

            int month = int.Parse(parts[1]);
            int year = int.Parse(parts[2]);

            return await _foodItemService.GetFoodItemWithFeedbackReport(month, year);
        }
    }


}

