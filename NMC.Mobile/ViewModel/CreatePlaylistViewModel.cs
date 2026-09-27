using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NMC.Mobile.Helper;
using NMC.Mobile.Models;
using NMC.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMC.Mobile.ViewModel
{
    public partial class CreatePlaylistViewModel : ObservableObject
    {
        private readonly PlaylistService _playlistService;
        private readonly SessionService _sessionService;
        private readonly MediaService _mediaService;

        [ObservableProperty]
        private string _playlistName = string.Empty;

        [ObservableProperty]
        private string? _coverPath;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public CreatePlaylistViewModel(PlaylistService playlistService, SessionService sessionService, MediaService mediaService)
        {
            _playlistService = playlistService;
            _sessionService = sessionService;
            _mediaService = mediaService;
        }

        [RelayCommand]
        private async Task SelectCoverAsync()
        {
            var path = await _mediaService.PickCoverImageAsync();
            if (!string.IsNullOrEmpty(path))
            {
                CoverPath = path;
            }
        }

        [RelayCommand]
        private async Task CreatePlaylistAsync()
        {
            ErrorMessage = string.Empty;

            var trimmedName = PlaylistName.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                ErrorMessage = "And how should we call the playlist?";
                return;
            }

            IsBusy = true;

            try
            {
                var userIdStr = await _sessionService.GetUserIdAsync();
                if (!int.TryParse(userIdStr, out int userId))
                {
                    ErrorMessage = "Invalid user session!";
                    return;
                }

                var request = new CreatePlaylistRequest(trimmedName, CoverPath);
                var (success, serverError) = await _playlistService.CreatePlaylistAsync(userId, request);

                if (success)
                {
                    var newPlaylist = new PlaylistModel
                    {
                        PlaylistName = trimmedName,
                        CoverPath = CoverPath,
                        UserId = userId,
                        TrackCount = 0
                    };

                    WeakReferenceMessenger.Default.Send(new PlaylistCreatedMessage(newPlaylist));

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = serverError;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}