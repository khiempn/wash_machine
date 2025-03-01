using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.PaidBy;

namespace WashMachine.Forms.Modules.LaundryDryerOption.PaymentItems
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
            paymentItem.PaymentCompletedCallBack += PaymentItem_PaymentCompleted;
        }

        private async void PaymentItem_PaymentCompleted(object sender)
        {
            try
            {
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;
                if (form.LaundryOptionItemSelected != null && form.TimeOptionItemSelected != null && form.TempOptionItemSelected != null)
                {
                    ProgressUI progressUI = new ProgressUI();
                    progressUI.SetParent(mainForm);
                    progressUI.Show();

                    await form.LaundryOptionItemSelected.Start();
                    progressUI.Hide();
                    mainForm.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Click()
        {
            try
            {
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;
                if (form.LaundryOptionItemSelected != null && form.TimeOptionItemSelected != null && form.TempOptionItemSelected != null)
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
