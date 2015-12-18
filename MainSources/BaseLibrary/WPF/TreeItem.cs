using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace BaseLibrary
{
    //Любой узел в любом дереве настроек
    public class TreeItem : INotifyPropertyChanged, IEnumerable<TreeItem>
    {
        public TreeItem(string type, string picture, string name, bool isnew = false)
        {
            Type = type;
            Picture = picture;
            _name = name;
            _oldname = isnew ? null : name;
            Properties = new Dictionary<string, string>();
        }

        //Имя узла, отображается в дереве
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        //Имя узла, с которым он был загружен в дерево в начале
        private readonly string _oldname;
        public string OldName
        { get { return _oldname; } }

        //Путь к картинке для привязки
        private string _picture;
        public string Picture
        {
            get { return _picture; }
            set { _picture = value; OnPropertyChanged(); }
        }

        //Путь к картинке, индицирующей ошибку, для привязки
        private string _errorpicture;
        public string ErrorPicture
        {
            get { return _errorpicture; }
            set { _errorpicture = value; OnPropertyChanged(); }
        }

        //Тип узла
        public string Type { get; set; }
        //Тип узла, задающий состав настроек Properties
        public string ProviderType { get; set; }
        //Настройки узла
        public Dictionary<string, string> Properties { get; set; }

        //Узел выбран
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        //Узел открыт
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }

                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        //Родитель узла
        private TreeItem _parent;
        public TreeItem Parent { get { return _parent; } }

        //Дети узла
        private ObservableCollection<TreeItem> _children = new ObservableCollection<TreeItem>();
        public ObservableCollection<TreeItem> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        //Добавить ребенка
        public void AddChild(TreeItem t)
        {
            Children.Add(t);
            t._parent = this;
        }

        //Стандартные переопределения и реализации методов для элемента дерева
        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                try
                {
                    var trace = new StackTrace(false);
                    string propertyName = trace.GetFrame(1).GetMethod().Name.Split('_')[1];
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                catch
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
                }
            }
        }

        public IEnumerator<TreeItem> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}