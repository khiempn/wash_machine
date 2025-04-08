using System;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.LaundryDryerOption;
using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.PaidBy;

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
            paymentItem.PaymentCompletedCallBack += PaymentItem_PaymentCompleted;
        }

        private void RemoveRegisterEvents()
        {
            paymentItem.PaymentCompletedCallBack -= PaymentItem_PaymentCompleted;
        }

        private async void PaymentItem_PaymentCompleted(Form form, Action done)
        {
            try
            {
                RemoveRegisterEvents();
                LaundryWashOptionForm laundryWashOptionForm = (LaundryWashOptionForm)mainForm;
                if (laundryWashOptionForm.LaundryOptionItemSelected != null)
                {
                    ProgressUI progressUI = new ProgressUI();
                    progressUI.SetParent(form);
                    progressUI.Show();
                    await laundryWashOptionForm.LaundryOptionItemSelected.Start();
                    progressUI.Hide();
                    done.Invoke();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public async void Click()
        {
            try
            {
                LaundryWashOptionForm form = (LaundryWashOptionForm)mainForm;
                ProgressUI progressUI = new ProgressUI();
                progressUI.SetParent(form);
                progressUI.Show();
                form.LaundryOptionItemSelected.HealthCheckCompleted += (isHealthCheck) =>
                {
                    mainForm.BeginInvoke((MethodInvoker)async delegate
                    {
                        progressUI.Hide();
                        if ((bool)isHealthCheck)
                        {
                            paymentItem.SetAmount(form.TimeOptionItemSelected.Amount);
                            if (followType == FollowType.TestMachineWithoutPayment)
                            {
                                await form.LaundryOptionItemSelected.Start();
                            }
                            else
                            {
                                PaidByForm paidByForm = new PaidByForm(followType, paymentItem);
                                paidByForm.Show();
                                paidByForm.FormClosed += PaidByForm_FormClosed;
                            }
                        }
                        else
                        {
                            MessageBox.Show("The machine is not available right now. Please try again later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    });
                };

                await form.LaundryOptionItemSelected.HealthCheck();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Apologies for the inconvenience. This feature may not be working properly at the moment. Please try again later. Thank you for your patience!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log(ex);
            }
        }

        private void PaidByForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Close();
            mainForm.Dispose();
        }

        public Control GetTemplate()
        {
            ButtonRoundedUI btnPaymentItem = new ButtonRoundedUI()
            {
                Dock = DockStyle.Fill,
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
