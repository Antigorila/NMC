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

        public async Task<List<PlaylistModel>> GetPlaylistsAsync(int userId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<PlaylistModel>>($"api/playlists?userId={userId}");
                return result ?? new List<PlaylistModel>();
            }
            catch (Exception)
            {
                return new List<PlaylistModel>();
            }
        }

        public async Task<PlaylistDetailModel?> GetPlaylistDetailAsync(int playlistId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PlaylistDetailModel>($"api/playlists/{playlistId}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeletePlaylistAsync(int playlistId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/playlists/{playlistId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveTrackFromPlaylistAsync(int playlistId, int userTrackId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/playlists/{playlistId}/tracks/{userTrackId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}