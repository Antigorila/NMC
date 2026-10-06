namespace NMC.App.MVVM.Views.ContentViews.Track;

public partial class TrackContentView : ContentView
{
    public event EventHandler OnBackRequested;
    public TrackContentView()
	{
		InitializeComponent();
	}

    private void BackBtn_Clicked(object sender, EventArgs e)
    {
        OnBackRequested?.Invoke(this, EventArgs.Empty);
    }
}