using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Views.Login;
using WashMachine.Forms.Views.Main.MainItems;
using WashMachine.Forms.Views.Payment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WashMachine.Forms.Views.Main
{
    public partial class MainForm : Form
    {
        TableLayoutPanel tblMainForm;
        public FollowType FollowType {  get; set; }
        public MainForm(FollowType followType)
        {
            InitializeComponent();
            FollowType = followType;

            ResizeRedraw = false;
            Padding = new Padding(10);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;
            Resize += MainForm_Resize;

            tblMainForm = new TableLayoutPanel()
            {
                Width = 550,
                Height = 250
            };

            tblMainForm.Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            tblMainForm.Location = new Point((Width - tblMainForm.Width) / 2, (Height - tblMainForm.Height) / 2);
            tblMainForm.RowStyles.Add(new RowStyle() { Height = 100, SizeType = SizeType.Absolute });
           
            List <IMainItem> cardItems = new List<IMainItem>
            {
                new Hkd5CoinExchangeMainItem(this, FollowType),
                new CouponMainItem(this)
            };

            for (int i = 0; i < cardItems.Count; i++)
            {
                tblMainForm.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Percent });

                IMainItem cardItem = cardItems[i];
                Control cardItemTemplate = cardItem.GetTemplate();
                tblMainForm.Controls.Add(cardItemTemplate, i, 0);
            }

            Controls.Add(tblMainForm);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            tblMainForm.Location = new Point((Width - tblMainForm.Width) / 2, (Height - tblMainForm.Height) / 2);
        }
    }
}
