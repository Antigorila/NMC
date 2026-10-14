namespace NMC.App.MVVM.Views.ContentViews.Profile;

public partial class ProfileContentView : ContentView
{
    public event EventHandler OnBackRequested;
    public ProfileContentView()
	{
		InitializeComponent();
	}
    private void BackBtn_Clicked(object sender, EventArgs e)
    {
        OnBackRequested?.Invoke(this, EventArgs.Empty);
    }
}