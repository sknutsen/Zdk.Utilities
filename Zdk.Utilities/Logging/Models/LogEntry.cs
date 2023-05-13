using Zdk.Utilities.DbEntityClasses;

namespace Zdk.Utilities.Logging;

public class LogEntry : Timestamped
{
    public string Text { get; set; }
    public int Level { get; set; }
    public int Type { get; set; }
    public string Source { get; set; }
    public DateTime Time { get; set; }
}
