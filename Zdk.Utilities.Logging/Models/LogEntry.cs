using Zdk.Utilities.DbEntityClasses.Models.Base;

namespace Zdk.Utilities.Logging.Models;

public class LogEntry : Timestamped
{
    public string Text { get; set; }
    public int Level { get; set; }
    public int Type { get; set; }
    public string Source { get; set; }
    public DateTime Time { get; set; }
}
