using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
