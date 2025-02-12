using System.Data;

namespace Libraries.Services
{
    public interface IExportExcelService
    {
        byte[] Export(DataTable dataTable);
    }
}
