

namespace SoundCOM.Service;

internal class DataGridService : IDataGridService
{
    private readonly ILogger _logger;
    private List<ComdataGrid> comdataGrids;

    public DataGridService(ILogger logger)
    {
        _logger = logger;
        comdataGrids = new List<ComdataGrid>();
        if (!Directory.Exists("Data"))
        {
            Directory.CreateDirectory("Data");
        }
        _logger.Information("DataGridService Loaded");
    }
    void IDataGridService.Add(ComdataGrid data)
    {
        comdataGrids.Add(data);
    }

    void IDataGridService.Clear()
    {
        comdataGrids.Clear();
    }

    List<ComdataGrid> IDataGridService.Get_Data()
    {
        return comdataGrids;
    }

    public bool Save_file()
    {
        try
        {
            MiniExcel.SaveAs($"Data\\{comdataGrids[0].Date}-{comdataGrids[0].Time.ToString().Replace(":", "_")}.xlsx", comdataGrids, true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Save file failed    Error: {ex}");
            return false;
        }
    }
}
