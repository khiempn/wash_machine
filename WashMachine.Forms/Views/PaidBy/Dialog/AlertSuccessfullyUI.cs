using WashMachine.Forms.Views.PaidBy.Service.Model;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.PaidBy.Dialog
{
    public class AlertSuccessfullyUI : Panel
    {
        TableLayoutPanel tblMain;
        Button btnHome;
        Button btnPrinter;
        public event EventHandler HomeClick;
        public event EventHandler PrinterClick;

        public AlertSuccessfullyUI()
        {
            Padding = new Padding(10);
            Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            Size = new Size(315, 210);

            tblMain = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };

            tblMain.RowStyles.Add(new RowStyle() { Height = 30, SizeType = SizeType.Absolute });
            tblMain.RowStyles.Add(new RowStyle() { Height = 90, SizeType = SizeType.Absolute });
            tblMain.RowStyles.Add(new RowStyle() { Height = 30, SizeType = SizeType.Absolute });
            tblMain.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Absolute });

            Panel pnStatus = new Panel()
            {
                Dock = DockStyle.Fill
            };
            pnStatus.Controls.Add(new Label() { Text = "Payment Successfully!", TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Height = 30 });
            tblMain.Controls.Add(pnStatus, 0, 0);

            byte[] bytes = Convert.FromBase64String("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");
            MemoryStream ms = new MemoryStream(bytes);

            Panel pnIcon = new Panel()
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromStream(ms),
                BackgroundImageLayout = ImageLayout.Center
            };
            tblMain.Controls.Add(pnIcon, 0, 1);

            TableLayoutPanel tblButtons = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tblButtons.RowStyles.Add(new RowStyle() { Height = 30, SizeType = SizeType.Absolute });
            tblButtons.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Absolute });
            tblButtons.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Absolute });

            btnHome = new Button() { Text = "Home", Dock = DockStyle.Fill };
            btnHome.Click += BtnHome_Click;
            btnPrinter = new Button() { Text = "Printer", Dock = DockStyle.Fill };
            btnPrinter.Click += BtnPrinter_Click;
            tblButtons.Controls.Add(btnHome, 0, 0);
            tblButtons.Controls.Add(btnPrinter, 1, 0);

            tblMain.Controls.Add(tblButtons, 0, 2);
            Controls.Add(tblMain);
            Visible = false;

            BorderStyle = BorderStyle.FixedSingle;
            Visible = false;
            SizeChanged += Panel_SizeChanged;
            VisibleChanged += AlertSuccessfullyUI_VisibleChanged;
        }

        private void BtnPrinter_Click(object sender, EventArgs e)
        {
            PrinterClick?.Invoke(sender, e);
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            HomeClick?.Invoke(sender, e);
        }

        private void Panel_SizeChanged(object sender, System.EventArgs e)
        {
            tblMain.Location = new Point((500 - tblMain.Width) / 2, (400 - tblMain.Height) / 2);
        }

        private void AlertSuccessfullyUI_VisibleChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Control[] mainLayouts = Parent.Controls.Find("MainLayout", true);

                if (mainLayouts != null && mainLayouts.Length > 0)
                {
                    Control mainLayout = mainLayouts[0];

                    if (Visible)
                    {
                        mainLayout.Enabled = false;
                    }
                    else
                    {
                        mainLayout.Enabled = true;
                    }
                }
            }
        }

        public void SetParent(Form parent)
        {
            if (parent != null)
            {
                Parent = parent;
                Parent.SizeChanged += Parent_SizeChanged;
                Parent.Controls.SetChildIndex(this, 0);
                Parent_SizeChanged(parent, EventArgs.Empty);
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            Location = new Point((Parent.Width - Width) / 2, (Parent.Height - Height) / 2);
        }

        public void SetPrintOrderModel(OrderModel orderModel)
        {
            btnPrinter.Tag = orderModel;
        }
    }
}
