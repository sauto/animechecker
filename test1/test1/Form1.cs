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
            _dataList = (Datas)serializer.Deserialize(sr);
            //ファイルを閉じる
            sr.Close();

            //取得データから表の生成
            foreach(Data data in _dataList.datas)
            {
                this.dataGridView1.Rows.Add(false, data.Title, data.Time);
                int.TryParse(data.Time, out result);
                summary += result;
            }

            //取得データの保存


            //表データを保存
            

            //残り時間の表示
            RestTime.Text = (summary/60).ToString() + "時間" + (summary%60).ToString() + "分";
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

        private void CheckBox_ClickEvent(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == this.dataGridView1.Columns["Check"].Index)
            {
                //連打するとキャストエラーが起こる
                if (!(bool)this.dataGridView1[e.ColumnIndex, e.RowIndex].Value)
                {
                    this.dataGridView1[this.dataGridView1.Columns["Time"].Index, e.RowIndex].Value = "0";
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = true;
                }
                else
                {
                    this.dataGridView1[this.dataGridView1.Columns["Time"].Index, e.RowIndex].Value =
                        _dataList.datas.Find(s => s.Title == this.dataGridView1[this.dataGridView1.Columns["Title"].Index, e.RowIndex].Value).Time;
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



