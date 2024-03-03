

namespace SoundCOM.Service;

public interface IUsbDeviceListener
{
    event EventHandler<string> UsbDeviceInserted;
    event EventHandler<string> UsbDeviceRemoved;

    void StartListening();
    void StopListening();
}
