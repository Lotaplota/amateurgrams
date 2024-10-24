internal class Program
{
    static List<Person> people = [];
    static List<Link> links = [];
    static List<Item> items = [];

    static void Main()
    {
        PopulatePeople();
        PopulateItems();
        GetMainInput();
    }

    // Shows the user a list of available options, then prompts them for an input
    static void GetMainInput()
    {
        // Console.Clear();

        Console.WriteLine("MAIN MENU\n" +
        "1. list people, items, or links\n" +
        "(TEST) 2. describe and update person or item\n" +
        "3. edit people or item list\n" +
        "exit. quits the application\n");

        string? input = "start";
        
        while (input != "exit")
        {
            input = GetString("Enter a number: ");

            IBranch command = input switch
            {
                "1" => new ListBranch(),
                "2" => new AccessBranch(),
                "3" => new EditListBranch(),
                _ => new VoidBranch()
                // New commands go here
            };

            // if the input is not valid, creates a valid branch to repeat the loop    
            if (command.GetType() == typeof(VoidBranch))
            {
                if (input == "exit") { } // this looks ugly
                else
                {
                Console.WriteLine("Invalid input");
                }
            }
            else
            {
                command.Go();
                break;
            }
        }
    }

    static void PopulatePeople()
    {
        string input;

        // Console.Clear(); DONKEY

        input = GetString("enter each user's name: ");
        string[] words = input.Split();

        for (int i = 0; i < words.Length; i++)
        {
            Console.WriteLine($"Added {words[i]}");
            people.Add(new Person(words[i]));
        }
    }

    static void PopulateItems()
    {
        while (true)
        {
            string input;

            // Console.Clear(); DONKEY
            Console.WriteLine("enter the starting items and their prices (enter 'done' when done)\n" +
            "if you don't specify anyone, everyone will contribute");

            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].Name} {items[i].Price}");
            }

            input = GetString("enter the item's name, price, and users: "); // MAYBE add a loop that forces the user to input correctly

            // Breaks the loop if the user enters 'done'
            if (input == "done") { break; }
            else
            {
                // Splitting the input into an array of words to adress possible interpretations
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
                    // Loops starting from the third command line argument
                    for (int i = 2; i < words.Length; i++)
                    {
                        links.Add(new(GetPerson(words[i]), newItem));
                    }
                }
            }
        }
    }

    // Adds a new item to the list of items
    // Links that item to everyone on the list if nobody else is specified
    void AddItem()
    {
        // Prompts for the new item name and price, then separates the input into an array
        string? input = GetString("name the item and its price\n" +
        "if you don't specify anyone, everyone will contribute\n");
        string[] words = input.Split();

        // Creates a new item using the given information and adds it to the list
        Item newItem = new(words[0], Convert.ToSingle(words[1]));
        items.Add(newItem);

        // If 2 arguments are given, links the item to everyone
        if (words.Length == 2)
        {
            for (int i = 0; i < people.Count; i++)
            {
                links.Add(new(people[i], newItem));
            }
        }
        // If more than 2 command line arguments are given, links the item to the specified people
        // starting from the 3rd command line argument
        else
        {
            for (int i = 2; i < words.Length; i++)
            {
                links.Add(new(GetPerson(words[i]), newItem));
            }
        }
    }

    static void AddPerson(string name)
    {
        Person newPerson = new(name);

        // Adds the person if not already on the list
        if (GetPerson(name) == null)
        {
            Console.WriteLine($"{name} was added to the list"); // DEBUG PRINT
            people.Add(newPerson);
        }
        // Warns the user if the person is already in the list and returns from the method
        else
        {
            Console.WriteLine($"{name} is already on the list!");
            return;
        }
        
        // Links the person to all of the items in the list
        foreach (Item item in items)
        {
            Console.WriteLine($"{newPerson.Name} contributes to {item.Name}");
            links.Add(new(newPerson, item));
        }
    }

    static void RemovePerson(string name)
    {
        // Retrieves the person to be removed if name matches
        Person removee = GetPerson(name);

        // If the person returned was null, warns the user that the person's name is not in the list and returns from the method
        if (removee == null)
        {
            Console.WriteLine($"can't remove {name} because they're not on the list!"); // DEBUG PRINT
            return;
        }
        else
        {
            people.Remove(removee);
        }

        // Loops through the list of links in reverse order, removing the link if the name matches
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if (links[i].Contributor.Name == name)
            {
                Console.WriteLine($"{links[i]} removed");
                links.Remove(links[i]);
            }
        }
    }

    static void RemoveItem(string name)
    {
        // Retrieves the person to be removed if name matches
        Item removee = GetItem(name);

        // If the person returned was null, warns the user that the person's name is not in the list and returns from the method
        if (removee == null)
        {
            Console.WriteLine($"can't remove {name} because it's not on the list!"); // DEBUG PRINT
            return;
        }
        else
        {
            items.Remove(removee);
        }

        // Loops through the list of links in reverse order, removing the link if the item name matches
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if (links[i].Item.Name == name)
            {
                Console.WriteLine($"{links[i]} removed");
                links.Remove(links[i]);
            }
        }
    }

    // Edits the list of people by comparing the names to user input
    // If the entered person is already on the list, removes the person
    // If the entered person is not on the list, adds the person
    static void EditPersonList()
    {
        // Prints, in one line, the name of each person on the list
        Console.WriteLine("People on the list: ");
        foreach (var person in people)
        {
            Console.Write(person.Name + " ");
        }
        Console.WriteLine("\nNow enter a list of people. People who match the names in the list will be removed. People that aren't on the list will be added");

        string input = Console.ReadLine();
        string[] names = input.Split();

        foreach (string name in names)
        {
            // Removes person if name is already on the list, adds if not on the list
            if (GetPerson(name) == null)
            {
                AddPerson(name);
            }
            else
            {
                RemovePerson(name);
            }
        }
    }

    static void EditItemList()
    {
        // TODO
    }

        // Calculates the values for a given item taking into account how many candidates are contributing
    float ShareOf(Item item)
    {
        int buyers = 0;

        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].Item.Name == item.Name)
            {
                buyers++;
            }
        }

        // Returns the calculated value with two decimal places, rounded up
        return (float)Math.Round(item.Price / buyers, 2, MidpointRounding.ToPositiveInfinity);
    }

    void DisplayInfo(Person person)
    {
        float debt = 0;

        Console.WriteLine($"{person.Name} bought: ");
        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].Contributor.Name == person.Name)
            {
                Console.WriteLine(links[i].Item.Name + " for " + ShareOf(links[i].Item));
                debt += ShareOf(links[i].Item);
            }
        }
        Console.WriteLine("owing the total amount of " + Math.Round(debt, 2, MidpointRounding.ToPositiveInfinity));
    }

    void DisplayAllInfo()
    {
        for (int i = 0; i < people.Count; i++)
        {
            Person currentPerson = people[i];
            List<Item> theirItems = GetItemsFrom(people[i]);

            float owedValue = 0;

            // Sums the value of all items the current person contributes
            for (int j = 0; j < theirItems.Count; j++)
            {
                owedValue += ShareOf(theirItems[j]);
            }

            Console.WriteLine($"{currentPerson.Name} owes {owedValue}");
        }
    }

    static List<Item> GetItemsFrom(Person person)
    {
        List<Item> theirItems = [];

        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].Contributor.Name == person.Name)
            {
                theirItems.Add(links[i].Item);
            }
        }

        return theirItems;
    }

    // Searches a name in the people list and returns the Person
    static Person GetPerson(string name)
    {
        foreach (Person person in people)
        {
            if (person.Name == name)
            {
                return person;
            }
        }

        // Returns null if no person is found
        return null;
    }

    static Item GetItem(string name)
    {
        foreach (Item item in items)
        {
            if (item.Name == name)
            {
                return item;
            }
        }

        // Returns null if no item is found
        return null;
    }

    static string? GetString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    float GetFloat(string prompt)
    {
        Console.Write(prompt);
        return Convert.ToSingle(Console.ReadLine());
    }

    private static void ListPeople()
    {
        for (int i = 0; i < people.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {people[i].Name}");
        }
    }

    static void UpdatePerson(string name)
    {
        // Gets the person with that name and their items
        Person person = GetPerson(name);
        List<Item> theirItems = GetItemsFrom(person);

        // Prints the items the person currently contributes to <-- WHOA LOOK AT THIS
        Console.WriteLine("This person currently contributes to: "); // °o°
        foreach (Item item in theirItems)
        {
            Console.WriteLine($"{item.Name}");
        }

        string input = GetString("Type in the items you want to add/remove, or [cancel]: ");
        string[] words = input.Split();

        if (input == "cancel")
        {
            return;
        }

        foreach (string word in words)
        {
            if (GetItem(word) == null)
            {
                Console.WriteLine($"{word} is not a valid item");
            }
            else
            {
                for (int i = links.Count - 1; i >= 0; i--)
                {
                    if (links[i].Contributor.Name == person.Name)
                    {
                        links.Remove(links[i]);
                    }
                }
            }
        }
    }

    static void UpdateItem()
    {
        // TODO
    }

    static void ListItems()
    {
        // Initializing variable to store the amount of letters in the biggest item name
        int maxLength = 0; 

        foreach (var item in items)
        {
            if (item.Name.Length > maxLength)
            {
                maxLength = item.Name.Length;
            }
        }

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Name.PadRight(maxLength + 4)}{item.Price}");
        }
    }

    public interface IBranch
    {
        public abstract void Go();
    }

    public class ListBranch : IBranch
    {
        public void Go()
        {
            // Console.Clear();

            Console.Write("Display the list of\n" +
            "1. People\n" +
            "2. Items\n" +
            "Choose one of the options: ");

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == "1" || input.ToLower() == "people")
                {
                    ListPeople();
                    break;
                }
                else if (input == "2" || input.ToLower() == "items")
                {
                    ListItems();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }


    public class AccessBranch : IBranch
    {
        public void Go()
        {
            Console.WriteLine("What would you like to access?\n" +
            "1. (TEST) Person\n" +
            "2. (NOT WORKING) Item");

            while (true)
            {
                string input = GetString("Choose an option (name or number): ");

                IBranch branch = input switch
                {
                    "1" => new AccessPersonBranch(),
                    "2" => new AccessItemBranch(),
                    _ => new VoidBranch()
                };

                if (branch.GetType() == typeof(VoidBranch))
                {
                    Console.WriteLine("Invalid input");
                }
                else
                {
                    branch.Go();
                    break;
                }
            }
        }
    }

    public class AccessPersonBranch : IBranch
    {
        public void Go()
        {
            // Prints the list of people
            Console.WriteLine("List of people:");
            ListPeople();

            // Prompts the user to input a person's name to access
            while (true)
            {
                string input = GetString("Type a name to access, [done] to go back: ");

                if (input == "done")
                {
                    break;
                }

                if (GetPerson(input) == null)
                {
                    Console.WriteLine("Invalid input");
                }
                else
                {
                    UpdatePerson(input);
                }
            }
        }
    }

    public class AccessItemBranch : IBranch
    {
        public void Go()
        {
            // TODO
        }
    }

    public class UpdateBranch : IBranch
    {
        public void Go()
        {
            // MAYBE this is useless
        }
    }

    public class EditListBranch : IBranch
    {
        public void Go()
        {
            // Console.Clear();

            Console.Write("Edit the list of\n" +
            "1. People\n" +
            "2. Items\n" +
            "Choose one of the options: ");

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == "1" || input.ToLower() == "people")
                {
                    EditPersonList();
                    break;
                }
                else if (input == "2" || input.ToLower() == "items")
                {
                    EditItemList();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }

    public class VoidBranch : IBranch
    {
        public void Go()
        {
            Console.WriteLine("e a s t e r   e g g");
        }
    }

    public class Person
    {
        public string Name {get; init;}

        public Person(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Item
    {
        public string? Name {get; init;}
        public float Price {get; init;}

        public Item(string name, float price)
        {
            Name = name;
            Price = price;
        }
    }

    public class Link
    {
        public Person Contributor { get; init;}
        public Item Item {get; init;}

        public Link(Person contributor, Item item)
        {
            Contributor = contributor;
            Item = item;
        }

        public override string ToString()
        {
            return $"{Contributor.Name} bought {Item.Name} for {Item.Price}";
        }
    }
}