using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems
{
    public class MidTempOptionItem : ITempOptionItem
    {
        Form mainForm;
        public string Name => nameof(MidTempOptionItem);

        public MidTempOptionItem(Form form)
        {
            mainForm = form;
        }

        public void Click()
        {
            if (mainForm.Controls.Find(nameof(LowTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(LowTempOptionItem), true).First();
                buttonRounded.IsSelected = false;
            }

            if (mainForm.Controls.Find(nameof(MidTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(MidTempOptionItem), true).First();
                buttonRounded.IsSelected = true;
            }

            if (mainForm.Controls.Find(nameof(HighTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(HighTempOptionItem), true).First();
                buttonRounded.IsSelected = false;
            }

            LaundryDryerOptionForm laundryDryerOptionForm = (LaundryDryerOptionForm)mainForm;
            laundryDryerOptionForm.TempOptionItemSelected = this;
            laundryDryerOptionForm.Refresh();
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btn = new ButtonRoundedUI()
            {
                Text = "中溫\nMid Temp",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#ffffff"),
                ShapeSelectedBackgroudColor = ColorTranslator.FromHtml("#156082"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 79,
                Dock = DockStyle.Fill,
                ForeColor = ColorTranslator.FromHtml("#156082"),
                Font = new Font(new Label().Font.FontFamily, 10),
                Name = nameof(MidTempOptionItem)
            };
            btn.Click += Btn_Click;

            return btn;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Click();
        }
    }
}
