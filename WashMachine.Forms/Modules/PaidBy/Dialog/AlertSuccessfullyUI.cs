using EFTSolutions;
using WashMachine.Forms.Modules.PaidBy.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Dialog
{
    public class AlertSuccessfullyUI : Panel
    {
        TableLayoutPanel tblMain;
        Button btnHome;
        Button btnPrinter;
        public event EventHandler HomeClick;
        public event EventHandler PrinterClick;
        Timer timer;

        public AlertSuccessfullyUI()
        {
            Padding = new Padding(10);
            Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            Size = new Size(450, 570);
            BackColor = Color.White;

            tblMain = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };

            tblMain.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tblMain.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tblMain.RowStyles.Add(new RowStyle() { Height = 7, SizeType = SizeType.Percent });
            tblMain.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tblMain.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.AutoSize });
            tblMain.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            Panel pnStatus = new Panel()
            {
                Dock = DockStyle.Fill
            };
            pnStatus.Controls.Add(new Label()
            {
                Text = "付款成功!\nPayment Successfully!",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Height = 30,
            });

            tblMain.Controls.Add(pnStatus, 0, 0);

            byte[] bytes = Convert.FromBase64String("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");
            MemoryStream ms = new MemoryStream(bytes);
            TableLayoutPanel tblIcons = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tblIcons.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblIcons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblIcons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblIcons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblIcons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblIcons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            Panel pnIcon = new Panel()
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Image.FromStream(ms),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            tblIcons.Controls.Add(new Control() { Dock = DockStyle.Fill }, 0, 0);
            tblIcons.Controls.Add(new Control() { Dock = DockStyle.Fill }, 1, 0);
            tblIcons.Controls.Add(pnIcon, 2, 0);
            tblIcons.Controls.Add(new Control() { Dock = DockStyle.Fill }, 3, 0);
            tblIcons.Controls.Add(new Control() { Dock = DockStyle.Fill }, 4, 0);

            tblMain.Controls.Add(tblIcons, 0, 1);
            tblMain.Controls.Add(new Panel() { Name = "pnInvoice", Dock = DockStyle.Fill,
                AutoSize = false,
                AutoScroll = true
            }, 0, 2);

            TableLayoutPanel tblButtons = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tblButtons.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblButtons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblButtons.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            btnHome = new Button() { Text = "QUIT", Dock = DockStyle.Fill };
            btnHome.Click += BtnHome_Click;
            btnPrinter = new Button() { Text = "PRINT", Dock = DockStyle.Fill };
            btnPrinter.Click += BtnPrinter_Click;
            tblButtons.Controls.Add(btnHome, 0, 0);
            tblButtons.Controls.Add(btnPrinter, 1, 0);

            tblMain.Controls.Add(tblButtons, 0, 3);

            tblMain.Controls.Add(new Label() { ForeColor = SystemColors.InfoText, Name = "lbTimeCounter", Dock = DockStyle.Fill }, 0, 4);

            Controls.Add(tblMain);
            Visible = false;

            BorderStyle = BorderStyle.FixedSingle;
            Visible = false;
            SizeChanged += Panel_SizeChanged;
            VisibleChanged += AlertSuccessfullyUI_VisibleChanged;
        }

        private void BtnPrinter_Click(object sender, EventArgs e)
        {
            PrinterClick?.Invoke(sender, e);
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            HomeClick?.Invoke(sender, e);
        }

        private void Panel_SizeChanged(object sender, System.EventArgs e)
        {
            tblMain.Location = new Point((500 - tblMain.Width) / 2, (400 - tblMain.Height) / 2);
        }

        private void AlertSuccessfullyUI_VisibleChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Control[] mainLayouts = Parent.Controls.Find("MainLayout", true);

                if (mainLayouts != null && mainLayouts.Length > 0)
                {
                    Control mainLayout = mainLayouts[0];

                    if (Visible)
                    {
                        mainLayout.Enabled = false;
                        timer = new Timer();
                        timer.Interval = 1000;
                        timer.Tick += Timer_Tick;
                        timer.Enabled = true;
                        timer.Interval = 1000;
                        timer.Tag = 1000 * 15; //Total Time
                        timer.Start();
                    }
                    else
                    {
                        mainLayout.Enabled = true;
                        if (timer != null)
                        {
                            timer.Stop();
                        }
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                int currentTime = int.Parse(timer.Tag.ToString());
                currentTime -= 1000;
                timer.Tag = currentTime;
                if (currentTime > 0)
                {
                    if (Controls.Find("lbTimeCounter", true).Any())
                    {
                        Label lbTimeCounter = (Label)Controls.Find("lbTimeCounter", true)[0];
                        lbTimeCounter.Text = $"The popup will be closed in {currentTime / 1000}s.";
                        var sizeText = TextRenderer.MeasureText(lbTimeCounter.Text, lbTimeCounter.Font);
                        lbTimeCounter.Width = sizeText.Width;
                        lbTimeCounter.Height = sizeText.Height;
                    }
                }
                else
                {
                    timer.Stop();
                    HomeClick.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SetParent(Form parent)
        {
            if (parent != null)
            {
                Parent = parent;
                Parent.SizeChanged += Parent_SizeChanged;
                Parent.Controls.SetChildIndex(this, 0);
                Parent_SizeChanged(parent, EventArgs.Empty);
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                Location = new Point((Parent.Width - Width) / 2, (Parent.Height - Height) / 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SetPrintOrderModel(OrderModel orderModel)
        {
            btnPrinter.Tag = orderModel;
        }

        public void SetOctopusInvoice(OrderModel orderModel)
        {
            Panel pnInvoice = (Panel)Controls.Find("pnInvoice", true)[0];
            pnInvoice.Dock = DockStyle.Fill;
            pnInvoice.Controls.Clear();

            Machine.Octopus.CardInfo cardInfo = JsonConvert.DeserializeObject<Machine.Octopus.CardInfo>(orderModel.CardJson);
            int typeCash = 1;
            int typeOnline = 2;
            int typeAAVS = 4;

            var lastAddValueMessage = string.Empty;
            var lastAddValueMessageEn = string.Empty;
            if (cardInfo.LastAddType == typeCash.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}現金增值";
                lastAddValueMessageEn = $"Last add value by Cash on {cardInfo.LastAddDate}";
            }
            else if (cardInfo.LastAddType == typeOnline.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}網上增值";
                lastAddValueMessageEn = $"Last add value by Online on {cardInfo.LastAddDate}";
            }
            else if (cardInfo.LastAddType == typeAAVS.ToString())
            {
                lastAddValueMessage = $"上一次於{cardInfo.LastAddDate}自動增值";
                lastAddValueMessageEn = $"Last add value by AAVS on {cardInfo.LastAddDate}";
            }

            List<string> lines = new List<string> {
                "----------------------------------------------------------------------------------------------------------------",
                $"Shop Name:　{ orderModel.ShopName }",
                $"Shop Code:　{ orderModel.ShopCode }",
                $"Print Time:　{ DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") }",
                "----------------------------------------------------------------------------------------------------------------",
                $"日期時間 (Date/Time): {orderModel.InsertTime.Value.ToString("@yyyy-MM-dd HH:mm:ss")}",
                $"店號 (Shop no.): {orderModel.ShopCode}",
                $"機號 (Device no.): {orderModel.DeviceId}",
                $"收據號碼 (Receipt no.): {orderModel.PaymentId}",
                $"總額 (Total): HKD {FormatDecimal(orderModel.Amount)}",
                "八達通付款(Octopus payment)",
                $"八達通號碼(Octopus no.): {cardInfo.OctopusNo}",
                $"扣除金額 (Amount deducted): -HKD {FormatDecimal(orderModel.Amount, 1)}",
                $"餘額 (Remaining Value)： HKD {FormatDecimal(cardInfo.RemainValue, 1)}"
            };

            if (!string.IsNullOrEmpty(lastAddValueMessageEn))
            {
                lines.Add(lastAddValueMessageEn);
            }

            if (!string.IsNullOrEmpty(lastAddValueMessage))
            {
                lines.Add(lastAddValueMessage);
            }

            TableLayoutPanel tblLines = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = false
            };

            tblLines.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                tblLines.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
                tblLines.Controls.Add(new Label()
                {
                    Text = line,
                    Dock = DockStyle.Fill
                }, 0, i);
            }
            pnInvoice.Controls.Add(tblLines);
        }

        public void SetAlipayInvoice(OrderModel orderModel)
        {
            Panel pnInvoice = (Panel)Controls.Find("pnInvoice", true)[0];
            pnInvoice.Dock = DockStyle.Fill;
                
            pnInvoice.Controls.Clear();

            TransactionRecord cardInfo = JsonConvert.DeserializeObject<TransactionRecord>(orderModel.CardJson);

            List<string> lines = new List<string> {
                "----------------------------------------------------------------------------------------------------------------",
                $"Shop Name:　{ orderModel.ShopName }",
                $"Shop Code:　{ orderModel.ShopCode }",
                $"Print Time:　{ DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") }",
                "----------------------------------------------------------------------------------------------------------------",
                $"交易編號 Transaction No.:　{ cardInfo.OrderNumber }",
                $"付款方法 Payment Method : 以AliPay支付 / AliPay Payment",
                $"總額 (Total): HKD { FormatDecimal(orderModel.Amount) }"
            };

            TableLayoutPanel tblLines = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = false
            };

            tblLines.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                tblLines.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
                tblLines.Controls.Add(new Label()
                {
                    Text = line,
                    Dock = DockStyle.Fill
                }, 0, i);
            }
            pnInvoice.Controls.Add(tblLines);
        }

        public void SetPaymeInvoice(OrderModel orderModel)
        {
            Panel pnInvoice = (Panel)Controls.Find("pnInvoice", true)[0];
            pnInvoice.Dock = DockStyle.Fill;
            pnInvoice.Controls.Clear();

            TransactionRecord cardInfo = JsonConvert.DeserializeObject<TransactionRecord>(orderModel.CardJson);

            List<string> lines = new List<string> {
                "----------------------------------------------------------------------------------------------------------------",
                $"Shop Name:　{ orderModel.ShopName }",
                $"Shop Code:　{ orderModel.ShopCode }",
                $"Print Time:　{ DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") }",
                "----------------------------------------------------------------------------------------------------------------",
                $"交易編號 Transaction No.:　{ cardInfo.OrderNumber }",
                $"付款方法 Payment Method : 以Payme支付 / Payme Payment",
                $"總額 (Total): HKD { FormatDecimal(orderModel.Amount) }"
            };

            TableLayoutPanel tblLines = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                AutoScroll = false
            };

            tblLines.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                tblLines.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
                tblLines.Controls.Add(new Label()
                {
                    Text = line,
                    Dock = DockStyle.Fill
                }, 0, i);
            }
            pnInvoice.Controls.Add(tblLines);
        }

        public string FormatDecimal(float? value, int n = 2)
        {
            if (value == null) return "0";
            return value.Value.ToString("n" + n);
        }
    }
}
