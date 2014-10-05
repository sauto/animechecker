using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using GridDatas;
using NewContorol;

namespace test1
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 表データのリスト
        /// </summary>
        public AllData _allData;
        const string DATA_FILE_NAME = "data.xml";
        const string CONFIG_FILE_NAME = "config.txt";
        string _dataFileName = System.Windows.Forms.Application.StartupPath + "\\" + DATA_FILE_NAME;
        string _configFileName = System.Windows.Forms.Application.StartupPath + "\\" + CONFIG_FILE_NAME;
        string _imgFilePath = string.Empty;


        /// <summary>
        /// 各コントロールの初期位置のリスト
        /// </summary>
        List<Point> _defaultLocationList = new List<Point>();


        enum DayOfWeek
        {
            月 = 0,
            火,
            水,
            木,
            金,
            土,
            日

        }

        public Form1()
        {
            InitializeComponent();

            SaveDefaultLayout();

            int summary = 0;
            int time;


            if(System.IO.File.Exists(_dataFileName))
            {
                //データファイルからデータを取得
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    _dataFileName, System.Text.Encoding.GetEncoding("shift_jis"));
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(AllData));
                _allData = (AllData)serializer.Deserialize(sr);

                //ファイルを閉じる
                sr.Close();

                //行の追加
                foreach (Data data in _allData.dataList)
                {
                    this.dataGridView1.Rows.Add(false, data.Title, data.Time,data.Day,data.ID);
                    int.TryParse(data.Time, out time);
                    summary += time;
                }
            }
            else
            {
                XmlDocument document = new XmlDocument();

                XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "shift_jis", null);  // XML宣言
                XmlElement root = document.CreateElement("anime");  // ルート要素

                document.AppendChild(declaration);
                document.AppendChild(root);

                // ファイルに保存する
                document.Save(_dataFileName);
            }

            
            //レイアウトのロード
            if (System.IO.File.Exists(_configFileName))
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
            //xml書き出し用データ生成
            CreateAllData();

            //xml書き出し
            WriteXml(_allData);
            
        }

        /// <summary>
        /// 表データリストの作成
        /// </summary>
        private void CreateAllData()
        {
            _allData = new AllData();

            //n行目をIDとし、列の総覧によってグリッドの各列を保存して１つのxml部分を作る
            for (int row = 0; row < this.dataGridView1.RowCount; row++)
            {
                Data data = new Data();
                for (int col = 0; col < this.dataGridView1.ColumnCount; col++)
                {
                    switch (col)
                    {
                        case 0:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Check = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Check = false.ToString();
                            break;
                        case 1:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Title = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Title = string.Empty;
                            break;
                        case 2:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Time = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Time = string.Empty;
                            break;
                        case 3:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Day = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Day = string.Empty;
                            break;
                        case 4:
                            if (this.dataGridView1[col, row].Value != null)
                                data.ID = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.ID = string.Empty;
                            break;
                    }

                }
                _allData.dataList.Add(data);
            }
        }

        /// <summary>
        /// xmlを書き出す
        /// </summary>
        /// <param name="obj"></param>
        private void WriteXml(object obj) 
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(AllData));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(_dataFileName, false, System.Text.Encoding.GetEncoding("shift_jis"));
            serializer.Serialize(sw, obj);
            sw.Close();
        }

        #region レイアウト
        /// <summary>
        /// 各コントロールの初期位置を保存
        /// </summary>
        private void SaveDefaultLayout()
        {
            foreach (Control ctrl in this.Controls)
                _defaultLocationList.Add(ctrl.Location);
        }
        /// <summary>
        /// レイアウトを初期化
        /// </summary>
        private void RestoreLayout()
        {
            for (int i = 0; i < _defaultLocationList.Count; i++)
                this.Controls[i].Location = _defaultLocationList[i];
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
            sw.Write(this.panel1.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.panel1.Top);
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
                        if (!string.IsNullOrEmpty(line))
                        {
                            this.pictureBox1.Image = System.Drawing.Image.FromFile(line);
                            _imgFilePath = line;
                        }
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
                        this.panel1.Left = int.Parse(line);
                        break;
                    case 7:
                        this.panel1.Top = int.Parse(line);
                        break;
                    case 8:
                        this.SaveButton.Left = int.Parse(line);
                        break;
                    case 9:
                        this.SaveButton.Top = int.Parse(line);
                        break;
                }
                row++;
            }
            sr.Close();
        }

        /// <summary>
        /// レイアウトを固定するか
        /// </summary>
        bool _isFixLayout = false;
        /// <summary>
        /// レイアウトの固定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isFixLayout)
            {
                this.FixLayoutToolStripMenuItem.Checked = true;
                _isFixLayout = true;
            }
            else
            {
                this.FixLayoutToolStripMenuItem.Checked = false;
                _isFixLayout = false;
            }
        }
        #endregion

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
        /// チェックボックスをクリックしたときに連動して残り時間を変更する
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
            //CellValueChangedイベントがプログラム起動時に発生するが、
            //そのときにはまだTime列が存在していない（列は存在するがTimeという名前の列がない）
            //ので例外が発生するのを防止するため判定
            if (this.dataGridView1.Columns.Contains("Time"))
            {
                if (e.ColumnIndex == this.dataGridView1.Columns["Time"].Index)
                {
                    //空白入力時と異常値入力時の空白入力時を回避
                    if (this.dataGridView1[e.ColumnIndex, e.RowIndex].Value != null &&
                        !string.IsNullOrEmpty(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()))
                    {
                        int result;
                        //半角数字以外が入力されたらエラー
                        if (int.TryParse(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out result))
                        {
                            RestTimeDisplay();

                            //正常値入力→異常値入力とすると入力した正常値に戻らない（データリストが変更されてない）ので
                            //データリストだけ作り直してelse内のRevert処理を行えるようにする。
                            //xml書き出しまでやると重いのでこれだけやる
                            CreateAllData();
                        }
                        else
                        {
                            MessageBox.Show("正しい値を入力してください。",
                                            "エラー",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);


                            //時間に半角数字以外を入力→隣のｾﾙをクリック→MouseDownイベント起動→エラーメッセージボックスとなり
                            //MouseUpイベントが起動しないままになる
                            //その状態でコントロールへカーソルを合わせるとMouseMoveが起動して動く
                            //のを防止
                            isDraggable = false;

                            //空白はキーにしない
                            if (_allData.dataList.Find(s =>
                                    s.Title == this.dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()
                                   && !string.IsNullOrEmpty(s.Title)
                                   ) != null)
                            {
                                //タイトルをキーとして元の視聴時間を取得
                                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value =
                                    _allData.dataList.Find(s =>
                                        s.Title == this.dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()).Time;
                            }
                            else
                            {
                                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = null;
                            }
                        }
                    }
                }
            }

            if (this.dataGridView1.Columns.Contains("WeekColumn"))
            {
                if (e.ColumnIndex == this.dataGridView1.Columns["WeekColumn"].Index)
                {
                    //空白入力時と異常値入力時の空白入力時を回避
                    if (this.dataGridView1[e.ColumnIndex, e.RowIndex].Value != null &&
                        !string.IsNullOrEmpty(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()))
                    {

                        //曜日以外が入力されたらエラー
                        if (Enum.IsDefined(typeof(DayOfWeek), this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()))
                        {
                            RestTimeDisplay();

                            //正常値入力→異常値入力とすると入力した正常値に戻らない（データリストが変更されてない）ので
                            //データリストだけ作り直してelse内のRevert処理を行えるようにする。
                            //xml書き出しまでやると重いのでこれだけやる
                            CreateAllData();
                        }
                        else if(!this.dataGridView1.IsCurrentCellInEditMode)
                        {
                            MessageBox.Show("正しい値を入力してください。",
                                            "エラー",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);


                            //時間に半角数字以外を入力→隣のｾﾙをクリック→MouseDownイベント起動→エラーメッセージボックスとなり
                            //MouseUpイベントが起動しないままになる
                            //その状態でコントロールへカーソルを合わせるとMouseMoveが起動して動く
                            //のを防止
                            isDraggable = false;

                            //タイトルが空白のまま、曜日に正常値入力→曜日に異常値入力とすると
                            //タイトル空白のため検索できないので正常値に戻らない

                            //曜日を入力→確定しないで列先頭をクリック→CellValueChangedではなくClickColumnHeaderが起動
                            //→値が確定していないので、グリッドの値を保存できない


                            //空白はキーにしない
                            if (_allData.dataList.Find(s =>
                                    s.ID == this.dataGridView1[this.dataGridView1.Columns["ID"].Index, e.RowIndex].Value.ToString()
                                   
                                   ) != null)
                            {
                                //非表示のID列を作って、IDをキーとして元の視聴時間を取得
                                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value =
                                    _allData.dataList.Find(s =>
                                        s.ID == this.dataGridView1[
                                        this.dataGridView1.Columns["ID"].Index, e.RowIndex].Value.ToString()).Day;
                            }
                            else
                            {
                                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = null;
                            }
                        }
                    }
                }
            }

            _nowInput = false;
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


        /// <summary>
        /// メニューの保存をクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private void InitLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreLayout();
        }

        /// <summary>
        /// 背景画像の変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            if (string.IsNullOrEmpty(_imgFilePath))
                ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(_imgFilePath);
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
        /// 行の追加ボタンのクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            //ドラッグ移動した場合は追加しない
            if (!isMouseMove)
                this.dataGridView1.Rows.Add(false, "", "", "", this.dataGridView1.RowCount + 1);
        }
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add(false, "", "", "", this.dataGridView1.RowCount + 1);
        }

        #region コントロールの移動イベント

        /// <summary>
        /// ドラッグ中か
        /// </summary>
        bool isDraggable = false;
        /// <summary>
        /// 移動の始点
        /// </summary>
        System.Drawing.Point point = new Point();

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.dataGridView1.Left += e.X - point.X;
                this.dataGridView1.Top += e.Y - point.Y;
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
            }
        }

        //境界線でリサイズするとマウスダウンが起動して移動フラグが立っちゃう

        private void AddButton_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        bool isMouseMove = false;

        private void AddButton_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.AddButton.Left += e.X - point.X;
                this.AddButton.Top += e.Y - point.Y;

                isMouseMove = true;
            }
        }

        private void AddButton_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
                isMouseMove = false;
            }
        }


        private void SaveButton_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void SaveButton_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.SaveButton.Left += e.X - point.X;
                this.SaveButton.Top += e.Y - point.Y;
            }
        }

        private void SaveButton_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
            }
        }

        /// <summary>
        /// ラベルとテキストボックスの移動を連動させると
        /// ラベルとテキストボックスの間の余白をドラッグすると反応しないので
        /// パネルの上に残り時間のやつおいてパネルを動かすことで余白でも動かせるようにしたい
        /// がなぜかパネル上のコントロールが消える　Visibleはtrueだった
        /// 新しくコントロールを追加するとちゃんと見える
        /// パネル上に配置しなければちゃんと見える
        /// LayoutLoadのせい
        /// まあ新規コントロールと既存コントロールの違いここぐらいしかなかったし
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.panel1.Left += e.X - point.X;
                this.panel1.Top += e.Y - point.Y;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                {
                    isDraggable = false;
                }
            }
        }


        private void RestTimeText_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void RestTimeText_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.panel1.Left += e.X - point.X;
                this.panel1.Top += e.Y - point.Y;
            }

        }

        private void RestTimeText_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
            }
        }

        private void RestTime_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void RestTime_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.panel1.Left += e.X - point.X;
                this.panel1.Top += e.Y - point.Y;
            }

        }

        private void RestTime_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
            }
        }

        #endregion



        /// <summary>
        /// 曜日列をクリックしたとき曜日順にソート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CreateAllData();
            if (e.ColumnIndex == this.dataGridView1.Columns["WeekColumn"].Index)
            {
                WeekSort();
            }
        }

        /// <summary>
        /// 曜日順にソート
        /// </summary>
        private void WeekSort()
        {
            //入力後、列先頭クリックするとデータが確定する前に削除しているので、再作成時にデータリストから復旧できない
            //CurrentCellDirtyStateChangedで入力中の値を取得できるのでこれで確定させるとできた
            //しかし異常値でいちいちエラーが出るうえ元に戻らない
            //コミットしたあと編集終了してもValueChangedイベントが起きない
            //
            
            //空白があったらその行はいったん退避してソート後に後ろに付け足す
            List<Data> tempDataList = new List<Data>();
            List<Data> emptyDataList = new List<Data>();

            if (_allData.dataList.Find(s => string.IsNullOrEmpty(s.Day)) != null)
            {
                foreach(var data in _allData.dataList)
                {
                    if (!string.IsNullOrEmpty(data.Day))
                    {
                        tempDataList.Add(data);
                    }
                    else
                    {
                        emptyDataList.Add(data);
                    }
                }
                _allData.dataList = tempDataList;
            }
            


                //曜日が空白のセルがあったらソートができなくなってる
                
            //stringをenumに変換しさらにそれをint変換してソート
            _allData.dataList.Sort((a, b) =>
                (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeek), a.Day)) - 
                (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeek), b.Day)));


                //for (int row = 0; row < this.dataGridView1.RowCount; row++)
                //{
                //    for (int col = 0; col < this.dataGridView1.ColumnCount; col++)
                //    {
                //        switch(col)
                //        {
                //            case 0:
                //                this.dataGridView1[col, row].Value = _allData.dataList[row].Check;
                //                break;
                //            case 1:
                //                this.dataGridView1[col, row].Value = _allData.dataList[row].Title;
                //                break;
                //            case 2:
                //                //row1col2でやるとリストの[1]と[3]の値が変わる
                //                this.dataGridView1[col, row].Value = _allData.dataList[row].Time;
                //                break;
                //            case 3:
                //                this.dataGridView1[col, row].Value = _allData.dataList[row].Day;
                //                break;
                //        }

                        
                //    }
                //}



            for (int i = this.dataGridView1.RowCount - 1; i >= 0; i--)
            {
                this.dataGridView1.Rows.RemoveAt(i);
            }

            foreach (Data data in emptyDataList)
            {
                _allData.dataList.Add(data);
            }

            foreach (Data data in _allData.dataList)
            {
                this.dataGridView1.Rows.Add(data.Check, data.Title, data.Time, data.Day,data.ID);
            }



        }

        bool _nowInput = false;
        /// <summary>
        /// 入力した時点でCellValueChangedイベントを起こす
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _nowInput = true;
            //this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            //編集中にソートして消えるのを防止するためにCurrentCellDirtyStateChanged
            //通常はこっちにする?

            //if (this.dataGridView1.Columns.Contains("Time"))
            //{
            //    if (e.ColumnIndex == this.dataGridView1.Columns["Time"].Index)
            //    {
            //        //空白入力時と異常値入力時の空白入力時を回避
            //        if (this.dataGridView1[e.ColumnIndex, e.RowIndex].Value != null &&
            //            !string.IsNullOrEmpty(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString()))
            //        {
            //            int result;
            //            //半角数字以外が入力されたらエラー
            //            if (int.TryParse(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString(), out result))
            //            {
            //                RestTimeDisplay();

            //                //正常値入力→異常値入力とすると入力した正常値に戻らない（データリストが変更されてない）ので
            //                //データリストだけ作り直してelse内のRevert処理を行えるようにする。
            //                //xml書き出しまでやると重いのでこれだけやる
            //                CreateAllData();
            //            }
            //            else
            //            {
            //                MessageBox.Show("正しい値を入力してください。",
            //                                "エラー",
            //                                MessageBoxButtons.OK,
            //                                MessageBoxIcon.Error);


            //                //時間に半角数字以外を入力→隣のｾﾙをクリック→MouseDownイベント起動→エラーメッセージボックスとなり
            //                //MouseUpイベントが起動しないままになる
            //                //その状態でコントロールへカーソルを合わせるとMouseMoveが起動して動く
            //                //のを防止
            //                isDraggable = false;

            //                //空白はキーにしない
            //                if (_allData.dataList.Find(s =>
            //                        s.Title == this.dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()
            //                       && !string.IsNullOrEmpty(s.Title)
            //                       ) != null)
            //                {
            //                    //タイトルをキーとして元の視聴時間を取得
            //                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value =
            //                        _allData.dataList.Find(s =>
            //                            s.Title == this.dataGridView1[e.ColumnIndex - 1, e.RowIndex].Value.ToString()).Time;
            //                }
            //                else
            //                {
            //                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = null;
            //                }
            //            }
            //        }
            //    }
            //}
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



