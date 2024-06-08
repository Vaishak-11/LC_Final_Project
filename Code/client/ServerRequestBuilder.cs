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
                default:
                    Console.WriteLine("Unknown command.");
                    return null;
            }
        }

        private static string BuildLoginRequest()
        {
            Console.WriteLine("Which role would you like to log in as?");
            Console.WriteLine("1. Employee\n2. Admin\n3. Chef\n");
            Console.Write("Enter your role - ");
            string role = Console.ReadLine();

            if (string.IsNullOrEmpty(role) || string.IsNullOrWhiteSpace(role))
            {
                throw new Exception("Invalid role");
            }

            string userName;
            string password;

            switch (role.ToLower())
            {
                case "1":
                case "employee":
                    Console.WriteLine("Enter your Employee Code:");
                    userName = Console.ReadLine();
                    Console.WriteLine("Enter your Password:");
                    password = Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Enter your Name:");
                    userName = Console.ReadLine();
                    Console.WriteLine("Enter your Password:");
                    password = Console.ReadLine();
                    break;
            }

            return $"login {userName} {password} {role}";
        }
    }
}