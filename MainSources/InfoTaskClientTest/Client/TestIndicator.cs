using System;
using System.Collections.Generic;
using BaseLibrary;
using ComClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoTaskClientTest
{
    //Установка какого-то свойства индикатора
    internal class TestIndicatorEvent
    {
        public TestIndicatorEvent(string type, string text)
        {
            Type = type;
            Text = text;
        }

        //Тип события
        public string Type { get; private set; }
        //Текст события 
        public string Text { get; private set; }
    }
    
    //-------------------------------------------------------------------------------------------------

    //Тестовый индикатор процесса
    internal class TestIndicator : IIndicator
    {
        //Последовательный список событий индикатора
        private readonly List<TestIndicatorEvent> _events = new List<TestIndicatorEvent>();
        public List<TestIndicatorEvent> Events { get { return _events; } }

        //Добавить событие в список
        private void AddEvent(string type, string text = "")
        {
            Events.Add(new TestIndicatorEvent(type, text));
        }

        //Сравнить значение свойства с заданным
        public void Compare(int num, string type, string text = "")
        {
            Assert.AreEqual(type, Events[num].Type);
            Assert.AreEqual(text, Events[num].Text);
        }


        public void OnShowIndicatorTexted(object sender, EventArgs e)
        {
            AddEvent("ShowTexted");
        }

        public void OnShowIndicatorTimed(object sender, EventArgs e)
        {
            AddEvent("ShowTimed");
        }

        public void OnHideIndicator(object sender, EventArgs e)
        {
            AddEvent("Hide");
        }

        public void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            AddEvent("Procent", e.Procent.ToString());
        }

        public void OnSetProcessTimed(object sender, SetProcentTimedEventArgs e)
        {
            AddEvent("ProcessTimed", e.EndTime.ToString());
        }

        public void OnSetProcessUsual(object sender, EventArgs e)
        {
            AddEvent("ProcessUsual");
        }

        private string _curText0 = "";
        private string _curText1 = "";
        private string _curText2 = "";

        public void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
        {
            if (e.TabloText[0] != _curText0)
            {
                _curText0 = e.TabloText[0];
                AddEvent("Text0", _curText0);
            }
            if (e.TabloText[1] != _curText1)
            {
                _curText1 = e.TabloText[1];
                AddEvent("Text1", _curText1);
            }
            if (e.TabloText[2] != _curText2)
            {
                _curText2 = e.TabloText[2];
                AddEvent("Text2", _curText2);
            }
        }

        public void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            AddEvent("BeginPeriod", e.BeginPeriod.ToString());
            AddEvent("EndPeriod", e.EndPeriod.ToString());
            AddEvent("ModePeriod", e.ModePeriod);
        }
    }
}