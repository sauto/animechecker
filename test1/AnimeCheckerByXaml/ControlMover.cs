using System.Drawing;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Input;

namespace Ctrl.ControlMover
{
    /// <summary>
    /// 右クリックでコントロールを移動できるようにする機能を提供するクラス
    /// </summary>
    class ControlMover
    {
        System.Windows.Point lastMouseDownPoint;
        System.Windows.FrameworkElement movedControl;

        public static bool IsFixLayout { get; set; }
        /// <summary>
        /// ドラッグ中の場合MouseMoveイベントを起こさないための変数
        /// </summary>
        bool isDraggable = false;

        /// <param name="moveCtrl">移動させるコントロール</param>
        public ControlMover(System.Windows.FrameworkElement moveCtrl)
        {
            this.movedControl = moveCtrl;

            movedControl.MouseRightButtonDown += new MouseButtonEventHandler(mouseListner_MouseRightButtonDown);
            movedControl.MouseMove += new MouseEventHandler(mouseListner_MouseMove);
            movedControl.MouseRightButtonUp += new MouseButtonEventHandler(mouseListner_MouseRightButtonUp);
        }

        void mouseListner_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var el = sender as System.Windows.UIElement;
                isDraggable = true;
                lastMouseDownPoint = e.GetPosition(el);
                //高速で動かしたときでも追従させる
                el.CaptureMouse();

            }
        }

        void mouseListner_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (!isDraggable || IsFixLayout)
                {
                    return;
                }
                var el = sender as System.Windows.UIElement;
                System.Windows.Point curPoint = e.GetPosition(el);

                //移動処理
                Canvas.SetLeft(movedControl, Canvas.GetLeft(movedControl) + (curPoint.X - lastMouseDownPoint.X));
                Canvas.SetTop(movedControl, Canvas.GetTop(movedControl) + (curPoint.Y - lastMouseDownPoint.Y));
                

            }
        }

        void mouseListner_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
            var el = sender as System.Windows.UIElement;
            el.ReleaseMouseCapture();
        }
    }
}