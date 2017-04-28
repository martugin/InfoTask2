using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Одна перегрузка функции
    public class FunOverload
    {
        //Конструктор 
        internal FunOverload(FunCompile fun, //Функция-владелец
                                        IRecordRead rec) //Рекордсет с таблицей FunctionsOverloads
        {
            _funCompile = fun;

            RealisationName = _funCompile.Code + "_";
            for (int i = 1; i <= 9; ++i)
            {
                var t = rec.GetString("Operand" + i);
                if (t.IsEmpty()) break;
                var fp = new FunParam(t, rec.GetString("Default" + i));
                _inputs.Add(fp);
                RealisationName += fp.DataType.ToLetter();
            }
            for (int i = 1; i <= 2; ++i)
            {
                var t = rec.GetString("More" + i);
                if (t.IsEmpty()) break;
                var fp = new FunParam(t);
                _inputsMore.Add(fp);
                RealisationName += fp.DataType.ToLetter();
            }

            _isCombined = rec.GetBool("IsCombined");
            var s = rec.GetString("Result");
            if (!_isCombined) _resultType = s.ToDataType();
            else
            {
                var p = s.Split('+');
                foreach (string c in p)
                {
                    if (c == "M1") _inputsMore[0].UsedInResult = true;
                    else if (c == "M2") _inputsMore[1].UsedInResult = true;
                    else _inputs[int.Parse(c) - 1].UsedInResult = true;
                }
            }
        }

        //Параметры
        private readonly List<FunParam> _inputs = new List<FunParam>();
        //Дополнительные параметры
        private readonly List<FunParam> _inputsMore = new List<FunParam>();
        //Тип результата формируется из типов параметрв
        private readonly bool _isCombined;
        //Тип результата (если не Combined)
        private readonly DataType _resultType;
        //Ссылка на FunCompile 
        private readonly FunCompile _funCompile;
        
        //Имя функции-реализации
        internal string RealisationName { get; private set; }

        //Проверка допустимости списка типов аргументов, 
        //Возвращает тип данных, или DataType.Error - если аргументы не подходят
        internal DataType Check(DataType[] par)
        {
            var res = DataType.Value;
            int i, k = 0;
            for (i = 0; i < par.Length; i++)
            {
                if (i < _inputs.Count)
                {
                    var inp = _inputs[i];
                    if (!par[i].LessOrEquals(inp.DataType)) break;
                    if (_isCombined && inp.UsedInResult)
                        res = res.Add(inp.DataType);
                }
                else
                {
                    if (_inputsMore.Count > 0)
                    {
                        var inp = _inputsMore[k];
                        if (!par[i].LessOrEquals(inp.DataType)) break;
                        if (_isCombined && inp.UsedInResult)
                            res = res.Add(inp.DataType);
                        k = (k + 1) % _inputsMore.Count;
                    }
                    else break;
                }
            }
            if (i < par.Length) return DataType.Error;
            if (i <_inputs.Count && _inputs[i].Default.IsEmpty()) return DataType.Error;
            if (!_isCombined) return _resultType;
            return res;
        }
    }
}