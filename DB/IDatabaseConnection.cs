namespace DB{
    public interface IDatabaseConnection<T> {
    int AddRecord(T obj);

    void UpdateRecord(int id, T obj);

    void DeleteRecord(int id);

    Record<T>? GetRecordById(int id);

    Record<T>? GetRecordByName(string name);

    List<Record<T>> GetAllRecords();
    T? GetLastRecord();

    void ShowAllRecords();
}
}