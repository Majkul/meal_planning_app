namespace Recipe.Commands{
public interface ICommand
{
    void Execute();
    void Undo();
}

}