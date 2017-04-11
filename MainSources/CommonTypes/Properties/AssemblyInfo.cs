using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Управление общими сведениями о сборке осуществляется с помощью 
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("CommonTypes")]
[assembly: AssemblyDescription("Общая библиотека для системы InfoTask")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("InfoTask")]
[assembly: AssemblyProduct("CommonTypes")]
[assembly: AssemblyCopyright("Copyright © InfoTask 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Параметр ComVisible со значением FALSE делает типы в сборке невидимыми 
// для COM-компонентов.  Если требуется обратиться к типу в этой сборке через 
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(true)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("aab40517-84a0-46ea-b9c9-387e4cf4cb00")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии 
//      Номер построения
//      Редакция
//
// Можно задать все значения или принять номер построения и номер редакции по умолчанию, 
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

[assembly: InternalsVisibleTo("CommonTypesTest")]
[assembly: InternalsVisibleTo("CalculationTest")]
[assembly: InternalsVisibleTo("GeneratorTest")]
[assembly: InternalsVisibleTo("TablikTest")]
[assembly: InternalsVisibleTo("InfoTaskLauncherTest")]
[assembly: InternalsVisibleTo("Experiments")]
