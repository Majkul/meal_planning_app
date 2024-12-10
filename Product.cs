public class Product {
    public enum Categories { Fruit, Vegetable, Meat, Dairy, Grain, Other }
    public enum Units { Grams, Milliliters, Units }
    public string Name { get; set; }
    public Categories Category { get; set; }
    public Units Unit { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }

    public Product(string name, Units unit, Categories category, double protein, double fat, double carbohydrates) {
        Name = name;
        Unit = unit;
        Category = category;
        Protein = protein;
        Fat = fat;
        Carbohydrates = carbohydrates;
    }
}
