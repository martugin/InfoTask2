using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Общие функции для InfoTask и конвертеры 
    public static class DifferentProviders
    {
        //Определяет, является ли провайдер источником данных
        public static bool IsProviderSource(this ProviderType t)
        {
            return t == ProviderType.Source || t == ProviderType.Imitator;
        }

        //Перевод из строки в ProviderType
        public static ProviderType ToProviderType(this string t)
        {
            if (t == null) return ProviderType.Error;
            switch (t.ToLower())
            {
                case "коммуникатор":
                case "communicator":
                    return ProviderType.Communicator;
                case "коммуникаторприемника":
                case "communicatorreceiver":
                    return ProviderType.CommReceiver;
                case "источник":
                case "source":
                    return ProviderType.Source;
                case "архив":
                case "archive":
                    return ProviderType.Archive;
                case "приемник":
                case "receiver":
                    return ProviderType.Receiver;
                case "имитатор":
                case "imitator":
                    return ProviderType.Imitator;
            }
            return ProviderType.Error;
        }

        //Перевод из ProviderType в русское имя
        public static string ToRussian(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Коммуникатор";
                case ProviderType.CommReceiver:
                    return "КоммуникаторПриемника";
                case ProviderType.Source:
                    return "Источник";
                case ProviderType.Archive:
                    return "Архив";
                case ProviderType.Receiver:
                    return "Приемник";
                case ProviderType.Imitator:
                    return "Имитатор";
            }
            return "Ошибка";
        }

        //Перевод из ProviderType в английское имя
        public static string ToEnglish(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Communicator";
                case ProviderType.CommReceiver:
                    return "CommunicatorReceiver";
                case ProviderType.Source:
                    return "Source";
                case ProviderType.Archive:
                    return "Archive";
                case ProviderType.Receiver:
                    return "Receiver";
                case ProviderType.Imitator:
                    return "Imitator";
            }
            return "Error";
        }

        //ToDo Переделать
        //Список совместимых кодов провайдеров для выбранного кода провайдера
        public static List<string> JointProviderCodes(this string code)
        {
            var list = new List<string> { code };
            using (var rec = new RecDao(DifferentIT.GetInfoTaskDir() + @"General\Config.accdb", "SysTabl"))
            {
                if (code == "CloneSource")
                {
                    while (rec.Read())
                        if (rec.GetString("ParamValue") == "Источник" && rec.GetString("ParamName") != "CloneSource")
                            list.Add(rec.GetString("ParamName"));
                }
                else
                {
                    rec.FindFirst("ParamName", code);
                    if (!rec.NoMatch())
                    {
                        var set = rec.GetString("ParamTag").ToPropertyDicS()["JointProviders"].ToPropertyHashSet();
                        foreach (var c in set)
                        {
                            rec.FindFirst("ParamName", c);
                            if (!rec.NoMatch()) list.Add(c);
                        }
                    }
                }
            }
            return list;
        }
    }
}