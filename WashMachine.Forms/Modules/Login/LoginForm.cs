using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Common.Utils;
using WashMachine.Forms.Modules.Laundry;
using WashMachine.Forms.Modules.Login.Account;
using WashMachine.Forms.Modules.Main;
using WashMachine.Forms.Modules.PaidBy;
using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using WashMachine.Forms.Modules.Payment.PaymentItems;
using WashMachine.Forms.Modules.Shop;

namespace WashMachine.Forms.Modules.Login
{
    public partial class LoginForm : Form
    {
        private TableLayoutPanel tlpLoginForm;
        private TextBox tbUserName;
        private TextBox tbPassword;
        private AccountService accountService;
        private ProgressUI progressUI;

        public LoginForm()
        {
            InitializeComponent();
            Text = "Wash Machines System";

            accountService = new AccountService();
            progressUI = new ProgressUI();
            progressUI.SetParent(this);

            ResizeRedraw = false;
            Padding = new Padding(10);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;
            Resize += LoginForm_Resize;

            tlpLoginForm = new TableLayoutPanel()
            {
                Width = 450,
                Height = 380,
                Padding = new Padding(10),
                Name = "MainLayout"
            };

            Font = new Font(Font.FontFamily, 12f, FontStyle.Regular);
            tlpLoginForm.Paint += TlpLoginForm_Paint;
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpLoginForm.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            ButtonRoundedUI btnLogin = new ButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Text = "Normal Operate 正常操作",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnLogin.Click += BtnLogin_Click;
            tlpLoginForm.Controls.Add(btnLogin, 0, 0);

            TableLayoutPanel tblInputs = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tblInputs.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblInputs.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblInputs.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tblInputs.ColumnStyles.Add(new ColumnStyle() { Width = 2, SizeType = SizeType.Percent });
            tbUserName = new TextBox() { Dock = DockStyle.Fill, Text = "admin@washmachine.com", Multiline = true };
            tbPassword = new TextBox() { Dock = DockStyle.Fill, Text = "admin@123", PasswordChar = '*', Multiline = true };

            tblInputs.Controls.Add(new Label() { Text = "USER ID:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblInputs.Controls.Add(tbUserName, 1, 0);

            tblInputs.Controls.Add(new Label() { Text = "PASSWORD:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblInputs.Controls.Add(tbPassword, 1, 1);

            tlpLoginForm.Controls.Add(tblInputs, 0, 1);

            ButtonRoundedUI btnPayment = new ButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Text = "Test Payment Only",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnPayment.Click += BtnPayment_Click;
            tlpLoginForm.Controls.Add(btnPayment, 0, 2);

            ButtonRoundedUI btnMachineWithoutPayment = new ButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Text = "Test Machine without Payment",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnMachineWithoutPayment.Click += BtnMachine_Click;
            tlpLoginForm.Controls.Add(btnMachineWithoutPayment, 0, 3);

            ButtonRoundedUI btnLogout = new ButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
                Text = "Logout",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnLogout.Click += BtnLogout_Click;
            tlpLoginForm.Controls.Add(btnLogout, 0, 4);

            Controls.Add(tlpLoginForm);
            FormClosed += LoginForm_FormClosed;

            InitialData();

            if (!File.Exists(Program.AppConfig.ShopConfigPath))
            {
                SignInShopForm signInShopForm = new SignInShopForm();
                signInShopForm.ShowDialog();
                signInShopForm.FormClosed += SignInShopForm_FormClosed;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Program.AppConfig.ShopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), Program.ShopConfigFile);
            if (File.Exists(Program.AppConfig.ShopConfigPath))
            {
                File.Delete(Program.AppConfig.ShopConfigPath);
            }

            SignInShopForm signInShopForm = new SignInShopForm();
            signInShopForm.ShowDialog();
            signInShopForm.FormClosed += SignInShopForm_FormClosed;
        }

        private void InitialOctopusAsync()
        {
            //Start a timer to upload files or download file from server
            if (Program.octopusService == null)
            {
                Program.octopusService = new OctopusService();
                Program.octopusService.SetCurrentForm(this);
                Program.octopusService.RunAJobToCompleteFiles();
                Program.octopusService.Initial();
                Program.octopusService.RunTimerHealthCheck();
            }
        }

        private void SignInShopForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.AppConfig.ShopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), Program.ShopConfigFile);
            if (!File.Exists(Program.AppConfig.ShopConfigPath))
            {
                Application.Exit();
            }
            Show();
            InitialData();
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Program.octopusService?.Disconect();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void InitialData()
        {
            Cursor = Cursors.WaitCursor;
            tlpLoginForm.Enabled = false;
            progressUI.Show();

            Task.Run(() =>
            {
                Thread.Sleep(3000);
                BeginInvoke((MethodInvoker)async delegate
                {
                    try
                    {
                        Program.ShopConfig = Program.AppConfig.GetShopConfig();
                        bool isLogin = await ReSignIn(Program.ShopConfig.Code);
                        if (isLogin)
                        {
                            Program.ShopConfig = Program.AppConfig.GetShopConfig();
                            //await Program.ShopConfig.ShopSetting.LoadImages();
                            InitialOctopusAsync();
                            Program.octopusService.SetUserIsUsingApp(true);
                        }
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                        tlpLoginForm.Enabled = true;
                        progressUI.Hide();
                        Refresh();
                    }
                });
            });
        }

        private async Task<bool> ReSignIn(string code)
        {
            Shop.Model.ShopModel shopModel = await new Modules.Shop.Service.ShopService().SignIn(code);
            if (shopModel == null)
            {
                MessageBox.Show("The shop code is not available, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                try
                {
                    Program.AppConfig.ShopConfigPath = Path.Combine(Directory.GetCurrentDirectory(), Program.ShopConfigFile);
                    if (!File.Exists(Program.AppConfig.ShopConfigPath))
                    {
                        Application.Exit();
                    }

                    using (FileStream fileStream = new FileStream(Program.AppConfig.ShopConfigPath, FileMode.OpenOrCreate))
                    {
                        string dataasstring = JsonConvert.SerializeObject(shopModel);
                        byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                        fileStream.Write(info, 0, info.Length);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    MessageBox.Show("The shop code is not available, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }
        }

        private async void BtnMachine_Click(object sender, EventArgs e)
        {
            progressUI.Show();
            string error = await accountService.SignIn(tbUserName.Text, tbPassword.Text);
            progressUI.Hide();

            if (string.IsNullOrWhiteSpace(error))
            {
                LaundryForm laundryForm = new LaundryForm(FollowType.TestMachineWithoutPayment);
                laundryForm.FormClosed += LaundryForm_FormClosed;
                laundryForm.Show();
                Hide();
            }
            else
            {
                MessageBox.Show(error, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LaundryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Show();
        }

        private async void BtnPayment_Click(object sender, EventArgs e)
        {
            progressUI.Show();
            string error = await accountService.SignIn(tbUserName.Text, tbPassword.Text);
            progressUI.Hide();

            if (string.IsNullOrWhiteSpace(error))
            {
                PaidByForm paidByForm = new PaidByForm(FollowType.TestPaymentOnly, new HkdPaymentItem(20));
                paidByForm.Show();
                paidByForm.FormClosed += PaidByForm_FormClosed;
                Hide();
            }
            else
            {
                MessageBox.Show(error, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PaidByForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(FollowType.Normal);
            mainForm.Show();
            mainForm.FormClosed += MainForm_FormClosed;
            Hide();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Show();
        }

        private void TlpLoginForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            ScaleUtil.ScaleAll(Controls, this);
            tlpLoginForm.Location = new Point((Width - tlpLoginForm.Width) / 2, (Height - tlpLoginForm.Height) / 2);
            Refresh();
        }
    }
}
