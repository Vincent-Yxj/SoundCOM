

namespace SoundCOM;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Services = ConfigureServices();
        
        
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogger>(_ =>
        {
            return new LoggerConfiguration().MinimumLevel
                .Debug()
                .WriteTo.File("log\\log.txt")
                .CreateLogger();
        });
        services.AddSingleton<IUsbDeviceListener,UsbDeviceListener>();
        services.AddSingleton<ISerialPortService, SerialPortService>();
        services.AddSingleton<IColorService, ColorService>();
        services.AddTransient<IDataGridService, DataGridService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainPageViewModel>();
        services.AddSingleton<MainPage>();
        services.AddSingleton<DataViewModel>();
        services.AddSingleton<DataPage>();
        services.AddSingleton<SettingWindowViewModel>();
        services.AddSingleton<SettingWindow>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        
        var mainWindow = Services.GetService<MainWindow>();
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        // 根据屏幕大小设置窗口尺寸
        SetWindowSize(mainWindow);
        mainWindow!.Show();
    }

    

    private void SetWindowSize(Window window)
    {
        // 获取屏幕的工作区大小
        double screenWidth = SystemParameters.WorkArea.Width;
        double screenHeight = SystemParameters.WorkArea.Height;

        // 设置窗口的尺寸（例如，设置为屏幕大小的一半）
        double newWidth = screenWidth / 2;
        double newHeight = screenHeight / 2;

        // 设置窗口的最小和最大尺寸（可选）
        window.MinWidth = 200; // 最小宽度
        window.MinHeight = 200; // 最小高度
        window.MaxWidth = screenWidth; // 最大宽度
        window.MaxHeight = screenHeight; // 最大高度

        // 设置窗口的位置和大小
        window.Left = (screenWidth - newWidth) / 2;
        window.Top = (screenHeight - newHeight) / 2;
        window.Width = newWidth;
        window.Height = newHeight;
    }
}
