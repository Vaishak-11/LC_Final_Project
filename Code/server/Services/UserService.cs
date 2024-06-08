using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;

namespace RecommendationEngineServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public UserService(IUserRepository userRepository, IEmployeeRepository employeeRepository)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<ServerResponse> Login(string name, string password, Role role)
        {
            ServerResponse response = new ServerResponse();

            if (role != null)
            {
                if (role.RoleName == "Employee")
                {
                    Employee employee = await _employeeRepository.GetByEmployeeCode(name);
                    response.Name = "Login";
                    response.Value = (employee != null && employee.EmployeeCode == name && employee.User.Password == password) ? "Login successful." : "Login failed.";
                }
                else
                {
                    User user = await _userRepository.GetUserByName(name);
                    response.Name = "Login";
                    response.Value = user != null ? "Login successful." : "Login failed. Invalid Credentials";
                }
            }
            else
            {
                response.Name = "Error";
                response.Value = "Invalid role.";
            }

            return response;
        }
    }
}
