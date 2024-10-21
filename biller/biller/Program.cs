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
            Console.WriteLine($"{i + 1}. {people[i].Name}");
        }

        input = GetString("enter name: ");
        if (input == "done") { break; }
        else
        {
            people.Add(new Person(input));
        }
    }
}

void PopulateItems()
{
    while (true)
    {
        string input;

        Console.Clear();
        Console.WriteLine("enter the starting items and their prices (enter 'done' when done)\nif you don't specify anyone, everyone will be involved");

        for ( int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name} {items[i].Value}");
        }

        input = GetString("enter the item's name, price, and users"); // maybe add a loop that forces the user to input correctly
        
        // Breaks the loop if the user enters 'done'
        if (input == "done") { break; }
        else
        {
            // Splitting the input into an array of words to adress possible
            string[] words = input.Split();
            
            // Creates a new item with the current information and adds it to the list of items
            Item newItem = new(words[0], Convert.ToSingle(words[1]));
            items.Add(newItem);

            // If no person is specified, links everyone to the current item
            if (words.Length == 2)
            {
                for (int i = 0; i < people.Count; i++)
                {
                    links.Add(new(people[i], newItem));
                }
            }

            // If more than 2 command line arguments are provided, links the item only to the specified people
            else if (words.Length > 2)
            {
                // Loops starting from the third command line
                for (int i = 2; i < words.Length; i++)
                {
                    links.Add(new(GetPerson(words[i]), newItem));
                }
            }
        }
    }
}

string GetString(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine();
}

float GetFloat(string prompt)
{
    Console.Write(prompt);
    return Convert.ToSingle(Console.ReadLine());
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

    public Item(string name, float value)
    {
        Name = name;
        Value = value;
    }
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

    public override string ToString()
    {
        return $"{Target} bought {Item}";
    }
}