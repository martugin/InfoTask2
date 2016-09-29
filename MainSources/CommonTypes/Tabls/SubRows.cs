﻿using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для строк таблиц и данных старшей таблицы
    public abstract class SubRows
    {
        //Список строк подчиненных таблиц
        private List<TablRow> _subList;
        public List<TablRow> SubList { get { return _subList ?? (_subList = new List<TablRow>()); } }
        //Словари строк подчиненных таблиц по полям Num и Code
        private DicI<TablRow> _subNums;
        public DicI<TablRow> SubNums { get { return _subNums ?? (_subNums = new DicI<TablRow>()); } }
        private DicS<TablRow> _subCodes;
        public DicS<TablRow> SubCodes { get { return _subCodes ?? (_subCodes = new DicS<TablRow>()); } }
        //Словарь списков полей, сгруппированых по полю Type 
        private DicS<List<TablRow>> _subTypes;
        public DicS<List<TablRow>> SubTypes { get { return _subTypes ?? (_subTypes = new DicS<List<TablRow>>()); } }
        //Ряд - родитель или null, если это данные старшей таблицы
        public SubRows Parent { get; set; }

        //Добавить строку подтаблицы
        public void AddRow(TablRow row)
        {
            row.Parent = this;
            SubList.Add(row);
            SubNums.Add(row.Num, row);
            if (!row.Code.IsEmpty())
                SubCodes.Add(row.Code, row);
            if (!row.Type.IsEmpty())
            {
                if (!SubTypes.ContainsKey(row.Type))
                    SubTypes.Add(row.Type, new List<TablRow>());
                SubTypes[row.Type].Add(row);
            }
        }
    }
}