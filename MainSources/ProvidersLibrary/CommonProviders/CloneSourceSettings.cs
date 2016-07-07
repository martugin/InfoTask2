using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Подключение к клону
    public class CloneSourceSettings : SourceSettings
    {
        //Комплект источника и коммуникатора, через которые сделан клон
        internal string Complect { get; set; }
        //Файл клона
        internal string CloneFile { get; private set; }
        //Кэш для идентификации соединения
        public override string Hash { get { return "Db=" + CloneFile; } }

        //Чтение настроек провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            CloneFile = dic["CloneDir"] + @"\Clone.accdb";
        }

        //Проверка соединения
        public override bool Connect()
        {
            bool b = DaoDb.Check(CloneFile, "InfoTaskClone");
            return b;
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (!Connect())
            {
                AddError(CheckConnectionMessage = "Файл не найден или не является файлом клона");
                return false;
            }
            if (SysTabl.ValueS(CloneFile, "CloneComplect") != Complect)
            {
                AddError(CheckConnectionMessage = "Файл является клоном для другого, несовместимого источника");
                return false;
            }
            CheckConnectionMessage = "Успешное соединение";
            return true;
        }

        public override string CheckSettings(DicS<string> infDic)
        {
            if (infDic["CloneDir"].IsEmpty())
                return "Не указан каталог клона";
            return "";
        }

        protected override void AddMenuCommands()
        {
            throw new NotImplementedException();
        }

        protected override TimeInterval GetSourceTime()
        {
            using (var sys = new SysTabl(CloneFile, false))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }
    }
}