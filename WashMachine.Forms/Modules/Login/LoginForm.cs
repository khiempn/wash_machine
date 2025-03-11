using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Login.Account;
using WashMachine.Forms.Modules.Main;
using WashMachine.Forms.Modules.PaidBy;
using WashMachine.Forms.Modules.PaidBy.Machine.Octopus;
using WashMachine.Forms.Modules.Payment;
using WashMachine.Forms.Modules.Payment.PaymentItems;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Modules.Laundry;

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
                Width = 420,
                Height = 290,
                Padding = new Padding(10),
                Name = "MainLayout"
            };

            tlpLoginForm.Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            //tlpLoginForm.Location = new Point((Width - tlpLoginForm.Width) / 2, (Height - tlpLoginForm.Height) / 2);
            tlpLoginForm.Paint += TlpLoginForm_Paint;
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.AutoSize });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 100, SizeType = SizeType.AutoSize });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.AutoSize });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.AutoSize });
            tlpLoginForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.AutoSize });
            tlpLoginForm.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Percent });

            ButtonRoundedUI btnLogin = new ButtonRoundedUI()
            {
                Height = 50,
                Width = Width,
                Text = "Normal Operate 正常操作",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnLogin.Click += BtnLogin_Click;
            tlpLoginForm.Controls.Add(btnLogin, 0, 0);

            TableLayoutPanel tblInputs = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Height = 100
            };
            tblInputs.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.Absolute });
            tblInputs.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.Absolute });
            tblInputs.ColumnStyles.Add(new ColumnStyle() { Width = 130, SizeType = SizeType.Absolute });
            tblInputs.ColumnStyles.Add(new ColumnStyle() { Width = 100, SizeType = SizeType.Percent });
            tbUserName = new TextBox() { Dock = DockStyle.Fill, Text = "admin@boxcut.hk" };
            tbPassword = new TextBox() { Dock = DockStyle.Fill, Text = "admin@123", PasswordChar = '*' };

            tblInputs.Controls.Add(new Label() { Text = "USER ID:", Width = 130, Height = 30, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            tblInputs.Controls.Add(tbUserName, 1, 0);

            tblInputs.Controls.Add(new Label() { Text = "PASSWORD:", Width = 170, Height = 30, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblInputs.Controls.Add(tbPassword, 1, 1);

            tlpLoginForm.Controls.Add(tblInputs, 0, 1);

            ButtonRoundedUI btnPayment = new ButtonRoundedUI()
            {
                Height = 50,
                Width = Width,
                Text = "Test Payment Only",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnPayment.Click += BtnPayment_Click;
            tlpLoginForm.Controls.Add(btnPayment, 0, 3);

            ButtonRoundedUI btnMachineWithoutPayment = new ButtonRoundedUI()
            {
                Height = 50,
                Width = Width,
                Text = "Test Machine without Payment",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#a5a5a5"),
                ShapeBorderColor = Color.Black
            };
            btnMachineWithoutPayment.Click += BtnMachine_Click;
            tlpLoginForm.Controls.Add(btnMachineWithoutPayment, 0, 4);

            Controls.Add(tlpLoginForm);

            Program.octopusService = new OctopusService();
            Program.octopusService.SetCurrentForm(this);
            Program.octopusService.RunAJobToCompleteFiles();
            Program.octopusService.Initial();
            Program.octopusService.SetUserIsUsingApp(true);
            LoadSettingImagesAsync();
        }

        private void LoadSettingImagesAsync()
        {
            Cursor = Cursors.WaitCursor;
            tlpLoginForm.Enabled = false;
            progressUI.Show();
            Task.Run(async () =>
            {
                try
                {
                    Program.ShopConfig = Program.AppConfig.GetShopConfig();
                    await Program.ShopConfig.ShopSetting.LoadImages();
                   
                }
                finally
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        Cursor = Cursors.Default;
                        tlpLoginForm.Enabled = true;
                        progressUI.Hide();
                        Refresh();
                    });
                }
            });
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
            tlpLoginForm.Location = new Point((Width - tlpLoginForm.Width) / 2, (Height - tlpLoginForm.Height) / 2);
        }
    }
}
