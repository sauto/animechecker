using System.Drawing;
using System.Windows.Input;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Mover.DAndDSizeChanger
{
    /// <summary>
    /// コントロールの端をD＆Dすることによってサイズを変更出来る機能を提供するクラス
    /// http://anis774.net/codevault/danddsizechanger.html
    /// </summary>
    class DAndDSizeChanger
    {
        FrameworkElement sizeChangeCtrl;
        DAndDArea sizeChangeArea;
        System.Windows.Size lastMouseDownSize;
        System.Windows.Point lastMouseDownPoint;
        DAndDArea status;
        int sizeChangeAreaWidth;
        Cursor defaultCursor;

        /// <param name="sizeChangeCtrl">マウス入力によってサイズが変更されるコントロール</param>
        /// <param name="sizeChangeArea">上下左右のサイズ変更が有効になる範囲を指定</param>
        /// <param name="sizeChangeAreaWidth">サイズ変更が有効になる範囲の幅を指定</param>
        public DAndDSizeChanger(FrameworkElement sizeChangeCtrl, DAndDArea sizeChangeArea, int sizeChangeAreaWidth)
        {
            this.sizeChangeCtrl = sizeChangeCtrl;
            this.sizeChangeAreaWidth = sizeChangeAreaWidth;
            this.sizeChangeArea = sizeChangeArea;
            defaultCursor = sizeChangeCtrl.Cursor;
            
            sizeChangeCtrl.MouseMove += new MouseEventHandler(sizeChangeCtrl_MouseMove);
            sizeChangeCtrl.MouseDown += new MouseButtonEventHandler(sizeChangeCtrl_MouseDown);
            sizeChangeCtrl.MouseUp += new MouseButtonEventHandler(sizeChangeCtrl_MouseUp);
        }


        void sizeChangeCtrl_MouseDown(object sender, MouseEventArgs e)
        {
            lastMouseDownPoint = e.GetPosition(sizeChangeCtrl);
            lastMouseDownSize = new System.Windows.Size(sizeChangeCtrl.Width,sizeChangeCtrl.Height);

            sizeChangeCtrl.CaptureMouse();

            //動作を決定
            status = DAndDArea.None;
            if (getTop().Contains((int)e.GetPosition(sizeChangeCtrl).X, (int)e.GetPosition(sizeChangeCtrl).Y))
            {
                status |= DAndDArea.Top;
            }
            if (getLeft().Contains((int)e.GetPosition(sizeChangeCtrl).X, (int)e.GetPosition(sizeChangeCtrl).Y))
            {
                status |= DAndDArea.Left;
            }
            if (getBottom().Contains((int)e.GetPosition(sizeChangeCtrl).X, (int)e.GetPosition(sizeChangeCtrl).Y))
            {
                status |= DAndDArea.Bottom;
            }
            if (getRight().Contains((int)e.GetPosition(sizeChangeCtrl).X, (int)e.GetPosition(sizeChangeCtrl).Y))
            {
                status |= DAndDArea.Right;
            }
        }

        void sizeChangeCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            var el = sender as System.Windows.UIElement;

            //カーソルを変更
            if (((getTop().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y)) &&
            getLeft().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y)) ||
            (getBottom().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y) &&
            getRight().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y)))
            {

                sizeChangeCtrl.Cursor = Cursors.SizeNWSE;
            }
            else if ((getTop().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y) &&
              getRight().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y)) ||
              (getBottom().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y) &&
              getLeft().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y)))
            {

                sizeChangeCtrl.Cursor = Cursors.SizeNESW;
            }
            else if (getTop().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y) ||
              getBottom().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y))
            {

                sizeChangeCtrl.Cursor = Cursors.SizeNS;
            }
            else if (getLeft().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y) ||
              getRight().Contains((int)e.GetPosition(el).X, (int)e.GetPosition(el).Y))
            {

                sizeChangeCtrl.Cursor = Cursors.SizeWE;
            }
            else
            {
                sizeChangeCtrl.Cursor = defaultCursor;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //スクロールバークリック時におけるMouseDownイベントを経由しない移動を禁止
                if (lastMouseDownPoint.X == int.MinValue && lastMouseDownPoint.Y == int.MinValue)
                {
                    return;
                }

                try
                {                    
                    double diffX = e.GetPosition(el).X - lastMouseDownPoint.X;
                    double diffY = e.GetPosition(el).Y - lastMouseDownPoint.Y;

                    if ((status & DAndDArea.Top) == DAndDArea.Top)
                    {
                        double h = sizeChangeCtrl.Height;
                        if (sizeChangeCtrl.Height - diffY > sizeChangeCtrl.MinHeight)
                        {
                            sizeChangeCtrl.Height -= diffY;
                            Canvas.SetTop(sizeChangeCtrl, Canvas.GetTop(sizeChangeCtrl) + h - sizeChangeCtrl.Height);
                        }
                    }
                    if ((status & DAndDArea.Bottom) == DAndDArea.Bottom)
                    {
                        if (lastMouseDownSize.Height + diffY > sizeChangeCtrl.MinHeight)
                            sizeChangeCtrl.Height = lastMouseDownSize.Height + diffY;
                    }
                    if ((status & DAndDArea.Left) == DAndDArea.Left)
                    {
                        double w = sizeChangeCtrl.Width;
                        if (sizeChangeCtrl.Width - diffX > sizeChangeCtrl.MinWidth)
                        {
                            sizeChangeCtrl.Width -= diffX;
                            Canvas.SetLeft(sizeChangeCtrl, Canvas.GetLeft(sizeChangeCtrl) + w - sizeChangeCtrl.Width);
                        }
                    }
                    if ((status & DAndDArea.Right) == DAndDArea.Right)
                    {
                        if (lastMouseDownSize.Width + diffX > sizeChangeCtrl.MinWidth)
                            sizeChangeCtrl.Width = lastMouseDownSize.Width + diffX;
                    }
                }
                catch (System.Exception exc)
                {
                    //サイズを小さくしすぎると高さ、幅がマイナスにいって例外が発生するため
                }
            }
        }

        void sizeChangeCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            var el = sender as System.Windows.UIElement;
            el.ReleaseMouseCapture();
            //移動始点の初期化
            lastMouseDownPoint = new System.Windows.Point(int.MinValue, int.MinValue);
        }

        /// <summary>
        /// ポイントがD＆Dするとサイズが変更されるエリア内にあるかどうかを判定します。
        /// </summary>
        public bool ContainsSizeChangeArea(System.Drawing.Point p)
        {
            return getTop().Contains(p) ||
                getBottom().Contains(p) ||
                getLeft().Contains(p) ||
                getRight().Contains(p);
        }

        private Rectangle getTop()
        {
            if ((sizeChangeArea & DAndDArea.Top) == DAndDArea.Top)
            {
                return new Rectangle(0, 0, (int)sizeChangeCtrl.Width, sizeChangeAreaWidth);
            }
            else
            {
                return new Rectangle();
            }
        }

        private Rectangle getBottom()
        {
            if ((sizeChangeArea & DAndDArea.Bottom) == DAndDArea.Bottom)
            {
                return new Rectangle(0, (int)sizeChangeCtrl.Height - sizeChangeAreaWidth,
                    (int)sizeChangeCtrl.Width, sizeChangeAreaWidth);
            }
            else
            {
                return new Rectangle();
            }
        }

        private Rectangle getLeft()
        {
            if ((sizeChangeArea & DAndDArea.Left) == DAndDArea.Left)
            {
                return new Rectangle(0, 0,
                    sizeChangeAreaWidth, (int)sizeChangeCtrl.Height);
            }
            else
            {
                return new Rectangle();
            }
        }

        private Rectangle getRight()
        {
            if ((sizeChangeArea & DAndDArea.Right) == DAndDArea.Right)
            {
                return new Rectangle((int)sizeChangeCtrl.Width - sizeChangeAreaWidth, 0,
                    sizeChangeAreaWidth, (int)sizeChangeCtrl.Height);
            }
            else
            {
                return new Rectangle();
            }
        }
    }

    public enum DAndDArea
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = 15
    }
}