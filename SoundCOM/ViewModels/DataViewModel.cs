

using System.Globalization;
using System.Windows.Data;

namespace SoundCOM.ViewModels;

public partial class DataViewModel:ObservableRecipient
{
    private readonly ISerialPortService _serialPortService;
    private readonly ILogger _logger;
    private readonly IDataGridService _gridService;
    public DataViewModel(ILogger logger, ISerialPortService serialPortService,IDataGridService dataGridService) 
    {   
        ComData = "0.0dB";
        ComDataToNum =800;
        
        this._logger = logger;
        this._serialPortService = serialPortService;
        this._gridService = dataGridService;
        _gridService.Clear();
        _serialPortService.DataReceived += SerialPortService_DataReceived;
        DataGrid_num = 0;
        _logger.Information("DataViewModel Loaded");
    }
    [ObservableProperty]
    private string comData;
    [ObservableProperty]
    private string serialPortName;
    private int DataGrid_num;
    [ObservableProperty]
    private string comDataBoolF;
    [ObservableProperty]
    private string comDataBoolS;
    [ObservableProperty]
    private string comDataBoolA;
    [ObservableProperty]
    private string comDataBoolC;
    private double _comDataToNum;

    public double ComDataToNum
    {
        get { return _comDataToNum; }
        set
        {
            if (_comDataToNum != value)
            {
                _comDataToNum = value;
                OnPropertyChanged(nameof(ComDataToNum));
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SerialPortService_DataReceived(object sender, string[] e)
    {
        Application.Current.Dispatcher.Invoke(() => 
        {
            ComData = e[0] + "dB";
            ComDataToNum = (Convert.ToDouble(e[0]) - 30) * 8;
            ComDataBoolA = e[1] == "A" ? "Orange" : "Gray";
            ComDataBoolC = e[1] == "C" ? "Orange" : "Gray";
            ComDataBoolF = e[2] == "Fast" ? "Orange" : "Gray";
            ComDataBoolS = e[2] == "Slow" ? "Orange" : "Gray";
        });
        _gridService.Add(new ComdataGrid() { Num = DataGrid_num + 1, dB = e[0], AC = e[1], FS = e[2], Time = DateTime.Now.ToString(@"hh\:mm\:ss.ff"), Date = DateTime.Today.ToShortDateString() });
        DataGrid_num++;
    }
    

    public void Refresh()
    {
        ComData = "0.0dB";
        ComDataToNum = 800;
        
        DataGrid_num = 0;
        _gridService.Clear();
        _serialPortService.StopListening();
        _serialPortService.DataReceived -= SerialPortService_DataReceived;
        _serialPortService.DataReceived += SerialPortService_DataReceived;
        _serialPortService.StartListening();
        _logger.Information("DataViewModel Loaded");
    }
    public void Closed()
    {
        _serialPortService.DataReceived -= SerialPortService_DataReceived;
        _gridService.Clear();
        _logger.Information("DataViewModel Closed");
    }

}




