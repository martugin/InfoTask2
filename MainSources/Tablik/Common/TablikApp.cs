using System;
using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Общие данные приложения, использующего Tablik
    internal class TablikApp
    {
        public TablikApp()
        {
            LoadFunctions();
        }

        //Словари всех функций ключи - коды или Id
        private readonly DicS<FunClass> _funs = new DicS<FunClass>();
        internal DicS<FunClass> Funs { get { return _funs; } }
        private readonly DicI<FunClass> _funsId = new DicI<FunClass>();
        internal DicI<FunClass> FunsId { get { return _funsId; } }

        //Загрузка списка функций из General.accdb
        private void LoadFunctions()
        {
            try
            {
                Funs.Clear();
                using (var gdb = new DaoDb(ItStatic.InfoTaskDir() + @"General\General.accdb"))
                {
                    using (var rec = new AdoReader(gdb, "SELECT * FROM Functions WHERE IsCompile = True"))
                        while (rec.Read())
                        {
                            //Сначала читаем сами функции
                            var fun = new FunClass(rec);
                            Funs.Add(fun.Name, fun);
                            if (fun.Synonym != null) Funs.Add(fun.Synonym, fun);
                            FunsId.Add(rec.GetInt("Id"), fun);
                        }

                    using (var rec = new AdoReader(gdb, "SELECT FunctionsOverloads.* FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId " +
                                "WHERE Functions. IsCompile = True ORDER BY FunctionsOverloads.FunctionId, FunctionsOverloads.RunNumber"))
                        while (rec.Read())
                        {  //Потом их перегрузки
                            var fun = _funsId[rec.GetInt("FunctionId")];
                            fun.Overloads.Add(new FunOverload(rec, fun));
                        }
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Системная ошибка компилятора. Ошибка загрузки функций");
            }
        }
    }
}