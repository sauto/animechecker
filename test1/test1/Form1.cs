using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GridDatas;

namespace test1
{
    public partial class Form1 : Form
    {
        public Datas _dataList;
        const string DATA_FILE_NAME = "data.xml";
        const string CONFIG_FILE_NAME = "config.txt";

        public Form1()
        {
            InitializeComponent();

            Datas dataListFromConfig = null;
            int summary = 0;
            int time;
            string dataFileName = System.Windows.Forms.Application.StartupPath + "\\" + DATA_FILE_NAME;
            string configFileName = System.Windows.Forms.Application.StartupPath + "\\" + CONFIG_FILE_NAME;

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
                dataFileName, System.Text.Encoding.GetEncoding("shift_jis"));
            //XMLファイルから読み込み、逆シリアル化する
            _dataList = (Datas)serializer.Deserialize(sr);

            //ファイルを閉じる
            sr.Close();

            //コンフィグファイルがないなら取得xmlデータから表の生成
            //コンフィグファイルはあるが、xmlにデータを追加した場合は？
            //→データリストを比較する
            if (!System.IO.File.Exists(configFileName))
            {
                foreach (Data data in _dataList.datas)
                {
                    this.dataGridView1.Rows.Add(false, data.Title, data.Time);
                    int.TryParse(data.Time, out time);
                    summary += time;
                }
            }
            else
            {
                System.IO.StreamReader sReader = new System.IO.StreamReader(
                configFileName, System.Text.Encoding.GetEncoding("shift_jis"));
                while (sReader.Peek() >= 0)
                {
                    string line = sReader.ReadLine();
                    string[] words = line.Split(',');

                    bool checkBoxValue;
                    bool.TryParse(words[0], out checkBoxValue);

                    //データリスト作ってxmlのデータと比較する用
                    Data data;
                    data.ID
                    dataListFromConfig.datas.Add()

                    this.dataGridView1.Rows.Add(checkBoxValue, words[1], words[2]);
                    int.TryParse(words[2], out time);
                    summary += time;
                }
                sReader.Close();
            }


            //表データを保存
            SaveGrid();

            //残り時間の表示
            RestTime.Text = (summary/60).ToString() + "時間" + (summary%60).ToString() + "分";
        }

        /// <summary>
        /// 表データの保存　config.txtに書き出す
        /// </summary>
        private void SaveGrid()
        {
            string configFileName = System.Windows.Forms.Application.StartupPath + "\\" + CONFIG_FILE_NAME;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(configFileName,false,System.Text.Encoding.GetEncoding("shift_jis"));

            for(int row=0;row<this.dataGridView1.RowCount;row++)
            {
                for(int col=0;col<this.dataGridView1.ColumnCount;col++)
                {
                    sw.Write(this.dataGridView1[col, row].Value+",");
                }
                sw.Write(System.Environment.NewLine);
            }
            sw.Close();
            
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

            //チェックボックスのCheckedが取得できない
            //CheckBoxクラスならプロパティがあるがこれだとない
            //連打するとチェック連動が反映されない
            //DataGridViewCheckBoxColumnからCheckBoxを参照できないのか？
            //CellContentClickはダブルクリックに反応しない

            CheckBox_ClickEvent(e);

            RestTimeDisplay();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CheckBox_ClickEvent(e);

            RestTimeDisplay();
        }

        /// <summary>
        /// 視聴時間列のデータを合計し残り視聴時間を表示する
        /// </summary>
        private void RestTimeDisplay()
        {
            int summary = 0;
            int result;

            //視聴時間列のデータを合計する
            for (int i = 0; i < this.dataGridView1.RowCount; i++)
            {
                int.TryParse(this.dataGridView1[this.dataGridView1.Columns["Time"].Index, i].Value.ToString(), out result);
                summary += result;
            }

            //残り時間の表示
            RestTime.Text = (summary / 60).ToString() + "時間" + (summary % 60).ToString() + "分";
        }

        /// <summary>
        /// チェックボックスをクリックしたときに連動して視聴時間を変更する
        /// </summary>
        /// <param name="e"></param>
        private void CheckBox_ClickEvent(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == this.dataGridView1.Columns["Check"].Index)
            {
                bool value;
                bool.TryParse(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out value);

                if (!value)
                {
                    this.dataGridView1[this.dataGridView1.Columns["Time"].Index, e.RowIndex].Value = "0";
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = true;
                }
                else
                {
                    this.dataGridView1[this.dataGridView1.Columns["Time"].Index, e.RowIndex].Value =
                        _dataList.datas.Find(
                        s => s.Title == this.dataGridView1[this.dataGridView1.Columns["Title"].Index, e.RowIndex].Value.ToString())
                        .Time;
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = false;
                }
            }
        }

        private void testcheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                RestTime.Text = "ファッ！？";
            }
            else
            {
                RestTime.Text = "おいKMRァ！！";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveGrid();
        }





    }
}
/*設計
 * アニメのリストをDLしてきて表示する
 * 設定によって自前も可能
 * リストのファイル形式はxml
 * 見終わったらチェックボックスをONにする
 * チェックONのアニメは視聴時間が0になる
 * チェックOFFのアニメは視聴時間が元に戻る
 * 視聴時間の合計を表示する
 */



