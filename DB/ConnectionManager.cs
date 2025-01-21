namespace DB{
    public class ConnectionManager {
    private static ConnectionManager? instance;
    private ConnectionManager() { }

    public static ConnectionManager getInstance() {
        if (instance == null) {
            instance = new ConnectionManager();
        }
        return instance;
    }
    public DatabaseConnection<T> getConnection<T>(string name) {
        return new DatabaseConnection<T>(name);
    }
}
}