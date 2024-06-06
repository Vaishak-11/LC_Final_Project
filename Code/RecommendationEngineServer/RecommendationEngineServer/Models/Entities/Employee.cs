﻿namespace RecommendationEngineServer.Models.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }

        public int UserId { get; set; } 

        public User User { get; set; }
    }
}
