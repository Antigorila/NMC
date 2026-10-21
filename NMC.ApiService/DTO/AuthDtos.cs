namespace NMC.ApiService.DTO
{
    public record RegisterRequestDto(string Username, string Password);
    public record LoginRequestDto(string Username, string Password);
    public record AuthResponseDto(int UserId, string Username, string Token);
    public record UpdateProfileRequestDto(string NewUsername);
    public record ChangePasswordRequestDto(string NewPassword);
}
