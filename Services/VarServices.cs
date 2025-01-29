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
}
}