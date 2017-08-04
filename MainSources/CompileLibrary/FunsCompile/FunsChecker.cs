using System;
using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Список всех функций для компиляции
    public class FunsChecker
    {
        public FunsChecker(FunsCheckType listType)
        {
            try
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
                        }
                    }

                    using (var rec = new AdoReader(db, "SELECT * FROM ParamProps"))
                        while (rec.Read())
                        {
                            var dt = rec.GetString("DataType").ToDataType();
                            ParamProps.Add(rec.GetString("EnglishCode"), dt);
                            ParamProps.Add(rec.GetString("RussianCode"), dt);
                        }
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Системная ошибка компилятора. Ошибка загрузки функций");
            }
        }

        //Словарь всех функций, ключи - все возможные имена
        private readonly DicS<FunCompile> _funs = new DicS<FunCompile>();
        public DicS<FunCompile> Funs { get { return _funs; } }

        //Словарь свойств расчетного параметра
        private readonly DicS<DataType> _paramProps = new DicS<DataType>();
        public DicS<DataType> ParamProps { get { return _paramProps; } }
    }
}