using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Database.Context;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.LaundryDryerOption;
using WashMachine.Forms.Modules.Login;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace WashMachine.Forms.Modules.Laundry.LaundryItems
{
    public class Dryer01LaundryItem : ILaundryItem
    {
        public string Name => nameof(Dryer01LaundryItem);
        Form mainForm;
        FollowType followType;
        Timer timer;
        string lbInforName = $"lbInfor_{nameof(Dryer01LaundryItem)}";

        public Dryer01LaundryItem(FollowType _followType, Form parent)
        {
            mainForm = parent;
            followType = _followType;
        }

        public void Click()
        {
            LaundryDryerOptionForm laundryDryerOptionForm = new LaundryDryerOptionForm(this, followType);
            laundryDryerOptionForm.Show();
            laundryDryerOptionForm.FormClosed += LaundryDryerOptionForm_FormClosed;
            mainForm.Hide();
        }

        private void LaundryDryerOptionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Close();
        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "乾衣 Dryer\n 01",
                BackgroundColor = "#8cd872",
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

            tblCardItem.RowStyles.Add(new RowStyle() { Height = 130, SizeType = SizeType.Absolute });
            tblCardItem.RowStyles.Add(new RowStyle() { Height = 170, SizeType = SizeType.AutoSize });
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

            MachineModel machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            if (machine != null && machine.IsRunning == 1)
            {
                if (MachineService.IsRunCompleted(machine))
                {
                    AppDbContext.Machine.ResetMachine(machine);
                }
                else
                {
                    Label lbInfor = new Label
                    {
                        Tag = cardItem.Title,
                        TextAlign = ContentAlignment.TopCenter,
                        Enabled = false,
                        Dock = DockStyle.Fill,
                        ForeColor = ColorTranslator.FromHtml("#ffffff"),
                        Name = lbInforName
                    };
                    
                    timer?.Stop();

                    timer = new Timer
                    {
                        Interval = 1000
                    };
                    timer.Tag = machine;
                    timer.Tick += Timer_Tick;
                    timer.Start();

                    string remainAt = MachineService.GetRemainTimeAsFormat(machine);
                    string tempName = MachineService.GetTempName(machine.Temp);
                    lbInfor.Text = $"Remain Time: {remainAt}\n Temp: {tempName}";
                    tblCardItem.Controls.Add(lbInfor, 0, 1);
                    cardButton.Enabled = false;
                }
            }

            cardButton.Controls.Add(tblCardItem);

            return cardButton;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                MachineModel machine = timer.Tag as MachineModel;
                if (MachineService.IsRunCompleted(machine))
                {
                    timer.Stop();
                    AppDbContext.Machine.ResetMachine(machine);
                    if (mainForm.Controls.Find(lbInforName, true).Any())
                    {
                        Label lbInfor = mainForm.Controls.Find(lbInforName, true).First() as Label;
                        mainForm.Controls.Remove(lbInfor);
                    }
                }
                else
                {
                    Label lbInfor = mainForm.Controls.Find(lbInforName, true).First() as Label;
                    string remainAt = MachineService.GetRemainTimeAsFormat(machine);
                    string tempName = MachineService.GetTempName(machine.Temp);
                    lbInfor.Text = $"Remain Time: {remainAt}\n Temp: {tempName}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                timer.Stop();
            }
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
