List<Person> people = new List<Person>();
List<Link> links = new List<Link>();

public interface ICommand
{
    public abstract void Run();
}

public class Person
{
    public string Name {get;}

    public Person(string name)
    {
        Name = name;
    }
}

public class Item
{
    public string? Name {get; private set;}
    public float Value {get; private set;}
}

public class Link
{
    public Person Target { get; init;}
    public Item Item {get; init;}

    public Link(Person target, Item item)
    {
        Target = target;
        Item = item;
    }
}