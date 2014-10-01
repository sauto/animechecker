using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// ID
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("id")]
        public String ID { get; set; }
        /// <summary>
        /// チェックボックスの値
        /// </summary>
        [System.Xml.Serialization.XmlElement("check")]
        public String Check { get; set; }
        /// <summary>
        /// 名前
        /// </summary>
        [System.Xml.Serialization.XmlElement("title")]
        public String Title { get; set; }
        /// <summary>
        /// 苗字
        /// </summary>
        [System.Xml.Serialization.XmlElement("time")]
        public String Time { get; set; }
    }
}
