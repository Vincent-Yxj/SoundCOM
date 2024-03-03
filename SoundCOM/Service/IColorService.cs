

using System.Drawing;

namespace SoundCOM.Service;

public interface IColorService
{
    void Save_Config(string? valuecolor = "blue", string? maxcolor = "red", string? mincolor = "greed");
    List<string> Get_Config();
    List<string> Get_ColorList();
}
