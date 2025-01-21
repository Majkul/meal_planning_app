namespace Adapters.FileSaveAdapter{
public class FileSaveManager
{
    public void SaveToFile(string filePath, object data)
    {
        IFileSaveAdapter adapter = GetAdapter(filePath);
        adapter.SaveToFile(filePath, data);
    }

    private IFileSaveAdapter GetAdapter(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".json" => new JsonFileSaveAdapter(),
            ".txt" => new TxtFileSaveAdapter(),
            _ => throw new NotSupportedException($"File format '{extension}' is not supported."),
        };
    }
}
}