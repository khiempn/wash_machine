using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Database.Context;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.Laundry;
using WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems;

namespace WashMachine.Forms.Modules.LaundryWashOption.LaundryOptionItems
{
    public class Wash01LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Wash01LaundryItem);

        public string ImplementCommand => "01 06 01 27 00 01 F9 FD";

        public Dictionary<string, string> ProgramCommands => new Dictionary<string, string>
        {
            {$"{nameof(Minute15TimeOptionItem)}", "01 06 01 26 00 01 A8 3D" },
            {$"{nameof(Minute30TimeOptionItem)}", "01 06 01 26 00 02 E8 3C" },
            {$"{nameof(Minute40TimeOptionItem)}", "01 06 01 26 00 03 29 FC" },
            {$"{nameof(Minute45TimeOptionItem)}", "01 06 01 26 00 04 68 3E" },
        };

        Form mainForm;

        Machine.MachineService machineService;

        public Wash01LaundryItem(ILaundryItem laundryItem, Form parent)
        {
            mainForm = parent;
            machineService = new Machine.MachineService();
        }

        public void Click()
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
            (control as CardButtonRoundedUI).IsDisabled = true;
        }

        public async Task Start()
        {
            await Task.Run(async () =>
            {
                Logger.Log($"{nameof(Wash01LaundryItem)} Step 1 START");
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{nameof(Wash01LaundryItem)} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DollarCom, appConfig.DollarBaudRate, appConfig.DollarData, appConfig.DollarParity, appConfig.DollarStopBits);

                if (isConnected || true)
                {
                    Logger.Log($"{nameof(Wash01LaundryItem)} Step 3");
                    string programCommand = ProgramCommands[$"{form.TimeOptionItemSelected.Name}"];
                    //Run selected program
                    machineService.ExecHexCommand(programCommand);
                    System.Threading.Thread.Sleep(2000);
                    // Run implement as START
                    machineService.ExecHexCommand(ImplementCommand);
                    System.Threading.Thread.Sleep(2000);
                    machineService.Disconect();
                    SetIsRunning();
                    Logger.Log($"{nameof(Wash01LaundryItem)} Step 4 END");
                }
                else
                {
                    Logger.Log($"{nameof(Wash01LaundryItem)} Can not connect device.");
                }
            });
        }

        public void SetIsRunning()
        {
            LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;

            MachineModel machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            machine.StartAt = DateTime.Now.Ticks.ToString();
            machine.EndAt = DateTime.Now.AddMinutes(form.TimeOptionItemSelected.TimeNumber).Ticks.ToString();
            machine.Time = form.TimeOptionItemSelected.TimeNumber;
            machine.IsRunning = 1;
            AppDbContext.Machine.Update(machine);
        }
    }
}
