using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int summary = 0;
            int result;
            string fileName = System.Windows.Forms.Application.StartupPath + "\\" + "data.xml";

            //データファイルがない場合DL
            //System.Net.WebClient wc = new System.Net.WebClient();
            //wc.DownloadFile("http://blog-imgs-60.fc2.com/t/o/t/tottirakatta/XMLFile1.xml", fileName);
            //wc.Dispose();

            
            //データファイルからデータを取得
            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(Datas));
            //読み込むファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(
                fileName, System.Text.Encoding.GetEncoding("shift_jis"));
            //XMLファイルから読み込み、逆シリアル化する
            Datas dataList = (Datas)serializer.Deserialize(sr);
            //ファイルを閉じる
            sr.Close();

            //取得データから表の生成
            foreach(Data data in dataList.datas)
            {
                this.dataGridView1.Rows.Add(false, data.Title, data.Time);
                int.TryParse(data.Time, out result);
                summary += result;
            }


            //チェックなどのデータを保存
            

            //残り時間の表示
            RestTime.Text = (summary/60).ToString() + "時間" + (summary%60).ToString() + "分";
        }

        [System.Xml.Serialization.XmlRoot("anime")]
        public class Datas
        {
            /// <summary>
            /// データ
            /// </summary>
            [System.Xml.Serialization.XmlElement("data")]
            public System.Collections.Generic.List<Data> datas { get; set; }
        }

        public class Data
        {
            /// <summary>
            /// ID
            /// </summary>
            [System.Xml.Serialization.XmlAttribute("id")]
            public String ID { get; set; }
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

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //DLしてきたやつからデータ取得

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //チェックしたら時間を0にする

            if (e.ColumnIndex == this.dataGridView1.Columns["Check"].Index)
            {

                this.dataGridView1[this.dataGridView1.Columns["Time"].Index, e.RowIndex].Value = "0";
            }

            int summary = 0;
            int result;
            foreach(var row in this.dataGridView1.Rows)
            {
                //int.TryParse(row, out result);
                //summary += result;
            }

            //残り時間の表示
            RestTime.Text = (summary / 60).ToString() + "時間" + (summary % 60).ToString() + "分";
        }




    }
}
/*設計
 * アニメのリストをDLしてきて表示する
 * 設定によって自前も可能
 * リストのファイル形式はxml
 * 見終わったらチェックボックスをONにする
 * チェックONのアニメは視聴時間が0になる
 * 視聴時間の合計を表示する
 */



