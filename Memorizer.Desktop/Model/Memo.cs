using System;

namespace Memorizer.Model
{
    public class Memo
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime RepeatDate { get; set; }
        public int PostponeLevel { get; set; }
        public int Scores { get; set; }
    }
}
