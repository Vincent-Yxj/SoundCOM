
using Microsoft.VisualBasic;
using System.Collections;
using System.Threading;

namespace SoundCOM.Service;

// 串口服务实现
public class SerialPortService : ISerialPortService
{
    private readonly ILogger _logger;
    private SerialPort serialPort;

    public event EventHandler<string[]> DataReceived;
    private DispatcherTimer timer;
    public SerialPortService(ILogger logger)
    {
        _logger = logger;
        serialPort = new SerialPort();
        Serial_table = new Hashtable();
        PortNames = new List<string>();
        serialPort.DataReceived += SerialPort_DataReceived;
        timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0,1,0);
        timer.Tick += Run_Time;
        _logger.Information("SerialPortService Loaded");
    }

    private string? _portName;
    public string? PortName;
    public List<string> PortNames;
    private string Mode;
    private Hashtable Serial_table;

    public void StartListening()
    {
        try
        {
            serialPort.PortName = _portName;
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Open();
            timer.Start();
            _logger.Information($"SerialPortService Open {_portName} Succeed");
        }
        catch
        {
            _logger.Error($"SerialPortService Open {_portName} Failed");
        }
        
        
    }
    public void StopListening()
    {

        serialPort.Close();
        _logger.Information($"SerialPortService Close {_portName} Succeed");
        timer.Stop();
    }
    public bool Used_bool()
    {
        if (serialPort.IsOpen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Run_Time(object sender, EventArgs e)
    {
        byte[] data = Mode=="A"?new byte[] { 0x55,0x01,0x01,0xAA}:new byte[] { 0x55,0x01,0x00,0xAA};
        serialPort.Write(data,0,data.Length);
    }

    
    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        int bytesToRead = serialPort.BytesToRead;
        byte[] buffer = new byte[bytesToRead];
        serialPort.Read(buffer, 0, bytesToRead);

        // 将字节数组转换为十六进制字符串
        if (buffer.Length == 6 && buffer[0] == 0x55 && buffer[5] == 0xAA)
        {
            // 提取数据高位、低位和dbA
            byte highByte = buffer[1];
            byte lowByte = buffer[2];
            byte dbAC = buffer[3];
            byte dbFS = buffer[4];

            // 合并数据高位和低位，得到完整的数据
            int rawData = (highByte << 8)| lowByte;
            double comdata = (float)rawData/10.0;
            if (comdata > 0)
            {
                string completeData = Math.Round(comdata, 1).ToString("N1");
                string AC = (dbAC == 0) ? "C" : "A";
                Mode = AC;
                string FS = dbFS == 0 ? "Slow" : "Fast";
                string[] Data = { completeData, AC, FS};

                // 使用解析出的数据
                DataReceived?.Invoke(this, Data);
            }
        }
        //if (buffer.Length == 12 && buffer[0] == 0x55 && buffer[5] == 0xAA && buffer[6] == 0x55 && buffer[11] == 0xAA)
        //{
        //    // 提取数据高位、低位和dbA
        //    byte highByte = buffer[1];
        //    byte lowByte = buffer[2];
        //    byte dbAC = buffer[3];
        //    byte dbFS = buffer[4];

        //    // 合并数据高位和低位，得到完整的数据
        //    int rawData = (highByte << 8) | lowByte;
        //    double comdata = (float)rawData / 10.0;
        //    if (comdata > 0)
        //    {
        //        string completeData = Math.Round(comdata, 1).ToString("N1");
        //        string AC = (dbAC == 0) ? "C" : "A";
        //        Mode = AC;
        //        string FS = dbFS == 0 ? "Slow" : "Fast";
        //        string[] Data = { completeData, AC, FS };

        //        // 使用解析出的数据
        //        DataReceived?.Invoke(this, Data);
        //    }
        //    // 提取数据高位、低位和dbA
        //    highByte = buffer[7];
        //    lowByte = buffer[8];
        //    dbAC = buffer[9];
        //     dbFS = buffer[4];

        //    // 合并数据高位和低位，得到完整的数据
        //    rawData = (highByte << 8) | lowByte;
        //    comdata = (float)rawData / 10.0;
        //    if (comdata > 0)
        //    {
        //        string completeData = Math.Round(comdata, 1).ToString("N1");
        //        string AC = (dbAC == 0) ? "C" : "A";
        //        Mode = AC;
        //        string FS = dbFS == 0 ? "Slow" : "Fast";
        //        string[] Data = { completeData, AC ,FS};

        //        // 使用解析出的数据
        //        DataReceived?.Invoke(this, Data);
        //    }
        //}
    }

    public void UsbInserted(string deviceId)
    {
        
        Get_PortName_DeviceID();
        if (_portName==null)
        {
            _portName = PortNames[0];
        }
        
    }
    public void UsbRemoved(string deviceId)
    {
        
        if (_portName == Serial_table[deviceId])
        {
            if (PortNames.Count==PortNames.IndexOf(_portName)+1)
            {
                _portName = PortNames.Count > 1 ?PortNames[PortNames.IndexOf(_portName)-1]:null;
            }
            else
            {
                _portName = PortNames[PortNames.IndexOf(_portName) + 1];
            }
        }
        PortNames.Remove(Serial_table[deviceId].ToString());
        Serial_table.Remove(deviceId);
    }

    public void Change_Port(string portName)
    {
        if(portName != _portName)
        {
            if (Used_bool())
            {
                StopListening();
            }
            _portName = portName;
        }
        StartListening();
    }
    public void Change_Mode(string mode)
    {
        if (mode!=Mode)
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            if (mode == "A")
            {
                byte[] data = { 0x55, 0x01, 0x01, 0xAA };
                serialPort.Write(data, 0, data.Length);
            }
            else
            {
                byte[] data = { 0x55, 0x01, 0x00, 0xAA };
                serialPort.Write(data, 0, data.Length);
            }
        }
        
    }

    public void Get_First_Port()
    {
        Get_PortName_DeviceID();
        _portName = PortNames.Count>0?PortNames[0]:null;
        if(_portName != null)
        {
            StartListening();
        }
    }
    

    public void Get_PortName_DeviceID()
    {
        string[] available_spectrometers = SerialPort.GetPortNames();
        ManagementObjectCollection.ManagementObjectEnumerator enumerator = null;
        // 构建 WMI 查询语句
        string query = $"SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%VID_1A86&PID_7523%'";

        try
        {
            // 使用 ManagementObjectSearcher 进行查询
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", query))
            {
                // 获取查询结果
                ManagementObjectCollection results = searcher.Get();

                // 遍历结果
                foreach (ManagementObject result in results)
                {
                    Console.WriteLine($"Caption: {result["Caption"]}");
                    int comIndex = result["Caption"].ToString().IndexOf("COM");
                    int rcomIndex = result["Caption"].ToString().IndexOf(")");
                    if (Serial_table.ContainsKey(result["DeviceID"]))
                    {
                        Serial_table[result["DeviceID"]] = result["Caption"].ToString()[comIndex..rcomIndex];
                    }
                    else
                    {
                        Serial_table.Add(result["DeviceID"], result["Caption"].ToString()[comIndex..rcomIndex]);
                    }

                }
            }
            PortNames.Clear();
            foreach (string i in Serial_table.Values)
            {
                PortNames.Add(i);
            }
            PortNames.Sort();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to get DeviceID\nError:{ex}");
        }
        

    }

    public List<string> GetPorts()  { return PortNames; }
    public string GetPort()  { return _portName; }
    public string GetMode() { return Mode; }


}
