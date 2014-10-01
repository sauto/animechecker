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
        public AllData _dataList;
        const string DATA_FILE_NAME = "data.xml";
        const string CONFIG_FILE_NAME = "config.txt";
        string _dataFileName = System.Windows.Forms.Application.StartupPath + "\\" + DATA_FILE_NAME;
        string _configFileName = System.Windows.Forms.Application.StartupPath + "\\" + CONFIG_FILE_NAME;
        string _imgFilePath = string.Empty;

        public Form1()
        {
            InitializeComponent();


            int summary = 0;
            int time;
            

            //データファイルがない場合DL
            //System.Net.WebClient wc = new System.Net.WebClient();
            //wc.DownloadFile("http://blog-imgs-60.fc2.com/t/o/t/tottirakatta/XMLFile1.xml", fileName);
            //wc.Dispose();

            //データファイルからデータを取得
            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(AllData));
            //読み込むファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(
                _dataFileName, System.Text.Encoding.GetEncoding("shift_jis"));
            //XMLファイルから読み込み、逆シリアル化する
            _dataList = (AllData)serializer.Deserialize(sr);

            //ファイルを閉じる
            sr.Close();

            foreach (Data data in _dataList.dataList)
            {
                this.dataGridView1.Rows.Add(false, data.Title, data.Time);
                int.TryParse(data.Time, out time);
                summary += time;
            }
            
            //レイアウトのロード
            LoadLayout();

            //表データを保存
            SaveGrid();

            //残り時間の表示
            RestTime.Text = (summary/60).ToString() + "時間" + (summary%60).ToString() + "分";
        }

        /// <summary>
        /// 表データの保存　data.xmlに書き出す
        /// </summary>
        private void SaveGrid()
        {

            AllData allData = new AllData();

            //n行目をIDとし、列の総覧によってグリッドの各列を保存して１つのxml部分を作る
            for(int row=0;row<this.dataGridView1.RowCount;row++)
            {
                Data data = new Data();
                data.ID = row.ToString();
                for(int col=0;col<this.dataGridView1.ColumnCount;col++)
                {
                    switch(col)
                    {
                        case 0:
                            data.Check = this.dataGridView1[col, row].Value.ToString();
                            break;
                        case 1:
                            data.Title = this.dataGridView1[col, row].Value.ToString();
                            break;
                        case 2:
                            data.Time = this.dataGridView1[col, row].Value.ToString();
                            break;
                    }

                }
                allData.dataList.Add(data);
            }

            //xml書き出し
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(AllData));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(_dataFileName, false, System.Text.Encoding.GetEncoding("shift_jis"));
            serializer.Serialize(sw, allData);
            sw.Close();
            
        }

        /// <summary>
        /// コントロールのレイアウト情報を保存する
        /// </summary>
        private void SaveLayout()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(_configFileName, false, System.Text.Encoding.GetEncoding("shift_jis"));
            //画像のパス
            sw.Write(_imgFilePath);
            sw.Write(Environment.NewLine);
            //行追加ボタン
            sw.Write(this.AddButton.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.AddButton.Top);
            sw.Write(Environment.NewLine);
            //グリッド
            sw.Write(this.dataGridView1.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.dataGridView1.Top);
            sw.Write(Environment.NewLine);
            //残り時間
            sw.Write(this.RestTimeText.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.RestTimeText.Top);
            sw.Write(Environment.NewLine);
            //残り時間表示
            sw.Write(this.RestTime.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.RestTime.Top);
            sw.Write(Environment.NewLine);
            //保存
            sw.Write(this.SaveButton.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.SaveButton.Top);
            sw.Write(Environment.NewLine);

            sw.Close();
        }

        /// <summary>
        /// コントロールのレイアウト情報を読みだす
        /// </summary>
        private void LoadLayout()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(
                _configFileName, System.Text.Encoding.GetEncoding("shift_jis"));

            int row = 1;
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                switch(row)
                {
                    case 1:
                        if(!string.IsNullOrEmpty(line))
                            this.pictureBox1.Image = System.Drawing.Image.FromFile(line);
                        break;
                    case 2:
                        this.AddButton.Left = int.Parse(line);
                        break;
                    case 3:
                        this.AddButton.Top = int.Parse(line);
                        break;
                    case 4:
                        this.dataGridView1.Left = int.Parse(line);
                        break;
                    case 5:
                        this.dataGridView1.Top = int.Parse(line);
                        break;
                    case 6:
                        this.RestTimeText.Left = int.Parse(line);
                        break;
                    case 7:
                        this.RestTimeText.Top = int.Parse(line);
                        break;
                    case 8:
                        this.RestTime.Left = int.Parse(line);
                        break;
                    case 9:
                        this.RestTime.Top = int.Parse(line);
                        break;
                    case 10:
                        this.SaveButton.Left = int.Parse(line);
                        break;
                    case 11:
                        this.SaveButton.Top = int.Parse(line);
                        break;
                }
                row++;
            }
            sr.Close();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "config.txt";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string configFileName = System.Windows.Forms.Application.StartupPath + "\\" + CONFIG_FILE_NAME;
                // ファイルを指定してメモ帳を起動する
                System.Diagnostics.Process.Start("Notepad", configFileName);
            }
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
                bool check;
                bool.TryParse(this.dataGridView1[this.dataGridView1.Columns["Check"].Index, i].Value.ToString(), out check);
                if (!check)
                {
                    int.TryParse(this.dataGridView1[this.dataGridView1.Columns["Time"].Index, i].Value.ToString(), out result);
                    summary += result;
                }
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
                bool check;
                bool.TryParse(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out check);

                //チェックがついてるなら視聴時間を0にする
                //チェックがついていないなら視聴時間を元に戻す
                if (!check)
                {
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = true;
                }
                else
                {
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = false;
                }
            }
        }


        /// <summary>
        /// 視聴時間を追加したときに残り時間を追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == this.dataGridView1.Columns["Time"].Index)
                {
                    RestTimeDisplay();
                }
            }
            catch(Exception exc)
            {
                //CellValueChangedイベントがプログラム起動時に発生するが、
                //そのときにはまだTime列が存在していない（列は存在するがTimeという名前の列がない）
                //ので例外が発生するのを防止するため
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

        /// <summary>
        /// 保存ボタンのクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveGrid();
        }



        private void CtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGrid();
        }

        /// <summary>
        /// 画面を閉じるときレイアウトを保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayout();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(ofd.FileName);
                this.pictureBox1.Image = img;
                _imgFilePath = ofd.FileName;
            }
            
        }

        /// <summary>
        /// ドラッグ中か
        /// </summary>
        bool isDraggable = false;
        System.Drawing.Point point = new Point();

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggable = true;
            point.X = e.X;
            point.Y = e.Y;
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ( !isDraggable )
            {
                return;
            }
            //移動処理
            this.dataGridView1.Left += e.X - point.X;
            this.dataGridView1.Top += e.Y - point.Y;
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
        }

        /// <summary>
        /// 行の追加ボタンのクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            //ドラッグ移動した場合は追加しない
            if (!isMouseMove)
                this.dataGridView1.Rows.Add(false, "", "");
        }

        private void AddButton_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggable = true;
            point.X = e.X;
            point.Y = e.Y;
        }

        bool isMouseMove = false;

        private void AddButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggable)
            {
                return;
            }
            //移動処理
            this.AddButton.Left += e.X - point.X;
            this.AddButton.Top += e.Y - point.Y;

            isMouseMove = true;
        }

        private void AddButton_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
            isMouseMove = false;
        }


        private void SaveButton_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggable = true;
            point.X = e.X;
            point.Y = e.Y;
        }

        private void SaveButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggable)
            {
                return;
            }
            //移動処理
            this.SaveButton.Left += e.X - point.X;
            this.SaveButton.Top += e.Y - point.Y;
        }

        private void SaveButton_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
        }


        private void RestTimeText_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggable = true;
            point.X = e.X;
            point.Y = e.Y;
        }

        private void RestTimeText_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggable)
            {
                return;
            }
            //移動処理
            this.RestTimeText.Left += e.X - point.X;
            this.RestTimeText.Top += e.Y - point.Y;

            //テキストボックスも連動して移動
            this.RestTime.Left += e.X - point.X;
            this.RestTime.Top += e.Y - point.Y;

        }

        private void RestTimeText_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
        }

        private void RestTime_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggable = true;
            point.X = e.X;
            point.Y = e.Y;
        }

        private void RestTime_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggable)
            {
                return;
            }
            //移動処理
            this.RestTimeText.Left += e.X - point.X;
            this.RestTimeText.Top += e.Y - point.Y;

            //テキストボックスも連動して移動
            this.RestTime.Left += e.X - point.X;
            this.RestTime.Top += e.Y - point.Y;
        }

        private void RestTime_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
        }

        /// <summary>
        /// ラベルとテキストボックスの移動を連動させると
        /// ラベルとテキストボックスの間の余白をドラッグすると反応しないので
        /// パネルの上に残り時間のやつおいてパネルを動かすことで余白でも動かせるようにしたい
        /// がなぜかパネル上のコントロールが消える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }








    }
}
/*設計
 * アニメのリストをDLしてきて表示する
 * 設定によって自前も可能
 * リストのファイル形式はxml
 * 行追加はAddボタンによって可能
 * 見終わったらチェックボックスをONにする
 * チェックONのアニメは視聴時間が0になる
 * チェックOFFのアニメは視聴時間が元に戻る
 * 視聴時間の合計を表示する
 */



