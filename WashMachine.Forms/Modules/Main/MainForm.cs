using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.Main.MainItems;
using WashMachine.Forms.Modules.Payment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Main
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
            BackColor = Color.White;

            tblMainForm = new TableLayoutPanel()
            {
                Width = (int)(0.7 * Screen.PrimaryScreen.WorkingArea.Width),
                Height = (int)(0.45 * Screen.PrimaryScreen.WorkingArea.Height)
            };

            tblMainForm.Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            tblMainForm.Location = new Point((Width - tblMainForm.Width) / 2, (Height - tblMainForm.Height) / 2);
            tblMainForm.RowStyles.Add(new RowStyle() { Height = 100, SizeType = SizeType.Absolute });
           
            List <IMainItem> cardItems = new List<IMainItem>
            {
                new SelfServiceLaundryMainItem(this),
                new DropOffServiceMainItem(this),
                new BuyeVoucherMainItem(this),
                new VendingMachineMainItem(this),
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
