using System.Net.Http.Json;
using System.Text.Json;

namespace NMC.Client.Services
{
    public abstract class ApiServiceBase
    {
        protected readonly HttpClient _httpClient;

        protected ApiServiceBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Requests with response data
        protected async Task<ApiResult<T>> GetAsync<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                return await ReadResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Failure(GetExceptionMessage(ex));
            }
        }

        protected async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, body);
                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Failure(GetExceptionMessage(ex));
            }
        }

        protected async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest body)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync(url, body);
                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Failure(GetExceptionMessage(ex));
            }
        }
        #endregion

        #region Requests without response data
        protected async Task<ApiResult> PostAsync<TRequest>(string url, TRequest body)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, body);
                return await ReadStatusAsync(response);
            }
            catch (Exception ex)
            {
                return ApiResult.Failure(GetExceptionMessage(ex));
            }
        }

        protected async Task<ApiResult> PutAsync<TRequest>(string url, TRequest body)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync(url, body);
                return await ReadStatusAsync(response);
            }
            catch (Exception ex)
            {
                return ApiResult.Failure(GetExceptionMessage(ex));
            }
        }

        protected async Task<ApiResult> DeleteAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);
                return await ReadStatusAsync(response);
            }
            catch (Exception ex)
            {
                return ApiResult.Failure(GetExceptionMessage(ex));
            }
        }
        #endregion

        #region Support Methods
        private static async Task<ApiResult<T>> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = await ReadErrorAsync(response);
                return ApiResult<T>.Failure(error);
            }

            T? data = await response.Content.ReadFromJsonAsync<T>();

            if (data is null)
            {
                return ApiResult<T>.Failure("The server sent an empty response.");
            }

            return ApiResult<T>.Success(data);
        }

        private static async Task<ApiResult> ReadStatusAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = await ReadErrorAsync(response);
                return ApiResult.Failure(error);
            }

            return ApiResult.Success();
        }

        private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            string error = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(error))
            {
                return $"Request failed ({(int)response.StatusCode}).";
            }

            return error.Trim('"');
        }

        private static string GetExceptionMessage(Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                return "The server did not respond in time.";
            }

            if (ex is HttpRequestException)
            {
                return "Cannot reach the server. Is the API running?";
            }

            if (ex is JsonException)
            {
                return "The server sent an unexpected response.";
            }

            return ex.Message;
        }
        #endregion
    }
}
