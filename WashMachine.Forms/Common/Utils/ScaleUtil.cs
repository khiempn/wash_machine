using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.LaundryDryerOption.TempOptionItems;

namespace WashMachine.Forms.Common.Utils
{
    public class ScaleUtil
    {
        public static Size BaseSize = new Size(1456, 868);

        public static void Scale(ref Control control, Form mainForm)
        {
            float widthPer = ((float)control.Width / BaseSize.Width);
            float heightPer = ((float)control.Height / BaseSize.Height);
            float paddingSize = Math.Min(mainForm.Width, mainForm.Height) * 0.02f;

            if (string.IsNullOrWhiteSpace(control.AccessibleName))
            {
                control.AccessibleName = $"{widthPer},{heightPer}";
            }
            else
            {
                string[] cacheValues = control.AccessibleName.Split(',');

                float.TryParse(cacheValues[0], out widthPer);
                float.TryParse(cacheValues[1], out heightPer);
            }

            Size formSize = mainForm.Size;
            control.Size = new Size((int)(formSize.Width * widthPer), (int)(formSize.Height * heightPer));

            if (control.Padding.All > 0)
            {
                control.Padding = new Padding((int)paddingSize);
            }

            if (control is CardButtonRoundedUI)
            {
                float radius = Math.Min(mainForm.Width, mainForm.Height) * 0.04f;

                CardButtonRoundedUI customControl = (CardButtonRoundedUI)control;
                if (customControl.CornerRadius > 0)
                {
                    customControl.CornerRadius = (int)radius;
                }
            }
        }

        public static void ScaleAll(Control.ControlCollection controlCollection, Form mainForm)
        {
            for (int i = 0; i < controlCollection.Count; i++)
            {
                Control control = controlCollection[i];
                Scale(ref control, mainForm);
                if (control.Controls.Count > 0)
                {
                    ScaleAll(control.Controls, mainForm);
                }
            }
        }
    }
}
