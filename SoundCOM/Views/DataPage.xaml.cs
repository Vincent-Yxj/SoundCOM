

using System.Windows.Data;
using Wpf.Ui.Mvvm.Interfaces;

namespace SoundCOM.Views;

/// <summary>
/// DataPage.xaml 的交互逻辑
/// </summary>
public partial class DataPage : Page
{
    public DataPage()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<DataViewModel>();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
    private DataViewModel DataViewModel => App.Current.Services.GetService<DataViewModel>();

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        DataViewModel.PropertyChanged += ViewModel_PropertyChanged;

    private void OnUnloaded(object sender, RoutedEventArgs e) =>
        DataViewModel.PropertyChanged += ViewModel_PropertyChanged;

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) =>
        ComDataRectangle.BeginAnimation(HeightProperty, new DoubleAnimation()
        {
            To = DataViewModel.ComDataToNum,
            Duration = TimeSpan.FromSeconds(0.5),
            RepeatBehavior=RepeatBehavior.Forever,
            AutoReverse=true
        });
}
