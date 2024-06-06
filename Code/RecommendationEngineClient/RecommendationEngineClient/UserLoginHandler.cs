using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngineClient
{
    public class UserLoginHandler
    {
        private static string? userRole;

        public void HandleUserLogin()
        {
            Console.WriteLine("Which role would you like to log in as?");
            Console.WriteLine("1. Employee\n2. Admin\n3. Chef \n");
            Console.Write("Enter your role - ");
            string role = Console.ReadLine();
            

            if (userRole == null) 
                throw new ArgumentNullException(nameof(userRole));

            if(userRole.ToLower() == "1" || userRole.ToLower() == "employee")
            {
                Console.WriteLine("Enter your Employee Code:");
                string employeeId = Console.ReadLine();
                Console.WriteLine("Enter your Name:");
                string employeeName = Console.ReadLine();
                Console.WriteLine(Login(employeeId, employeeName));
            }
            else
            {
                Console.WriteLine("Enter your Name:");
                string userName = Console.ReadLine();
                Console.WriteLine("Enter your Password:");
                string password = Console.ReadLine();
                Console.WriteLine(Login(userName, password));
            }
        }
        
        public static string Login(string userName, string password)
        {
            return $"login {userName} {password}";
        }
    }
}
