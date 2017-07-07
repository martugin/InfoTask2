using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Одна функция
    public class FunCompile
    {
        //Конструктор, на входе рекордсет с таблицей Functions
        internal FunCompile(IRecordRead rec)
        {
            Name = rec.GetString("Name");
            Synonym = rec.GetString("Synonym");
            Code = (rec.GetString("Code") ?? (Synonym ?? Name)).ToLower();
            Id = rec.GetInt("Id");
        }

        //Код для записи в польскую запись
        internal string Name { get; private set; }
        //Английский синоним
        internal string Synonym { get; private set; }
        //Код на английском, без спецсимволов, маленькими буквами
        public string Code { get; private set; }
        //Id
        internal int Id { get; private set; }

        //Перегрузки
        private readonly List<FunOverload> _overloads = new List<FunOverload>();
        internal List<FunOverload> Overloads { get { return _overloads; } }

        //Определение подходящей перегрузки, возвращает перегрузку и тип данных
        public FunSelected DefineOverload(DataType[] par, //Типы данных принимемых аргументов
                                                              int startPos = 0, //С какой позиции начинать проверку
                                                              ArrayType[] apar = null) //Типы массива принимаемых аргументов
        {
            foreach (var ov in Overloads)
                return ov.Check(par, startPos, apar);
            return null;
        }
    }
}