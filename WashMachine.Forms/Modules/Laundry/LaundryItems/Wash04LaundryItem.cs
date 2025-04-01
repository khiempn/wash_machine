using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Database.Context;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.Laundry.Dialog;
using WashMachine.Forms.Modules.LaundryWashOption;
using WashMachine.Forms.Modules.Login;

namespace WashMachine.Forms.Modules.Laundry.LaundryItems
{
    public class Wash04LaundryItem : ILaundryItem
    {
        public string Name => nameof(Wash04LaundryItem);

        Form mainForm;
        FollowType followType;
        Timer timer;
        string lbInforName = $"lbInfor_{nameof(Wash04LaundryItem)}";

        public Wash04LaundryItem(FollowType _followType, Form parent)
        {
            mainForm = parent;
            followType = _followType;
        }

        public void Click()
        {
            if (AppDbContext.Machine.Get(nameof(Wash04LaundryItem)).IsRunning == 0)
            {
                LaundryWashOptionForm laundryWashOptionForm = new LaundryWashOptionForm(this, followType);
                laundryWashOptionForm.Show();
                laundryWashOptionForm.FormClosed += LaundryWashOptionForm_FormClosed;
                mainForm.Hide();
            }
            else
            {
                MachineModel machine = timer.Tag as MachineModel;
                if (MachineService.IsRunCompleted(machine) == false)
                {
                    RunningDetailUI runningDetailUI = new RunningDetailUI(machine);
                    runningDetailUI.FormClosed += RunningDetailUI_FormClosed;
                    runningDetailUI.ShowDialog();
                }
                else
                {
                    LaundryWashOptionForm laundryWashOptionForm = new LaundryWashOptionForm(this, followType);
                    laundryWashOptionForm.Show();
                    laundryWashOptionForm.FormClosed += LaundryWashOptionForm_FormClosed;
                    mainForm.Hide();
                }
            }
        }

        private void RunningDetailUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Refresh();
            ResetTemplate();
        }

        private void LaundryWashOptionForm_FormClosed(object sender, FormClosedEventArgs e)
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
                ShapeBackgroudColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                ShapeBorderColor = Color.Black,
                CornerRadius = 50,
                Margin = new Padding(0, 0, 10, 0),
                Name = $"CardButtonRoundedUI_{nameof(Wash04LaundryItem)}"
            };
            cardButton.Click += CardItem_Click;

            TableLayoutPanel tblCardItem = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                Enabled = false
            };

            tblCardItem.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblCardItem.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblCardItem.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

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
                ForeColor = ColorTranslator.FromHtml("#ffffff"),
                Name = $"Title_{nameof(Wash04LaundryItem)}"
            };

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
                    lbInfor.Text = $"Remaining Time: {remainAt}";
                    tblCardItem.Controls.Add(lbInfor, 0, 1);
                    cardButton.IsDisabled = true;
                    lbTitle.ForeColor = Color.Green;
                }
            }


            lbTitle.Paint += LbTitle_Paint;
            pnCover.Controls.Add(lbTitle);
            tblCardItem.Controls.Add(pnCover, 0, 0);

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
                    ResetTemplate();
                }
                else
                {
                    Label lbInfor = mainForm.Controls.Find(lbInforName, true).First() as Label;
                    string remainAt = MachineService.GetRemainTimeAsFormat(machine);
                    string tempName = MachineService.GetTempName(machine.Temp);
                    lbInfor.Text = $"Remaining Time: {remainAt}";
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

        private void ResetTemplate()
        {
            MachineModel machine = timer.Tag as MachineModel;
            if (MachineService.IsRunCompleted(machine))
            {
                timer.Stop();
                AppDbContext.Machine.ResetMachine(machine);
                if (mainForm.Controls.Find(lbInforName, true).Any())
                {
                    CardButtonRoundedUI cardButtonRounded = mainForm.Controls.Find($"CardButtonRoundedUI_{nameof(Wash04LaundryItem)}", true).First() as CardButtonRoundedUI;
                    cardButtonRounded.IsDisabled = false;
                    cardButtonRounded.Refresh();

                    Label lbTitle = mainForm.Controls.Find($"Title_{nameof(Wash04LaundryItem)}", true).First() as Label;
                    lbTitle.ForeColor = ColorTranslator.FromHtml("#ffffff");

                    Label lbInfor = mainForm.Controls.Find(lbInforName, true).First() as Label;
                    mainForm.Controls.RemoveByKey(lbInforName);
                    lbInfor.Dispose();
                    mainForm.Refresh();
                }
            }
        }
    }
}
