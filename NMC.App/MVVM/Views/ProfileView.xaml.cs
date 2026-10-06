namespace NMC.App.MVVM.Views;
public partial class ProfileView : ContentView
{
    public event EventHandler OnBackRequested;

    public ProfileView()
    {
        InitializeComponent();
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        OnBackRequested?.Invoke(this, EventArgs.Empty);
    }
}