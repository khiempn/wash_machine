using System;

namespace WashMachine.Repositories.Entities
{
    public class Log
    {
        public Log()
        {
        }

        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
