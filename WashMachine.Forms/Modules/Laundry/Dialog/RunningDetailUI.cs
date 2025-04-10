﻿using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.LaundryDryerOption.LaundryOptionItems;
using WashMachine.Forms.Modules.LaundryWashOption.LaundryOptionItems;

namespace WashMachine.Forms.Modules.Laundry.Dialog
{
    public partial class RunningDetailUI : Form
    {
        Timer timer;
        MachineModel _machineModel;
        ProgressUI progressUI;

        public RunningDetailUI(MachineModel machineModel)
        {
            InitializeComponent();
            _machineModel = machineModel;
            Text = "Information";

            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            ResizeRedraw = false;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            BackColor = Color.White;
            Font = new Font(Font.FontFamily, 12f, FontStyle.Regular);

            Width = 500;
            Height = 350;
            TableLayoutPanel tblDetailForm = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Name = "MainLayout",
            };
            tblDetailForm.RowStyles.Add(new RowStyle() { Height = 3, SizeType = SizeType.Percent });
            tblDetailForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblDetailForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblDetailForm.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tblDetailForm.RowStyles.Add(new RowStyle() { Height = 2, SizeType = SizeType.Percent });
            tblDetailForm.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            TableLayoutPanel tbHeader = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tbHeader.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tbHeader.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tbHeader.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });

            tbHeader.Controls.Add(new Label() { Text = $"Started At: {MachineService.GetStartAtFormat(_machineModel)}\nEnd At: {MachineService.GetEndAtFormat(_machineModel)}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            tbHeader.Controls.Add(new Label() { Name = "lbRemainningTime", Text = $"Remaining Time: \n {MachineService.GetRemainTimeAsFormat(_machineModel)}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);

            tblDetailForm.Controls.Add(tbHeader);

            tblDetailForm.Controls.Add(new Label() { Text = "Settings" + new string('-', Width), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            tblDetailForm.Controls.Add(new Label() { Text = $"Time: {_machineModel.Time} min(s)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);

            if (_machineModel.Name.StartsWith("Dryer"))
            {
                tblDetailForm.Controls.Add(new Label() { Text = $"Temp: {MachineService.GetTempName(_machineModel.Temp)}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            }

            TableLayoutPanel tbFooter = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill
            };
            tbFooter.RowStyles.Add(new RowStyle() { Height = 1, SizeType = SizeType.Percent });
            tbFooter.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            tbFooter.ColumnStyles.Add(new ColumnStyle() { Width = 1, SizeType = SizeType.Percent });
            Button btnClose = new Button() { Text = "Cancel", Dock = DockStyle.Fill };
            btnClose.Click += BtnClose_Click;
            Button btnStop = new Button() { Text = "Stop", BackColor = Color.DarkRed, Dock = DockStyle.Fill };
            btnStop.Click += BtnStop_Click;

            tbFooter.Controls.Add(btnClose, 0, 0);
            tbFooter.Controls.Add(btnStop, 1, 0);
            tblDetailForm.Controls.Add(tbFooter, 0, 4);
            progressUI = new ProgressUI();
            Controls.Add(progressUI);
            Controls.Add(tblDetailForm);
            Resize += RunningDetailUI_Resize;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Tag = _machineModel;
            timer.Start();
            RunningDetailUI_Resize(this, null);
        }

        private void RunningDetailUI_Resize(object sender, System.EventArgs e)
        {
            progressUI.Location = new Point((Width - progressUI.Width) / 2, (Height - progressUI.Height) / 2);
            Refresh();
        }

        private async void BtnStop_Click(object sender, System.EventArgs e)
        {
            progressUI.Show();
            timer.Stop();
            timer.Dispose();
            await StopMachine();
            Close();
        }

        private async Task StopMachine()
        {
            if (_machineModel.Name.Equals(nameof(Dryer01LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Dryer01LaundryItem dryerLaundryItem = new Dryer01LaundryItem(null, ParentForm);
                await dryerLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Dryer02LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Dryer02LaundryItem dryerLaundryItem = new Dryer02LaundryItem(null, ParentForm);
                await dryerLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Dryer03LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Dryer03LaundryItem dryerLaundryItem = new Dryer03LaundryItem(null, ParentForm);
                await dryerLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Dryer04LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Dryer04LaundryItem dryerLaundryItem = new Dryer04LaundryItem(null, ParentForm);
                await dryerLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Wash01LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Wash01LaundryItem washLaundryItem = new Wash01LaundryItem(null, ParentForm);
                await washLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Wash02LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Wash02LaundryItem washLaundryItem = new Wash02LaundryItem(null, ParentForm);
                await washLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Wash03LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Wash03LaundryItem washLaundryItem = new Wash03LaundryItem(null, ParentForm);
                await washLaundryItem.Stop();
                return;
            }

            if (_machineModel.Name.Equals(nameof(Wash04LaundryItem), StringComparison.OrdinalIgnoreCase))
            {
                Wash04LaundryItem washLaundryItem = new Wash04LaundryItem(null, ParentForm);
                await washLaundryItem.Stop();
                return;
            }
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            try
            {
                MachineModel machine = timer.Tag as MachineModel;
                if (MachineService.IsRunCompleted(machine))
                {
                    BtnStop_Click(sender, e);
                }
                else
                {
                    Label lbRemainningTime = Controls.Find("lbRemainningTime", true).First() as Label;
                    lbRemainningTime.Text = $"Remaining Time: \n {MachineService.GetRemainTimeAsFormat(machine)}";
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                timer.Stop();
                timer.Dispose();
            }
        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            Close();
        }
    }
}
