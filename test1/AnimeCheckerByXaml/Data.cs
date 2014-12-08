using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using BindingCommand;
using AnimeCheckerByXaml;

namespace DataGridDatas
{
    [System.Xml.Serialization.XmlRoot("anime")]
    public class AllDataForXaml
    {
        //コンストラクタ
        public AllDataForXaml()
        {
            dataList = new SortableObservableCollection();
        }
        /// <summary>
        /// データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("data")]
        public SortableObservableCollection dataList { get; set; }
    }
    
    public class DataForXaml : INotifyPropertyChanged
    {
        
        /*プロパティにバインドしているのでグリッド更新処理は不要
         * データをいじれば勝手に追従する
         * てかなんでフォーム版はいちいちグリッドから取得してたんだ
         */

        bool _check;
        /// <summary>
        /// チェックボックスの値
        /// </summary>
        [System.Xml.Serialization.XmlElement("check")]
        public bool Check
        {
            get
            {
                return _check;
            }
            set
            {
                _check = value;
                OnPropertyChanged("Check");
            }
        }
        
        string _title;
        /// <summary>
        /// 名前
        /// </summary>
        [System.Xml.Serialization.XmlElement("title")]
        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                //これをしないとデータをいじったときにxaml側の値が更新されない
                OnPropertyChanged("Title");
                OnPropertyChanged("RowHeader");
            }
        }
        string _time;
        /// <summary>
        /// 視聴時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("time")]
        public String Time
        {
            get { return _time; }
            set
            {
                int result;
                if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out result))
                {
                    MessageBox.Show(AnimeCheckerByXaml.Properties.Settings.Default.E0001,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _time = value;                    
                }
                OnPropertyChanged("Time");
            }
        }


        string _day;
        /// <summary>
        /// 放送曜日
        /// </summary>
        [System.Xml.Serialization.XmlElement("day")]
        public String Day
        {
            get { return _day; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !Enum.IsDefined(typeof(MainWindow.DayOfWeek), value))
                {
                    MessageBox.Show(AnimeCheckerByXaml.Properties.Settings.Default.E0002,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //入力値が間違っていてもOnPropertyChangedで更新すれば元通り
                }
                else
                {
                    _day = value;

                }

                OnPropertyChanged("Day");

                OnPropertyChanged("Limit");
            }
        }
        /// <summary>
        /// ID
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        string _limit;

        public String Limit {
            get
            {
                

                if (!string.IsNullOrEmpty(Day))
                {
                    int limit = (int)((MainWindow.DayOfWeek)Enum.Parse(typeof(MainWindow.DayOfWeek), Day))
                             - (int)DateTime.Today.DayOfWeek + 7;

                    _limit = "あと" + ((limit % 7 == 0 ? 7 : 0) + (limit % 7)) + "日";
                }
                else
                    _limit = string.Empty;

                return _limit;
            }
            set
            {
                _limit = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }

    /// <summary>
    /// ソート可能なObservableCollection
    /// http://smdn.jp/programming/netfx/collections/3_objectmodel_2_observablecollection/
    /// </summary>
    public class SortableObservableCollection : ObservableCollection<DataForXaml>
    {
        public ICommand AddRowCommand { get; private set; }
        public ICommand DeleteRowCommand { get; private set; }

        public BindingCommands _bindingCommands = new BindingCommands();

        public SortableObservableCollection()
        {
            AddRowCommand = _bindingCommands.AddRowCommandImplementClass;
            DeleteRowCommand = _bindingCommands.DeleteRowCommandImplementClass;
        }

        public void Sort()
        {
            // IListインターフェイスからArrayListのラッパーを作り、IComparer<T>を使ってソートする
            //ArrayListのSortが使えるようにした
            System.Collections.ArrayList.Adapter(this).Sort(new DayOfWeekComparer());
        }

        public void Reverse()
        {
            // IListインターフェイスからArrayListのラッパーを作りリバースする
            System.Collections.ArrayList.Adapter(this).Reverse();
        }

    }

    /// <summary>
    /// 曜日enumに変換して比較
    /// http://dobon.net/vb/dotnet/programing/icomparer.html
    /// </summary>
    public class DayOfWeekComparer : System.Collections.IComparer, System.Collections.Generic.IComparer<DataForXaml>
    {
        public int Compare(object x, object y)
        {
            //nullが最も小さいとする
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            if (string.IsNullOrEmpty(((DataForXaml)x).Day) || string.IsNullOrEmpty(((DataForXaml)y).Day))
            {
                return 0;
            }

            return (int)((MainWindow.DayOfWeek)Enum.Parse(typeof(MainWindow.DayOfWeek), ((DataForXaml)x).Day)) -
            (int)((MainWindow.DayOfWeek)Enum.Parse(typeof(MainWindow.DayOfWeek), ((DataForXaml)y).Day));
        }

        public int Compare(DataForXaml x, DataForXaml y)
        {
            //nullが最も小さいとする
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            if (string.IsNullOrEmpty(x.Day) || string.IsNullOrEmpty(y.Day))
            {
                return 0;
            }

            return (int)((MainWindow.DayOfWeek)Enum.Parse(typeof(MainWindow.DayOfWeek), x.Day)) -
            (int)((MainWindow.DayOfWeek)Enum.Parse(typeof(MainWindow.DayOfWeek), y.Day));
        }
    }
}
