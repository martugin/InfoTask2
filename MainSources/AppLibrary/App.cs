using System;
using BaseLibrary;
using CommonTypes;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : ProcessApp
    {
        public App(string code) 
            : base(code, new AppIndicator()) { }

        //Создание клона источника, выполняется синхронно
        public void MakeClone(string cloneDir) //Каталог клона
        {
            var ti = GetCloneInterval(cloneDir);
            RunSyncCommand(ti.Begin, ti.End, () => RunMakeClone(cloneDir));
        }

        //Создание клона источника, выполняется асинхронно
        public void MakeCloneAsync(DateTime periodBegin, //Начало периода клона
                                                   DateTime periodEnd, //Конец периода клона
                                                   string cloneDir) //Каталог клона
        {
            var ti = GetCloneInterval(cloneDir);
            RunAsyncCommand(ti.Begin, ti.End, () => RunMakeClone(cloneDir));
        }

        private TimeInterval GetCloneInterval(string cloneDir)
        {
            using (var sys = new SysTabl(cloneDir.EndDir() + "Clone.accdb"))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }

        private void RunMakeClone(string cloneDir)
        {
            using (StartProgress("Создание клона"))
                using (StartLog(0, 100, "Создание клона источника"))
                    new ClonerConnect(this, ProvidersFactory).MakeClone(cloneDir);
        }
    }
}