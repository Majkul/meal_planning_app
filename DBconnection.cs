using System.Data;
namespace DBconnection{
    
public class ConnectionManager{
    private static ConnectionManager? instance;
    //private static ObjectPool Pooler = new ObjectPool();

    private ConnectionManager(){

    }
    public static ConnectionManager getInstance(){
        if(instance == null){
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
    public DatabaseConnection getConnection(string name){
        return new DatabaseConnection(name);
    }

}


public interface IDatabaseConnection {
    int AddRecord(Object obj);

    void UpdateRecord(int id, Object obj);

    void DeleteRecord(int id);

    Record<Object>? GetRecord(int id);

    void ShowAllRecords();
}

// Prosty rekord w bazie danych
public class Record<Object> {
    public int Id { get; set; }

    public Object obj;

    public Record(int Id,Object obj){
        this.Id = Id;
        this.obj = obj;
    }

    public override string ToString() {
        return obj.ToString() ?? "Brak ToStringa()";
    }
}

// Prosta baza danych
public class Database {

    private static Dictionary<String,Database> instances = new Dictionary<string, Database>();
    
    public readonly List<Record<Object>> records; // Lista przechowująca rekordy
    // WARN: CAN HOLD MULTIPLE TYPES, THERE IS NO DEFENSE MECHANISM AGAINST THIS.
    public int nextId = 1; // Licznik do generowania unikalnych ID
    public int dbId = 0;
    private Database() {
        this.dbId = nextId++;
        records = new List<Record<Object>>();
    }

    public static Database getInstance(string key){
        if(! instances.ContainsKey(key)){
            //init multiton inst
            instances.Add(key,new Database());
            
        }
        return instances[key];
        
    }
}
public class DatabaseConnection : IDatabaseConnection {
        private readonly Database db;

        public DatabaseConnection(string name) {
            db = Database.getInstance(name);
        }

        // Dodawanie nowego rekordu
        public int AddRecord(Object obj) {
            Record<Object> newRecord = new(db.nextId++, obj);
            db.records.Add(newRecord);
            Console.WriteLine($"Inserted: {newRecord}");
            return db.nextId - 1; // zwracamy id dodanego rekordu
        }

        // Pobieranie rekordu po ID
        public Record<Object>? GetRecord(int id) {
            return db.records.Where(rec => rec.Id == id).FirstOrDefault();
        }

        // Aktualizowanie rekordu po ID
        public void UpdateRecord(int id, object obj) {
            Record<Object>? optionalRecord = GetRecord(id);

            if (optionalRecord != null) {
                Record<Object> record = optionalRecord;
                record.obj = obj;
                Console.WriteLine($"Updated: {record}");
            } else {
                Console.WriteLine($"Record with ID {id} not found.");
            }
        }

        // Usuwanie rekordu po ID
        public void DeleteRecord(int id) {
            Record<Object>? optionalRecord = GetRecord(id);

            if (optionalRecord != null) {
                db.records.Remove(optionalRecord);
                Console.WriteLine($"Deleted record with ID {id}");
            } else {
                Console.WriteLine($"Record with ID {id} not found.");
            }
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
class AbstractProd{

    public override string ToString()
    {
        return "AHAHAH";
    }

}

// public class Ztp01 {
//     public static void Main(string[] args) {

//         ConnectionManager connectionManager = ConnectionManager.getInstance();

//         // Uzyskanie połączenia do Baz Danych

//         DatabaseConnection connection1 = connectionManager.getConnection("Produkty");
        

//         connection1.AddRecord(new AbstractProd());

//         connection1.ShowAllRecords();

//         //It just works, somehow

//     }
// }
}