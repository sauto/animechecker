using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataGridDatas;
using System.Windows.Controls;

namespace BindingCommand
{
    public class BindingCommands
    {

        private static BindingCommands instance = new BindingCommands();

        private BindingCommands() { }

        public static BindingCommands GetSingleton
        {
            get
            {
                return instance;
            }
        }

       

        #region　行の追加

        public AddRowCommandImplement AddRowCommandImplementClass = new AddRowCommandImplement();

        /// <summary>
        /// 普通にstaticメソッドでよかったかも
        /// </summary>
        public class AddRowCommandImplement : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                //初期化
                var newData = new DataForXaml();
                newData.Check = false;
                newData.Title = newData.Time = newData.Day = string.Empty;

                //データコンテキスト取得
                var dataList = ((SortableObservableCollection)((Canvas)parameter).DataContext);

                //ID生成
                newData.ID = GenerateNewId(dataList);

                dataList.Add(newData);
            }

            /// <summary>
            /// 重複しないように新しいIDを生成
            /// </summary>
            /// <returns></returns>
            int GenerateNewId(SortableObservableCollection dataList)
            {
                List<int> idList = new List<int>();
                for (int index = 0; index < dataList.Count; index++)
                {
                    idList.Add(dataList[index].ID);
                }

                int id = 0;
                while (true)
                {
                    if (!idList.Contains(id))
                    {
                        break;
                    }
                    id++;
                }

                return id;
            }
        }

        #endregion

        #region　行の削除

        public DeleteRowCommandImplement DeleteRowCommandImplementClass = new DeleteRowCommandImplement();

        /// <summary>
        /// 削除処理中フラグ
        /// </summary>
        public bool _deleteflag = false;

        /// <summary>
        /// 普通にstaticメソッドでよかったかも
        /// </summary>
        public class DeleteRowCommandImplement : ICommand
        {
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                BindingCommands.instance._deleteflag = true;
                DataGrid datagrid = null;
                foreach (var childElement in ((Canvas)parameter).Children)
                {
                    if (childElement is DataGrid)
                    {
                        datagrid = (DataGrid)childElement;
                    }
                }

                //データコンテキスト取得
                var dataList = ((SortableObservableCollection)((Canvas)parameter).DataContext);

                if (dataList.Count > 0)
                {
                    var selectCells = datagrid.SelectedCells;

                    List<int> idList = new List<int>();

                    //削除する行のデータのIDリストを作成
                    foreach (DataGridCellInfo cell in datagrid.SelectedCells)
                    {
                        if (!idList.Contains(((DataForXaml)cell.Item).ID))
                        {
                            idList.Add(((DataForXaml)cell.Item).ID);
                        }
                    }

                    //全てのIDに対して全行を検索しIDが一致する行数を削除
                    foreach (int id in idList)
                    {
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            if (dataList[i].ID == id)
                            {
                                dataList.RemoveAt(i);
                                break;
                            }

                        }
                    }


                }

                BindingCommands.instance._deleteflag = false;
            }
        }

        #endregion


    }
}
