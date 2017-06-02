using BaseLibrary;
using System;

namespace ProcessingLibrary
{
    //Поток разовых расчетов
    public class SingleThread : BaseThread
    {
        public SingleThread(ProcessProject project, int id, string name, IIndicator indicator) 
            : base(project, id, name, indicator, LoggerStability.Single) { }

        //Запуск обработки
        protected override void RunProcess()
        {
            try
            {
                using (StartPeriod(ThreadPeriodBegin, ThreadPeriodEnd, "Разовый"))
                    using (StartProgress("Расчет"))
                    {
                        Start(0, 20).Run(Prepare);
                        if (!CheckFinishing())
                            Start(20, 90).Run(Cycle);
                        Start(90, 100).Run(ClearMemory);
                    }
                MakeStopped();
            }
            catch (BreakException)
            {
                MakeStopped();
                throw;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при работе потока", ex);
            }
        }

        //Подготовка потока
        #region Prepare
        protected override void Prepare()
        {
            Start(0, 60).Run(LoadModules);
            StartLog(60, 80, "Очистка файла результатов").Run(ClearResults);
            StartLog(80, 100, "Удаление старых ведомостей").Run(ClearVed);
        }

        //Очистка результатов
        private void ClearResults()
        {
            throw new NotImplementedException();
        }

        //Удаление старых ведомостей
        private void ClearVed()
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override void Waiting() { }

        //Цикл обработки
        #region Cycle
        protected override void Cycle()
        {
            Start(0, 40).Run(ReadSources);
            Start(40, 60).Run(ClaculateModules);
            Start(60, 70).Run(SaveResults);
            Start(70, 80).Run(MakeVed);
            Start(80, 90).Run(WriteReceivers);
            Start(90, 100).Run(ClearValues);
        }

        //Запись отладочных результатов
        private void SaveResults()
        {
            throw new NotImplementedException();
        }

        //Формирование ведомостей
        private void MakeVed()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}