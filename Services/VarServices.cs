namespace Services{
public static class VarServices{
    public static double GetDoubleFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToDouble(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa liczba. Spróbuj ponownie.\n");
            }
        }
    }

    public static int GetIntFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToInt32(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa liczba. Spróbuj ponownie.\n");
            }
        }
    }

    public static DateTime GetDateFromString(string message) {
        while (true) {
            Console.WriteLine(message);
            try {
                return Convert.ToDateTime(Console.ReadLine());
            } catch (FormatException) {
                Console.WriteLine("Nieprawidłowa data. Spróbuj ponownie.\n");
            }
        }
    }

    public static T GetEnumFromString<T>(string message) where T : Enum {
        while (true) {
            Console.WriteLine(message);
            try {
                return (T)Enum.Parse(typeof(T), Console.ReadLine()!, true);
            } catch (ArgumentException) {
                Console.WriteLine("Nieprawidłowe dane. Spróbuj ponownie.\n");
            }
        }
    }
}
}