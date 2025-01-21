namespace DB{
public class Record<T> {
    public int Id { get; set; }
    public T obj;

    public Record(int Id, T obj) {
        this.Id = Id;
        this.obj = obj;
    }

    public override string ToString() {
        return obj?.ToString() ?? "Brak ToStringa()";
    }
}
}