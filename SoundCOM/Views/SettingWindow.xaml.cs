

namespace SoundCOM.Views;

/// <summary>
/// SettingWindow.xaml 的交互逻辑
/// </summary>
public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<SettingWindowViewModel>();
    }

}
