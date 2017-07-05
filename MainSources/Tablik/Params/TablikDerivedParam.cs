using BaseLibrary;
using CompileLibrary;

namespace Tablik
{
    //Порожденный параметр
    internal class TablikDerivedParam : BaseDerivedParam
    {
        public TablikDerivedParam(string paramCode, string fullCode, string paramName, string fullName, string task, 
                                                 SuperProcess superProcess, string units, double? min, double? max, string objectCode)
        {
            ParamCode = paramCode;
            Code = fullCode;
            ParamName = paramName;
            Name = fullName;
            Task = task;
            SuperProcess = superProcess;
            Units = units;
            Min = min;
            Max = max;
            ObjectCode = objectCode;
        }

        public void ToRecordset(IRecordAdd rec)
        {
            rec.AddNew();
            rec.Put("ParamCode", ParamCode);
            rec.Put("FullCode", Code);
            rec.Put("ParamName", ParamName);
            rec.Put("FullName", Name);
            rec.Put("Task", Task);
            rec.Put("SuperProcess", SuperProcess.ToRussian());
            rec.Put("Units", Units);
            rec.Put("Min", Min);
            rec.Put("Max", Max);
            rec.Put("ObjectCode", ObjectCode);
            rec.Update();
        }
    }
}