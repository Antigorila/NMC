using NMC.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NMC.Mobile.Services
{
    public class TrackService
    {
        private readonly HttpClient _httpClient;

        public TrackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UploadTrackAsync(CreateUserTrackRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/tracks/upload", request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrWhiteSpace(errorContent) ? "Failed to upload track to the server." : errorContent);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}