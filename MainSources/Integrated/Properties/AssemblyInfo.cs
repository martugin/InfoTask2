using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Управление общими сведениями о сборке осуществляется с помощью 
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("Integrated")]
[assembly: AssemblyDescription("Сборка, объединяющая вызов всех библиотет InfoTask")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("InfoTask")]
[assembly: AssemblyProduct("Integrated")]
[assembly: AssemblyCopyright("Copyright © InfoTask 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Параметр ComVisible со значением FALSE делает типы в сборке невидимыми 
// для COM-компонентов.  Если требуется обратиться к типу в этой сборке через 
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("7655c873-16ca-41b0-823c-4e21902f7e2c")]

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

[assembly: InternalsVisibleTo("BaseLibraryTest")]
[assembly: InternalsVisibleTo("CommonTypesTest")]
[assembly: InternalsVisibleTo("CalculationTest")]
[assembly: InternalsVisibleTo("GeneratorTest")]
[assembly: InternalsVisibleTo("TablikTest")]
[assembly: InternalsVisibleTo("ProvidersTest")]
[assembly: InternalsVisibleTo("InfoTaskLauncherTest")]
[assembly: InternalsVisibleTo("Experiments")]