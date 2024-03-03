
using LiveCharts.Configurations;
using SoundCOM.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoundCOM.ViewModels;

public partial class MainPageViewModel : ObservableRecipient
{
    private readonly ISerialPortService _serialPortService;
    private readonly ILogger _logger;
    private readonly IColorService _colorService;
    private readonly IDataGridService _gridService;

    private DispatcherTimer dispatcherTimer;

    public MainPageViewModel(ILogger logger, ISerialPortService serialPortService,IColorService colorService,IDataGridService dataGridService)
    {
        this._logger = logger;
        this._serialPortService = serialPortService;
        this._colorService = colorService;
        this._gridService = dataGridService;
        _gridService.Clear();
        ChartValueColor = _colorService.Get_Config()[0];
        MaxValueColor = _colorService.Get_Config()[1];
        MinValueColor = _colorService.Get_Config()[2];
        ComDataGrid = new ObservableCollection<ComdataGrid>();
        DataGrid_Add();
        DataFromSerialPort = "0.0dB";
        DataToNum = "-100";
        
        _serialPortService.DataReceived += SerialPortService_DataReceived;
        DataGrid_num = 0;

        dispatcherTimer = new DispatcherTimer
        {
            Interval = new TimeSpan(8, 0, 0)
        };
        dispatcherTimer.Tick += new EventHandler(Time_Save);
        dispatcherTimer.Start();

        
        
        var mapper = Mappers.Xy<MeasureModel>()
               .X(model => model.Index)
               .Y(model => model.Value);
        Charting.For<MeasureModel>(mapper);
        ComDataChartValues = new ChartValues<MeasureModel>();
        ComDataMaxValues = new ChartValues<MeasureModel>();
        ComDataMinValues = new ChartValues<MeasureModel>();
        comDataMaxValue = 0;
        comDataMinValue = 100;
        _logger.Information("MainPageViewModel Loaded");
        
    }
    

    /// <summary>
    /// 串口
    /// </summary>
    [ObservableProperty]
    private string portName;
    /// <summary>
    /// 串口数据
    /// </summary>
    [ObservableProperty]
    private string dataFromSerialPort;
    [ObservableProperty]
    private string dataToNum;
    /// <summary>
    /// 获取串口数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SerialPortService_DataReceived(object sender, string[] e)
    {
        
        Application.Current.Dispatcher.Invoke(() => 
        {
            DataFromSerialPort = e[0] +"dB";
            DataToNum = $"{(Convert.ToDouble(e[0])-80)*2}";
            if (DataGrid_num == ComDataGrid.Count)
            {
                DataGrid_Add();
            }
            ComDataGrid[DataGrid_num] = new ComdataGrid() { Num=DataGrid_num+1, dB = e[0], AC = e[1], FS = e[2], Time = DateTime.Now.ToString(@"hh\:mm\:ss.ff"), Date = DateTime.Today.ToString("yyyy-MM-dd") };
            _gridService.Add(new ComdataGrid() { Num = DataGrid_num+1, dB = e[0], AC = e[1], FS = e[2], Time = DateTime.Now.ToString(@"hh\:mm\:ss.ff"), Date = DateTime.Today.ToString("yyyy-MM-dd") });
            ComDataChartValues.Add(new MeasureModel() { Index=DataGrid_num,Value= float.Parse(e[0]) });
            if (float.Parse(e[0]) > comDataMaxValue)
            {
                comDataMaxValue = float.Parse(e[0]);
            }
            ComDataMaxValues.Add(new MeasureModel() { Index = DataGrid_num, Value = comDataMaxValue });
            if (float.Parse(e[0]) < comDataMinValue)
            {
                comDataMinValue = float.Parse(e[0]);
            }
            if (DataGrid_num>10) 
            {
                ComDataChartValues.RemoveAt(0);
                ComDataMaxValues.RemoveAt(0);
                ComDataMinValues.RemoveAt(0);
            }
            ComDataMinValues.Add(new MeasureModel() { Index = DataGrid_num, Value = comDataMinValue });
            DataGrid_num++;
        });
        
    }

    private void Time_Save(object sender, EventArgs e)
    {
        
        try
        {
            _gridService.Save_file();
            _logger.Information("Save to file Sucessed");
        }
        catch
        {
            _logger.Error("Save to file failed");
        }
        
    }
    /// <summary>
    /// 表格数据
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ComdataGrid> comDataGrid;
    /// <summary>
    /// 插入表格行数
    /// </summary>
    private int DataGrid_num;
    /// <summary>
    /// 增加表格行数
    /// </summary>
    private void DataGrid_Add()
    {
        for (int i = 0; i < 120; i++)
        {
            ComDataGrid.Add(new ComdataGrid() { Num=i+DataGrid_num+1});
        }
    }


    [ObservableProperty]
    private ChartValues<MeasureModel> comDataChartValues;
    [ObservableProperty]
    private ChartValues<MeasureModel> comDataMaxValues;
    private float comDataMaxValue;
    [ObservableProperty]
    private ChartValues<MeasureModel> comDataMinValues;
    private float comDataMinValue;

    [ObservableProperty]
    private string chartValueColor;
    [ObservableProperty]
    private string maxValueColor;
    [ObservableProperty]
    private string minValueColor;
    
    public void Refresh()
    {
        DataFromSerialPort = "0.0dB";
        DataToNum = "-100";
        // 重置表格行数和清空相关集合
        DataGrid_num = 0;
        _gridService.Clear();
        ComDataGrid = new ObservableCollection<ComdataGrid>();
        ComDataGrid.Clear();
        DataGrid_Add();

        // 停止并重新启动计时器
        dispatcherTimer.Stop();
        dispatcherTimer.Start();

        // 清空图表数据集合
        ComDataChartValues = new ChartValues<MeasureModel>();
        ComDataMaxValues = new ChartValues<MeasureModel>();
        ComDataMinValues = new ChartValues<MeasureModel>();

        // 停止并重新启动串口监听
        _serialPortService.StopListening();
        _serialPortService.DataReceived -= SerialPortService_DataReceived;
        _serialPortService.DataReceived += SerialPortService_DataReceived;

        _logger.Information("MainPageViewModel Loaded");
    }
    public void Closed()
    {
        _serialPortService.DataReceived -= SerialPortService_DataReceived;
        dispatcherTimer.Stop();
        _logger.Information("MainPageViewModel Closed");
    }
}

