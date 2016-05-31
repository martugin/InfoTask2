using System;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Тип базы данных
    public enum DatabaseType
    {
        Access,
        SqlServer,
        OleDb //Другие соединения OldDb
    }

    //---------------------------------------------------------------------------------------
    //Общий интерфейс для всех рекордсетов читающих данные
    public interface IRecordRead : IDisposable
    {
        bool Read();
        bool EOF { get;}
        bool HasRows { get; }

        string GetString(string field, string nullValue = null);
        string GetString(int num, string nullValue = null);
        double GetDouble(string field, double nullValue = 0);
        double GetDouble(int num, double nullValue = 0);
        double? GetDoubleNull(string field);
        double? GetDoubleNull(int num);
        int GetInt(string field, int nullValue = 0);
        int GetInt(int num, int nullValue = 0);
        int? GetIntNull(string field);
        int? GetIntNull(int num);
        DateTime GetTime(string field);
        DateTime GetTime(int num);
        DateTime? GetTimeNull(string field);
        DateTime? GetTimeNull(int num);
        bool GetBool(string field, bool nullValue = false);
        bool GetBool(int num, bool nullValue = false);
        bool? GetBoolNull(string field);
        bool? GetBoolNull(int num);

        bool IsNull(string field);
        bool IsNull(int num);

        void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null);
        void GetToDataGrid(string field, DataGridViewRow row, string gridField = null);
    }

    //---------------------------------------------------------------------------------------------
    //Интерфейс добавления данных в таблицу
    public interface IRecordAdd : IDisposable
    {
        void AddNew();
        void Update();

        void Put(string field, string val, bool cut = false);
        void Put(string field, double val);
        void Put(string field, double? val);
        void Put(string field, int val);
        void Put(string field, int? val);
        void Put(string field, DateTime val);
        void Put(string field, DateTime? val);
        void Put(string field, bool val);
        void Put(string field, bool? val);
        void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null);
        void PutFromDataGrid(string field, DataGridViewRow row, string gridField = null);
    }
    
    //---------------------------------------------------------------------------------------------
    //Общий интерфейс для всех рекордсетов позволяющих запись, чтение и поиск данных
    public interface IRecordSet : IRecordRead, IRecordAdd
    {
        int RecordCount { get; }
        bool MoveFirst();
        bool MoveLast();
        bool MoveNext();
        bool MovePrevious();
        bool BOF { get; }

        bool FindFirst(string field, string val);
        bool FindFirst(string field, int val);
        bool FindLast(string field, string val);
        bool FindLast(string field, int val);
        bool FindNext(string field, string val);
        bool FindNext(string field, int val);
        bool FindPrevious(string field, string val);
        bool FindPrevious(string field, int val);
    }
}
