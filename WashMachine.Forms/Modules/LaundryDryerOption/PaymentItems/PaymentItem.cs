﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WashMachine.Forms.Common.UI;
using WashMachine.Forms.Modules.Login;
using WashMachine.Forms.Modules.PaidBy;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

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
            paymentItem.OnPaymentCompleted += PaymentItem_PaymentCompleted;
        }

        private void RemoveRegisterEvents()
        {
            paymentItem.OnPaymentCompleted -= PaymentItem_PaymentCompleted;
        }

        private async void PaymentItem_PaymentCompleted(Form form, Action done, OrderModel order)
        {
            try
            {
                RemoveRegisterEvents();
                LaundryDryerOptionForm laundryDryerOptionForm = (LaundryDryerOptionForm)mainForm;
                if (laundryDryerOptionForm.LaundryOptionItemSelected != null && laundryDryerOptionForm.TimeOptionItemSelected != null && laundryDryerOptionForm.TempOptionItemSelected != null)
                {
                    ProgressUI progressUI = new ProgressUI();
                    progressUI.SetParent(form);
                    progressUI.Show();
                    await laundryDryerOptionForm.LaundryOptionItemSelected.Start(order);
                    progressUI.Hide();
                    done.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public async void Click()
        {
            try
            {
                LaundryDryerOptionForm form = (LaundryDryerOptionForm)mainForm;
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
                                await form.LaundryOptionItemSelected.Start(new OrderModel()
                                {
                                    Amount = form.TimeOptionItemSelected.Amount,
                                    DeviceId = "Test-Device"
                                });
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
                Text = "付款 Payment",
                ShapeBackgroudColor = ColorTranslator.FromHtml("#3a7d22"),
                ShapeBorderColor = Color.Black,
                CornerRadius = 30,
                ForeColor = Color.White,
                Dock = DockStyle.Fill
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
