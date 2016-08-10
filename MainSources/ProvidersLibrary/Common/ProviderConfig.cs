using System.IO;
using CommonTypes;

namespace ProvidersLibrary
{
    //Описание одного комплекта провайдеров из Config
    public class ComplectConfig
    {
        public ComplectConfig(string complect, string dllFile)
        {
            Complect = complect;
            DllFile = DifferentIT.InfoTaskDir() + dllFile;
            if (DllFile != null)
            {
                var fileInfo = new FileInfo(DllFile);
                if (fileInfo.Exists)
                    DllDir = fileInfo.DirectoryName;    
            }
        }

        //Код комплекта провайдера
        public string Complect { get; private set; }

        //Пути к файлу и папке dll комплекта провайдеров
        public string DllFile { get; private set; }
        public string DllDir { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------------------

    //Описание одного провайдера из Config
    public class ProviderConfig
    {
        public ProviderConfig(ComplectConfig complect, ProviderType type, string code)
        {
            Complect = complect;
            Type = type;
            Code = code;
        }
        
        //Тип провайдера
        public ProviderType Type { get; private set; }
        //Код провайдера
        public string Code { get; private set; }
        //Комплект провайдеров
        public ComplectConfig Complect { get; private set; }
    }
}