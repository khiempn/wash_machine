namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus.Email
{
    public interface IEmailService
    {
        void SendGenerationError(string deviceId);
        void SendDisconnectError();
        void SendUploadFileError();
        void SendDownloadError();
    }
}
