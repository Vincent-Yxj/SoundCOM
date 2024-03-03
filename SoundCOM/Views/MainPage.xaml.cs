

namespace SoundCOM.Views;

/// <summary>
/// MainPageView.xaml 的交互逻辑
/// </summary>
public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainPageViewModel>();
    }
}
