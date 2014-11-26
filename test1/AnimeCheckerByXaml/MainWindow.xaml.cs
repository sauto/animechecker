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
using GridDatas;
using System.Xml;
using System.Collections.ObjectModel;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;


namespace AnimeCheckerByXaml
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public AllDataForXaml _allData;
        const string DATA_FILE_NAME = "data.xml";
        const string CONFIG_FILE_NAME = "config.txt";
        string _dataFileName = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\" + DATA_FILE_NAME;
        string _configFileName = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\" + CONFIG_FILE_NAME;
        string _imgFilePath = string.Empty;

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
            }
            catch (Exception e)
            {
                //なんかバインディングエラーのxamlParseExceptionとか出るので
            }
            

            MainPanel.DataContext = _allData.dataList;
        }

        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if(e.Key == Key.LeftCtrl)
        //    {
        //        textBox1.Text = "11";
        //    }
        //}

        //private void textBox0_KeyDown(object sender, KeyEventArgs e)
        //{
        //    ((DataForXaml)_MainWindow.DataContext).Title = "dksajf;";
        //}

        //private void textBox0_Loaded(object sender, RoutedEventArgs e)
        //{
        //    _MainWindow.DataContext = new DataForXaml() { Title = "bbb" };

        //    //Binding bindingTitle = new Binding("Title");
        //    //bindingTitle.Mode = BindingMode.OneWay;
        //    //bindingTitle.Source = bindingData;
        //    //textBlock0.SetBinding(TextBlock.TextProperty, bindingTitle);

        //}

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //MainPanel.DataContext = 
        }

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

        /// <summary>
        /// 異常値入力してから同行の別セルをクリックしてもsetプロパティがなぜか動かずエラーが出ないため、
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
                }
                else if (e.Column.Header.ToString() == "放送曜日")
                {
                    _allData.dataList.First(s => s.ID == ((DataForXaml)e.Row.Item).ID).Day = ((TextBox)e.EditingElement).Text;
                }
               
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
        }



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

            //textbox型の場合はデフォのドロップを封印
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

                //内部にDisplayIndexをしちゃうと途中で移動してしまうので
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


    }
}
