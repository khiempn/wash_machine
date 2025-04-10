using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Database.Context;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.Laundry;
using WashMachine.Forms.Modules.LaundryDryerOption.Email;
using WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems;
using WashMachine.Forms.Modules.LaundryDryerOption.TimeOptionItems;
using WashMachine.Forms.Modules.LaundryWashOption.Api;
using WashMachine.Forms.Modules.Main;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using WashMachine.Forms.Modules.Shop.Model;

namespace WashMachine.Forms.Modules.LaundryDryerOption.LaundryOptionItems
{
    public class Dryer02LaundryItem : ILaundryOptionItem
    {
        public string Name => nameof(Dryer02LaundryItem);
        public Dictionary<string, string> TemperatureCommands { get; set; } = new Dictionary<string, string>()
        {
            { nameof(HighTempOptionItem), "02 06 01 66 00 01 A9 DA" },
            { nameof(MidTempOptionItem), "02 06 01 66 00 02 E9 DB" },
            { nameof(LowTempOptionItem), "02 06 01 66 00 03 28 1B" },
        };
        public Dictionary<string, string> TimeCommands { get; set; } = new Dictionary<string, string>()
        {
            { nameof(Minute30TimeOptionItem), "02 06 01 67 00 1E B9 D2" },
            { nameof(Minute40TimeOptionItem), "02 06 01 67 00 3C 39 CB" },
            { nameof(Minute50TimeOptionItem), "02 06 01 67 00 5A B9 E1" },
            { nameof(Minute60TimeOptionItem), "02 06 01 67 00 78 F8 39" }
        };
        public string ImplementCommand { get; set; } = "02 06 01 68 00 01 C8 19";
        public string StopCommand { get; set; }
        public string HealthCheckCommand { get; set; } = "02 03 01 5C 00 0A 04 10";
        public Action<object> HealthCheckCompleted { get; set; }
        Form mainForm;
        Machine.MachineService machineService;
        EmailService emailService;
        DryerApiService dryerApiService;

        public Dryer02LaundryItem(ILaundryItem laundryItem, Form parent)
        {
            mainForm = parent;
            machineService = new Machine.MachineService();
            emailService = new EmailService();
            dryerApiService = new DryerApiService();
            LoadConfig();
        }
        private void LoadConfig()
        {
            ShopConfigModel shopConfig = Program.ShopConfig;
            MachineCommandModel command = shopConfig.ShopSetting.MachineCommandConfig;

            TemperatureCommands = new Dictionary<string, string>()
            {
                { nameof(HighTempOptionItem), command.Dryer02LaundryItem_HighTempOptionItem },
                { nameof(MidTempOptionItem), command.Dryer02LaundryItem_MidTempOptionItem },
                { nameof(LowTempOptionItem), command.Dryer02LaundryItem_LowTempOptionItem },
            };

            TimeCommands = new Dictionary<string, string>()
            {
                { nameof(Minute30TimeOptionItem), command.Dryer02LaundryItem_Minute30TimeOptionItem },
                { nameof(Minute40TimeOptionItem), command.Dryer02LaundryItem_Minute40TimeOptionItem },
                { nameof(Minute50TimeOptionItem), command.Dryer02LaundryItem_Minute50TimeOptionItem },
                { nameof(Minute60TimeOptionItem), command.Dryer02LaundryItem_Minute60TimeOptionItem }
            };

            ImplementCommand = command.Dryer02LaundryItem_ImplementCommand;
            StopCommand = command.Dryer02LaundryItem_StopCommand;
        }

        public void Click()
        {

        }

        public Control GetTemplate()
        {
            CardItemProperty cardItem = new CardItemProperty()
            {
                Title = "乾衣 Dryer\n 02",
                BackgroundColor = "#8cd872",
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

        public async Task Start(OrderModel order)
        {
            await Task.Run(async () =>
            {
                Logger.Log($"{Name} Step 1 START");
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{Name} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DryerMachineCom, appConfig.DryerMachineBaudRate, appConfig.DryerMachineData, appConfig.DryerMachineParity, appConfig.DryerMachineStopBits);

                if (isConnected)
                {
                    Logger.Log($"{Name} Step 3");

                    //Run temp program
                    string tempCommand = TemperatureCommands[form.TempOptionItemSelected.Name];
                    machineService.ExecHexCommand(tempCommand, (tempCommandRecived) =>
                    {
                        bool isValidate = machineService.ValidateProgramCommand(tempCommand, tempCommandRecived);

                        if (isValidate == false)
                        {
                            OnStartError(order, tempCommand);
                            return;
                        }

                        //Run temp program
                        string timeCommand = TimeCommands[form.TimeOptionItemSelected.Name];
                        machineService.ExecHexCommand(timeCommand, (timeCommandRecived) =>
                        {
                            isValidate = machineService.ValidateProgramCommand(timeCommand, timeCommandRecived);

                            if (isValidate == false)
                            {
                                OnStartError(order, timeCommand);
                                return;
                            }

                            // Run implement as START
                            machineService.ExecHexCommand(ImplementCommand, (implementCommandRecived) =>
                            {
                                isValidate = machineService.ValidateProgramCommand(ImplementCommand, implementCommandRecived);

                                if (isValidate == false)
                                {
                                    OnStartError(order, ImplementCommand);
                                    return;
                                }

                                SetIsRunning();
                            });

                        });
                    });

                    Logger.Log($"{Name} Step 4 END");
                }
                else
                {
                    MessageBox.Show("Unable connect to device, please try agiain", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Log($"{Name} Can not connect device.");
                }
            });
        }

        private void OnStartError(OrderModel order, string command)
        {
            emailService.SendEmailStartError(order, Name, command);
            dryerApiService.TrackingMachineError(order, Name, command);
        }

        public void SetIsRunning()
        {
            LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;

            MachineModel machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            machine.StartAt = DateTime.Now.Ticks.ToString();
            machine.EndAt = DateTime.Now.AddMinutes(form.TimeOptionItemSelected.TimeNumber).Ticks.ToString();
            machine.Time = form.TimeOptionItemSelected.TimeNumber;
            machine.Temp = form.TempOptionItemSelected.TypeId;
            machine.IsRunning = 1;
            AppDbContext.Machine.Update(machine);

            machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            dryerApiService.UpdateMachineInfo(machine);
        }

        public void SetIsStop()
        {
            MachineModel machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            AppDbContext.Machine.ResetMachine(machine);

            machine = AppDbContext.Machine.Get(new MachineModel() { Name = Name });
            dryerApiService.UpdateMachineInfo(machine);
        }

        public async Task Stop()
        {
            await Task.Run(async () =>
            {
                Logger.Log($"{Name} Step 1 STOP");
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;

                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{Name} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DryerMachineCom, appConfig.DryerMachineBaudRate, appConfig.DryerMachineData, appConfig.DryerMachineParity, appConfig.DryerMachineStopBits);

                if (isConnected)
                {
                    Logger.Log($"{Name} Step 3");
                    //Run stop program
                    machineService.ExecHexCommand(StopCommand);
                    System.Threading.Thread.Sleep(2000);
                    SetIsStop();
                    Logger.Log($"{Name} Step 4 END");
                }
                else
                {
                    MessageBox.Show("Unable connect to device, please try agiain", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Log($"{Name} Can not connect device.");
                }
            });
        }

        public async Task HealthCheck()
        {
            await Task.Run(async () =>
            {
                Logger.Log($"{Name} Step 1 HealthCheck");
                AppConfigModel appConfig = Program.AppConfig;
                Logger.Log($"{Name} Step 2 {JsonConvert.SerializeObject(appConfig)}");
                bool isConnected = await machineService.ConnectAsync(appConfig.DryerMachineCom, appConfig.DryerMachineBaudRate, appConfig.DryerMachineData, appConfig.DryerMachineParity, appConfig.DryerMachineStopBits);

                if (isConnected)
                {
                    // Run health check command
                    machineService.ExecHexCommand(HealthCheckCommand, (dtRecived) =>
                    {
                        Logger.Log($"{Name} MachineService_DataReceived {dtRecived}");
                        if (!string.IsNullOrWhiteSpace(dtRecived))
                        {
                            bool isValidateCrc = machineService.ValidateCRCCommand(dtRecived);
                            if (isValidateCrc == false)
                            {
                                OnHealthCheckError();
                            }

                            HealthCheckCompleted?.Invoke(isValidateCrc);
                        }
                        else
                        {
                            OnHealthCheckError();
                            HealthCheckCompleted?.Invoke(false);
                        }
                    });

                    if (Program.AppConfig.ByPassHealthCheckMachine == 1)
                    {
                        machineService.FakeInvokeDataReceived();
                    }
                    Logger.Log($"{Name} Step 4 END");
                }
                else
                {
                    MessageBox.Show("Unable connect to device, please try agiain", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Log($"{Name} Can not connect device.");
                }
            });
        }

        private void OnHealthCheckError()
        {
            emailService.SendEmailHealthCheckError(Name, HealthCheckCommand);
        }
    }
}
