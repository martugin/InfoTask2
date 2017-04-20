﻿namespace CommonTypes
{
    //Тип провайдера
    public enum ProviderType
    {
        Source,
        Receiver,
        HandInput,
        Archive,
        Error
    }

    //---------------------------------------------------------------------------------------------------------------------------
    //Тип значения
    public enum ValueType
    {
        Moments, //Одно значение или список значений по времени
        Segments, //Сегменты
        List, //Список
        DicInt, //Словарь с целыми ключами
        DicString, //Словарь со строковыми ключами
        Signal, //Сигнал
        Param, //Параметр
        Var, //Переменная
        Tabl, //Таблица
        Grafic, //График
        Prev, //Параметр функции Пред
        Void, //Нет значения
        Error, //Значение после ошибки парсера
        Undef, //Значение еще не определено
        Max //Максимум в иерархии классов, ошибка
    }

    //------------------------------------------------------------------------------
    //Тип ошибки
    public enum ErrQuality
    {
        Good = 0,
        Warning = 1,
        Error = 2
    }

    //------------------------------------------------------------------------------
    //Тип функции
    public enum FunType
    {
        Scalar, //Скалярные
        ScalarComplex, //Скалярные с указанием переменных, используемых в данной точке
        ScalarObject, //Скалярные с первым не скалярным параметром 
        Const, //Без параметров 
        Object, //С первым не скалярным параметром (Tabl, Prev), а остальными - Mom
        Moments, //Работа со списками мгновенных значений
        CalcData, //Работа с сегментами и статистические функции
        Calc, //Работа с расчетными значениями
        Val, //Общие функции
        Operator //Операторы
    }
    
    //--------------------------------------------------------------------
    //Тип графика
    public enum GraficType
    {
        Grafic,//График
        Grafic0,//График - ступенчатая интерполяция
        Diagramm,//Диаграмма
        Error
    }

    //----------------------------------------------------------------------
    //Типы накопления
    public enum SuperProcess
    {
        Moment, //Мгновенные
        First, //Первое
        Last, //Последнее
        Min, //Минимум
        Max, //Максимум
        Average, //Среднее
        Summ, //Сумма
        None, //Нет накопления
        Error //Не правильно заполненное
    }

    //-----------------------------------------------------------------
    //Приложения
    public enum ApplicationType
    {
        Constructor,//Конструктор расчетов
        Excel,//Построитель отчетов Excel
        PowerPoint,//Построитель отчетов PowerPoint
        Controller, //Контроллер расчетов
        Analyzer,//Анализатор 
        Ras,//РАС
        Error//Ошибка
    }
   
    //--------------------------------------------------------------------------
   //Тип отчета в архиве
    public enum ReportType
    {
        Calc,
        Template,
        Source,
        Excel,
        PowerPoint,
        Error
    }

    //--------------------------------------------------------------------------
    //Тип интервала для записи в архив
    public enum IntervalType
    {
        Single,
        Named,
        NamedAdd, //Именованный интервал с добавлением по времени и параметрам
        NamedAddParams, //Именованный интервал с добавлением по параметрам
        Periodic,
        Moments,
        Base,
        Hour,
        Day,
        Absolute,
        AbsoluteDay,
        Combined, //Для запроса данных для отчета Базовые, Часовые, Суточные, Часовые, Базовые 
        Empty
    }

    //--------------------------------------------------------------------------
    //Режим формирования имитационного значения
    public enum ImitMode 
    {
        NoImit,
        FromBegin,
        FromHour,
        FromDay
    }
}