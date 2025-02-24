using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryWashOption.TimeOptionItems
{
    public class Minute40TimeOptionItem : ITimeOptionItem
    {
        public Minute40TimeOptionItem()
        {
            
        }

        public void Click()
        {
            throw new NotImplementedException();
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btn = new ButtonRoundedUI()
            {
                Height = 65,
                Width = 150,
                Text = "40 分鐘 (mins) / HKD 30",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#ffc000"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                Dock = DockStyle.Fill,
                ForeColor = Color.White
            };
            return btn;
        }
    }
}
