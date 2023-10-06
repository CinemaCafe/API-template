using System.Data;

namespace _UniTechService.Interfaces
{
    public enum fLoginDB { MS_2017, MS_2008, MS_2000, MS_UTADM }
    public interface IUniService
    {
        Task<IEnumerable<TReturn>> SqlCmdModel<TReturn>(string querySql, object param, fLoginDB loginDB, CommandType commandType = CommandType.Text);
        Task<int> SqlExecAsync(string querySql, object param, fLoginDB loginDB, CommandType commandType = CommandType.Text);

    }
}