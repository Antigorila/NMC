namespace NMC.Client.Navigation
{
    public static class Routes
    {
        #region Root (ShellContent) routes
        public const string Login = "LoginPage";
        public const string Main = "MainPage";

        public const string LoginRoot = "//" + Login;
        public const string MainRoot = "//" + Main;
        #endregion

        #region Pushed routes
        public const string Register = "RegisterPage";
        public const string Library = "LibraryPage";
        public const string TrackDetail = "TrackDetailPage";
        public const string AddTrack = "AddTrackPage";
        public const string PlaylistDetail = "PlaylistDetailPage";
        public const string PlaylistEdit = "PlaylistEditPage";
        public const string Filter = "FilterPage";
        public const string Recommendation = "RecommendationPage";
        public const string Profile = "ProfilePage";
        public const string NowPlaying = "NowPlayingPage";
        #endregion
    }
}
