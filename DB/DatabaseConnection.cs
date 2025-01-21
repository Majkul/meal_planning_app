namespace DB{
public class DatabaseConnection<T> : IDatabaseConnection<T> {
    private readonly Database<T> db;

    public DatabaseConnection(string name) {
        db = Database<T>.getInstance(name);
    }

    // Dodawanie nowego rekordu
    public int AddRecord(T obj) {
        // Sprawdzanie, czy rekord o danym imieniu już istnieje
        if (db.records.Any(rec => rec.obj?.ToString() == obj?.ToString())) {
            //Console.WriteLine($"Record with name {obj?.ToString()} already exists. Skipping insertion.");
            return -1;
        }
        Record<T> newRecord = new(db.nextId++, obj);
        db.records.Add(newRecord);
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