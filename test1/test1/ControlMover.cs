using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace test.ControlMover
{
    /// <summary>
    /// 右クリックでコントロールを移動できるようにする機能を提供するクラス
    /// </summary>
    class ControlMover
    {
        Control mouseListner;
        Control moveControl;
        Point lastMouseDownPoint;
        public static bool IsFixLayout { get; set; }
        /// <summary>
        /// ドラッグ中の場合MouseMoveイベントを起こさないための変数
        /// </summary>
        bool isDraggable = false;

        /// <param name="mouseListner">マウス入力を受け取るコントロール</param>
        /// <param name="moveCtrl">移動させるコントロール</param>
        public ControlMover(Control mouseListner, Control moveCtrl)
        {
            this.mouseListner = mouseListner;
            this.moveControl = moveCtrl;

            mouseListner.MouseDown += new MouseEventHandler(mouseListner_MouseDown);
            mouseListner.MouseMove += new MouseEventHandler(mouseListner_MouseMove);
            mouseListner.MouseUp += new MouseEventHandler(mouseListner_MouseUp);
        }

        void mouseListner_MouseDown(object sender, MouseEventArgs e)
        {

            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                isDraggable = true;
                lastMouseDownPoint = e.Location;
            }
        }

        void mouseListner_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (!isDraggable || IsFixLayout)
                {
                    return;
                }
                //移動処理
                this.moveControl.Left += e.X - lastMouseDownPoint.X;
                this.moveControl.Top += e.Y - lastMouseDownPoint.Y;
            }
        }

        void mouseListner_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggable = false;
        }
    }
}