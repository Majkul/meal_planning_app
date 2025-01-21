namespace Adapters.FileSaveAdapter
{
    public interface IFileSaveAdapter
{
    void SaveToFile(string filePath, object data);
}

}