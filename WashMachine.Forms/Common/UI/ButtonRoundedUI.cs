using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WashMachine.Forms.Common.UI
{
    public class ButtonRoundedUI : Panel
    {
        public int CornerRadius { get; set; } = 10;
        public int BorderWidth { get; set; } = 1;
        public bool IsFillBottomLeft { get; set; } = false;
        public bool IsFillBottomRight { get; set; } = false;
        public bool IsFillTopLeft { get; set; } = false;
        public bool IsFillTopRight { get; set; } = false;
        public Color ShapeBackgroudColor { get; set; }
        public Color ShapeBorderColor { get; set; }
        public bool IsSelected { get; set; }
        public Color ShapeSelectedBackgroudColor { get; set; }

        public ButtonRoundedUI()
        {
            DoubleBuffered = true;
            MouseHover += ButtonRoundedUI_MouseHover;
            MouseLeave += ButtonRoundedUI_MouseLeave;
        }

        private void ButtonRoundedUI_MouseLeave(object sender, System.EventArgs e)
        {
            BorderWidth = 1;
            Refresh();
        }

        private void ButtonRoundedUI_MouseHover(object sender, System.EventArgs e)
        {
            BorderWidth = 3;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           
            using (GraphicsPath path = GetGraphicsPath(ClientRectangle))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color bgColor = IsSelected ? ShapeSelectedBackgroudColor : ShapeBackgroudColor;

                SolidBrush solidBrush = new SolidBrush(bgColor);
                Pen borderColor = new Pen(ShapeBorderColor, BorderWidth);

                e.Graphics.FillPath(solidBrush, path);
                e.Graphics.DrawPath(borderColor, path);
                Color txtColor = IsSelected ? Color.White : ForeColor;

                TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, txtColor);
            }
        }

        private GraphicsPath GetGraphicsPath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int left = rect.X;
            int top = rect.Y;
            int right = rect.X + rect.Width - BorderWidth;
            int bottom = rect.Y + rect.Height - BorderWidth;

            if (IsFillTopLeft)
            {
                path.AddLine(left, top + CornerRadius, left, top);
                path.AddLine(left, top, left + CornerRadius, top);
            }
            else
            {
                path.AddArc(rect.X, rect.Y, CornerRadius, CornerRadius, 180, 90);
            }

            if (IsFillTopRight)
            {
                path.AddLine(right - CornerRadius, top, right, top);
                path.AddLine(right, top, right, top + CornerRadius);
            }
            else
            {
                path.AddArc(rect.X + rect.Width - CornerRadius - BorderWidth, rect.Y, CornerRadius, CornerRadius, 270, 90);
            }

            if (IsFillBottomRight)
            {
                path.AddLine(right, bottom - CornerRadius, right, bottom);
                path.AddLine(right, bottom, right - CornerRadius, bottom);
            }
            else
            {
                path.AddArc(rect.X + rect.Width - CornerRadius - BorderWidth, rect.Y + rect.Height - CornerRadius - BorderWidth, CornerRadius, CornerRadius, 0, 90);
            }

            if (IsFillBottomLeft)
            {
                path.AddLine(left + CornerRadius, bottom, left, bottom);
                path.AddLine(left, bottom, left - CornerRadius, bottom);
            }
            else
            {
                path.AddArc(rect.X, rect.Y + rect.Height - CornerRadius - BorderWidth, CornerRadius, CornerRadius, 90, 90);
            }

            path.CloseAllFigures();
            return path;
        }
    }

}
