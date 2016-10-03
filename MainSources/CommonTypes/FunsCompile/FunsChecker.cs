using System;
using BaseLibrary;

namespace CommonTypes
{
    //��� ������ �������, ��� ���� �� ������������
    public enum FunsCheckType
    {
        Calc, //������
        Gen //���������
    }

    //------------------------------------------------------------------------------------------------------------
    //������ ���� ������� ��� ����������
    public class FunsChecker
    {
        public FunsChecker(FunsCheckType listType)
        {
            bool isGenerate = listType == FunsCheckType.Gen;
            using (var db = new DaoDb(DifferentIt.InfoTaskDir() + @"General\General.accdb"))
            {
                var funsId = new DicI<FunCompile>();
                var where = " WHERE (Functions.NotLoadCompile = False)" + (isGenerate ? "AND (Functions.LoadGen = True) " : " ");
                using (var rec = new ReaderAdo(db, "SELECT * FROM Functions" + where))
                    while (rec.Read())
                    {
                        var f = new FunCompile(rec);
                        funsId.Add(f.Id, f);
                        _funs.Add(f.Name, f);
                        if (!f.Synonym.IsEmpty())
                            _funs.Add(f.Synonym, f);
                    }    
                using ( var rec = new ReaderAdo(db, "SELECT FunctionsOverloads.* FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId "
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

        //������� ���� �������, ����� - ��� ��������� �����
        private readonly DicS<FunCompile> _funs = new DicS<FunCompile>();

        //����������, ����� ���������� ������� ������������ � ���������
        //���������� ���� - ��� ���������� ������� � ��� ������ ����������
        //���� ���������� �� �������, �� ���������� ��������� �� ������
        public Tuple<string, DataType> DefineFun(string name, //��� ������� � ���������
                                                 params DataType[] par) //���� ���������� � ���������
        {
            if (!_funs.ContainsKey(name))
                return new Tuple<string, DataType>("����������� �������", DataType.Error);
            foreach (var ov in _funs[name].Overloads)
            {
                var dt = ov.Check(par);
                if (dt != DataType.Error)
                    return new Tuple<string, DataType>(ov.RealisationName, dt);
            }
            return new Tuple<string, DataType>("������������ ���� ������ ���������� �������", DataType.Error);
        }
    }
}