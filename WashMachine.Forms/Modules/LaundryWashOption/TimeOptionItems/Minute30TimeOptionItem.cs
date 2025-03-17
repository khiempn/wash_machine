using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems
{
    public class Minute30TimeOptionItem : ITimeOptionItem
    {
        Form mainForm;
        public string Name => nameof(Minute30TimeOptionItem);
        public int TimeNumber => 30;
        public int Amount => 20;
        public Minute30TimeOptionItem(Form form)
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
                btn.IsSelected = true;
            }

            if (mainForm.Controls.Find(nameof(Minute40TimeOptionItem), true).Any())
            {
                ButtonRoundedUI btn = (ButtonRoundedUI)mainForm.Controls.Find(nameof(Minute40TimeOptionItem), true).First();
                btn.IsSelected = false;
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
                Text = "30 分鐘 (mins) / HKD 20",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#ffc000"),
                ShapeSelectedBackgroudColor = Color.Blue,
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Name = nameof(Minute30TimeOptionItem)
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
