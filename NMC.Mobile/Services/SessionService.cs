using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.Services
{
    public class SessionService
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";
        private const string UsernameKey = "username";

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }

        public async Task<string?> GetUserIdAsync()
        {
            return await SecureStorage.Default.GetAsync(UserIdKey);
        }

        public async Task<string?> GetUsernameAsync()
        {
            return await SecureStorage.Default.GetAsync(UsernameKey);
        }

        public async Task SaveSessionAsync(string token, string userId, string username = "")
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
            await SecureStorage.Default.SetAsync(UserIdKey, userId);
            if (!string.IsNullOrEmpty(username))
            {
                await SecureStorage.Default.SetAsync(UsernameKey, username);
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public void ClearSession()
        {
            try
            {
                SecureStorage.Default.Remove(TokenKey);
                SecureStorage.Default.Remove(UserIdKey);
                SecureStorage.Default.Remove(UsernameKey);
            }
            catch
            {

            }
        }
    }
}
