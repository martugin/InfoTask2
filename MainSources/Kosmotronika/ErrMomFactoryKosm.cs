using BaseLibrary;
using CommonTypes;

namespace Kosmotronika
{
    public class KosmMomErrFactory : IMomErrFactory
    {
        //Код источника ошибок
        public string ErrSourceCode
        {
            get { return "KosmotronikaRetroSource"; }
        }

        //Получить описание ошибки по ее номеру
        public ErrDescr GetDescr(int number)
        {
            if (number == 0) return null;
            return new ErrDescr(number, NdText(number), NdQuality(number), MomErrType.Source);
        }

        //Текст в случае неопределенной ошибки
        public string UndefinedErrorText { get { return "Неопределенная ошибка"; } }

        //Качество недостоверности
        private static ErrQuality NdQuality(int nd)
        {
            if (nd == 0) return ErrQuality.Good;
            return nd.GetBit(5) || nd.GetBit(15) ? ErrQuality.Error : ErrQuality.Warning;
        }

        //Расшифровка недостоверности
        private string NdText(int nd)
        {
            if (nd == 0) return "";
            string s = "";
            s = AddNdBit(nd, 15, "НД по КТС", s);
            s = AddNdBit(nd, 14, "Сомнение", s);
            s = AddNdBit(nd, 13, "Зашкал", s);
            s = AddNdBit(nd, 12, "НД по расчетной величине", s);
            s = AddNdBit(nd, 11, "НД по ОДП", s);
            s = AddNdBit(nd, 10, "НД по уставкам диапазона", s);
            s = AddNdBit(nd, 9, "НД по скорости изменения", s);
            s = AddNdBit(nd, 8, "НД по времени изменения", s);
            s = AddNdBit(nd, 7, "Запрет обработки", s);
            s = AddNdBit(nd, 6, "Ручной ввод", s);
            s = AddNdBit(nd, 5, "Условное заведение", s);
            s = AddNdBit(nd, 4, "Опробуемое значение", s);
            s = AddNdBit(nd, 3, "Имитация", s);
            s = AddNdBit(nd, 2, "НД по дублю", s);
            s = AddNdBit(nd, 1, "НД 2 из 3-х", s);
            return s;
        }

        private static string AddNdBit(int nd, int bit, string mess, string res)
        {
            if (!nd.GetBit(bit)) return res;
            string s = res;
            if (s != "") s += "; ";
            s += mess;
            return s;
        }
    }
}