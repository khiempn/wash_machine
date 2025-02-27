namespace WashMachine.Forms.Database.Tables.Machine
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
        public int Time {  get; set; }
        public int Temp { get; set; }
        public int IsRunning { get; set; }
    }
}
