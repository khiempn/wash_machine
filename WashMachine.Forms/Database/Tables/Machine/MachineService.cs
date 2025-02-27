using Microsoft.SqlServer.Server;
using System;

namespace WashMachine.Forms.Database.Tables.Machine
{
    public class MachineService
    {
        public MachineService() { }

        public static string GetRemainTimeAsFormat(MachineModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.StartAt) && !string.IsNullOrWhiteSpace(model.EndAt))
            {
                DateTime endAt = new DateTime(long.Parse(model.EndAt));
                TimeSpan remainAt = endAt - DateTime.Now;
                return remainAt.ToString(@"hh\:mm\:ss");
            }

            return string.Empty;
        }

        public static bool IsRunCompleted(MachineModel model)
        {
            DateTime endAt = new DateTime(long.Parse(model.EndAt));
            DateTime currentAt = DateTime.Now;
            return currentAt.CompareTo(endAt) > 0;
        }

    }
}
