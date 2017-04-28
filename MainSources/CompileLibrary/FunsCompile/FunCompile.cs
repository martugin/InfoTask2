using System.Collections.Generic;
using BaseLibrary;

namespace CompileLibrary
{
    //Одна функция
    internal class FunCompile
    {
        //Конструктор, на входе рекордсет с таблицей Functions
        internal FunCompile(IRecordRead rec)
        {
            Name = rec.GetString("Name");
            Synonym = rec.GetString("Synonym");
            Code = rec.GetString("Code") ?? (Synonym ?? Name);
            Id = rec.GetInt("Id");
        }

        //Код для записи в польскую запись
        internal string Name { get; private set; }
        //Английский синоним
        internal string Synonym { get; private set; }
        //Код на английском, без спецсимволов, маленькими буквами
        internal string Code { get; private set; }
        //Id
        internal int Id { get; private set; }

        //Перегрузки
        private readonly List<FunOverload> _overloads = new List<FunOverload>();
        internal List<FunOverload> Overloads { get { return _overloads; } }
    }
}