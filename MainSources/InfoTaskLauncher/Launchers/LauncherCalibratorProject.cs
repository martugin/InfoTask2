using System;
using System.Runtime.InteropServices;
using AppLibrary;
using CommonTypes;
using ProcessingLibrary;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для LauncherCalibratorProject
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherCalibratorProject
    {
        //Код проекта
        string Code { get; }
        //Имя проекта
        string Name { get; }
        //Каталог проекта
        string Dir { get; }
        //Каталог локальных данных проекта
        string LocalDir { get; }

        //Открыть потоки 
        void OpenThreads(double periodSeconds, //Частота опроса в секундах
                                   double lateSeconds); //Опоздание источника в секундах

        //Добавить сигнал
        LauncherRealTimeSignal AddSignal(string connectCode, //Код соединения источника
                                                            string fullCode, //Полный код сигнала
                                                            string dataType, //Тип данных
                                                            string signalType, //Тип сигнала
                                                            string infObject, //Свойства объекта
                                                            string infOut = "", //Свойства выхода относительно объекта
                                                            string infProp = ""); //Свойства сигнала относительно выхода
        //Удалить сигнал
        void RemoveSignal(string connectCode, string fullCode);
        //Очистить список сигналов
        void ClearSignals();

        //Запуск процесса
        void StartProcess();
        //Остановка процесса
        void StopProcess();

        //Присвоить в сигналы текущие значения
        void ReadValues();
    }
    
    //---------------------------------------------------------------------------------------------------------
    //Класс для запуска проета калибратора
    [ClassInterface(ClassInterfaceType.None)]
    public class LauncherCalibratorProject : ILauncherCalibratorProject
    {
        internal LauncherCalibratorProject(ItLauncher launcher, CalibratorProject project)
        {
            _launcher = launcher;
            _project = project;
        }

        //Лаунчер
        private readonly ItLauncher _launcher;
        //Ссылка на проект
        private readonly CalibratorProject _project;

        //Код проекта
        public string Code { get { return _project.Code; } }
        //Имя проекта
        public string Name { get { return _project.Name; } }
        //Каталог проекта
        public string Dir { get { return _project.Dir; } }
        //Каталог локальных данных проекта
        public string LocalDir { get { return _project.LocalDir; } }

        //Открыть потоки 
        public void OpenThreads(double periodSeconds, //Частота опроса в секундах
                                              double lateSeconds) //Опоздание источника в секундах
        {
            _project.OpenThreads(periodSeconds, lateSeconds);
        }

        //Ссылки на прокси
        private ProxyConnect UserProxy { get { return _project.UserThread.Proxies["UserProxy"]; } }
        private ProxyConnect ArchiveProxy { get { return _project.ArchiveThread.Proxies["ArchiveProxy"]; } }

        //Добавить сигнал
        public LauncherRealTimeSignal AddSignal(string connectCode, //Код соединения источника
                                                                      string fullCode, //Полный код сигнала
                                                                      string dataType, //Тип данных
                                                                      string signalType, //Тип сигнала
                                                                      string infObject, //Свойства объекта
                                                                      string infOut = "", //Свойства выхода относительно объекта
                                                                      string infProp = "") //Свойства сигнала относительно выхода
        {
            try
            {
                var con = _project.ReadThread.Sources[connectCode];
                var sig = con.AddSignal(fullCode, dataType.ToDataType(), signalType.ToSignalType(), infObject, infOut, infProp);
                ArchiveProxy.AddSignal(con, sig);
                return new LauncherRealTimeSignal(UserProxy.AddSignal(con, sig));
            }
            catch (Exception ex)
            {
                _project.AddError("Ошибка добавления сигнала", ex);
            }
            return null;
        }

        //Удалить сигнал
        public void RemoveSignal(string connectCode, string fullCode)
        {
            try
            {
                var con = _project.ReadThread.Sources[connectCode];
                con.RemoveSignal(fullCode);
                UserProxy.RemoveSignal(connectCode, fullCode);
                ArchiveProxy.RemoveSignal(connectCode, fullCode);
            }
            catch (Exception ex)
            {
                _project.AddError("Ошибка удаления сигнала", ex);
            }
        }

        //Очистить список сигналов
        public void ClearSignals()
        {
            try
            {
                foreach (var p in _project.ArchiveThread.Proxies.Values)
                    p.ClearSignals();
                foreach (var p in _project.ArchiveThread.Receivers.Values)
                    p.ClearSignals();
                foreach (var p in _project.UserThread.Proxies.Values)
                    p.ClearSignals();
                foreach (var p in _project.ReadThread.Proxies.Values)
                    p.ClearSignals();
                foreach (var p in _project.ReadThread.Sources.Values)
                    p.ClearSignals();
            }
            catch (Exception ex)
            {
                _project.AddError("Ошибка очистки списка сигналов", ex);
            }
        }

        //Запуск процесса
        public void StartProcess()
        {
            _project.StartThreads();
        }

        //Остановка процесса
        public void StopProcess()
        {
            _project.StopThreads();
        }

        //Присвоить в сигналы текущие значения
        public void ReadValues()
        {
            try { UserProxy.ReadValues(); }
            catch (Exception ex) { _project.AddError("Ошиба при фиксации значений", ex); }
        }
    }
}