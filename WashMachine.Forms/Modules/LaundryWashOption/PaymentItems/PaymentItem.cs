using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.LaundryWashOption.LaundryOptionItems;
using WashMachine.Forms.Modules.LaundryWashOption.Machine;
using WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems;

namespace WashMachine.Forms.Modules.LaundryWashOption.PaymentItems
{
    public class PaymentItem : IPaymentItem
    {
        Form mainForm;
        MachineService machineService;

        Dictionary<string, string> programCommands = new Dictionary<string, string>
        {
            {$"{nameof(Wash01LaundryItem)} {nameof(Minute15TimeOptionItem)}", "01 06 01 26 00 01 A8 3D" },
            {$"{nameof(Wash01LaundryItem)} {nameof(Minute30TimeOptionItem)}", "01 06 01 26 00 02 E8 3C" },
            {$"{nameof(Wash01LaundryItem)} {nameof(Minute40TimeOptionItem)}", "01 06 01 26 00 03 29 FC" },
            {$"{nameof(Wash01LaundryItem)} {nameof(Minute45TimeOptionItem)}", "01 06 01 26 00 04 68 3E" },

            {$"{nameof(Wash02LaundryItem)} {nameof(Minute15TimeOptionItem)}", "02 06 01 26 00 01 A8 0E" },
            {$"{nameof(Wash02LaundryItem)} {nameof(Minute30TimeOptionItem)}", "02 06 01 26 00 02 E8 0F" },
            {$"{nameof(Wash02LaundryItem)} {nameof(Minute40TimeOptionItem)}", "02 06 01 26 00 03 29 CF" },
            {$"{nameof(Wash02LaundryItem)} {nameof(Minute45TimeOptionItem)}", "02 06 01 26 00 04 68 0D" },

            {$"{nameof(Wash03LaundryItem)} {nameof(Minute15TimeOptionItem)}", "03 06 01 26 00 01 A9 DF" },
            {$"{nameof(Wash03LaundryItem)} {nameof(Minute30TimeOptionItem)}", "03 06 01 26 00 02 E9 DE" },
            {$"{nameof(Wash03LaundryItem)} {nameof(Minute40TimeOptionItem)}", "03 06 01 26 00 03 28 1E" },
            {$"{nameof(Wash03LaundryItem)} {nameof(Minute45TimeOptionItem)}", "03 06 01 26 00 04 69 DC" },

            {$"{nameof(Wash04LaundryItem)} {nameof(Minute15TimeOptionItem)}", "01 06 01 26 00 01 A8 68" },
            {$"{nameof(Wash04LaundryItem)} {nameof(Minute30TimeOptionItem)}", "01 06 01 26 00 02 E8 69" },
            {$"{nameof(Wash04LaundryItem)} {nameof(Minute40TimeOptionItem)}", "01 06 01 26 00 03 29 A9" },
            {$"{nameof(Wash04LaundryItem)} {nameof(Minute45TimeOptionItem)}", "01 06 01 26 00 04 68 6B" },
        };

        public PaymentItem(Form form)
        {

            mainForm = form;
            machineService = new MachineService();
        }

        public async void Click()
        {
            try
            {
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;
                if (form.LaundryOptionItemSelected != null && form.TimeOptionItemSelected != null)
                {
                    await Task.Run(async () =>
                    {
                        System.Threading.Thread.Sleep(3000);
                        Logger.Log($"{nameof(PaymentItem)} Step 1 START");
                        string programCommand = programCommands[$"{form.LaundryOptionItemSelected.Name} {form.TimeOptionItemSelected.Name}"];

                        AppConfigModel appConfig = Program.AppConfig;
                        Logger.Log($"{nameof(PaymentItem)} Step 8 {JsonConvert.SerializeObject(appConfig)}");
                        bool isConnected = await machineService.ConnectAsync(appConfig.DollarCom, appConfig.DollarBaudRate, appConfig.DollarData, appConfig.DollarParity, appConfig.DollarStopBits);

                        if (isConnected)
                        {
                            Logger.Log($"{nameof(PaymentItem)} Step 10");
                            if (Program.AppConfig.XorMethod == 1)
                            {
                                machineService.ExecHexCommand(programCommand);
                            }
                            else
                            {
                                machineService.ExecCommand(programCommand);
                            }
                            machineService.Disconect();
                            Logger.Log($"{nameof(PaymentItem)} Step 12 END");
                        }
                        else
                        {
                            Logger.Log($"{nameof(PaymentItem)} Can not connect device.");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btnPaymentItem = new ButtonRoundedUI()
            {
                Height = 65,
                Width = 150,
                Text = "付款 Payment",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#3a7d22"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                ForeColor = Color.White
            };
            btnPaymentItem.Click += BtnPaymentItem_Click;
            return btnPaymentItem;
        }

        private void BtnPaymentItem_Click(object sender, EventArgs e)
        {
            Click();
        }
    }
}
