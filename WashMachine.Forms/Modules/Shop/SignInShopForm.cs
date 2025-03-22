using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WashMachine.Forms.Common.Utils;
using WashMachine.Forms.Modules.Shop.Model;
using WashMachine.Forms.Modules.Shop.Service;

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
            Text = "Wash Machines System";

            ResizeRedraw = false;
            Padding = new Padding(10);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            shopService = new ShopService();
            Font = new Font(Font.FontFamily, 12f, FontStyle.Regular);

            tlpSignInShopForm = new TableLayoutPanel()
            {
                Width = 550,
                Height = 150,
                Padding = new Padding(10),
                Name = "MainLayout",
            };

            tlpSignInShopForm.Paint += TlpSignInShopForm_Paint;
            tlpSignInShopForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tlpSignInShopForm.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tlpSignInShopForm.ColumnStyles.Add(new ColumnStyle() { Width = 5, SizeType = SizeType.Percent });
            tlpSignInShopForm.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            tlpSignInShopForm.Controls.Add(new Label() { Text = "Wellcome to Wash Machines System", Dock = DockStyle.Fill });

            tbShopCode = new TextBox() { Multiline = true, Height = 45, Width = 250, MaxLength = 3, Dock = DockStyle.Fill };
            tbShopCode.KeyUp += TbShopCode_KeyUp;
            tlpSignInShopForm.Controls.Add(tbShopCode, 0, 1);

            Panel pnlButton = new Panel()
            {
                Dock = DockStyle.Fill
            };
            btnSignIn = new Button() { Text = "Login", Dock = DockStyle.Fill };
            btnSignIn.Click += BtnSignIn_Click;
            pnlButton.Controls.Add(btnSignIn);

            tlpSignInShopForm.Controls.Add(pnlButton, 1, 1);
            Controls.Add(tlpSignInShopForm);
            Resize += SignInShopForm_Resize;
        }

        private void TbShopCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSignIn_Click(sender, e);
            }
        }

        private void SignInShopForm_Resize(object sender, System.EventArgs e)
        {
            ScaleUtil.ScaleAll(Controls, this);
            tlpSignInShopForm.Location = new Point((Width - tlpSignInShopForm.Width) / 2, (Height - tlpSignInShopForm.Height) / 2);
            Refresh();
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

                        Close();
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

        private void TlpSignInShopForm_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Black, ButtonBorderStyle.Solid);
            tlpSignInShopForm.Location = new Point((Width - tlpSignInShopForm.Width) / 2, (Height - tlpSignInShopForm.Height) / 2);
        }
    }
}
