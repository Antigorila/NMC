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

        public async Task<ApiResult> DeleteTrackAsync(int userTrackId)
        {
            return await DeleteAsync($"api/tracks/{userTrackId}");
        }

        #region Support Methods
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
