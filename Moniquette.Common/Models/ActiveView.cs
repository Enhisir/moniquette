namespace Moniquette.Common.Models;

public class ActiveView
{
    public long Id { get; set; }
    public int Pid { get; set; }
    public string Class { get; set; } = null!;
    public string Title { get; set; } = null!;
}
