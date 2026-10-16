using NMC.Client.DTO;
using NMC.Client.Mapping;
using NMC.Client.Models;

namespace NMC.Client.Services
{
    public class PlaylistService : ApiServiceBase
    {
        public PlaylistService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ApiResult<List<PlaylistModel>>> GetPlaylistsAsync(int userId)
        {
            ApiResult<List<PlaylistDto>> result = await GetAsync<List<PlaylistDto>>($"api/playlists?userId={userId}");

            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<List<PlaylistModel>>.Failure(result.ErrorMessage);
            }

            var playlists = new List<PlaylistModel>();
            foreach (PlaylistDto dto in result.Data)
            {
                playlists.Add(dto.ToModel());
            }

            return ApiResult<List<PlaylistModel>>.Success(playlists);
        }

        public async Task<ApiResult<PlaylistModel>> GetPlaylistAsync(int playlistId)
        {
            ApiResult<PlaylistDetailDto> result = await GetAsync<PlaylistDetailDto>($"api/playlists/{playlistId}");

            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<PlaylistModel>.Failure(result.ErrorMessage);
            }

            return ApiResult<PlaylistModel>.Success(result.Data.ToModel());
        }

        public async Task<ApiResult<PlaylistModel>> CreatePlaylistAsync(int userId, string name, string? coverPath)
        {
            var request = new CreatePlaylistDto(name, coverPath);
            ApiResult<PlaylistDto> result = await PostAsync<CreatePlaylistDto, PlaylistDto>($"api/playlists?userId={userId}", request);

            return ToPlaylistResult(result);
        }

        public async Task<ApiResult<PlaylistModel>> UpdatePlaylistAsync(int playlistId, string name, string? coverPath)
        {
            var request = new UpdatePlaylistDto(name, coverPath);
            ApiResult<PlaylistDto> result = await PutAsync<UpdatePlaylistDto, PlaylistDto>($"api/playlists/{playlistId}", request);

            return ToPlaylistResult(result);
        }

        public async Task<ApiResult> DeletePlaylistAsync(int playlistId)
        {
            return await DeleteAsync($"api/playlists/{playlistId}");
        }

        public async Task<ApiResult> AddTrackAsync(int playlistId, int userTrackId)
        {
            var request = new AddTrackToPlaylistDto(userTrackId);
            return await PostAsync($"api/playlists/{playlistId}/tracks", request);
        }

        public async Task<ApiResult> RemoveTrackAsync(int playlistId, int userTrackId)
        {
            return await DeleteAsync($"api/playlists/{playlistId}/tracks/{userTrackId}");
        }

        #region Support Methods
        private static ApiResult<PlaylistModel> ToPlaylistResult(ApiResult<PlaylistDto> result)
        {
            if (!result.IsSuccess || result.Data is null)
            {
                return ApiResult<PlaylistModel>.Failure(result.ErrorMessage);
            }

            return ApiResult<PlaylistModel>.Success(result.Data.ToModel());
        }
        #endregion
    }
}
