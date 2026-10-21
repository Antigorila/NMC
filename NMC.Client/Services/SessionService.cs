using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class SessionService
    {
        private const string UserIdKey = "session_user_id";
        private const string UsernameKey = "session_username";
        private const string TokenKey = "session_token";

        public UserSessionModel? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public int UserId => CurrentUser?.UserId ?? 0;

        public string Username => CurrentUser?.Username ?? string.Empty;

        public async Task SaveAsync(UserSessionModel user)
        {
            CurrentUser = user;

            try
            {
                await SecureStorage.Default.SetAsync(UserIdKey, user.UserId.ToString());
                await SecureStorage.Default.SetAsync(UsernameKey, user.Username);
                await SecureStorage.Default.SetAsync(TokenKey, user.Token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session could not be persisted: {ex.Message}");
            }
        }

        public async Task<bool> TryRestoreAsync()
        {
            try
            {
                string? userIdText = await SecureStorage.Default.GetAsync(UserIdKey);
                string? username = await SecureStorage.Default.GetAsync(UsernameKey);
                string? token = await SecureStorage.Default.GetAsync(TokenKey);

                if (!int.TryParse(userIdText, out int userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                {
                    return false;
                }

                CurrentUser = new UserSessionModel(userId, username, token);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session could not be restored: {ex.Message}");
                Clear();
                return false;
            }
        }

        public void Clear()
        {
            CurrentUser = null;

            try
            {
                SecureStorage.Default.Remove(UserIdKey);
                SecureStorage.Default.Remove(UsernameKey);
                SecureStorage.Default.Remove(TokenKey);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Session could not be cleared: {ex.Message}");
            }
        }
    }
}
