

using System.Collections;

namespace SoundCOM.Service;

public interface ISerialPortService
{
    event EventHandler<string[]> DataReceived;
    void StartListening();
    void StopListening();
    bool Used_bool();
    void UsbRemoved(string deviceId);
    void UsbInserted(string deviceId);
    void Get_First_Port();
    List<string> GetPorts();
    string GetPort();
    string GetMode();
    void Change_Port(string portName);
    void Change_Mode(string mode);
}

