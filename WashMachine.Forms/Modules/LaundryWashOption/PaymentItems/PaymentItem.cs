using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.LaundryDryerOption.PaymentItems;
using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.PaidBy;
using WashMachine.Forms.Modules.Payment;

namespace WashMachine.Forms.Modules.LaundryWashOption.PaymentItems
{
    public class PaymentItem : IPaymentItem
    {
        Form mainForm;
        Payment.IPaymentItem paymentItem;
        FollowType followType;

        public PaymentItem(FollowType followType, Form form)
        {
            mainForm = form;
            this.followType = followType;
            paymentItem = new Payment.PaymentItems.HkdPaymentItem();
            paymentItem.PaymentCompleted += PaymentItem_PaymentCompleted; 
        }

        private async void PaymentItem_PaymentCompleted(object sender, EventArgs e)
        {
            LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;
            if (form.LaundryOptionItemSelected != null)
            {
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(mainForm);
                progressUI.Show();

                await form.LaundryOptionItemSelected.Start();
                progressUI.Hide();
                mainForm.Close();
            }
        }

        public void Click()
        {
            try
            {
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;
                if (form.LaundryOptionItemSelected != null && form.TimeOptionItemSelected != null)
                {
                    paymentItem.SetAmount(form.TimeOptionItemSelected.Amount);

                    PaidByForm paidByForm = new PaidByForm(followType, paymentItem);
                    paidByForm.Show();
                    paidByForm.FormClosed += PaidByForm_FormClosed;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void PaidByForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Close();
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
