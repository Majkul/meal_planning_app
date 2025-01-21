namespace Adapters.FileSaveAdapter{
public class TxtFileSaveAdapter : IFileSaveAdapter
{
    public void SaveToFile(string filePath, object data)
    {
        var text = data.ToString();
        File.WriteAllText(filePath, text);
    }
}
}