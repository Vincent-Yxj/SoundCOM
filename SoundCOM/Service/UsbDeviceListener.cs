

namespace SoundCOM.Service;

public class UsbDeviceListener : IUsbDeviceListener
{
    private ManagementEventWatcher insertWatcher;
    private ManagementEventWatcher removeWatcher;

    public event EventHandler<string> UsbDeviceInserted;
    public event EventHandler<string> UsbDeviceRemoved;

    public void StartListening()
    {
        WqlEventQuery insertQuery = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(1), "TargetInstance isa 'Win32_USBControllerDevice'");
        WqlEventQuery removeQuery = new WqlEventQuery("__InstanceDeletionEvent", TimeSpan.FromSeconds(1), "TargetInstance isa 'Win32_USBControllerDevice'");

        insertWatcher = new ManagementEventWatcher(insertQuery);
        removeWatcher = new ManagementEventWatcher(removeQuery);

        insertWatcher.EventArrived += (sender, e) => HandleUsbDeviceEvent(e, UsbDeviceInserted);
        removeWatcher.EventArrived += (sender, e) => HandleUsbDeviceEvent(e, UsbDeviceRemoved);

        insertWatcher.Start();
        removeWatcher.Start();
    }

    public void StopListening()
    {
        insertWatcher?.Stop();
        removeWatcher?.Stop();
    }

    private void HandleUsbDeviceEvent(EventArrivedEventArgs e, EventHandler<string> eventHandler)
    {
        try
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string pnpDeviceID = targetInstance["Dependent"].ToString().Replace("\\\\", "\\");

            int lindex = pnpDeviceID.IndexOf("\"") + 1;
            int rindex = pnpDeviceID.Length - 1;

            if (lindex > 0 && rindex > lindex)
            {
                string id = pnpDeviceID.Substring(lindex, rindex - lindex);
                if (id.Contains("VID_1A86&PID_7523"))
                {
                    eventHandler?.Invoke(this, id);
                }
            }
        }
        catch (Exception ex)
        {
            // 处理异常，例如记录日志
            Console.WriteLine($"Exception in HandleUsbDeviceEvent: {ex.Message}");
        }

    }
}