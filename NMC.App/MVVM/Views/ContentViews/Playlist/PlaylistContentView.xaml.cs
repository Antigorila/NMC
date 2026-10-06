namespace NMC.App.MVVM.Views.ContentViews.Playlist;

public partial class PlaylistContentView : ContentView
{
    public event EventHandler OnBackRequested;
    public PlaylistContentView()
	{
		InitializeComponent();
	}

    private void BackBtn_Clicked(object sender, EventArgs e)
    {
        OnBackRequested?.Invoke(this, EventArgs.Empty);
    }
}