using System.Net.Http.Headers;
using NMC.Client.DTO;
using NMC.Client.Mapping;
using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class TrackService : ApiServiceBase
    {
        public TrackService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResult<List<TrackModel>>> GetTracksAsync(int userId)
        {
            ApiResult<List<UserTrackDto>> result = await GetAsync<List<UserTrackDto>>($"api/tracks?userId={userId}");
            return ToTrackListResult(result);
        }

        public async Task<ApiResult<List<TrackModel>>> FilterTracksAsync(int userId, int genreId, bool isHardFilter, double threshold)
        {
            var request = new TrackFilterDto(genreId, isHardFilter, threshold);
            ApiResult<List<UserTrackDto>> result = await PostAsync<TrackFilterDto, List<UserTrackDto>>($"api/tracks/filter?userId={userId}", request);

            return ToTrackListResult(result);
        }

        public async Task<ApiResult<TrackModel>> UploadTrackAsync(int userId, string title, string artist, string filePath, string? coverPath)
        {
            ApiResult<TrackCheckDto> check = await CheckTrackAsync(title, artist);

            if (!check.IsSuccess || check.Data is null)
            {
                return ApiResult<TrackModel>.Failure(check.ErrorMessage);
            }

            bool sendAudioFile = !check.Data.Exists;

            try
            {
                using MultipartFormDataContent content = CreateUploadContent(userId, title, artist, filePath, coverPath, sendAudioFile);
                ApiResult<UserTrackDto> result = await PostMultipartAsync<UserTrackDto>("api/analysis/upload", content);

                if (!result.IsSuccess || result.Data is null)
                {
                    return ApiResult<TrackModel>.Failure(result.ErrorMessage);
                }

                return ApiResult<TrackModel>.Success(result.Data.ToModel());
            }
            catch (Exception ex)
            {
                return ApiResult<TrackModel>.Failure($"The audio file could not be read: {ex.Message}");
            }
        }

        public async Task<ApiResult> UpdateCoverAsync(int userTrackId, string coverPath)
        {
            var request = new UpdateCoverDto(coverPath);
            return await PutAsync($"api/tracks/{userTrackId}/cover", request);
        }

        public async Task<ApiResult> DeleteTrackAsync(int userTrackId)
        {
            return await DeleteAsync($"api/tracks/{userTrackId}");
        }

        #region Support Methods
        private async Task<ApiResult<TrackCheckDto>> CheckTrackAsync(string title, string artist)
        {
            string url = $"api/analysis/check?title={Uri.EscapeDataString(title)}&artist={Uri.EscapeDataString(artist)}";
            return await GetAsync<TrackCheckDto>(url);
        }

        private static MultipartFormDataContent CreateUploadContent(int userId, string title, string artist, string filePath, string? coverPath, bool sendAudioFile)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(userId.ToString()), "UserId");
            content.Add(new StringContent(title), "Title");
            content.Add(new StringContent(artist), "Artist");
            content.Add(new StringContent(filePath), "FilePath");

            if (!string.IsNullOrWhiteSpace(coverPath))
            {
                content.Add(new StringContent(coverPath), "CoverPath");
            }

            if (sendAudioFile)
            {
                var fileContent = new StreamContent(File.OpenRead(filePath));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                content.Add(fileContent, "File", Path.GetFileName(filePath));
            }

            return content;
        }

        private static ApiResult<List<TrackModel>> ToTrackListResult(ApiResult<List<UserTrackDto>> result)
        {
            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<List<TrackModel>>.Failure(result.ErrorMessage);
            }

            var tracks = new List<TrackModel>();
            foreach (UserTrackDto dto in result.Data)
            {
                tracks.Add(dto.ToModel());
            }

            return ApiResult<List<TrackModel>>.Success(tracks);
        }
        #endregion
    }
}
