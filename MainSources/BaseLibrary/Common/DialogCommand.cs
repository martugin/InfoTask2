using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Тип комманды открытия диалога
    public enum DialogType
    {
        OpenFile,
        OpenExcel,
        CreateFile,
        CreateExcel,
        OpenDir
    }

    //--------------------------------------------------------------------
    //Интерфейс для комманд меню
    public interface IMenuCommand
    {
        //Запуск комманды, возвращающей новое значение свойства, initial - исходное значение своиства
        string Run(string initial);
    }

    //--------------------------------------------------------------------
    //Описание одной комманды открытия диалога
    public class DialogCommand : IMenuCommand
    {
        public DialogCommand(DialogType commandType)
        {
            CommandType = commandType;
            if (commandType == DialogType.OpenExcel || commandType == DialogType.CreateExcel)
            {
                Extension = "xlsx";
                DialogFilter = @"Файлы MS Excel (.xlsx) | *.xlsx";
            }
            else
            {
                Extension = "accdb";
                DialogFilter = @"Файлы MS Access (.accdb) | *.accdb";    
            }
            switch (commandType)
            {
                case DialogType.OpenFile:
                case DialogType.OpenExcel:
                    DialogTitle = "Открытие файла";
                    ErrorMessage = "Выбран недопустимый файл";
                    break;
                case DialogType.OpenDir:
                    DialogTitle = "Открытие каталога";
                    ErrorMessage = "Выбран недопустимый каталог";
                    break;
                case DialogType.CreateFile:
                case DialogType.CreateExcel:
                    DialogTitle = "Создание файла";
                    ErrorMessage = "Указан недопустимый файл";
                    break;
            }
        }

        //Тип комманды
        public DialogType CommandType { get; private set; }
        //Расширение файла
        public string Extension { get; set; }
        //Заголовок и фильтр для диалогового окна
        public string DialogTitle { get; set; }
        public string DialogFilter { get; set; }
        //Список таблиц для проверки файла
        public IEnumerable<string> FileTables { get; set; }
        //Сообщение, выдаваемое при ошибке
        public string ErrorMessage { get; set; }
        //Путь к шаблону файла для создания
        public string TemplateFile { get; set; }

        //Запуск комманды, initial - путь к файлу или каталогу заданнный до выхода
        //Возвращает путь к открытому или созданному файлу или каталогу
        public string Run(string initial = null)
        {
            switch (CommandType)
            {
                case DialogType.OpenFile:
                case DialogType.OpenExcel:
                    return RunOpenFile(initial);
                case DialogType.CreateFile:
                case DialogType.CreateExcel:
                    return RunCreateFile(initial);
                case DialogType.OpenDir:
                    return RunOpenDir(initial);
            }
            return null;
        }

        //Запуск открытия файла
        private string RunOpenFile(string initial)
        {
            var op = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = Extension,
                Multiselect = false,
                Title = DialogTitle,
                Filter = DialogFilter
            };
            op.ShowDialog();
            if (op.FileName.IsEmpty()) return initial;
            if (FileTables != null && !DaoDb.Check(op.FileName, FileTables))
            {
                Different.MessageError(ErrorMessage, op.FileName);
                return initial;
            }
            return op.FileName;
        }

        //Запуск создания файла
        private string RunCreateFile(string initial)
        {
            var op = new OpenFileDialog
            {
                CheckFileExists = false,
                AddExtension = true,
                DefaultExt = Extension,
                Multiselect = false,
                FileName = "NewFile." + Extension,
                Title = DialogTitle,
                Filter = DialogFilter
            };
            op.ShowDialog();
            try
            {
                if (op.FileName.IsEmpty())
                    return initial;
                if (TemplateFile != null)
                {
                    var f = new FileInfo(op.FileName);
                    if (!f.Exists || Different.MessageQuestion("По указанному пути файл уже существует. Заменить файл на новый?"))
                    {
                        if (f.Directory != null && !f.Directory.Exists) 
                            f.Directory.Create();
                        var fnew = new FileInfo(TemplateFile);
                        fnew.CopyTo(f.FullName, true);
                    }
                }
                return op.FileName;
            }
            catch (Exception ex)
            {
                ex.MessageError(ErrorMessage);
                return initial;
            }            
        }

        //Запуск открытия каталога
        private string RunOpenDir(string initial)
        {
            var op = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = initial
            };
            op.ShowDialog();
            try
            {
                string s = op.SelectedPath;
                if (s.IsEmpty()) return initial;
                var d = new DirectoryInfo(s);
                if (!d.Exists) d.Create();
                return s;
            }
            catch (Exception ex)
            {
                ex.MessageError(ErrorMessage);
                return initial;
            }
        }
    }
}
