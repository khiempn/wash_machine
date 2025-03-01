using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.PaidBy.Dialog
{
    public class OutOfServiceUI : Panel
    {
        public OutOfServiceUI()
        {
            Font font = new Font(Font.FontFamily, 30);
            string text = "Out Of Service\n Octopus is working, please wait a second.";
            var size = TextRenderer.MeasureText(text, font);

            Label lbOutOfService = new Label()
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = font,
                Size = size,
                Name = "lbOutOfService"
            };

            Name = "pnOutOfService";
            Controls.Add(lbOutOfService);
            SizeChanged += OutOfServiceUI_SizeChanged;
            return;
        }

        private void OutOfServiceUI_SizeChanged(object sender, System.EventArgs e)
        {
            if (Controls.Find("lbOutOfService", true).Any())
            {
                Label lbOutOfService = (Label)Controls.Find("lbOutOfService", true).First();

                lbOutOfService.Location = new Point((Width - lbOutOfService.Width) / 2, (Height - lbOutOfService.Height) / 2);
            }
        }
    }
}
