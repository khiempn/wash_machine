﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Laundry;

namespace WashMachine.Forms.Modules.LaundryDryerOption.LaundryOptionItems
{
    public class Wash01LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Wash01LaundryItem);

        public Dictionary<string, string> TemperatureCommands => throw new NotImplementedException();

        public Dictionary<string, string> TimeCommands => throw new NotImplementedException();

        public string ImplementCommand => throw new NotImplementedException();

        Form mainForm;

        public Wash01LaundryItem(ILaundryItem laundryItem, Form parent)
        {
            mainForm = parent;
        }

        public async void Click()
        {
           
        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "洗衣 Washing\n01",
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

        public void DisableItem(Control control)
        {
            control.Enabled = false;
        }

        public Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
