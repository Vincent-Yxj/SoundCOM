

namespace SoundCOM.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient
{
    private readonly ILogger _logger;
    private readonly ISerialPortService _serialPortService;
    private readonly IDataGridService _gridService;
    private readonly IUsbDeviceListener _usbDeviceListener;

    public MainWindowViewModel(ILogger logger,ISerialPortService serialPortService,IColorService colorService,IDataGridService dataGridService,IUsbDeviceListener usbDeviceListener)
    {
        _logger = logger;
        _serialPortService = serialPortService;
        _gridService = dataGridService;
        _usbDeviceListener = usbDeviceListener;
        f = 0;
        ToMainPage();
        _logger.Information("MainWindow Loaded");
        _usbDeviceListener.UsbDeviceInserted += OnUsbInserted;
        _usbDeviceListener.UsbDeviceRemoved += OnUsbRemoved;
        _usbDeviceListener.StartListening();
        
    }

    /// <summary>
    /// 页面切换
    /// </summary>
    [ObservableProperty]
    private object frameContent;
    private int f;

    private void OnUsbInserted(object sender, string deviceId)
    {
        _serialPortService.UsbInserted(deviceId);
        var s = App.Current.Services.GetRequiredService<SettingWindowViewModel>();
        s.Refresh();

    }
    private void OnUsbRemoved(object sender, string deviceId)
    {
        _serialPortService.UsbRemoved(deviceId);
        var s = App.Current.Services.GetRequiredService<SettingWindowViewModel>();
        s.Refresh();
    }

    private void ChangePage()
    {
        if (f == 1)
        {
            var s1 = App.Current.Services.GetRequiredService<MainPageViewModel>();
            s1.Refresh();
        }
        else
        {
            var s2 = App.Current.Services.GetRequiredService<DataViewModel>();
            s2.Refresh();
        }
    }

    [RelayCommand]
    private void ToMainPage()
    {
        
        switch (f) 
        {
            case 0:
                f = 1;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FrameContent = App.Current.Services.GetService<MainPage>();
                });
                _serialPortService.Get_First_Port();
                break;
            case 1:
                _serialPortService.StopListening();
                var s1 = App.Current.Services.GetRequiredService<MainPageViewModel>();
                s1.Refresh();
                _serialPortService.StartListening();
                break;
            case 2:
                f = 1;
                _serialPortService.StopListening();
                var s2 = App.Current.Services.GetRequiredService<DataViewModel>();
                s2.Closed();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FrameContent = App.Current.Services.GetService<MainPage>();
                });
                _serialPortService.StartListening();
                break;
        }

    }
    [RelayCommand]
    private void ToDataPage()
    {
        switch (f)
        {
            case 1:
                f= 2;
                _serialPortService.StopListening();
                var s3 = App.Current.Services.GetRequiredService<MainPageViewModel>();
                s3.Closed();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FrameContent = App.Current.Services.GetService<DataPage>();
                });
                _serialPortService.StartListening();
                break;
            case 2:
                _serialPortService.StopListening();
                var s4 = App.Current.Services.GetRequiredService<DataViewModel>();
                s4.Refresh();
                _serialPortService.StartListening();
                break;
        }
    }

    [RelayCommand]
    private void ShowSettingWindow()
    {
        
        // 创建 SettingWindow，并设置 DataContext
        SettingWindow settingWindow = App.Current.Services.GetService<SettingWindow>();

        var mainWindow = App.Current.Services.GetService<MainWindow>();

        // 设置窗口位置为主窗口中心
        settingWindow.Left = mainWindow.Left + (mainWindow.Width - settingWindow.Width) / 2;
        settingWindow.Top = mainWindow.Top + (mainWindow.Height - settingWindow.Height) / 2;

        _serialPortService.StopListening();
        // 显示 SettingWindow
        settingWindow.ShowDialog();
    }
    [RelayCommand]
    private void SaveToFile()
    {
        
        try
        {
            _gridService.Save_file();
            MessageBox.Show("Save Sucessed");
            ChangePage();
        }
        catch
        {
            MessageBox.Show("Save to xlsx faled");
        }
        

    }
    [RelayCommand]
    private void Close_Window()
    {
        if (_serialPortService.Used_bool())
            {
                _serialPortService.StopListening();
            }
        _usbDeviceListener.StopListening();
        MainWindow? mainWindow = App.Current.Services.GetService<MainWindow>();
        mainWindow?.Close();
    }

    [RelayCommand]
    private void Minimize_Window()
    {
        MainWindow? mainWindow = App.Current.Services.GetService<MainWindow>();
        mainWindow.WindowState = WindowState.Minimized;
    }
}
