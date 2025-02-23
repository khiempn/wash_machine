using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.LaundryOption;
using WashMachine.Forms.Modules.Login;

namespace WashMachine.Forms.Modules.Laundry.LaundryItems
{
    public class Wash04LaundryItem : ILaundryItem
    {
        public string Name => nameof(Wash04LaundryItem);

        Form mainForm;
        FollowType followType;

        public Wash04LaundryItem(FollowType _followType, Form parent)
        {
            mainForm = parent;
            followType = _followType;
        }

        public async void Click()
        {
            LaundryOptionForm laundryOptionForm = new LaundryOptionForm(this, followType);
            laundryOptionForm.Show();
            laundryOptionForm.FormClosed += LaundryOptionForm_FormClosed;
            mainForm.Hide();
        }

        private void LaundryOptionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Close();
        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "洗衣 Washing\n04",
                BackgroundColor = "#4892dc",
                CoverImageBase64 = ""
            };

            CardButtonRoundedUI cardButton = new CardButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Height = 50,
                Width = 300,
                ShapeBackgroudColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                ShapeBorderColor = Color.Black,
                CornerRadius = 50,
                Margin = new Padding(0, 0, 10, 0),
            };
            cardButton.Click += CardItem_Click;

            TableLayoutPanel tblCardItem = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                Enabled = false
            };

            tblCardItem.RowStyles.Add(new RowStyle() { Height = 150, SizeType = SizeType.Absolute });
            tblCardItem.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Percent });
            Panel pnCover = new Panel
            {
                BackColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                Dock = DockStyle.Fill,
                Enabled = false
            };

            Label lbTitle = new Label
            {
                Tag = cardItem.Title,
                TextAlign = ContentAlignment.TopCenter,
                Enabled = false,
                Dock = DockStyle.Fill,
                ForeColor = ColorTranslator.FromHtml("#ffffff")
            };

            lbTitle.Paint += LbTitle_Paint;
            pnCover.Controls.Add(lbTitle);
            tblCardItem.Controls.Add(pnCover, 0, 0);

            cardButton.Controls.Add(tblCardItem);

            return cardButton;
        }

        private void LbTitle_Paint(object sender, PaintEventArgs e)
        {
            Label lb = (Label)sender;
            using (Brush aBrush = new SolidBrush(lb.ForeColor))
            {
                e.Graphics.DrawString(lb.Tag.ToString(), lb.Font, aBrush, lb.ClientRectangle, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }
        }

        private void CardItem_Click(object sender, EventArgs e)
        {
            Click();
        }
    }
}
