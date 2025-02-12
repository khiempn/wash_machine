using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.Shop.Model;
using WashMachine.Forms.Modules.Shop.Service;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Shop
{
    public partial class SignInShopForm : Form
    {
        TableLayoutPanel tlpSignInShopForm;
        Button btnSignIn;
        TextBox tbShopCode;
        ShopService shopService;

        public SignInShopForm()
        {
            InitializeComponent();
            ResizeRedraw = false;
            Padding = new Padding(10);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            Resize += SignInShopForm_Resize;
            shopService = new ShopService();

            tlpSignInShopForm = new TableLayoutPanel()
            {
                Width = 400,
                Height = 130,
                Padding = new Padding(10),
                Name = "MainLayout",
            };

            tlpSignInShopForm.Font = new Font(Font.FontFamily, 14f, FontStyle.Regular);
            tlpSignInShopForm.Paint += TlpSignInShopForm_Paint;
            tlpSignInShopForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.Absolute });
            tlpSignInShopForm.RowStyles.Add(new RowStyle() { Height = 50, SizeType = SizeType.Absolute });
            tlpSignInShopForm.ColumnStyles.Add(new ColumnStyle() { Width = 250, SizeType = SizeType.Absolute });
            tlpSignInShopForm.ColumnStyles.Add(new ColumnStyle() { Width = 130, SizeType = SizeType.AutoSize });

            tlpSignInShopForm.Controls.Add(new Label() { Text = "Sign In" });

            tbShopCode = new TextBox() { Multiline = true, Height = 45, Width = 250, MaxLength = 3 };
            tlpSignInShopForm.Controls.Add(tbShopCode, 0, 1);

            Panel pnlButton = new Panel()
            {
                Dock = DockStyle.Fill,
                Width = 130,
            };
            btnSignIn = new Button() { Text = "Sign In", Height = 46, Width = 130 };
            btnSignIn.Click += BtnSignIn_Click;
            pnlButton.Controls.Add(btnSignIn);

            tlpSignInShopForm.Controls.Add(pnlButton, 1, 1);

            Controls.Add(tlpSignInShopForm);
        }

        private void SignInShopForm_Resize(object sender, System.EventArgs e)
        {
            tlpSignInShopForm.Location = new Point((Width - tlpSignInShopForm.Width) / 2, (Height - tlpSignInShopForm.Height) / 2);
        }

        private async void BtnSignIn_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbShopCode.Text))
            {
                Cursor = Cursors.WaitCursor;
                Enabled = false;
                ShopModel shopModel = await shopService.SignIn(tbShopCode.Text.Trim());
                if (shopModel == null)
                {
                    MessageBox.Show("The shop code is not available, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Enabled = true;
                }
                else
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(Program.AppConfig.ShopConfigPath, FileMode.OpenOrCreate))
                        {
                            string dataasstring = JsonConvert.SerializeObject(shopModel);
                            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                            fileStream.Write(info, 0, info.Length);
                        }
                        Cursor = Cursors.Default;
                        Enabled = true;

                        LoginForm loginForm = new LoginForm();
                        loginForm.Show();
                        loginForm.FormClosed += LoginForm_FormClosed;
                        Hide();
                    }
                    catch (Exception ex)
                    {
                        Cursor = Cursors.Default;
                        Enabled = true;
                        Logger.Log(ex);
                        MessageBox.Show("The shop code is not available, please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter shop code is active.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void TlpSignInShopForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Black, ButtonBorderStyle.Solid);
            tlpSignInShopForm.Location = new Point((Width - tlpSignInShopForm.Width) / 2, (Height - tlpSignInShopForm.Height) / 2);
        }
    }
}
