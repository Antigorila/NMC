using NMC.Client.DTO;
using NMC.Client.Mapping;
using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class RecommendationService : ApiServiceBase
    {
        public RecommendationService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResult<List<TrackModel>>> GenerateAsync(int userId, int referencePlaylistId)
        {
            var request = new GenerateRecommendationDto(referencePlaylistId);
            ApiResult<List<UserTrackDto>> result = await PostAsync<GenerateRecommendationDto, List<UserTrackDto>>($"api/recommendations/generate?userId={userId}", request);

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
    }
}
