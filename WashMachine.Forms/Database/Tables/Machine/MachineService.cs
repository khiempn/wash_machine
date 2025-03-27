using System;
using System.Collections.Generic;

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

        public static string GetStartAtFormat(MachineModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.StartAt))
            {
                DateTime startAt = new DateTime(long.Parse(model.StartAt));
                return startAt.ToString(@"hh\:mm tt");
            }

            return string.Empty;
        }

        public static bool IsRunCompleted(MachineModel model)
        {
            DateTime endAt = new DateTime(long.Parse(model.EndAt));
            DateTime currentAt = DateTime.Now;
            return currentAt.CompareTo(endAt) > 0;
        }

        public static string GetTempName(int typeId)
        {
            Dictionary<int, string> temps = new Dictionary<int, string>
            {
                { 1, "Low" },
                { 2, "Medium" },
                { 3, "High" },
            };
            temps.TryGetValue(typeId, out string tempName);
            return tempName;
        }
    }
}
