﻿using System;
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
using test.DAndDSizeChanger;

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



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //チェックしたら時間を0にする

            //チェックボックスのCheckedが取得できない
            //CheckBoxクラスならプロパティがあるがこれだとない
            //連打するとチェック連動が反映されない
            //DataGridViewCheckBoxColumnからCheckBoxを参照できないのか？
            //CellContentClickはダブルクリックに反応しない

            RestTimeDisplay();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RestTimeDisplay();
        }

        /// <summary>
        /// 視聴時間列のデータを合計し残り視聴時間を表示する
        /// </summary>
        private void RestTimeDisplay()
        {
            int summary = 0;
            int result = 0;

            //視聴時間列のデータを合計する
            for (int i = 0; i < this.dataGridView1.RowCount; i++)
            {
                bool check;
                bool.TryParse(this.dataGridView1[this.dataGridView1.Columns["Check"].Index, i].Value.ToString(), out check);
                if (!check)
                {
                    if (this.dataGridView1[this.dataGridView1.Columns["Time"].Index, i].Value != null)
                    {
                        int.TryParse(this.dataGridView1[this.dataGridView1.Columns["Time"].Index, i].Value.ToString(), out result);
                    }
                    summary += result;
                }
            }

            //残り時間の表示
            RestTime.Text = (summary / 60).ToString() + "時間" + (summary % 60).ToString() + "分";
        }


        /// <summary>
        /// Dirty受ける用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        #region 表データ保存
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
            if(GridInputCheck())
                SaveGrid();
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
        #endregion


        /// <summary>
        /// 背景画像の変更 ピクチャボックスはフォームにドッキングしてる
        /// ピクチャボックス右上の再生ボタン的なのをクリックすると設定可
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            if (string.IsNullOrEmpty(_imgFilePath))
                ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(_imgFilePath);
            //[ファイルの種類]ではじめに「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = test1.Properties.Settings.Default.I0001;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(ofd.FileName);
                this.pictureBox1.Image = img;
                _imgFilePath = ofd.FileName;
            }

            //画像がでかい場合縮小表示する
            /*if (this.pictureBox1.Image.Height >= SystemInformation.MaxWindowTrackSize.Height
            //    || this.pictureBox1.Image.Width >= SystemInformation.MaxWindowTrackSize.Width)
            //{
            //    //描画先とするImageオブジェクトを作成する
            //    Bitmap canvas = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            //    //ImageオブジェクトのGraphicsオブジェクトを作成する
            //    Graphics g = Graphics.FromImage(canvas);

            //    //Bitmapオブジェクトの作成
            //    Bitmap image = new Bitmap(_imgFilePath);
            //    //補間方法として高品質双三次補間を指定する
            //    g.InterpolationMode =
            //        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    //画像を縮小して描画する
            //    g.DrawImage(image, 0, this.menuStrip1.Size.Height, SystemInformation.MaxWindowTrackSize.Width, SystemInformation.MaxWindowTrackSize.Height);

            //    //BitmapとGraphicsオブジェクトを破棄
            //    image.Dispose();
            //    g.Dispose();

            //    //PictureBox1に表示する
            //    this.pictureBox1.Image = canvas;
            }*/
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
            if(GridInputCheck())
                this.dataGridView1.Rows.Add(false, "", "", "", this.dataGridView1.RowCount + 1);
        }

        /// <summary>
        /// 曜日列をクリックしたとき曜日順にソート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            GridInputCheck();

            CreateAllData();

            if (e.ColumnIndex == this.dataGridView1.Columns["WeekColumn"].Index)
            {
                WeekSort();
            }

            RestTimeDisplay();
        }

        /// <summary>
        /// 行削除フラグ
        /// </summary>
        bool _rowDeleteFlag = false;
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
                            
            //stringをenumに変換しさらにそれをint変換してソート
            _allData.dataList.Sort((a, b) =>
                (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeek), a.Day)) - 
                (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeek), b.Day)));

            //行削除でCurrentCellChangedが起動するとCreateAllDataが走ってデータを増やして行が増えるのを防止
            _rowDeleteFlag = true;
            for (int i = this.dataGridView1.RowCount - 1; i >= 0; i--)
            {
                this.dataGridView1.Rows.RemoveAt(i);
            }
            _rowDeleteFlag = false;

            foreach (Data data in emptyDataList)
            {
                _allData.dataList.Add(data);
            }

            foreach (Data data in _allData.dataList)
            {
                this.dataGridView1.Rows.Add(data.Check, data.Title, data.Time, data.Day,data.ID);
            }
        }

        /// <summary>
        /// 入力した時点で値を確定しCellValueChangedイベントを起こす
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }


        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (!_rowDeleteFlag)
              GridInputCheck();
        }

        /// <summary>
        /// セルの入力をチェック 行を指定することで余計なセルを検索しないようにできる
        /// </summary>
        private bool GridInputCheck(int startRow = 0)
        {
            for (int row = startRow; row < this.dataGridView1.RowCount; row++)
            {
                for (int col = this.dataGridView1.Columns["Time"].Index; col <= this.dataGridView1.Columns["WeekColumn"].Index; col++)
                {
                    switch (col)
                    {
                        case 2:
                            //空白入力時と異常値入力時の空白入力時を回避
                            if (this.dataGridView1[col, row].Value != null &&
                                !string.IsNullOrEmpty(this.dataGridView1[col, row].Value.ToString()))
                            {
                                
                                int result;
                                //半角数字以外が入力されたらエラー
                                if (int.TryParse(this.dataGridView1[col, row].Value.ToString(), out result))
                                {
                                    RestTimeDisplay();                                    
                                }
                                else
                                {
                                    MessageBox.Show(test1.Properties.Settings.Default.E0002,
                                                    "エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);

                                    if (FindDataFromID(row) != null)
                                    {
                                        //非表示のID列を作って、IDをキーとして元の視聴時間を取得
                                        this.dataGridView1[col, row].Value = FindDataFromID(row).Time;
                                    }
                                    else
                                    {
                                        this.dataGridView1[col, row].Value = null;
                                    }
                                    //これがないと入力→列先頭クリックしてエラーだしても値が元に戻らず
                                    //セルを離れると元に戻る
                                    this.dataGridView1.RefreshEdit();

                                    RestTimeDisplay();

                                    return false;
                                }
                            }
                            break;
                        case 3:
                            if (this.dataGridView1[col, row].Value != null &&
                                !string.IsNullOrEmpty(this.dataGridView1[col, row].Value.ToString()))
                            {
                                if (Enum.IsDefined(typeof(DayOfWeek), this.dataGridView1[col, row].Value.ToString()))
                                {
                                    
                                }
                                else
                                {
                                    MessageBox.Show(test1.Properties.Settings.Default.E0001,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    if (FindDataFromID(row) != null)
                                    {
                                        //非表示のID列を作って、IDをキーとして元の曜日を取得
                                        this.dataGridView1[col, row].Value = FindDataFromID(row).Day;
                                    }
                                    else
                                    {
                                        this.dataGridView1[col, row].Value = null;
                                    }
                                    //これがないと入力→列先頭クリックしてエラーだしても値が元に戻らず
                                    //セルを離れると元に戻る
                                    this.dataGridView1.RefreshEdit();

                                    RestTimeDisplay();
                                    return false;
                                }
                                
                            }
                            break;
                    }


                }
            }
            //異常値がなければグリッドデータ更新
            CreateAllData();
            return true;
        }

        /// <summary>
        /// データリストからIDで現在のセルのデータを検索
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Data FindDataFromID(int row)
        {
            return _allData.dataList.Find(
                s =>s.ID == this.dataGridView1[this.dataGridView1.Columns["ID"].Index, row].Value.ToString());
        }

        /// <summary>
        /// 編集中のコントロールを取得する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.PreviewKeyDown -= new PreviewKeyDownEventHandler(dataGridView1_PreviewKeyDown);

                //該当する列か調べる
                if (this.dataGridView1.CurrentCell.OwningColumn.Name == "WeekColumn"
                    || this.dataGridView1.CurrentCell.OwningColumn.Name == "Time")
                {
                    //KeyPressイベントハンドラを追加
                    tb.PreviewKeyDown += new PreviewKeyDownEventHandler(dataGridView1_PreviewKeyDown);
                }
            }
        }

        /// <summary>
        /// KewDownイベントだとEnterキー押下を検知しないのでこっち
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GridInputCheck();
        }


        /// <summary>
        /// 他のコントロールクリック時に入力チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            GridInputCheck();
        }

        /// <summary>
        /// 選択行の削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                dataGridView1.Rows.RemoveAt(_selectRow);
                //最終行を削除した場合選択行のインデックスが変わらないのでエラーになるのを防止
                if (_selectRow == dataGridView1.Rows.Count)
                    _selectRow -= 1;
            }
        }

        /// <summary>
        /// セルクリック時に選択したセルの行数
        /// </summary>
        int _selectRow = 0;

        /// <summary>
        /// 現在選択中の行を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _selectRow = e.RowIndex;
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
            //else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
            //    isDraggable = true;
            //    _lastMouseMovePoint.X = e.X;
            //    _lastMouseMovePoint.Y = e.Y;
            //    _lastMouseMoveSize = this.dataGridView1.Size;
            //}

        }

        Size _lastMouseMoveSize = new Size();
        Point _lastMouseMovePoint = new Point();
        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (Math.Abs(this.dataGridView1.Size.Height - e.Y) < 10)
            //{
            //    this.dataGridView1.Cursor = Cursors.SizeNS;
            //}
            //else
            //{
            //    this.dataGridView1.Cursor = Cursors.Arrow;
            //}

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
            //else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
                
            //    int diffY = _lastMouseMovePoint.Y - e.Y;

            //        this.dataGridView1.Size =
            //            new Size(this.dataGridView1.Size.Width,
            //                _lastMouseMoveSize.Height - diffY);
            //}
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
                isDraggable = false;
                
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

        private void DeleteButton_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                point.X = e.X;
                point.Y = e.Y;
            }
        }

        private void DeleteButton_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || _isFixLayout)
                {
                    return;
                }
                //移動処理
                this.DeleteButton.Left += e.X - point.X;
                this.DeleteButton.Top += e.Y - point.Y;
            }
        }

        private void DeleteButton_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = false;
            }
        }

        #endregion

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
            //画面サイズ
            sw.Write(this.Size.Height);
            sw.Write(Environment.NewLine);
            sw.Write(this.Size.Width);
            sw.Write(Environment.NewLine);
            //行削除ボタン
            sw.Write(this.DeleteButton.Left);
            sw.Write(Environment.NewLine);
            sw.Write(this.DeleteButton.Top);
            sw.Write(Environment.NewLine);
            //グリッドのサイズ
            sw.Write(this.dataGridView1.Height);
            sw.Write(Environment.NewLine);
            sw.Write(this.dataGridView1.Width);
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

            int height = 0;
            int row = 1;
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                switch (row)
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
                    case 10:
                        height = int.Parse(line);
                        break;
                    case 11:
                        //Height、Widthプロパティには値型を直接入力できない
                        //参考：http://www.atmarkit.co.jp/fdotnet/onepoint/onepoint02/onepoint02_01.html
                        this.Size = new Size(int.Parse(line), height);
                        break;
                    case 12:
                        this.DeleteButton.Left = int.Parse(line);
                        break;
                    case 13:
                        this.DeleteButton.Top = int.Parse(line);
                        break;
                    case 14:
                        height = int.Parse(line);
                        break;
                    case 15:
                        this.dataGridView1.Size = new Size(int.Parse(line), height);
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

        /// <summary>
        /// 画面を閉じるときレイアウトを保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayout();
        }

        /// <summary>
        /// レイアウトの固定ON/OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreLayout();
        }
        #endregion

        DAndDSizeChanger sizeChanger;
        private void Form1_Load(object sender, EventArgs e)
        {
            sizeChanger = new DAndDSizeChanger(this.dataGridView1, this.dataGridView1, DAndDArea.All, 8);
        }

        private void AllCheckOFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int row=0;row<this.dataGridView1.RowCount;row++)
            {
                this.dataGridView1[this.dataGridView1.Columns["Check"].Index, row].Value = false;
            }
            this.dataGridView1.RefreshEdit();
            RestTimeDisplay();
        }


        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

            //セルペースト　
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (this.dataGridView1.CurrentCell == null || this.dataGridView1.CurrentCell.Value == null)
                    return;

                string pasteText = Clipboard.GetText();
                if (string.IsNullOrEmpty(pasteText))
                    return;
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimStart(new char[] { '\t' });//行コピー時ヘッダのタブ文字削除
                pasteText = pasteText.TrimEnd(new char[] { '\n' });


                int colCount = 0;
                int rowCount = 0;
                string[] rowParts = pasteText.Split('\n');//Lengthが行数
                //コピーセル格納配列
                object[,] array = new object[this.dataGridView1.ColumnCount - 1, rowParts.Length];

                for (int row = 0; row < rowParts.Length; row++)
                {
                    string[] colParts = rowParts[row].Split('\t');

                    for (int col = 0; col < colParts.Length; col++)
                    {
                        array[col,row] = colParts[col];
                    }
                }



                    ////改行の度に行カウントを増加、タブ文字の度に列カウント増加
                    ////Splitしても各値が改行とタブ文字のどっちで区切ったかわからん
                    //for (int i = 0; i < pasteText.Length; i++)
                    //{
                    //    if (pasteText[i] == '\n')
                    //    {
                    //        rowCount++;
                    //    }
                    //    else if (pasteText[i] == '\t')
                    //    {
                    //        colCount++;
                    //    }
                    //}

                //カレントセルを起点として取得
                //そこをforの起点として順次入れていく　途中でエラーしたらそれまでをどうやって戻す？データリストから再生


                if (this.dataGridView1.CurrentCell.ColumnIndex == this.dataGridView1.Columns["Check"].Index)
                {
                    bool result;
                    bool.TryParse(pasteText, out result);
                    this.dataGridView1.CurrentCell.Value = result;
                    this.dataGridView1.RefreshEdit();
                }
                else if (this.dataGridView1.CurrentCell.ColumnIndex == this.dataGridView1.Columns["Title"].Index)
                {
                    this.dataGridView1.CurrentCell.Value = pasteText;
                }
                else if (this.dataGridView1.CurrentCell.ColumnIndex == this.dataGridView1.Columns["Time"].Index)
                {
                    int result;
                    if (int.TryParse(pasteText, out result))
                        this.dataGridView1.CurrentCell.Value = result;
                    else
                        MessageBox.Show(test1.Properties.Settings.Default.E0002,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (this.dataGridView1.CurrentCell.ColumnIndex == this.dataGridView1.Columns["WeekColumn"].Index)
                {
                    if (Enum.IsDefined(typeof(DayOfWeek), this.dataGridView1.CurrentCell.Value.ToString()))
                    {
                        this.dataGridView1.CurrentCell.Value = Enum.Parse(typeof(DayOfWeek), pasteText);
                    }
                    else
                        MessageBox.Show(test1.Properties.Settings.Default.E0001,
                                                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                GridInputCheck();
            }
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



