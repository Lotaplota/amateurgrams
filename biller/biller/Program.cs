List<Person> people = new List<Person>();
List<Link> links = new List<Link>();
List<Item> items = new List<Item>();

PopulatePeople(); // DEBUG
for (int i = 0; i < people.Count; i++)
{
    Console.WriteLine(people[i].Name);
}

while (true)
{
    // GetMainInput();
}

// Shows the user a list of options
void GetMainInput()
{
    while (true)
    {
        Console.WriteLine("What would you like to do? (enter a number)\n" + 
        "1. list people" +
        "2. list items" +
        "5. describe person" +
        "6. describe item" +
        "7. update person" +
        "8. update item" +
        "3. add person" +
        "4. add item");
    }
}

void PopulatePeople()
{
    while (true)
    {
        string input;

        Console.Clear();
        Console.WriteLine("enter the names of the people (enter 'done' when done)");
        
        for (int i = 0; i < people.Count; i++)
        {
            Console.WriteLine($"{i}. {people[i].Name}");
        }

        input = GetString("enter name: ");
        if (input == "done") { break; }
        else
        {
            people.Add(new Person(input));
        }
    }
}

string GetString(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine();
}

void DisplayPersonInfo(string name)
{
    Person person = GetPerson(name);
    float debt = 0;
    
    Console.WriteLine($"{person.Name} bought: ");
    for (int i = 0; i < links.Count; i++)
    {
        if (links[i].Target.Name == name)
        {
            Console.WriteLine(links[i].Item.Name + " for " + links[i].Item.Value);
            debt += links[i].Item.Value;
        }
    }
    Console.WriteLine("owing the total amount of " + debt);
}

Person GetPerson(string name)
{
    for (int i = 0; i < people.Count; i++)
    {
        if (people[i].Name == name)
        {
            return people[i];
        }
        else
        {
            return null;
        }
    }

    return null;
}

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