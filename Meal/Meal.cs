namespace Meal{
public class Meal {
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public MealType Type { get; set; }
    public List<Recipe.Recipe> Recipes { get; set; }
    public enum MealType {
        Breakfast,
        SnackI,
        Lunch,
        SnackII,
        Dinner
    }
    public Meal(){
        Name = "";
        Recipes = new List<Recipe.Recipe>();
    }
    public class MealMemento {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public MealType Type { get; set; }
        public List<Recipe.Recipe> Recipes { get; set; }

        public MealMemento() {
            Name = "";
            Recipes = new List<Recipe.Recipe>();
        }
    }

    public MealMemento Save() {
        return new MealMemento {
            Name = Name,
            Date = Date,
            Type = Type,
            Recipes = Recipes
        };
    }

    public void Restore(MealMemento memento) {
        Name = memento.Name;
        Date = memento.Date;
        Type = memento.Type;
        Recipes = memento.Recipes;
    }
}
}