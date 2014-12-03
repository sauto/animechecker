using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataGridDatas;
using System.Xml;
using System.Collections.ObjectModel;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using Ctrl.ControlMover;

namespace AnimeCheckerByXaml
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region メンバ変数
        public AllDataForXaml _allData;
        const string DATA_FILE_NAME = "data.xml";
        const string CONFIG_FILE_NAME = "config.txt";
        string _dataFileName = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\" + DATA_FILE_NAME;
        string _configFileName = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\" + CONFIG_FILE_NAME;
        string _imgFilePath = string.Empty;

        /// <summary>
        /// 保存する必要があるか
        /// </summary>
        bool IsNotNeedSaved { get; set; }
        
        /// <summary>
        /// 行を追加したかどうか　セーブ時リセット
        /// </summary>
        bool IsAddRow { get; set; }

        /// <summary>
        /// 起動時および保存直後の行数
        /// </summary>
        int DefaultRowNumber { get; set; }

        public enum DayOfWeek
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
        #endregion

        #region 起動時処理
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                if (System.IO.File.Exists(_dataFileName))
                {
                    //データファイルからデータを取得
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(
                        _dataFileName, System.Text.Encoding.GetEncoding("shift_jis")))
                    {
                        System.Xml.Serialization.XmlSerializer serializer =
                            new System.Xml.Serialization.XmlSerializer(typeof(AllDataForXaml));
                        _allData = (AllDataForXaml)serializer.Deserialize(sr);
                    }

                    MainPanel.DataContext = _allData.dataList;
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

                    _allData = new AllDataForXaml();

                    MainPanel.DataContext = _allData.dataList;
                }
            }
            catch (Exception e)
            {
                //なんかバインディングエラーのxamlParseExceptionとか出るので
            }

            IsNotNeedSaved = true;
            IsAddRow = false;
            DefaultRowNumber = _allData.dataList.Count;
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 起動後にDataGridにフォーカスがあっていないので、フォーカス当てて行の追加ができるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upperdatagrid_Loaded(object sender, RoutedEventArgs e)
        {
            ((DataGrid)sender).Focus();
        }

        private void SaveBottun_Initialized(object sender, EventArgs e)
        {
            new ControlMover(((System.Windows.FrameworkElement)sender));
        }

        private void DeleteBottun_Initialized(object sender, EventArgs e)
        {
            new ControlMover(((System.Windows.FrameworkElement)sender));
        }

        private void AddBottun_Initialized(object sender, EventArgs e)
        {
            new ControlMover(((System.Windows.FrameworkElement)sender));
        }

        private void upperdatagrid_Initialized(object sender, EventArgs e)
        {
            new ControlMover(((System.Windows.FrameworkElement)sender));
        }

        private void TextBorder_Initialized(object sender, EventArgs e)
        {
            new ControlMover(((System.Windows.FrameworkElement)sender));
        }
#endregion

        #region 未使用
        /// <summary>
        /// 列の自動生成中にヘッダ名を書き換える　そのままだとプロパティ名になってしまうので
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headerName = e.Column.Header.ToString();

            if (headerName == "Check")
            {
                e.Column.Header = string.Empty;
            }
            else if (headerName == "Title")
            {
                e.Column.Header = "タイトル";
            }
            else if (headerName == "Time")
            {
                e.Column.Header = "視聴時間(分)";
            }
            else if (headerName == "Day")
            {
                e.Column.Header = "放送曜日";
            }
            else if (headerName == "Limit")
            {
                e.Column.Header = "視聴期限";
                e.Column.IsReadOnly = true;
            }
            else if (headerName == "ID")
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            
        }
        #endregion

        #region 残り時間
        /// <summary>
        /// 残り時間の計算
        /// </summary>
        public string RestTime
        {
            get
            {
                int summary = 0;
                foreach (var data in _allData.dataList)
                {
                    if (!data.Check)
                    {
                        int result;
                        if (int.TryParse(data.Time, out result))
                            summary += result;
                    }
                }

                string restTime = "残り時間：" + (summary / 60).ToString() + "時間" + (summary % 60).ToString() + "分";

                TextBlockWidthStretcher(restTime);

                return  restTime;
            }
        }

        /// <summary>
        /// 残り時間を表示するテキストブロックを文字数に合わせて伸ばす
        /// </summary>
        /// <param name="text"></param>
        void TextBlockWidthStretcher(string text)
        {
            TextBorder.Width = text.Length * 12;
            TextBorder.Margin = new Thickness(
                TextBorder.Margin.Left - text.Length * 12, TextBorder.Margin.Top,
                TextBorder.Margin.Right - text.Length * 12, TextBorder.Margin.Bottom);

            RestTimeText.Width = text.Length * 12;
        }

        /// <summary>
        /// チェックが変化したら残り時間変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RestTimeText.Text = RestTime;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RestTimeText.Text = RestTime;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            RestTimeText.Text = RestTime;

            IsNotNeedSaved = false;
        }

        #endregion

        #region 背景画像
        private void pictureBox_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            BitmapImage bmi = null;
            try
            {
                
                bmi = new BitmapImage(new System.Uri(files[0]));
            }
            catch(Exception exp)
            {
                //画像ファイル以外をつっこんだらエラー
                MessageBox.Show(string.Format("エラー：{0}", exp.Message), "エラー",
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
            }

            //Sourceにnull代入すると以後D&Dできなくなるのでnull時は元のソースを代入する
            ((System.Windows.Controls.Image)e.Source).Source = (bmi==null)?((System.Windows.Controls.Image)e.Source).Source:bmi;

            IsNotNeedSaved = false;
        }

        private void pictureBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) != null)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            //textbox型の場合はデフォのドロップを封印　ここでは特に関係ないがメモ
            e.Handled = true;
        }


        private void upperdatagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control & e.Key == Key.V)
            {
                string buffer = Clipboard.GetText();
                if (buffer == string.Empty) return;

                DataGrid dataGrid = (DataGrid)sender;

                int rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
                if (rowIndex < 0) rowIndex = 0;

                //改行を統一
                buffer = buffer.Replace("\r\n", "\n");
                buffer = buffer.Replace('\r', '\n');

                //内部にDisplayIndexを使うと途中で移動してしまうので
                int currentCellIndex = dataGrid.CurrentCell.Column.DisplayIndex;

                foreach (string line in buffer.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (rowIndex > dataGrid.Items.Count - 1) return;

                    int i = currentCellIndex;
                    try
                    {
                        foreach (string pasteData in line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            DataGridColumn column = dataGrid.Columns[i];
                            
                            if (column.Header.ToString() != string.Empty)
                            {
                                //チェックボックス以外
                                column.OnPastingCellClipboardContent(dataGrid.Items[rowIndex], pasteData);
                            }
                            else
                            {
                                PastingCellClipboardContentForCheckBox(dataGrid.Items[rowIndex], pasteData);
                            }
                            i++;
                        }
                    }
                    catch(Exception exp)
                    {
                        Debug.WriteLine(exp);
                    }

                    rowIndex++;
                }

                e.Handled = true;
            }
        }
        #endregion

        #region データグリッド
        /// <summary>
        /// 異常値入力してから同行の別セルをクリックしてもsetプロパティがなぜか動かずエラーが出ない仕様のため、
        /// CellEditEndingイベントで選択セルの行のバインドデータのIDからデータリストのデータを引いて
        /// そのデータの当該プロパティに現在編集中のセルの値を代入させて判定を発生させる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datagrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column.Header.ToString() == "視聴時間(分)")
                {
                    _allData.dataList.First(s => s.ID == ((DataForXaml)e.Row.Item).ID).Time = ((TextBox)e.EditingElement).Text;

                    RestTimeText.Text = RestTime;
                }
                else if (e.Column.Header.ToString() == "放送曜日")
                {
                    _allData.dataList.First(s => s.ID == ((DataForXaml)e.Row.Item).ID).Day = ((TextBox)e.EditingElement).Text;
                }

                IsNotNeedSaved = false;
            }
        }

        /// <summary>
        /// 曜日ソート時だけ独自ソートにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upperdatagrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == "放送曜日")
            {
                e.Handled = true;//デフォのソート実行をさせない

                _allData.dataList.Sort();

                //データバインドし直す
                MainPanel.DataContext = null;
                MainPanel.DataContext = _allData.dataList;
            }

            IsNotNeedSaved = false;
        }

        /// <summary>
        /// チェックボックスだけペーストイベント発生させると組み込みのValidationが起動して
        /// 行頭に！マークがついてチェックボックス以外操作不能になるので直接代入
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pasteData"></param>
        void PastingCellClipboardContentForCheckBox(object data, string pasteData)
        {
            bool result;
            if (!bool.TryParse(pasteData.ToString(), out result))
            {
                MessageBox.Show("エラー", "エラー", MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
            }
            ((DataForXaml)data).Check = result;
        }

        private void Title_PastingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {
            ((DataForXaml)e.Item).Title = e.Content.ToString();
        }

        private void Time_PastingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {
            ((DataForXaml)e.Item).Time = e.Content.ToString();
        }

        private void Day_PastingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {
            ((DataForXaml)e.Item).Day = e.Content.ToString();
        }

        /// <summary>
        /// 削除時にSelectedCellが変化するのを利用して削除フラグが立っている間だけ残り時間を更新する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upperdatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (BindingCommand.BindingCommands.GetSingleton._deleteflag)
                RestTimeText.Text = RestTime;
        }
        #endregion

        #region 保存
        /// <summary>
        /// デフォで実装されているSaveコマンドの実体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_ExecutedSave(object sender, ExecutedRoutedEventArgs e)
        {
            WriteXml(CreateSaveDataFromDataGrid());

            DefaultRowNumber = _allData.dataList.Count;

            IsNotNeedSaved = true;
        }

        /// <summary>
        /// datagridからセーブデータを生成
        /// </summary>
        /// <returns></returns>
        AllDataForXaml CreateSaveDataFromDataGrid()
        {
            var alldataForSave = new AllDataForXaml();

            foreach (DataForXaml item in upperdatagrid.Items)
            {
                alldataForSave.dataList.Add(item);
            }

            return alldataForSave;
        }

        /// <summary>
        /// xmlを書き出す
        /// </summary>
        /// <param name="obj"></param>
        private void WriteXml(object obj)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(AllDataForXaml));
                System.IO.StreamWriter sw = new System.IO.StreamWriter(_dataFileName, false, System.Text.Encoding.GetEncoding("shift_jis"));

                serializer.Serialize(sw, obj);
                sw.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }


        }

        

        /// <summary>
        /// 終了時にデータ保存確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (DefaultRowNumber != _allData.dataList.Count)
                IsNotNeedSaved = false;

            if (!IsNotNeedSaved)
            {
                switch (MessageBox.Show("保存して閉じますか？", "終了の確認", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No))
                {
                    case MessageBoxResult.Yes:
                        WriteXml(CreateSaveDataFromDataGrid());
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }


#endregion

        /// <summary>
        /// デフォで実装されているCloseコマンドの実体 メニューの閉じる実行時発動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_ExecutedClose(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        



        

        
        

    }
}
