using NMC.Client.DTO;
using NMC.Client.Mapping;
using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class MetadataService : ApiServiceBase
    {
        public MetadataService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResult<List<CategoryModel>>> GetGenresAsync()
        {
            ApiResult<List<GenreScoreDto>> result = await GetAsync<List<GenreScoreDto>>("api/metadata/genres");

            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<List<CategoryModel>>.Failure(result.ErrorMessage);
            }

            var genres = new List<CategoryModel>();
            foreach (GenreScoreDto dto in result.Data)
            {
                genres.Add(dto.ToCategory());
            }

            return ApiResult<List<CategoryModel>>.Success(genres);
        }

        public async Task<ApiResult<List<CategoryModel>>> GetAcousticAttributesAsync()
        {
            ApiResult<List<AcousticScoreDto>> result = await GetAsync<List<AcousticScoreDto>>("api/metadata/acoustic-attributes");

            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<List<CategoryModel>>.Failure(result.ErrorMessage);
            }

            var attributes = new List<CategoryModel>();
            foreach (AcousticScoreDto dto in result.Data)
            {
                attributes.Add(dto.ToCategory());
            }

            return ApiResult<List<CategoryModel>>.Success(attributes);
        }
    }
}
