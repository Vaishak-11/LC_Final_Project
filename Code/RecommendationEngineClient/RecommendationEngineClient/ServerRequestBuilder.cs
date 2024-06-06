using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                case "addmenu":
                    return BuildAddMenuRequest();
                case "recommend":
                    return BuildRecommendRequest();
                case "feedback":
                    return BuildFeedbackRequest();
                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private static string BuildLoginRequest()
        {
            Console.WriteLine("Enter UserId:");
            string userId = Console.ReadLine();
            Console.WriteLine("Enter Name:");
            string name = Console.ReadLine();
            return $"login {userId} {name}";
        }

        private static string BuildAddMenuRequest()
        {
            Console.WriteLine("Enter Menu Name:");
            string menuName = Console.ReadLine();
            Console.WriteLine("Enter Menu Price:");
            string menuPrice = Console.ReadLine();
            Console.WriteLine("Enter Availability Status (Available/Not Available):");
            string availabilityStatus = Console.ReadLine();
            return $"addmenu {menuName} {menuPrice} {availabilityStatus}";
        }

        private static string BuildRecommendRequest()
        {
            Console.WriteLine("Enter Menu ID to recommend:");
            string menuId = Console.ReadLine();
            return $"recommend {menuId}";
        }

        private static string BuildFeedbackRequest()
        {
            Console.WriteLine("Enter User ID:");
            string userId = Console.ReadLine();
            Console.WriteLine("Enter Menu ID:");
            string menuId = Console.ReadLine();
            Console.WriteLine("Enter Comment:");
            string comment = Console.ReadLine();
            Console.WriteLine("Enter Rating:");
            string rating = Console.ReadLine();
            return $"feedback {userId} {menuId} {comment} {rating}";
        }
    }
}
