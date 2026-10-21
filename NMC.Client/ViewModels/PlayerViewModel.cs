using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMC.Client.Models;
using NMC.Client.Navigation;
using NMC.Client.Services;

namespace NMC.Client.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private const double RestartThresholdSeconds = 3;

        private readonly AudioPlayerService _audioPlayer;

        private readonly List<TrackModel> _queue = new();
        private readonly List<int> _playOrder = new();
        private int _orderPosition = -1;

        private IDispatcherTimer? _positionTimer;
        private bool _isUpdatingPosition;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTrack))]
        [NotifyPropertyChangedFor(nameof(NowPlayingText))]
        [NotifyPropertyChangedFor(nameof(CoverSource))]
        private TrackModel? currentTrack;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseIcon))]
        private bool isPlaying;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShuffleIcon))]
        private bool isShuffleEnabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PositionText))]
        private double position;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DurationText))]
        [NotifyPropertyChangedFor(nameof(SliderMaximum))]
        private double duration;

        [ObservableProperty]
        private bool isSeeking;

        public bool HasTrack => CurrentTrack != null;

        public string NowPlayingText => CurrentTrack?.DisplayName ?? "Nothing is playing";

        public string PlayPauseIcon => IsPlaying ? "pause.png" : "play.png";

        public string ShuffleIcon => IsShuffleEnabled ? "shuffle_on.png" : "shuffle_off.png";

        public string CoverSource => CurrentTrack?.CoverSource ?? TrackModel.PlaceholderCover;

        public string PositionText => FormatTime(Position);

        public string DurationText => FormatTime(Duration);

        public double SliderMaximum => Duration > 0 ? Duration : 1;

        public PlayerViewModel(AudioPlayerService audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _audioPlayer.PlaybackEnded += OnPlaybackEnded;
        }

        #region Public API
        public void PlayTrack(TrackModel track)
        {
            var tracks = new List<TrackModel> { track };
            PlayTracks(tracks);
        }

        public void PlayTracks(IEnumerable<TrackModel> tracks)
        {
            _queue.Clear();
            foreach (TrackModel track in tracks)
            {
                _queue.Add(track);
            }

            if (_queue.Count == 0)
            {
                return;
            }

            BuildPlayOrder(-1);
            _orderPosition = 0;
            PlayCurrent();
        }

        public void Stop()
        {
            _positionTimer?.Stop();
            _audioPlayer.Unload();

            _queue.Clear();
            _playOrder.Clear();
            _orderPosition = -1;

            SetCurrentTrack(null);
            IsPlaying = false;
            UpdatePositionSilently(0);
            Duration = 0;
        }
        #endregion

        #region Navigation
        [RelayCommand]
        private async Task OpenNowPlaying()
        {
            await NavigateAsync(Routes.NowPlaying);
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void TogglePlay()
        {
            if (!_audioPlayer.IsLoaded)
            {
                ShowAlert("Nothing to play", "Start a track or a playlist first.");
                return;
            }

            if (_audioPlayer.IsPlaying)
            {
                _audioPlayer.Pause();
                IsPlaying = false;
                return;
            }

            _audioPlayer.Play();
            IsPlaying = true;
        }

        [RelayCommand]
        private void Next()
        {
            if (_orderPosition + 1 >= _playOrder.Count)
            {
                EndOfQueue();
                return;
            }

            _orderPosition++;
            PlayCurrent();
        }

        [RelayCommand]
        private void Previous()
        {
            if (Position > RestartThresholdSeconds || _orderPosition <= 0)
            {
                _audioPlayer.Seek(0);
                UpdatePositionSilently(0);
                return;
            }

            _orderPosition--;
            PlayCurrent();
        }

        [RelayCommand]
        private void ToggleShuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;

            if (_orderPosition < 0)
            {
                return;
            }

            int currentQueueIndex = _playOrder[_orderPosition];
            BuildPlayOrder(currentQueueIndex);
            _orderPosition = _playOrder.IndexOf(currentQueueIndex);
        }

        [RelayCommand]
        private void SeekStarted()
        {
            IsSeeking = true;
        }

        [RelayCommand]
        private void SeekCompleted()
        {
            IsSeeking = false;
            _audioPlayer.Seek(Position);
        }

        partial void OnPositionChanged(double value)
        {
            if (_isUpdatingPosition || IsSeeking)
            {
                return;
            }

            _audioPlayer.Seek(value);
        }
        #endregion

        #region Support Methods
        private void PlayCurrent()
        {
            TrackModel track = _queue[_playOrder[_orderPosition]];

            if (!track.IsFileAvailable)
            {
                ShowAlert("Cannot play", $"The audio file of \"{track.Title}\" is not on this device.");
                EndOfQueue();
                return;
            }

            if (!_audioPlayer.Load(track.FilePath))
            {
                ShowAlert("Cannot play", $"\"{track.Title}\" could not be opened.");
                EndOfQueue();
                return;
            }

            SetCurrentTrack(track);
            Duration = _audioPlayer.Duration;
            UpdatePositionSilently(0);

            _audioPlayer.Play();
            IsPlaying = true;

            StartPositionTimer();
        }

        private void EndOfQueue()
        {
            _audioPlayer.Pause();
            _audioPlayer.Seek(0);
            IsPlaying = false;
            UpdatePositionSilently(0);
        }

        private void BuildPlayOrder(int firstQueueIndex)
        {
            _playOrder.Clear();

            for (int i = 0; i < _queue.Count; i++)
            {
                _playOrder.Add(i);
            }

            if (!IsShuffleEnabled)
            {
                return;
            }

            for (int i = _playOrder.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                int temp = _playOrder[i];
                _playOrder[i] = _playOrder[j];
                _playOrder[j] = temp;
            }

            if (firstQueueIndex >= 0)
            {
                _playOrder.Remove(firstQueueIndex);
                _playOrder.Insert(0, firstQueueIndex);
            }
        }

        private void SetCurrentTrack(TrackModel? track)
        {
            if (CurrentTrack != null)
            {
                CurrentTrack.IsPlaying = false;
            }

            CurrentTrack = track;

            if (CurrentTrack != null)
            {
                CurrentTrack.IsPlaying = true;
            }
        }

        private void StartPositionTimer()
        {
            if (_positionTimer == null)
            {
                IDispatcher? dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                {
                    return;
                }

                _positionTimer = dispatcher.CreateTimer();
                _positionTimer.Interval = TimeSpan.FromMilliseconds(500);
                _positionTimer.Tick += OnPositionTimerTick;
            }

            _positionTimer.Start();
        }

        private void OnPositionTimerTick(object? sender, EventArgs e)
        {
            if (IsSeeking || !_audioPlayer.IsLoaded)
            {
                return;
            }

            if (Duration <= 0)
            {
                Duration = _audioPlayer.Duration;
            }

            UpdatePositionSilently(_audioPlayer.CurrentPosition);
        }

        private void UpdatePositionSilently(double value)
        {
            _isUpdatingPosition = true;
            Position = value;
            _isUpdatingPosition = false;
        }

        private void OnPlaybackEnded(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(Next);
        }

        private static string FormatTime(double seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            return TimeSpan.FromSeconds(seconds).ToString(@"m\:ss");
        }
        #endregion
    }
}
