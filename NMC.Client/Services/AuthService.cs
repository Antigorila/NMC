using NMC.Client.DTO;
using NMC.Client.Mapping;
using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class AuthService : ApiServiceBase
    {
        public AuthService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResult<UserSessionModel>> LoginAsync(string username, string password)
        {
            var request = new LoginRequestDto(username, password);
            ApiResult<AuthResponseDto> result = await PostAsync<LoginRequestDto, AuthResponseDto>("api/auth/login", request);

            return ToSessionResult(result);
        }

        public async Task<ApiResult<UserSessionModel>> RegisterAsync(string username, string password)
        {
            var request = new RegisterRequestDto(username, password);
            ApiResult<AuthResponseDto> result = await PostAsync<RegisterRequestDto, AuthResponseDto>("api/auth/register", request);

            return ToSessionResult(result);
        }

        public async Task<ApiResult<UserSessionModel>> UpdateUsernameAsync(int userId, string newUsername)
        {
            var request = new UpdateProfileRequestDto(newUsername);
            ApiResult<AuthResponseDto> result = await PutAsync<UpdateProfileRequestDto, AuthResponseDto>($"api/auth/{userId}/username", request);

            return ToSessionResult(result);
        }

        public async Task<ApiResult> ChangePasswordAsync(int userId, string newPassword)
        {
            var request = new ChangePasswordRequestDto(newPassword);
            return await PutAsync($"api/auth/{userId}/password", request);
        }

        public async Task<ApiResult> DeleteAccountAsync(int userId)
        {
            return await DeleteAsync($"api/auth/{userId}");
        }

        #region Support Methods
        private static ApiResult<UserSessionModel> ToSessionResult(ApiResult<AuthResponseDto> result)
        {
            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<UserSessionModel>.Failure(result.ErrorMessage);
            }

            return ApiResult<UserSessionModel>.Success(result.Data.ToModel());
        }
        #endregion
    }
}
