namespace DB{
public class Database<T> {
    private static Dictionary<string, Database<T>> instances = new();

    public readonly List<Record<T>> records; // Lista przechowująca rekordy
    public int nextId = 1; // Licznik do generowania unikalnych ID
    public int dbId = 0;

    private Database() {
        this.dbId = nextId++;
        records = new List<Record<T>>();
    }

    public static Database<T> getInstance(string key) {
        if (!instances.ContainsKey(key)) {
            // init multiton instance
            instances.Add(key, new Database<T>());
        }
        return instances[key];
    }
}
}