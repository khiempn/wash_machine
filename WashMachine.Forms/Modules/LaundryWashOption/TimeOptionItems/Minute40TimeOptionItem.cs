using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems
{
    public class Minute40TimeOptionItem : ITimeOptionItem
    {
        Form mainForm;
        public string Name => nameof(Minute40TimeOptionItem);
        public Minute40TimeOptionItem(Form form)
        {
            mainForm = form;
        }

        public void Click()
        {
            if (mainForm.Controls.Find(nameof(Minute15TimeOptionItem), true).Any())
            {
                ButtonRoundedUI btn = (ButtonRoundedUI)mainForm.Controls.Find(nameof(Minute15TimeOptionItem), true).First();
                btn.IsSelected = false;
            }

            if (mainForm.Controls.Find(nameof(Minute30TimeOptionItem), true).Any())
            {
                ButtonRoundedUI btn = (ButtonRoundedUI)mainForm.Controls.Find(nameof(Minute30TimeOptionItem), true).First();
                btn.IsSelected = false;
            }

            if (mainForm.Controls.Find(nameof(Minute40TimeOptionItem), true).Any())
            {
                ButtonRoundedUI btn = (ButtonRoundedUI)mainForm.Controls.Find(nameof(Minute40TimeOptionItem), true).First();
                btn.IsSelected = true;
            }

            if (mainForm.Controls.Find(nameof(Minute45TimeOptionItem), true).Any())
            {
                ButtonRoundedUI btn = (ButtonRoundedUI)mainForm.Controls.Find(nameof(Minute45TimeOptionItem), true).First();
                btn.IsSelected = false;
            }

            LaundryWashOptionForm laundryWashOptionForm = (LaundryWashOptionForm)mainForm;
            laundryWashOptionForm.TimeOptionItemSelected = this;
            laundryWashOptionForm.Refresh();
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btn = new ButtonRoundedUI()
            {
                Height = 65,
                Width = 150,
                Text = "40 分鐘 (mins) / HKD 30",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#ffc000"),
                ShapeSelectedBackgroudColor = Color.Blue,
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Name = nameof(Minute40TimeOptionItem)
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
