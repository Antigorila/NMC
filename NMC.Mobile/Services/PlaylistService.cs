using NMC.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NMC.Mobile.Services
{
    public class PlaylistService
    {
        private readonly HttpClient _httpClient;

        public PlaylistService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CreatePlaylistAsync(int userId, CreatePlaylistRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/playlists?userId={userId}", request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrWhiteSpace(errorContent) ? "Failed to create playlist." : errorContent);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}