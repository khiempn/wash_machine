using System.Collections.Generic;

namespace WashMachine.Forms.Database.Tables
{
    public interface ICRUD<T>
    {
        int Insert(T model);
        int Update(T model);
        int Delete(T model);
        T Get(T model);
        List<T> GetAll();
    }
}
