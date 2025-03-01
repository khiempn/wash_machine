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
    public class Wash02LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Wash02LaundryItem);

        public string ImplementCommand => "02 06 01 27 00 01 F9 CE";

        public Dictionary<string, string> ProgramCommands => new Dictionary<string, string>
        {
            {$"{nameof(Minute15TimeOptionItem)}", "02 06 01 26 00 01 A8 0E" },
            {$"{nameof(Minute30TimeOptionItem)}", "02 06 01 26 00 02 E8 0F" },
            {$"{nameof(Minute40TimeOptionItem)}", "02 06 01 26 00 03 29 CF" },
            {$"{nameof(Minute45TimeOptionItem)}", "02 06 01 26 00 04 68 0D" },
        };

        Form mainForm;

        Machine.MachineService machineService;

        public Wash02LaundryItem(ILaundryItem laundryItem, Form parent)
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
                Title = "洗衣 Washing\n02",
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

        public async Task Start()
        {
            await Task.Run(async () =>
            {
                System.Threading.Thread.Sleep(2000);
                Logger.Log($"{nameof(Wash02LaundryItem)} Step 1 START");
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{nameof(Wash02LaundryItem)} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DollarCom, appConfig.DollarBaudRate, appConfig.DollarData, appConfig.DollarParity, appConfig.DollarStopBits);

                if (isConnected || true)
                {
                    Logger.Log($"{nameof(Wash02LaundryItem)} Step 3");
                    string programCommand = ProgramCommands[$"{form.TimeOptionItemSelected.Name}"];
                    //Run selected program
                    machineService.ExecHexCommand(programCommand);
                    System.Threading.Thread.Sleep(2000);
                    // Run implement as START
                    machineService.ExecHexCommand(ImplementCommand);
                    System.Threading.Thread.Sleep(2000);
                    machineService.Disconect();
                    SetIsRunning();
                    Logger.Log($"{nameof(Wash02LaundryItem)} Step 4 END");
                }
                else
                {
                    Logger.Log($"{nameof(Wash02LaundryItem)} Can not connect device.");
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
