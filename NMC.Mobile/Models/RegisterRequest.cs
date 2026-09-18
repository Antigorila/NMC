using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public RegisterRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
