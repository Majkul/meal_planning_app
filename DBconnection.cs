using System.Data;
using System.Text.Json;
namespace DBconnection {
    
public class ConnectionManager {
    private static ConnectionManager? instance;
    private ConnectionManager() { }

    public static ConnectionManager getInstance() {
        if (instance == null) {
            instance = new ConnectionManager();
        }
        return instance;
    }

    /// <summary>
    /// Creates or gets instance of a table inside the DB
    /// Param is the name of said table
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public DatabaseConnection<T> getConnection<T>(string name) {
        return new DatabaseConnection<T>(name);
    }
}

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

// Prosty rekord w bazie danych
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

// Prosta baza danych
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

public class DatabaseConnection<T> : IDatabaseConnection<T> {
    private readonly Database<T> db;

    public DatabaseConnection(string name) {
        db = Database<T>.getInstance(name);
    }

    // Dodawanie nowego rekordu
    public int AddRecord(T obj) {
        Record<T> newRecord = new(db.nextId++, obj);
        db.records.Add(newRecord);
        //Console.WriteLine($"Inserted: {newRecord}");
        return db.nextId - 1; // Zwracamy id dodanego rekordu
    }

    // Pobieranie rekordu po ID
    public Record<T>? GetRecordById(int id) {
        return db.records.Where(rec => rec.Id == id).FirstOrDefault();
    }

    // Pobieranie rekordu po nazwie
    public Record<T>? GetRecordByName(string name) {
        return db.records.FirstOrDefault(rec => rec.obj?.ToString() == name); // Wymaga, aby T miało poprawne ToString()
    }

    // Aktualizowanie rekordu po ID
    public void UpdateRecord(int id, T obj) {
        Record<T>? optionalRecord = GetRecordById(id);

        if (optionalRecord != null) {
            Record<T> record = optionalRecord;
            record.obj = obj;
            Console.WriteLine($"Updated: {record}");
        } else {
            Console.WriteLine($"Record with ID {id} not found.");
        }
    }

    // Usuwanie rekordu po ID
    public void DeleteRecord(int id) {
        Record<T>? optionalRecord = GetRecordById(id);

        if (optionalRecord != null) {
            db.records.Remove(optionalRecord);
            Console.WriteLine($"Deleted record with ID {id}");
        } else {
            Console.WriteLine($"Record with ID {id} not found.");
        }
    }

    // Pobieranie wszystkich rekordów
    public List<Record<T>> GetAllRecords() {
        return db.records;
    }
    // Pobieranie ostatniego rekordu
    public T? GetLastRecord() {
        var record = db.records.LastOrDefault();
        return record != null ? record.obj : default;
    }
    
    // Wyświetlanie wszystkich rekordów
    public void ShowAllRecords() {
        if (db.records.Any()) {
            Console.WriteLine("All records:");
            db.records.ForEach(r => Console.WriteLine(r));
        } else {
            Console.WriteLine("No records in the database.");
        }
    }
}
}
