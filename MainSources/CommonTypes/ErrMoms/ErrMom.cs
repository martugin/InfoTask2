﻿using BaseLibrary;

namespace CommonTypes
{
    //Полная ошибка мгновенного значения
    public class ErrMom
    {
        public ErrMom(ErrDescr errDescr, IContextable addr)
        {
            ErrDescr = errDescr;
            AddressLink = addr;
        }

        //Ссылка на описание ошибки
        public ErrDescr ErrDescr { get; private set; }
        //Ссылка на объект, породивший ошибку
        public IContextable AddressLink { get; private set; }

        //Строка - адрес происхождения ошибки
        public string Address { get { return AddressLink.Context; } }
        //Сообщение об ошибке
        public string Text { get { return ErrDescr.Text; } }
        //Качество ошибки
        public ErrorQuality Quality { get { return ErrDescr.Quality; } }
        //Номер ошибки
        public int Number { get { return ErrDescr.Number; } }
        //Номер ошибки
        public ErrMomType ErrType { get { return ErrDescr.ErrType; } }
    }
}