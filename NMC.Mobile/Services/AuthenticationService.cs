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

        public async Task<AuthResponse?> UpdateUsernameAsync(int userId, string newUsername)
        {
            try
            {
                var request = new { NewUsername = newUsername };
                var response = await _httpClient.PutAsJsonAsync($"api/auth/{userId}/username", request);

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

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            try
            {
                var request = new { NewPassword = newPassword };
                var response = await _httpClient.PutAsJsonAsync($"api/auth/{userId}/password", request);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/auth/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
