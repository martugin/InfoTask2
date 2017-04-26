using System;
using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Тип списка функций, для чего он предназначен
    public enum FunsCheckType
    {
        Calc, //Расчет
        Gen //Генерация
    }

    //------------------------------------------------------------------------------------------------------------
    //Список всех функций для компиляции
    public class FunsChecker
    {
        public FunsChecker(FunsCheckType listType)
        {
            bool isGenerate = listType == FunsCheckType.Gen;
            using (var db = new DaoDb(ItStatic.InfoTaskDir() + @"General\General.accdb"))
            {
                var funsId = new DicI<FunCompile>();
                var where = " WHERE (Functions.IsCompile = True)" + (isGenerate ? "AND (Functions.IsGen = True) " : " ");
                using (var rec = new AdoReader(db, "SELECT * FROM Functions" + where))
                    while (rec.Read())
                    {
                        var f = new FunCompile(rec);
                        funsId.Add(f.Id, f);
                        _funs.Add(f.Name, f);
                        if (!f.Synonym.IsEmpty())
                            _funs.Add(f.Synonym, f);
                    }    
                using ( var rec = new AdoReader(db, "SELECT FunctionsOverloads.* FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId "
                                                    + where + "ORDER BY FunctionsOverloads.FunctionId, FunctionsOverloads.RunNumber"))
                {
                    rec.Read();
                    while (!rec.EOF)
                    {
                        var id = rec.GetInt("FunctionId");
                        var fun = funsId[id];
                        while (rec.GetInt("FunctionId") == id)
                        {
                            fun.Overloads.Add(new FunOverload(fun, rec));
                            if (!rec.Read()) break;
                        }
                    }}
            }
        }

        //Словарь всех функций, ключи - все возможные имена
        private readonly DicS<FunCompile> _funs = new DicS<FunCompile>();

        //Определяет, какая перегрузка функции используется в выражении
        //Возвращает пару - код реализации функции и тип данных результата
        //Если перегрузка не найдена, то возвращает сообщение об ошибке
        public Tuple<string, DataType> DefineFun(string name, //Имя функции в выражении
                                                 params DataType[] par) //Типы параметров в выражении
        {
            if (!_funs.ContainsKey(name))
                return new Tuple<string, DataType>("Неизвестная функция", DataType.Error);
            foreach (var ov in _funs[name].Overloads)
            {
                var dt = ov.Check(par);
                if (dt != DataType.Error)
                    return new Tuple<string, DataType>(ov.RealisationName, dt);
            }
            return new Tuple<string, DataType>("Недопустимые типы данных параметров функции", DataType.Error);
        }
    }
}