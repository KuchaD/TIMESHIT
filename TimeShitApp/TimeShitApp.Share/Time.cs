namespace TimeShitApp.Share;

public class Time
{
    public double Hours { get; set; }
    public DateTime Date { get; set; }
    public string Project { get; set; }
    public string Task { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
}