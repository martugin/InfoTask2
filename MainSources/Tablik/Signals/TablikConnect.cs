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
        private readonly DicS<ObjectType> _baseObjectsTypes = new DicS<ObjectType>();
        public DicS<ObjectType> BaseObjectsTypes { get { return _baseObjectsTypes; } }
        //Типы объектов 
        private readonly DicS<ObjectType> _objectsTypes = new DicS<ObjectType>();
        public DicS<ObjectType> ObjectsTypes { get { return _objectsTypes; } }
        //Объекты
        private readonly DicS<TablikObject> _objects = new DicS<TablikObject>();
        public DicS<TablikObject> Objects { get { return _objects; } }

        //Загрузка сигналов
        public void LoadSignals()
        {
            StartLog("Загрузка сигналов", null, Type + " " + Code).Run(() =>
            {
                BaseObjectsTypes.Clear();
                ObjectsTypes.Clear();
                Objects.Clear();
                var typesId = new DicI<ObjectType>();
                var baseTypesId = new DicI<ObjectType>();
                var objectsId = new DicI<TablikObject>();

                using (var db = new DaoDb(SignalsFile))
                {
                    using (var rec = new DaoRec(db, "BaseObjectTypes"))
                        while (rec.Read())
                        {
                            var t = new ObjectType(rec.GetInt("BaseTypeId"), rec.GetString("CodeType"), rec.GetString("NameType"), rec.GetInt("SignalCodeColumn"));
                            baseTypesId.Add(t.Id, t);
                            BaseObjectsTypes.Add(t.Code, t);
                            BaseObjectsTypes.Add(Code + "." + t.Code, t);
                        }

                    using (var rec = new DaoRec(db, "ObjectTypes"))
                        while (rec.Read())
                        {
                            var t = new ObjectType(rec.GetInt("TypeId"), rec.GetString("TypeObject"), rec.GetString("TypeName"), 1);
                            typesId.Add(t.Id, t);
                            ObjectsTypes.Add(t.Code, t);
                            ObjectsTypes.Add(Code + "." + t.Code, t);
                            var list = rec.GetString("CalcTypes").ToPropertyList();
                            foreach (var bt in list)
                                if (BaseObjectsTypes.ContainsKey(bt))
                                    t.BaseTypes.Add(BaseObjectsTypes[bt]);
                        }

                    using (var rec = new DaoRec(db, "Objects"))
                        while (rec.Read())
                        {
                            var t = rec.GetString("TypeObject");
                            if (ObjectsTypes.ContainsKey(t))
                            {
                                var ob = new TablikObject(this, ObjectsTypes[t], rec);
                                objectsId.Add(ob.Id, ob);
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

                    using (var rec = new DaoRec(db, "BaseSignals"))
                        while (rec.Read())
                        {
                            var s = new TypeSignal(rec.GetString("CodeSignal"), rec.GetString("NameSignal"));
                            var t = baseTypesId[rec.GetInt("BaseTypeId")];
                            t.Signals.Add(s.Code, s);
                            //if (rec.GetBool("Default")) t.Signal = s.Signal;
                        }

                    using (var rec = new DaoRec(db, "Signals"))
                        while (rec.Read())
                        {
                            var s = new TablikSignal(rec);
                            var t = typesId[rec.GetInt("TypeId")];
                            t.Signals.Add(s.Code, s);
                            if (rec.GetBool("Default")) t.Signal = s;
                            foreach (var bt in t.BaseTypes)
                            {
                                var bcode = rec.GetString("CodeSignal" + (bt.SignalCodeColumn == 1 ? "" : bt.SignalCodeColumn.ToString()));
                                if (!bcode.IsEmpty() && bt.Signals.ContainsKey(bcode))
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