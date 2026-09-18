using NMC.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NMC.Mobile.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest(username, password);
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponse>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                var request = new RegisterRequest(username, password);
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
