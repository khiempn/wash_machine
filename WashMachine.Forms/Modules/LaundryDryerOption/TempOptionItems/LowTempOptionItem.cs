using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems
{
    public class LowTempOptionItem : ITempOptionItem
    {
        Form mainForm;

        public LowTempOptionItem(Form form)
        {
            mainForm = form;
        }

        public void Click()
        {
            
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btn = new ButtonRoundedUI()
            {
                Text = "低溫\nLow Temp",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#ffffff"),
                ShapeSelectedBackgroudColor = ColorTranslator.FromHtml("#156082"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 79,
                Dock = DockStyle.Fill,
                ForeColor = ColorTranslator.FromHtml("#156082"),
                Font = new Font(new Label().Font.FontFamily, 10),
                Name = nameof(LowTempOptionItem)
            };
            btn.Click += Btn_Click;
            return btn;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Click();
            if (mainForm.Controls.Find(nameof(LowTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(LowTempOptionItem), true).First();
                buttonRounded.IsSelected = true;
                buttonRounded.Refresh();
            }

            if (mainForm.Controls.Find(nameof(MidTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(MidTempOptionItem), true).First();
                buttonRounded.IsSelected = false;
                buttonRounded.Refresh();
            }

            if (mainForm.Controls.Find(nameof(HighTempOptionItem), true).Any())
            {
                ButtonRoundedUI buttonRounded = (ButtonRoundedUI)mainForm.Controls.Find(nameof(HighTempOptionItem), true).First();
                buttonRounded.IsSelected = false;
                buttonRounded.Refresh();
            }
        }
    }
}
