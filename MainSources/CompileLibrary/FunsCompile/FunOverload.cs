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
            _isCombined = rec.GetBool("IsCombined");
            _arrayType = rec.GetString("ResultArray").ToArrayType();
            Code = fun.Code + "_";

            for (int i = 1; i <= 9; ++i)
            {
                var dtype = rec.GetString("Operand" + i);
                if (dtype.IsEmpty()) break;
                string atype = i == 1 ? rec.GetString("Operand1Array") : "";
                var fp = new FunParam(dtype, atype, rec.GetString("Default" + i));
                _inputs.Add(fp);
                Code += fp.DataType.ToLetter();
            }
            for (int i = 1; i <= 2; ++i)
            {
                var dtype = rec.GetString("More" + i);
                if (dtype.IsEmpty()) break;
                var fp = new FunParam(dtype);
                _inputsMore.Add(fp);
                Code += fp.DataType.ToLetter();
            }
            
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
        //Тип массива
        private readonly ArrayType _arrayType;

        //Код для записи в скомпилированное выражение
        public string Code { get; private set; }

        //Проверка допустимости списка типов аргументов, 
        //Если перегрузка подходит, то возвращает ее вместе с итоговым типом данных, иначе возвращает null
        internal FunSelected Check(DataType[] par, //Типы данных принимемых аргументов
                                                 int startPos, //С какой позиции начинать проверку
                                                 ArrayType[] apar) //Типы массива принимаемых аргументов
        {
            var res = DataType.Value;
            int i, k = 0;
            for (i = startPos; i < par.Length; i++)
            {
                if (i < _inputs.Count)
                {
                    var inp = _inputs[i];
                    if (!par[i].LessOrEquals(inp.DataType)) break;
                    if (apar != null && apar[i] != inp.ArrayType) break;
                    if (_isCombined && inp.UsedInResult)
                        res = res.Add(inp.DataType);
                }
                else
                {
                    if (_inputsMore.Count > 0)
                    {
                        var inp = _inputsMore[k];
                        if (!par[i].LessOrEquals(inp.DataType)) break;
                        if (apar != null && apar[i] != inp.ArrayType) break;
                        if (_isCombined && inp.UsedInResult)
                            res = res.Add(inp.DataType);
                        k = (k + 1) % _inputsMore.Count;
                    }
                    else break;
                }
            }
            if (i < par.Length) return null;
            if (i <_inputs.Count && _inputs[i].Default.IsEmpty()) return null;
            return new FunSelected(this, _isCombined ? res : _resultType, _arrayType);
        }
    }
}