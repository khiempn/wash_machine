﻿using Newtonsoft.Json;
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
using WashMachine.Forms.Modules.Shop.Model;

namespace WashMachine.Forms.Modules.LaundryWashOption.LaundryOptionItems
{
    public class Wash03LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Wash03LaundryItem);

        public string ImplementCommand { get; set; } = "03 06 01 27 00 01 F8 1F";

        public Dictionary<string, string> ProgramCommands { get; set; } = new Dictionary<string, string>
        {
            {$"{nameof(Minute15TimeOptionItem)}", "03 06 01 26 00 01 A9 DF" },
            {$"{nameof(Minute30TimeOptionItem)}", "03 06 01 26 00 02 E9 DE" },
            {$"{nameof(Minute40TimeOptionItem)}", "03 06 01 26 00 03 28 1E" },
            {$"{nameof(Minute45TimeOptionItem)}", "03 06 01 26 00 04 69 DC" },
        };
        public string StopCommand { get; set; }
        public string UnlockCommand { get; set; }

        Form mainForm;

        Machine.MachineService machineService;

        public Wash03LaundryItem(ILaundryItem laundryItem, Form parent)
        {
            mainForm = parent;
            machineService = new Machine.MachineService();
            LoadConfig();
        }

        private void LoadConfig()
        {
            ShopConfigModel shopConfig = Program.ShopConfig;
            MachineCommandModel command = shopConfig.ShopSetting.MachineCommandConfig;

            ProgramCommands = new Dictionary<string, string>()
            {
                {$"{nameof(Minute15TimeOptionItem)}", command.Wash03LaundryItem_Minute15TimeOptionItem },
                {$"{nameof(Minute30TimeOptionItem)}", command.Wash03LaundryItem_Minute30TimeOptionItem },
                {$"{nameof(Minute40TimeOptionItem)}", command.Wash03LaundryItem_Minute40TimeOptionItem },
                {$"{nameof(Minute45TimeOptionItem)}", command.Wash03LaundryItem_Minute45TimeOptionItem },
            };

            ImplementCommand = command.Wash03LaundryItem_ImplementCommand;
            StopCommand = command.Wash03LaundryItem_StopCommand;
            UnlockCommand = command.Wash03LaundryItem_UnlockCommand;
        }

        public void Click()
        {
           
        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "洗衣 Washing\n03",
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
            };
            cardButton.Click += CardItem_Click;

            TableLayoutPanel tblCardItem = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml(cardItem.BackgroundColor),
                Enabled = false
            };

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
                Logger.Log($"{nameof(Wash03LaundryItem)} Step 1 START");
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{nameof(Wash03LaundryItem)} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.WashMachineCom, appConfig.WashMachineBaudRate, appConfig.WashMachineData, appConfig.WashMachineParity, appConfig.WashMachineStopBits);

                if (isConnected || Program.AppConfig.AutoRunning == 1)
                {
                    Logger.Log($"{nameof(Wash03LaundryItem)} Step 3");
                    string programCommand = ProgramCommands[$"{form.TimeOptionItemSelected.Name}"];
                    //Run selected program
                    machineService.ExecHexCommand(programCommand);
                    System.Threading.Thread.Sleep(2000);
                    // Run implement as START
                    machineService.ExecHexCommand(ImplementCommand);
                    System.Threading.Thread.Sleep(2000);
                    SetIsRunning();
                    Logger.Log($"{nameof(Wash03LaundryItem)} Step 4 END");
                }
                else
                {
                    MessageBox.Show("Unable connect to device, please try agiain", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Log($"{nameof(Wash03LaundryItem)} Can not connect device.");
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
