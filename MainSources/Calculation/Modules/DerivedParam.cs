using BaseLibrary;
using CompileLibrary;

namespace Calculation
{
    //Порожденный параметр
    internal class DerivedParam : BaseDerivedParam
    {
        public DerivedParam(IRecordRead rec)
        {
            Code = rec.GetString("FullCode");
            ParamCode = rec.GetString("ParamCode");
            Name = rec.GetString("FullName");
            ParamName = rec.GetString("ParamName");
            Task = rec.GetString("Task");
            SuperProcess = rec.GetString("SuperProcess").ToSuperProcess();
            LoadEdited(rec);
        }

        public void LoadEdited(IRecordRead rec)
        {
            Units = rec.GetString("Units");
            Min = rec.GetDouble("Min");
            Max = rec.GetDouble("Max");
            ObjectCode = rec.GetString("ObjectCode");
        }
    }
}