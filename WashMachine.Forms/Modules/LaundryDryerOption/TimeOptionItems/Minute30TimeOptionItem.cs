using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryDryerOption.TimeOptionItems
{
    public class Minute30TimeOptionItem : ITimeOptionItem
    {
        public Minute30TimeOptionItem()
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
                Text = "30 分鐘 (mins) / HKD 20",
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
