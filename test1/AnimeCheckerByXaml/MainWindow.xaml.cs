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


    }
}
