using System.Windows.Input;

namespace NMC.Client.Controls
{
    public partial class PageHeader : ContentView
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(PageHeader), string.Empty);

        public static readonly BindableProperty BackCommandProperty =
            BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(PageHeader));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ICommand? BackCommand
        {
            get { return (ICommand?)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        public PageHeader()
        {
            InitializeComponent();
        }
    }
}
