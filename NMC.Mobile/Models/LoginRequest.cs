using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
