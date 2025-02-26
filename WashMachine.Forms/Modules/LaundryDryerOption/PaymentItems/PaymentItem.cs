using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;

namespace WashMachine.Forms.Modules.LaundryDryerOption.PaymentItems
{
    public class PaymentItem : IPaymentItem
    {
        Form mainForm;

        public PaymentItem(Form form)
        {
            mainForm = form;
        }

        public async void Click()
        {
            try
            {
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;
                if (form.LaundryOptionItemSelected != null)
                {
                    ProgressUI progressUI = new ProgressUI();
                    progressUI.SetParent(mainForm);
                    progressUI.Show();

                    await form.LaundryOptionItemSelected.Start();
                    progressUI.Hide();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btnPaymentItem = new ButtonRoundedUI()
            {
                Height = 65,
                Width = 150,
                Text = "付款 Payment",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#3a7d22"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                ForeColor = Color.White
            };
            btnPaymentItem.Click += BtnPaymentItem_Click;
            return btnPaymentItem;
        }

        private void BtnPaymentItem_Click(object sender, EventArgs e)
        {
            Click();
        }
    }
}
