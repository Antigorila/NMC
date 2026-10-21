using Plugin.Maui.Audio;

namespace NMC.Client.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;

        private IAudioPlayer? _player;
        private Stream? _audioStream;

        public event EventHandler? PlaybackEnded;

        public bool IsLoaded => _player != null;

        public bool IsPlaying => _player?.IsPlaying ?? false;

        public double Duration => _player?.Duration ?? 0;

        public double CurrentPosition => _player?.CurrentPosition ?? 0;

        public AudioPlayerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public bool Load(string filePath)
        {
            Unload();

            try
            {
                _audioStream = File.OpenRead(filePath);
                _player = _audioManager.CreatePlayer(_audioStream);
                _player.PlaybackEnded += OnPlaybackEnded;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio file could not be loaded: {ex.Message}");
                Unload();
                return false;
            }
        }

        public void Play()
        {
            _player?.Play();
        }

        public void Pause()
        {
            _player?.Pause();
        }

        public void Seek(double seconds)
        {
            _player?.Seek(seconds);
        }

        public void Unload()
        {
            if (_player != null)
            {
                _player.PlaybackEnded -= OnPlaybackEnded;
                _player.Stop();
                _player.Dispose();
                _player = null;
            }

            _audioStream?.Dispose();
            _audioStream = null;
        }

        #region Support Methods
        private void OnPlaybackEnded(object? sender, EventArgs e)
        {
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
