namespace Tablik
{
    //Стадия компиляции параметра
    public enum CompileStage
    {
        NotStarted, //Компиляция еще не начиналась
        Started, //Компиляция идет, типы данных не менялись
        Changed, //Компиляция идет, типы данных менялись
        Finished //Компиляция завершена
    }
}