﻿using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Models
{    
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserView
    {
        public int Id { get; set; }
        public string Username { get; set; }        
        public UserRole Role { get; set; }
    }
}
