﻿using System;
using System.Reflection;
using System.Windows.Forms;

namespace CommonTypes
{
    //Базовый класс для скалярных функций
    public abstract class ScalarBaseFun : CalcBaseFun
    {
        protected ScalarBaseFun(FunctionsBase funs, string code, int errNum) 
            : base(funs, code, errNum) { }

        //Делегат скалярных функций
        public delegate void ScalarDelegate(IMean[] par);
        //Ссылка на реализацию функции
        public ScalarDelegate Fun { get; private set; }
        //Создание экземпляра делегата функции
        protected override void CreateDelegateInstance(FunctionsBase funs, MethodInfo met)
        {
            Fun = (ScalarDelegate) Delegate.CreateDelegate(typeof (ScalarDelegate), funs, met);
        }
    }
}