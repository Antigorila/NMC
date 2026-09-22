using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Services
{
    public class SessionService
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";

        public async Task SaveSessionAsync(string token, string userId)
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
            await SecureStorage.Default.SetAsync(UserIdKey, userId);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public void ClearSession()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(UserIdKey);
        }

        public async Task<string?> GetUserIdAsync()
        {
            return await SecureStorage.Default.GetAsync(UserIdKey);
        }
    }
}
