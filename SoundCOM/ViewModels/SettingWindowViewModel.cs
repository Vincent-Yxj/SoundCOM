

using CommunityToolkit.Mvvm.Messaging;
using SoundCOM.Service;
using System.Collections;
using System.Drawing;
using Wpf.Ui.Controls;

namespace SoundCOM.ViewModels;

public partial class SettingWindowViewModel : ObservableRecipient
{
    private readonly ILogger _logger;
    private readonly ISerialPortService _serialPortService;
    private readonly IColorService _colorService;
    public SettingWindowViewModel(ILogger logger,ISerialPortService serialPortService,IColorService colorService) 
    {
        _logger = logger;
        _serialPortService = serialPortService;
        _colorService = colorService;
        ComNums = _serialPortService.GetPorts();
        ComNum = _serialPortService.GetPort();
        ComMode = _serialPortService.GetMode();
        ComModes = new string[] { "A", "C"};

        ColorList = _colorService.Get_ColorList();
        RealTimeColor = _colorService.Get_Config()[0];
        MaxColor = _colorService.Get_Config()[1];
        MinColor = _colorService.Get_Config()[2];
    }

    [ObservableProperty]
    private string comNum;
    [ObservableProperty]
    private List<string> comNums;
    [ObservableProperty]
    private string comMode;
    [ObservableProperty]
    private string[] comModes;

    [ObservableProperty]
    private List<string> colorList;
    [ObservableProperty]
    private string realTimeColor;
    [ObservableProperty]
    private string maxColor;
    [ObservableProperty]
    private string minColor;


    [RelayCommand]
    private void Save_Config()
    {
        _serialPortService.Change_Port(ComNum);

        _serialPortService.Change_Mode(ComMode);

        _colorService.Save_Config(RealTimeColor, MaxColor, MinColor);
        SettingWindow settingWindow = App.Current.Services.GetService<SettingWindow>();
        settingWindow.Hide();
    }
    [RelayCommand]
    private void Cancellation() 
    {
        SettingWindow settingWindow = App.Current.Services.GetService<SettingWindow>();
        settingWindow.Hide();
    }

    public void Refresh()
    {
        Application.Current.Dispatcher.Invoke(() => 
        {
            ComNums = new List<string>();
            ComNums = _serialPortService.GetPorts();
            ComNum = _serialPortService.GetPort();
            ComMode = _serialPortService.GetMode();
        });
    }
}
