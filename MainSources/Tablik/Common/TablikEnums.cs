namespace Tablik
{
    //Стадия компиляции параметра
    internal enum CompileStage
    {
        NotStarted, //Компиляция еще не начиналась
        Started, //Компиляция идет, типы данных не менялись
        Changed, //Компиляция идет, типы данных менялись
        Finished //Компиляция завершена
    }
}