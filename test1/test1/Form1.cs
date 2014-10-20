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
using test.DAndDSizeChanger;
using test.ControlMover;

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
            日 = 0,
            月,
            火,
            水,
            木,
            金,
            土,
        }

        enum ColumnName
        {
            Check = 0,
            Title,
            Time,
            DayOfWeek,
            ID,
            Limit
        }

        public Form1()
        {
            InitializeComponent();

            SaveDefaultLayout();

            if(System.IO.File.Exists(_dataFileName))
            {
                //データファイルからデータを取得
                using (System.IO.StreamReader sr = new System.IO.StreamReader(
                    _dataFileName, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    System.Xml.Serialization.XmlSerializer serializer =
                        new System.Xml.Serialization.XmlSerializer(typeof(AllData));
                    _allData = (AllData)serializer.Deserialize(sr);
                }

                //行の追加
                foreach (Data data in _allData.dataList)
                {
                    this.dataGridView1.Rows.Add(data.Check, data.Title, data.Time,data.Day,data.ID);
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

            RestTimeDisplay();
        }



        /// <summary>
        /// グリッドデータリストの作成
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
                        case (int)ColumnName.Check:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Check = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Check = false.ToString();
                            break;
                        case (int)ColumnName.Title:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Title = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Title = string.Empty;
                            break;
                        case (int)ColumnName.Time:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Time = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Time = string.Empty;
                            break;
                        case (int)ColumnName.DayOfWeek:
                            if (this.dataGridView1[col, row].Value != null)
                                data.Day = this.dataGridView1[col, row].Value.ToString();
                            else
                                data.Day = string.Empty;
                            break;
                        case (int)ColumnName.ID:
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


        #region チェックボックス
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //チェックしたら時間を0にする

            //チェックボックスのCheckedが取得できない
            //CheckBoxクラスならプロパティがあるがこれだとない
            //連打するとチェック連動が反映されない
            //DataGridViewCheckBoxColumnからCheckBoxを参照できないのか？
            //CellContentClickはダブルクリックに反応しない

            RestTimeDisplay();

            CellColorChangeByCheck(e.ColumnIndex, e.RowIndex);

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RestTimeDisplay();

            CellColorChangeByCheck(e.ColumnIndex,e.RowIndex);
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
        /// チェックボックスの値に応じて視聴期限セルを黒く塗りつぶす
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        void CellColorChangeByCheck(int col,int row)
        {
            if (col == (int)ColumnName.Check && row >= 0)
            {
                bool check;
                bool.TryParse(this.dataGridView1[col, row].Value.ToString(), out check);
                if (check)
                {
                    this.dataGridView1[(int)ColumnName.Limit, row].Style.BackColor = Color.Black;
                }
                else
                {
                    this.dataGridView1[(int)ColumnName.Limit, row].Style.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        /// ソート時にセル塗りつぶしが追従しないのを防止（全削除しているため）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CellColorChangeByCheck((int)ColumnName.Check, e.RowIndex);
        }
        #endregion

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

        #region ピクチャボックス
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
        #endregion

        #region 行の追加
        /// <summary>
        /// 行の追加ボタンのクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add(false, "", "", "", this.dataGridView1.RowCount + 1);


        }
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(GridInputCheck())
                this.dataGridView1.Rows.Add(false, "", "", "", this.dataGridView1.RowCount + 1);
        }
        #endregion

        #region datagridview関連
        /// <summary>
        /// Dirty受ける用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }


        /// <summary>
        /// 曜日列をクリックしたとき曜日順にソート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //入力チェック
            GridInputCheck();

            CreateAllData();

            if (e.ColumnIndex == this.dataGridView1.Columns["WeekColumn"].Index)
            {
                WeekSort();
            }

            RestTimeDisplay();

            LimitRenewal();
　
        }

        /// <summary>
        /// 視聴期限の更新
        /// </summary>
        void LimitRenewal()
        {
            //視聴期限算出　残り0日の場合は残り7日にする
            for (int row = 0; row < this.dataGridView1.RowCount; row++)
            {
                if (Enum.IsDefined(typeof(DayOfWeek), this.dataGridView1[(int)ColumnName.DayOfWeek, row].Value))
                {
                    int limit = (int)((DayOfWeek)Enum.Parse(typeof(DayOfWeek), this.dataGridView1[(int)ColumnName.DayOfWeek, row].Value.ToString()))
                         - (int)DateTime.Today.DayOfWeek + 7;

                    this.dataGridView1[(int)ColumnName.Limit, row].Value = "あと" + ((limit % 7 == 0 ? 7 : 0) + (limit % 7)) + "日";
                }
            }
        }

        /// <summary>
        /// 行削除中フラグ
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
                        case (int)ColumnName.Time:
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
                        case (int)ColumnName.DayOfWeek:
                            if (this.dataGridView1[col, row].Value != null &&
                                !string.IsNullOrEmpty(this.dataGridView1[col, row].Value.ToString()))
                            {
                                if (Enum.IsDefined(typeof(DayOfWeek), this.dataGridView1[col, row].Value.ToString()))
                                {
                                    LimitRenewal();
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
        /// datagridviewで編集中のコントロールを取得してエンターキー押下時に入力チェックするイベントを追加する
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
        /// KewDownイベントだとEnterキー押下を検知しないのでこっちで入力チェック
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


        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            #region コピー
            List<Point> rowAndColList = new List<Point>();
            List<int> copyRowList = new List<int>();
            List<int> copyColumnList = new List<int>();

            //コピー
            if (e.Control && e.KeyCode == Keys.C)
            {
                var cells = this.dataGridView1.SelectedCells;
                foreach (DataGridViewCell cell in cells)
                {
                    rowAndColList.Add(new Point(cell.RowIndex, cell.ColumnIndex));
                    int b = copyColumnList.Find(s => s == cell.ColumnIndex);
                    if (copyColumnList.Count == 0 || !copyColumnList.Contains(cell.ColumnIndex))
                    {
                        copyColumnList.Add(cell.ColumnIndex);
                    }
                    if (copyRowList.Count == 0 || !copyRowList.Contains(cell.RowIndex))
                    {
                        copyRowList.Add(cell.RowIndex);
                    }
                }
                int copyRangeColumn = copyColumnList.Count;
                int copyRangeRow = copyRowList.Count;
                bool validCopyFlag = true;

                //全てのXがコピーした列数分だけ存在するか、すべてのYがコピーした行数分だけ存在する
                foreach (var y in copyColumnList)
                {
                    validCopyFlag = true;
                    if (rowAndColList.FindAll(s => s.Y == y).Count != copyRangeRow)
                    {
                        validCopyFlag = false;
                        break;
                    }
                }
                foreach (var x in copyRowList)
                {
                    validCopyFlag = true;
                    if (rowAndColList.FindAll(s => s.X == x).Count != copyRangeColumn)
                    {
                        validCopyFlag = false;
                        break;
                    }
                }
                if (!validCopyFlag)
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0004,
                                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            #endregion

            #region ペースト

            if (e.Control && e.KeyCode == Keys.V)
            {
                if (this.dataGridView1.CurrentCell == null || this.dataGridView1.CurrentCell.Value == null)
                    return;

                string pasteText = Clipboard.GetText();

                if (string.IsNullOrEmpty(pasteText))
                    return;

                //改行を統一
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimStart(new char[] { '\t' });//行コピー時ヘッダのタブ文字削除
                pasteText = pasteText.TrimEnd(new char[] { '\n' });//終端の改行削除

                //Lengthが行数
                string[] rowParts = pasteText.Split('\n');

                //タブ文字を挿入して表が作れるようにする　カレントセルの列の分だけタブ文字を追加することでnullマスを先頭に追加
                string tabString = string.Empty;
                for (int i = 0; i < this.dataGridView1.CurrentCell.ColumnIndex; i++)
                {
                    tabString += "\t";
                }
                for (int i = 0; i < rowParts.Length; i++)
                {
                    //Insert(0,value)だとできない　先頭や終端に挿入する場合は連結演算子の方が早いらしい
                    //http://www.dotnetperls.com/insert
                    string temp = tabString;
                    temp += rowParts[i];
                    rowParts[i] = temp;
                }

                //コピーセル格納配列
                string[,] array = new string[this.dataGridView1.ColumnCount, this.dataGridView1.RowCount];

                //カレントセルを基準として一番列数が多いセルに合わせてタブ文字が増える　金\n\t30\t\n遊戯王\t\t
                //異なる行列を選択した場合コピーは受け付けない

                int maxEnableCopyRow = this.dataGridView1.RowCount - this.dataGridView1.CurrentCell.RowIndex;
                int pasteRange = 0;

                pasteRange = rowParts.Length <= maxEnableCopyRow ? rowParts.Length : maxEnableCopyRow;

                int copyRow = 0;
                for (int row = this.dataGridView1.CurrentCell.RowIndex;
                    row < this.dataGridView1.CurrentCell.RowIndex + pasteRange;
                    row++)
                {
                    string[] colParts = rowParts[copyRow].Split('\t');

                    for (int col = 0; col < colParts.Length; col++)
                    {
                        array[col, row] = colParts[col];
                    }
                    copyRow++;
                }



                bool breakFlag = false;
                //カレントセルを起点として取得
                //そこをforの起点として順次入れていく　途中でエラーしたらデータリストから再生
                for (int row = 0; row < this.dataGridView1.RowCount; row++)
                {
                    if (breakFlag)
                        break;

                    for (int col = 0; col < this.dataGridView1.ColumnCount; col++)
                    {
                        if (!string.IsNullOrEmpty(array[col, row]))
                        {
                            if (!PasteValidCheck(col, row, array))
                            {
                                breakFlag = true;
                                break;
                            }
                        }
                    }
                }

                if (breakFlag)
                {
                    for (int row = 0; row < this.dataGridView1.RowCount; row++)
                    {
                        for (int col = 0; col < this.dataGridView1.ColumnCount; col++)
                        {
                            switch (col)
                            {
                                case (int)ColumnName.Check:
                                    this.dataGridView1[col, row].Value = _allData.dataList[row].Check;
                                    break;
                                case (int)ColumnName.Title:
                                    this.dataGridView1[col, row].Value = _allData.dataList[row].Title;
                                    break;
                                case (int)ColumnName.Time:
                                    this.dataGridView1[col, row].Value = _allData.dataList[row].Time;
                                    break;
                                case (int)ColumnName.DayOfWeek:
                                    this.dataGridView1[col, row].Value = _allData.dataList[row].Day;
                                    break;
                                case (int)ColumnName.ID:
                                    this.dataGridView1[col, row].Value = _allData.dataList[row].ID;
                                    break;
                            }
                        }
                    }
                }

                GridInputCheck(this.dataGridView1.CurrentCell.RowIndex);
            }
            #endregion
        }

        /// <summary>
        /// ペーストした値が正しいかチェック
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="array">コピーしたセルすべて</param>
        /// <returns></returns>
        bool PasteValidCheck(int col, int row, string[,] array)
        {
            if (col == this.dataGridView1.Columns["Check"].Index)
            {
                bool result;
                if (bool.TryParse(array[col, row], out result))
                {
                    this.dataGridView1[col, row].Value = result;
                    this.dataGridView1.RefreshEdit();
                    return true;
                }
                else
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0003,
                                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else if (col == this.dataGridView1.Columns["Title"].Index)
            {
                this.dataGridView1[col, row].Value = array[col, row];
            }
            else if (col == this.dataGridView1.Columns["Time"].Index)
            {
                int result;
                if (int.TryParse(array[col, row], out result))
                {
                    this.dataGridView1[col, row].Value = result;
                    return true;
                }
                else
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0002,
                                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else if (col == this.dataGridView1.Columns["WeekColumn"].Index)
            {
                if (Enum.IsDefined(typeof(DayOfWeek), array[col, row]))
                {
                    this.dataGridView1[col, row].Value =
                        Enum.Parse(typeof(DayOfWeek), array[col, row]);
                    return true;
                }
                else
                {
                    MessageBox.Show(test1.Properties.Settings.Default.E0001,
                                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 行の削除
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
        #endregion

        #region コントロールの移動イベント

        DAndDSizeChanger _sizeChanger;
        ControlMover _controlMover;
        private void Form1_Load(object sender, EventArgs e)
        {
            _sizeChanger = new DAndDSizeChanger(this.dataGridView1, this.dataGridView1, DAndDArea.All, 8);
            _controlMover = new ControlMover(this.DeleteButton, this.DeleteButton);
            _controlMover = new ControlMover(this.SaveButton,this.SaveButton);
            _controlMover = new ControlMover(this.panel1,this.panel1);
            _controlMover = new ControlMover(this.RestTime,this.panel1);
            _controlMover = new ControlMover(this.RestTimeText,this.panel1);
            _controlMover = new ControlMover(this.AddButton, this.AddButton);
            _controlMover = new ControlMover(this.dataGridView1, this.dataGridView1);
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
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_configFileName, false, System.Text.Encoding.GetEncoding("shift_jis")))
            {
                //画像のパス
                sw.Write(_imgFilePath + Environment.NewLine);
                //行追加ボタン
                sw.Write(this.AddButton.Left + Environment.NewLine);
                sw.Write(this.AddButton.Top + Environment.NewLine);
                //グリッド
                sw.Write(this.dataGridView1.Left + Environment.NewLine);
                sw.Write(this.dataGridView1.Top + Environment.NewLine);
                //残り時間
                sw.Write(this.panel1.Left + Environment.NewLine);
                sw.Write(this.panel1.Top + Environment.NewLine);
                //保存
                sw.Write(this.SaveButton.Left + Environment.NewLine);
                sw.Write(this.SaveButton.Top + Environment.NewLine);
                //画面サイズ
                sw.Write(this.Size.Height + Environment.NewLine);
                sw.Write(this.Size.Width + Environment.NewLine);
                //行削除ボタン
                sw.Write(this.DeleteButton.Left + Environment.NewLine);
                sw.Write(this.DeleteButton.Top + Environment.NewLine);
                //グリッドのサイズ
                sw.Write(this.dataGridView1.Height + Environment.NewLine);
                sw.Write(this.dataGridView1.Width + Environment.NewLine);
                //レイアウトを固定するかどうか
                sw.Write(this.FixLayoutToolStripMenuItem.Checked + Environment.NewLine);
            }
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
                    case 16:
                        this.FixLayoutToolStripMenuItem.Checked = bool.Parse(line);
                        break;
                }
                row++;
            }
            sr.Close();
        }

        /// <summary>
        /// レイアウトの固定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ControlMover.IsFixLayout)
            {
                this.FixLayoutToolStripMenuItem.Checked = true;
                ControlMover.IsFixLayout = true;
            }
            else
            {
                this.FixLayoutToolStripMenuItem.Checked = false;
                ControlMover.IsFixLayout = false;
            }
        }
        
        /// <summary>
        /// Checkedプロパティの変更時に変更した値をControlMoverに設定する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixLayoutToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ControlMover.IsFixLayout = this.FixLayoutToolStripMenuItem.Checked;
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
        /// レイアウトの初期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreLayout();
        }
        #endregion



        private void AllCheckOFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int row=0;row<this.dataGridView1.RowCount;row++)
            {
                this.dataGridView1[this.dataGridView1.Columns["Check"].Index, row].Value = false;
                this.dataGridView1[this.dataGridView1.Columns["Limit"].Index, row].Style.BackColor = Color.White;
            }
            this.dataGridView1.RefreshEdit();
            RestTimeDisplay();
        }






    }
}



