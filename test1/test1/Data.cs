using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace GridDatas
{
    [System.Xml.Serialization.XmlRoot("anime")]
    public class AllData
    {
        //コンストラクタ
        public AllData()
        {
            dataList = new List<Data>();
        }
        /// <summary>
        /// データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("data")]
        public System.Collections.Generic.List<Data> dataList { get; set; }
    }

    [System.Xml.Serialization.XmlRoot("anime")]
    public class AllDataForXaml
    {
        //コンストラクタ
        public AllDataForXaml()
        {
            dataList = new ObservableCollection<DataForXaml>();
        }
        /// <summary>
        /// データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("data")]
        public ObservableCollection<DataForXaml> dataList { get; set; }
    }

    public class Data
    {
        
        /// <summary>
        /// チェックボックスの値
        /// </summary>
        [System.Xml.Serialization.XmlElement("check")]
        public bool Check { get; set; }
        /// <summary>
        /// 名前
        /// </summary>
        [System.Xml.Serialization.XmlElement("title")]
        public String Title { get; set; } 
        /// <summary>
        /// 視聴時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("time")]
        public String Time { get; set; }
        /// <summary>
        /// 放送曜日
        /// </summary>
        [System.Xml.Serialization.XmlElement("day")]
        public String Day { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }
    }

    public class DataForXaml : INotifyPropertyChanged
    {
        
        /*プロパティにバインドしているのでグリッド更新処理は不要
         * データをいじれば勝手に追従する
         * てかなんでフォーム版はいちいちグリッドから取得してたんだ
         */

        /// <summary>
        /// チェックボックスの値
        /// </summary>
        [System.Xml.Serialization.XmlElement("check")]
        public bool Check { get; set; }

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
                if(!int.TryParse(value,out result))
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0002,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
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
                if (!Enum.IsDefined(typeof(test1.Form1.DayOfWeek), value))
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0001,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _day = value;

                    OnPropertyChanged("Day");
                    OnPropertyChanged("Limit");
                }
            }
        }
        /// <summary>
        /// ID
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }

        string _limit;

        public String Limit {
            get
            {
                int limit = (int)((test1.Form1.DayOfWeek)Enum.Parse(typeof(test1.Form1.DayOfWeek), Day))
                         - (int)DateTime.Today.DayOfWeek + 7;

                _limit = "あと" + ((limit % 7 == 0 ? 7 : 0) + (limit % 7)) + "日";

                return _limit;
            }
            set
            {
                _limit = value;
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
