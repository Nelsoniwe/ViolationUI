namespace UI.Models;

public class FileModel
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public byte[] data { get; set; }
    public string Hash { get; set; }
}