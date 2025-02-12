using System;

namespace WashMachine.Business.Models
{
    public class LogModel
    {
        public LogModel()
        {
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
