namespace CompileLibrary
{
    //Один график для расчета
    public class Grafic
    {
        public Grafic(string code, int dim, GraficType graficType)
        {
            Code = code;
            Dim = dim;
            GraficType = graficType;
        }

        //Код графика
        public string Code { get; private set; }
        //Размерность
        public int Dim { get; private set; }
        //Тип графика
        public GraficType GraficType { get; private set; }
    }
}