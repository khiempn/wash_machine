using System;
using System.Collections.Generic;
using System.Text;

namespace WashMachine.Business
{

    public class Messages
    {
        public const string LoginIncorrect = "Your login information is incorrect!";
        public const string IncorectPassword = "Your password is incorrect";
        public const string NewsDateFormatVn = "dddd, dd/MM/yyyy";

        public const string ActionInvalid = "Action is invalid";
        public const string SaveSuccess = "This item has been successfully saved";
        public const string ItemExisted = "Item already exists in the system";
        public const string ItemNotExisted = "Item does not exists in the system";
        public const string DeletedSuccess = "This item has been successfully deleted";
        public const string ItemInvalid = "{0} is invalid";

        public const string SentSuccessfully = "{0} was sent successfully.";
        public const string SentUnSuccessfully = "{0} was sent successfully.";
        public const string UnSuccessVerifyEmail = "Email confirmation failed!";
        public const string EmailIsActivated = "This email has been activated";
        public const string UnSuccessFogotPassword = "Email address is required";
        public const string SuccessVerifyEmail = "Your account has been verified successfully!";
        public const string EmailNotExist = "Email does not exist";
        public const string EmailDuplicate = "Email is duplicate";
        public const string PhoneDuplicate = "Phone is duplicate";


        public const string ResetPasswordSuccess = "Reset password is successfully";
        public const string AccessDenied = "Access is denied";
        public const string RequestInvalid = "Request is invalid";
        public const string DataExisted = "{0} already exists in the system";
        public const string DataNotExisted = "{0} does not exist in the system";
        public const string UserNotFound = "Không tìm thấy user";
        public const string UserInactive = "Vui lòng truy cập email và kích hoạt tài khoản trước khi đăng nhập.";
        public const string UserDisabled = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với Quản trị viên để được hỗ trợ.";


        public const string QuotaLimited = "Your quotas is limited!";

    }
    public class MessagesVi
    {
        public const string AccessDenied = "Truy cập bị từ chối.";

        public const string LoginIncorrect = "Thông tin đăng nhập không đúng!";
        public const string IncorectPassword = "Password không đúng!";
        public const string ChangePasswordSuccess = "Đổi password thành công!";
        public const string ResetPasswordSuccess = "Đặt lại password thành công!";
        public const string NewsDateFormatVn = "dddd, dd/MM/yyyy";

        public const string RecordExisted = "This record is existed";

        public const string SaveSuccess = "Đã lưu dữ liệu thành công";
        public const string ItemExisted = "Dữ liệu không tìm thấy";
        public const string ItemNotExisted = "Bản ghi không tồn tại trong hệ thống";
        public const string DeletedSuccess = "Đã xóa dữ liệu thành công";
        public const string UpdateSuccess = "Đã cập nhập dữ liệu thành công";
        public const string ItemInvalid = "Dữ liệu không đúng";

        public const string SentSuccessfully = "{0} was sent successfully.";
        public const string SentUnSuccessfully = "{0} was sent successfully.";
        public const string UnSuccessVerifyEmail = "Email confirmation failed!";
        public const string EmailIsActivated = "This email has been activated";
        public const string UnSuccessFogotPassword = "Email address is required";
        public const string SuccessVerifyEmail = "Your account has been verified successfully!";
        public const string EmailNotExist = "Email does not exist";


        public const string PostingSuccess = "Đăng tin thành công";
        public const string DataExisted = "{0} đã tồn tại";
        public const string DataNotExisted = "{0} không tồn tại";
        public const string UserNotFound = "Không tìm thấy user";
        public const string UserInactive = "Vui lòng truy cập email và kích hoạt tài khoản trước khi đăng nhập.";
        public const string UserDisabled = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với Quản trị viên để được hỗ trợ.";


        public const string QuotaLimited = "Your quotas is limited!";

    }
}