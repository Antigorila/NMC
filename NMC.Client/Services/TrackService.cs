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

        public async Task<ApiResult> DeleteTrackAsync(int userTrackId)
        {
            return await DeleteAsync($"api/tracks/{userTrackId}");
        }
    }
}
