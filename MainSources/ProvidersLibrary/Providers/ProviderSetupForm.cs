using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    public partial class ProviderSetupForm : Form
    {
        public ProviderSetupForm()
        {
            InitializeComponent();
            _dialogDelegates = new List<DialogDelegate> { _run0, _run1, _run2, _run3, _run4, _run5, _run6, _run7, _run8, _run9 };
        }

        //Ссылка на провайдер
        private ProviderBase _provider;
        public ProviderBase Provider
        {
            get { return _provider; }
            set
            {
                //_provider = value;
                //_namesDic = new Dictionary<string, string>();
                //_infDic = _provider.Inf.ToPropertyDictionary();
                //string config = DifferentIT.GetInfoTaskDir() + @"General\Config.accdb";
                //using (var rec = new RecDao(config, "SELECT SysSubTabl.SubParamNum, SysSubTabl.SubParamName, SysSubTabl.SubParamDescription, SysSubTabl.SubParamTag, SysSubTabl.SubParamRowSource " +
                //                                    "FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                //                                    "WHERE (SysTabl.ParamName='" + _provider.Code + "') AND (SysSubTabl.SubParamType='Property') ORDER BY SysSubTabl.SubParamNum"))
                //{
                //    int nmenu = 0;
                //    while (rec.Read())
                //    {
                //        var tag = rec.GetString("SubParamTag").ToPropertyDicS();
                //        string apps = tag["Applications"];
                //        bool b = true;
                //        if (!apps.IsEmpty())
                //        {
                //            var list = apps.Split(new[] { ',' });
                //            b = list.Aggregate(false, (current, s) => current);
                //        }
                //        if (b)
                //        {
                //            var cells = Props.Rows[Props.Rows.Add()].Cells;
                //            rec.GetToDataGrid("SubParamNum", cells, "PropNum");
                //            rec.GetToDataGrid("SubParamName", cells, "PropCode");
                //            rec.GetToDataGrid("SubParamDescription", cells, "PropName");
                //            var pcode = rec.GetString("SubParamName");
                //            _namesDic.Add(pcode, rec.GetString("SubParamDescription"));

                //            //Выпадающий список
                //            string rowSource = rec.GetString("SubParamRowSource");
                //            if (!rowSource.IsEmpty() || tag.ContainsKey("SpecialComboBox"))
                //            {
                //                var cell = new DataGridViewComboBoxCell();
                //                cell.Items.Clear();
                //                cells["PropValue"] = cell;
                //                List<string> list;
                //                if (tag.ContainsKey("SpecialComboBox"))
                //                {
                //                    list = _provider.ComboBoxList(AddInf(_infDic), cells.Get("PropCode"));
                //                    cell.Tag = "SpecialComboBox";
                //                }
                //                else list = rowSource.Split(new[] {';'}).ToList();
                //                if (list.Count > 0)
                //                {
                //                    cell.Value = list[0];
                //                    foreach (var s in list)
                //                        cell.Items.Add(s);
                //                }
                //            }
                            
                //            //Контекстное меню
                //            if (_provider.MenuCommands != null && _provider.MenuCommands.ContainsKey(pcode))
                //            {
                //                var menu = new ContextMenuStrip();
                //                foreach (var k in _provider.MenuCommands[pcode])
                //                {
                //                    menu.Items.Add(k.Key);
                //                    _dialogs.Add(k.Value);
                //                    menu.Items[menu.Items.Count-1].Click += new EventHandler(_dialogDelegates[nmenu]);
                //                    _dialogsRows.Add(cells);
                //                    nmenu++;
                //                }
                //                cells["PropValue"].ContextMenuStrip = menu;
                //            }

                //            //Значение
                //            if (_infDic.ContainsKey(pcode))
                //                try
                //                {
                //                    if (!(cells["PropValue"] is DataGridViewComboBoxCell) || ((DataGridViewComboBoxCell)cells["PropValue"]).Items.Contains(_infDic[pcode]))
                //                        cells["PropValue"].Value = _infDic[pcode];
                //                }
                //                catch { }
                //        }
                //    }
                //}
            }
        }

        //Словарь имен свойств
        private Dictionary<string, string> _namesDic;
        //Прежние настройки
        private Dictionary<string, string> _infDic;

        //Добавляет к словарю настроек текущие настройки
        public Dictionary<string, string> AddInf(Dictionary<string, string> dicInf)
        {
            var dic = dicInf.ToDictionary(k => k.Key, k => k.Value);
            foreach (DataGridViewRow row in Props.Rows)
            {
                var pcode = row.Get("PropCode");
                var pval = row.Get("PropValue");
                if (dic.ContainsKey(pcode)) dic[pcode] = pval;
                else dic.Add(pcode, pval);
            }
            return dic;
        }

        //Список комманд контекстных меню, и список свойств для которых эти комманды нужны
        private readonly List<IMenuCommand> _dialogs = new List<IMenuCommand>();
        private readonly List<DataGridViewCellCollection> _dialogsRows = new List<DataGridViewCellCollection>();

        private void RunDialog(int num)
        {
            _dialogsRows[num][3].Value = _dialogs[num].Run(_dialogsRows[num].Get("PropValue")) ?? "";
        }

        //Обработчики событий для  комманд контекстных меню
        private void _run0(object sender, EventArgs e) { RunDialog(0); }
        private void _run1(object sender, EventArgs e) { RunDialog(1); }
        private void _run2(object sender, EventArgs e) { RunDialog(2); }
        private void _run3(object sender, EventArgs e) { RunDialog(3); }
        private void _run4(object sender, EventArgs e) { RunDialog(4); }
        private void _run5(object sender, EventArgs e) { RunDialog(5); }
        private void _run6(object sender, EventArgs e) { RunDialog(6); }
        private void _run7(object sender, EventArgs e) { RunDialog(7); }
        private void _run8(object sender, EventArgs e) { RunDialog(8); }
        private void _run9(object sender, EventArgs e) { RunDialog(9); }

        //Делегат для обработчиков
        private delegate void DialogDelegate(object sender, EventArgs e);
        //Список делегатов
        private readonly List<DialogDelegate> _dialogDelegates;

        //Вормирование выпадающего списка
        private void Props_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //var cells = Props.Rows[e.RowIndex].Cells;
            //if (e.ColumnIndex == 3 && (string)cells[e.ColumnIndex].Tag == "SpecialComboBox")
            //{
            //    List<string> list = _provider.ComboBoxList(AddInf(_infDic), cells.Get("PropCode"));
            //    var cell = (DataGridViewComboBoxCell) cells[e.ColumnIndex];
            //    var v = cell.Value;
            //    cell.Value = null;
            //    cell.Items.Clear();
            //    foreach (var s in list)
            //        cell.Items.Add(s);
            //    if (list.Contains(v)) cell.Value = v;
            //    else if (list.Count > 0) cell.Value = list[0];
            //}
        }

        private void ButOK_Click(object sender, EventArgs e)
        {
            //var dic = AddInf(_infDic);
            //string err = Provider.CheckSettings(dic);
            //if (err.IsEmpty() || Different.MessageQuestion(err + Environment.NewLine + "Закрыть форму настроек?", "Недопустимые настройки"))
            //{
            //    Provider.Inf = dic.ToPropertyString();
            //    Close();
            //    Provider.IsSetup = false;
            //}    
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            //Provider.Inf = _infDic.ToPropertyString();
            //Close();
            //Provider.IsSetup = false;
        }

        private void ButCheck_Click(object sender, EventArgs e)
        {
            //var dic = AddInf(_infDic);
            //string err = Provider.CheckSettings(dic);
            //if (!err.IsEmpty())
            //    Different.MessageError(err, "Недопустимые настройки");
            //else
            //{
            //    Provider.Inf = dic.ToPropertyString();
            //    bool b = Provider.CheckConnection();
            //    MessageBox.Show(Provider.CheckConnectionMessage, "InfoTask", MessageBoxButtons.OK, b ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            //}
        }

        private void ProviderSetupForm_Load(object sender, EventArgs e)
        {

        }
    }
}
