using SQLite;

namespace MauiApp1;

public class ScanRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Mail { get; set; } = "";

    public string Tablet { get; set; } = "";

    public DateTime ScanDate { get; set; }
}