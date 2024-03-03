

namespace SoundCOM.Service;

public interface IDataGridService
{
    List<ComdataGrid> Get_Data();
    void Add(ComdataGrid data);
    void Clear();

    bool Save_file();
}
