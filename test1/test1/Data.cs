using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;


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

}
