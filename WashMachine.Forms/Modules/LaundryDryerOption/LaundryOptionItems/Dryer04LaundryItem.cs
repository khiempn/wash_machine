using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Laundry;
using WashMachine.Forms.Modules.LaundryDryerOption.Machine;
using WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems;
using WashMachine.Forms.Modules.LaundryDryerOption.TimeOptionItems;

namespace WashMachine.Forms.Modules.LaundryDryerOption.LaundryOptionItems
{
    public class Dryer04LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Dryer04LaundryItem);

        public Dictionary<string, string> TemperatureCommands => new Dictionary<string, string>()
        {
            { nameof(HighTempOptionItem), "04 06 01 66 00 01 A9 BC" },
            { nameof(MidTempOptionItem), "04 06 01 66 00 01 E9 BD" },
            { nameof(LowTempOptionItem), "04 06 01 66 00 01 28 7D" },
        };

        public Dictionary<string, string> TimeCommands => new Dictionary<string, string>()
        {
            { nameof(Minute30TimeOptionItem), "04 06 01 67 00 1E B9 B4" },
            { nameof(Minute40TimeOptionItem), "04 06 01 67 00 3C 39 AD" },
            { nameof(Minute50TimeOptionItem), "04 06 01 67 00 5A B9 87" },
            { nameof(Minute60TimeOptionItem), "04 06 01 67 00 78 39 9E" }
        };

        public string ImplementCommand => "04 06 01 68 00 01 C8 7F";

        Form mainForm;

        MachineService machineService;

        public Dryer04LaundryItem(ILaundryItem laundryItem, Form parent)
        {
            mainForm = parent;
            machineService = new MachineService();
        }

        public void Click()
        {
           
        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "乾衣 Dryer\n 04",
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
                Logger.Log($"{nameof(Dryer04LaundryItem)} Step 1 START");
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{nameof(Dryer04LaundryItem)} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DollarCom, appConfig.DollarBaudRate, appConfig.DollarData, appConfig.DollarParity, appConfig.DollarStopBits);

                if (isConnected)
                {
                    Logger.Log($"{nameof(Dryer04LaundryItem)} Step 3");
                    string tempCommand = TemperatureCommands[$"{form.TempOptionItemSelected.Name}"];
                    //Run temp program
                    machineService.ExecHexCommand(tempCommand);

                    string timeCommand = TimeCommands[$"{form.TimeOptionItemSelected.Name}"];
                    //Run temp program
                    machineService.ExecHexCommand(timeCommand);

                    // Run implement as START
                    machineService.ExecHexCommand(ImplementCommand);

                    machineService.Disconect();
                    Logger.Log($"{nameof(Dryer04LaundryItem)} Step 4 END");
                }
                else
                {
                    Logger.Log($"{nameof(Dryer04LaundryItem)} Can not connect device.");
                }
            });
        }
    }
}
