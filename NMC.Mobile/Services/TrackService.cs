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

        public async Task<bool> UploadTrackAsync(CreateUserTrackRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/tracks/upload", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}