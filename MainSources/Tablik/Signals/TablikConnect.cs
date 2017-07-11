using BaseLibrary;
using CommonTypes;
using CompileLibrary;
using Microsoft.Office.Interop.Access.Dao;

namespace Tablik
{
    //Проайдер для компилятора
    internal abstract class TablikConnect : BaseConnect
    {
        protected TablikConnect(TablikProject tablik, string code, string complect) 
            : base(tablik.Project.App, code, complect, tablik.Project.Code) { }

        //Файл исходных сигналов
        protected string SignalsFile { get; set; }

        //Расчетные типы объектов
        private readonly DicS<ObjectType> _objectsCalcTypes = new DicS<ObjectType>();
        public DicS<ObjectType> ObjectsCalcTypes { get { return _objectsCalcTypes; } }
        private readonly DicI<ObjectType> _objectCalcTypesId = new DicI<ObjectType>();
        public DicI<ObjectType> ObjectCalcTypesId { get { return _objectCalcTypesId; } }
        //Типы объектов 
        private readonly DicS<ObjectType> _objectsTypes = new DicS<ObjectType>();
        public DicS<ObjectType> ObjectsTypes { get { return _objectsTypes; } }
        private readonly DicI<ObjectType> _objectsTypesId = new DicI<ObjectType>();
        public DicI<ObjectType> ObjectsTypesId { get { return _objectsTypesId; } }
        //Сигналы типов
        private readonly DicS<TablikSignal> _signals = new DicS<TablikSignal>();
        public DicS<TablikSignal> Signals { get { return _signals; } }
        //Объекты
        private readonly DicS<TablikObject> _objects = new DicS<TablikObject>();
        public DicS<TablikObject> Objects { get { return _objects; } }
        private readonly DicI<TablikObject> _objectsId = new DicI<TablikObject>();
        public DicI<TablikObject> ObjectsId { get { return _objectsId; } }

        //Загрузка сигналов
        public void LoadSignals()
        {
            StartLog("Загрузка сигналов", null, Type + " " + Code).Run(() =>
            {
                ObjectsCalcTypes.Clear();
                ObjectCalcTypesId.Clear();
                ObjectsTypes.Clear();
                ObjectsTypesId.Clear();
                Objects.Clear();
                ObjectsId.Clear();

                using (var db = new DaoDb(SignalsFile))
                {
                    using (var rec = new DaoRec(db, "ObjectTypesCalc"))
                        while (rec.Read())
                        {
                            var t = new ObjectType(rec.GetInt("TypeCalcId"), rec.GetString("CodeType"),
                                rec.GetString("NameType"), rec.GetInt("SignalCodeColumn"));
                            ObjectCalcTypesId.Add(t.Id, t);
                            ObjectsCalcTypes.Add(t.Code, t);
                            ObjectsCalcTypes.Add(Code + "." + t.Code, t);
                        }

                    using (var rec = new DaoRec(db, "ObjectTypes"))
                        while (rec.Read())
                        {
                            var t = new ObjectType(rec.GetInt("TypeId"), rec.GetString("TypeObject"),
                                rec.GetString("TypeName"), 1);
                            ObjectsTypesId.Add(t.Id, t);
                            ObjectsTypes.Add(t.Code, t);
                            ObjectsTypes.Add(Code + "." + t.Code, t);
                            var list = rec.GetString("CalcTypes").ToPropertyList();
                            foreach (var bt in list)
                                if (ObjectsCalcTypes.ContainsKey(bt))
                                    t.BaseTypes.Add(ObjectsCalcTypes[bt]);
                        }

                    using (var rec = new DaoRec(db, "Objects"))
                        while (rec.Read())
                        {
                            var t = rec.GetString("TypeObject");
                            if (ObjectsTypes.ContainsKey(t))
                            {
                                var ob = new TablikObject(this, ObjectsTypes[t], rec);
                                ObjectsId.Add(ob.Id, ob);
                                Objects.Add(ob.Code, ob);
                                Objects.Add(Code + "." + ob.Code, ob);
                                foreach (Field f in rec.Recordset.Fields)
                                    if (f.Name != "Otm" && f.Name != "ObjectId" && f.Name != "SysField")
                                    {
                                        var dt = f.Type.ToDataType();
                                        var m = rec.GetMean(dt, f.Name);
                                        ob.Props.Add(f.Name, new ObjectProp(f.Name, dt, m));
                                    }
                            }
                        }

                    using (var rec = new DaoRec(db, "SignalsCalc"))
                        while (rec.Read())
                        {
                            var s = new TablikSignal(rec);
                            var t = ObjectCalcTypesId[rec.GetInt("ObjectId")];
                            t.Signals.Add(s.Code, s);
                            Signals.Add(t.Code + "." + s.Code, s);
                            Signals.Add(Code + "." + t.Code + "." + s.Code, s);
                        }

                    using (var rec = new DaoRec(db, "Signals"))
                        while (rec.Read())
                        {
                            var s = new TablikSignal(rec);
                            var t = ObjectsTypesId[rec.GetInt("TypeId")];
                            t.Signals.Add(s.Code, s);
                            Signals.Add(t.Code + "." + s.Code, s);
                            Signals.Add(Code + "." + t.Code + "." + s.Code, s);
                            foreach (var bt in t.BaseTypes)
                            {
                                var bcode = rec.GetString("CodeSignal" + (bt.SignalCodeColumn == 1 ? "" : bt.SignalCodeColumn.ToString()));
                                if (!bcode.IsEmpty() && t.Signals.ContainsKey(bcode))
                                    ((TypeSignal)bt.Signals[bcode]).Signal = s;
                            }
                        }
                }
            });
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //Источник для компилятора
    internal class TablikSource : TablikConnect
    {
        public TablikSource(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect)
        {
            SignalsFile = tablik.Project.Dir + @"SignalsSource\" + code + ".accdb";
        }

        //Тип
        public override ProviderType Type { get { return ProviderType.Source; }}
    }

    //--------------------------------------------------------------------------------------------------------
    //Приемник для компилятора
    internal class TablikReceiver : TablikConnect
    {
        public TablikReceiver(TablikProject tablik, string code, string complect)
            : base(tablik, code, complect)
        {
            SignalsFile = tablik.Project.Dir + @"SignalsReceiver\" + code + ".accdb";
        }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
    }
}