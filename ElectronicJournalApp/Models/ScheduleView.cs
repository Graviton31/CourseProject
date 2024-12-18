namespace ElectronicJournalApp.Models
{
    public class ScheduleView
    {
        public sbyte? WeekDay { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int IdGroup { get; set; }
        public string GroupName { get; set; } = null!;
        public string Classroom { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
    }
}
