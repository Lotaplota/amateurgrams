internal class Program
{
    static List<Person> people = [];
    static List<Link> links = [];
    static List<Item> items = [];

    static void Main()
    {
        PopulatePeople();
        AddItems();
        // PopulateItems();
        MainMenu();
    }

    // Shows the user a list of available options, then prompts them for an input
    static void MainMenu()
    {
        string? input = "start";
        
        while (input != "exit")
        {
            Clear();
            
            // Prints the main menu, showing the user all of his options
            Console.WriteLine("MAIN MENU\n"
            + "1. Display the lists of people, items, or links (DONE)\n"
            + "2. edit people or item list\n"
            + "3. change data of a person or item (TEST)\n"
            + "4. Print all info (DONE)\n"
            + "exit. quits the application\n");

            input = GetString("Enter a 'number': ");

            IBranch command = input switch
            {
                "1" => new ListBranch(),
                "2" => new EditListBranch(),
                "3" => new ChangeBranch(),
                "4" => new InfoBranch(),
                // New commands go here
                _ => new VoidBranch()
            };

            // if the input is not valid, creates a void branch to repeat the loop    
            if (command.GetType() == typeof(VoidBranch))
            {
                if (input == "exit")
                {
                    Console.WriteLine("Goodbye!");
                }
                else
                {
                    BadPrompt("Invalid input");
                }
            }
            // Enters the chosen branch
            else
            {
                command.Go();
            }
        }
    }

    static void PopulatePeople()
    {
        Clear();

        string? input;

        while (true)
        {
            input = GetString("enter each user's name: ");

            if (input == null || input == "")
            {
                BadPrompt("Invalid input");
            }
            else
            {
                break;
            }
        }

        string[] words = input!.Split();
        foreach (string word in words)
        {
            people.Add(new Person(word));
        }
    }

    static void AddPerson(string name)
    {
        Person newPerson = new(name);

        // Adds the person if not already on the list
        if (GetPerson(name) == null)
        {
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
            links.Add(new(newPerson, item));
        }
    }

    // Prompts the user to add items to the list, asking for a tag and a price
    // Can link the new item to everyone at once or just to the specified people
    static void AddItems()
    {
        Clear();

        string? input;

        while (true)
        {
            ListItems();
            Item newItem;
            // float itemPrice = 0; inlining? DONKEY

            // Prompts for the new item name and price, then separates the input into an array
            input = GetString("Enter the item's prices and contributors\n"
                + "If you don't specify anyone, everyone will contribute\n"
                + "Enter 'name price [person...]' or 'done' to continue\n");
            string[] words = input!.Split();

            // Returns if user chooses to
            if (input == "done" || input == "")
            {
                return;
            }

            // Checking if the number of arguments is bigger than 2
            if (words.Length < 2)
            {
                BadPrompt("Argument must contain at least 2 words");
                continue;
            }

            // Checking if second argument can be converted to a float
            if (!float.TryParse(words[1], out float itemPrice))
            {
                BadPrompt("Second argument must be numeric");
                continue;
            }
            
            // Initializing item with it's name and price then adding it to the list
            newItem = new(words[0], itemPrice);
            items.Add(newItem);

            // If no person is specified, links everyone to the current item
            if (words.Length == 2)
            {
                foreach (Person person in people)
                {
                    links.Add(new(person, newItem));
                }
            }

            // If more than 2 command line arguments are provided, links the item only to the specified people
            else if (words.Length > 2)
            {
                // Creates links between the item and each person in the command-line arguments, if the person exists
                for (int i = 2; i < words.Length; i++)
                {
                    // Initializing person to receive the item
                    Person receiver = GetPerson(words[i]);

                    // Adding link between person and item to the list if the person exists
                    if (receiver == null)
                    {
                        BadPrompt($"{words[i]} is not on the list");
                    }
                    else
                    {
                        Link newLink = new(receiver, newItem);
                        links.Add(newLink);
                        Console.WriteLine($"Adding the link {newLink}");
                    }
                }
            }
        }
    }

    static void RemovePerson(string name)
    {
        // Retrieves the person to be removed if name matches
        Person removee = GetPerson(name);

        // If the person returned was null, warns the user that the person's name is not in the list and returns from the method
        if (removee == null)
        {
            BadPrompt($"can't remove {name} because they're not on the list!");
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
                links.Remove(links[i]);
            }
        }
    }

    static void RemoveItem(string tag)
    {
        // Retrieves the item to be removed if name matches
        Item removee = GetItem(tag);

        // If the item returned was null, warns the user that the item's tag is not in the list and returns from the method
        if (removee == null)
        {
            Console.WriteLine($"Couldn't remove {tag} because it's not on the item list!");
            return;
        }
        else
        {
            items.Remove(removee);
        }

        // Loops through the list of links in reverse order, removing the link if the item tag matches
        for (int i = links.Count - 1; i >= 0; i--)
        {
            if (links[i].Item.Tag == tag)
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
        Clear();
        
        // Prints, in one line, the name of each person on the list
        ListPeople();
        Console.WriteLine("Now type in a list of people. People who match the names in the list will be removed. People that aren't on the list will be added");
        
        string? input = GetString("Enter the list of 'names' or 'done' to go back: ");
        string[] names = input!.Split();

        if (input == "done" || input == "")
        {
            return;
        }

        foreach (string name in names)
        {
            // Removes person if name is already on the list, adds if not on the list
            if (GetPerson(name) == null)
            {
                AddPerson(name);
                Console.WriteLine($"Added {name}");
            }
            else
            {
                RemovePerson(name);
                Console.WriteLine($"Removed {name}");
            }
        }

        HoldForKey();
    }

    static void EditItemList()
    {
        // Prints, in one line, the name of each person on the list
        ListItems();
        Console.WriteLine("If you want to remove any items, type in their tags.\n" +
        "If you want to add more items, type 'add'\n" +
        "Enter 'done' to go back");

        string? input = Console.ReadLine();
        string[] tags = input!.Split();

        // Allows the user to add more items to the list
        if (input == "add")
        {
            AddItems();
            return;
        }

        Clear();

        foreach (string tag in tags)
        {
            // Removes item if if exists
            if (GetItem(tag) == null)
            {
                Console.WriteLine($"Couldn't remove {tag} because it doesn't exist");
            }
            else
            {
                Console.WriteLine($"Removed {tag}");
                RemoveItem(tag);
            }

            Console.WriteLine();
        }

        HoldForKey();
    }

    // Calculates the values for a given item taking into account how many candidates are contributing
    static float ShareOf(Item item)
    {
        int buyers = 0;

        for (int i = 0; i < links.Count; i++)
        {
            if (links[i].Item.Tag == item.Tag)
            {
                buyers++;
            }
        }

        // Returns the calculated value with two decimal places, rounded up
        return (float)Math.Round(item.Price / buyers, 2, MidpointRounding.ToPositiveInfinity);
    }

    // Displays the links that contain the specified person
    static void DisplayLinksFrom(Person person, int indentation)
    {
        Console.WriteLine($"{person.Name} bought: ");
        for (int i = 0; i < links.Count; i++)
        {
            // Prints the item's name and how much said person contributes indenting by the specified amount
            if (links[i].Contributor.Name == person.Name)
            {
                Console.Write(String.Concat(Enumerable.Repeat("    ", indentation)));
                Console.WriteLine($"{links[i].Item.Tag} for {ShareOf(links[i].Item)}");
            }
        }
    }

    static void DisplaySummary()
    {
        float finalValue = 0;
        
        for (int i = 0; i < people.Count; i++)
        {
            // Initializing the person and their items
            Person currentPerson = people[i];
            List<Item> theirItems = GetItemsFrom(people[i]);

            // Stores the total value owed by the person
            float owedValue = 0;

            // Displaying all items associated with such person
            DisplayLinksFrom(currentPerson, 1);


            // Sums the value of all items the current person contributes
            for (int j = 0; j < theirItems.Count; j++)
            {
                float share = ShareOf(theirItems[j]);
                finalValue += share;
                owedValue += share;
            }

            Printy($"Owing a total of {owedValue}", ConsoleColor.White, ConsoleColor.Black);
            Console.WriteLine("\n");
        }

        // Prints the total price of the items
        Printy("The final bill amounts to ", ConsoleColor.Black, ConsoleColor.Green);
        Printy($"{finalValue}", ConsoleColor.Green, ConsoleColor.Black);
        Console.WriteLine();

        HoldForKey();
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
        return null!;
    }

    static Item GetItem(string name)
    {
        foreach (Item item in items)
        {
            if (item.Tag == name)
            {
                return item;
            }
        }

        // Returns null if no item is found
        return null!;
    }

    static string? GetString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    static void Printy(string text, ConsoleColor b)
    {
        Console.BackgroundColor = b;
        Console.Write(text);
        Console.BackgroundColor = ConsoleColor.Black;
    }

    static void Printy(string text, ConsoleColor b, ConsoleColor f)
    {
        Console.BackgroundColor = b; Console.ForegroundColor = f;
        Console.Write(text);
        Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor= ConsoleColor.White;
    }

    static void UpdatePerson(string name)
    {
        // Gets the person with that name and their items
        Person person = GetPerson(name);
        List<Item> theirItems = GetItemsFrom(person);

        // Prints the items the person currently contributes to <-- WHOA LOOK AT THIS
        Console.WriteLine("This person currently contributes to: "); // °o°
        
        if (theirItems.Count == 0)
        {
            Console.WriteLine("Nothing! Such a loser...");
        }
        foreach (Item item in theirItems)
        {
            Console.WriteLine($"{item.Tag}");
        }

        string? input = GetString("Type in the [item...] you want to add/remove, or 'cancel' to go back: ");
        string[] words = input!.Split();

        if (input == "cancel" || input == "")
        {
            return;
        }

        foreach (string word in words)
        {
            // Initializes the item to be removed
            Item currentItem = GetItem(word);

            if (currentItem == null)
            {
                Console.WriteLine($"{word} is not a valid item");
            }

            else
            {
                // Loops through the link list
                for (int i = links.Count - 1; i >= 0; i--)
                {
                    // Removes the link that links the person to the item
                    if (links[i].Item.Tag == word && links[i].Contributor.Name == person.Name) // MAYBE will bug out
                    {
                        Console.WriteLine($"Removed {links[i].Item.Tag} from {person.Name}");
                        links.Remove(links[i]);
                    }
                }
            }
        }

        HoldForKey();
    }

    static void UpdateItem()
    {
        // TODO
    }

    private static void ListPeople()
    {
        Clear(); 
        
        Console.WriteLine("People:");
        for (int i = 0; i < people.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {people[i].Name}");
        }

        Console.WriteLine();
    }

    static void ListItems()
    {
        Clear();
        
        // Initializing variable to store the amount of letters in the biggest item name
        int maxLength = 0; 

        Console.WriteLine("Items:");
        foreach (var item in items)
        {
            if (item.Tag!.Length > maxLength)
            {
                maxLength = item.Tag.Length;
            }
        }

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Tag!.PadRight(maxLength + 4)}{item.Price}");
        }

        Console.WriteLine();
    }

    // Prints all of the links in the list
    static void ListLinks()
    {
        Clear();
        
        foreach ( Person person in people)
        {
            foreach (Link link in links)
            {
                if (link.Contributor.Name == person.Name)
                Console.WriteLine(link);
            }
        }

        Console.WriteLine();
    }

    public interface IBranch
    {
        public abstract void Go();
    }

    public class ListBranch : IBranch
    {
        public void Go()
        {
            Clear();
            
            while (true)
            {
                Console.Write("Display the list of\n" +
                "1. People\n" +
                "2. Items\n" +
                "3. Links\n" +
                "Type in one of the (number) options or 'cancel' to go back: ");

                string? input = Console.ReadLine();

                // Returns if user chooses to
                if (input == "cancel" || input == "")
                {
                    return;
                }
                // Executes the specified command
                else if (input == "1" || input!.ToLower() == "people")
                {
                    ListPeople();
                    break;
                }
                else if (input == "2" || input.ToLower() == "items")
                {
                    ListItems();
                    break;
                }
                else if (input == "3" || input.ToLower() == "links")
                {
                    ListLinks();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }

            HoldForKey();
        }

    }

    static void Clear()
    {
        Console.WriteLine("\n--------------------------------------------------------------------------------------------------------------------------------\n");
    }

    private static void HoldForKey()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        Clear();
    }

    // Prints a warning, padding lines around it and changing the console's colors
    static void BadPrompt(string warning)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        Console.Write(warning);

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n");

        HoldForKey();
    }

    public class EditListBranch : IBranch
    {
        public void Go()
        {
            Clear();
            
            Console.Write("Edit the list of\n" +
            "1. People\n" +
            "2. Items\n" +
            "Type in one the options (name or number) or 'cancel' to go back: ");

            while (true)
            {
                string? input = Console.ReadLine();

                // Returns if user chooses to
                if (input == "cancel" || input == "")
                {
                    return;
                }
                // Executes the specified command
                if (input == "1" || input!.ToLower() == "people")
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

    public class ChangeBranch : IBranch
    {
        public void Go()
        {
            // Initializing branch
            IBranch? nextBranch;

            string? input = "";

            while (input != "cancel") // MAYBE changing this condition to check if the branch is null
            {
                Clear();

                Console.WriteLine($"What would you like to change?\n"
                + "1. A person\n"
                + "2. An item (NOT WORKING)\n");

                input = GetString("Choose a 'number' or 'cancel' to go back: ");

                nextBranch = input switch
                {
                    "1" => new ChangePersonBranch(),
                    "2" => new ChangeItemBranch(),
                    "cancel" => new VoidBranch(),
                    _ => null
                };

                if (nextBranch == null)
                {
                    BadPrompt("Invalid input");
                }
                else
                {
                    nextBranch.Go();
                    break;
                }
            }
        }
    }

    public class ChangePersonBranch : IBranch
    {
        public void Go()
        {
            while (true)
            {
                ListPeople();
                Console.WriteLine("Which person would you like to access?");
                string? input = GetString("Enter a 'name' or 'cancel' to go back: ");

                if (input == "cancel" || input == "")
                {
                    return;
                }

                Person person = GetPerson(input);

                if (GetPerson(input) == null)
                {
                    BadPrompt(input + " is not on the list");
                }
                else
                {
                    Edit(person);
                    break;
                }
            }
        }
    }

    static void Edit(Person person)
    {
        while (true)
        {
            string? input = GetString($"Rename {person.Name} or 'cancel' to go back: ");

            if (input == "cancel" || input == "")
            {
                break;
            }
            else
            {
                Console.WriteLine($"\nYou changed {person.Name}'s name to {input}");
                HoldForKey();

                person.Name = input!;
                break;
            }
        }
    }

    static void Edit(Item item)
    {
        // TODO
    }

    public class ChangeItemBranch : IBranch
    {
        public void Go()
        {
            // CONTINUE
        }
    }

    public class InfoBranch : IBranch
    {
        public void Go()
        {
            Clear();
            DisplaySummary();
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
        public string Name {get; set;}

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
        public string? Tag {get; set;}
        public float Price {get; set;}

        public Item(string tag, float price)
        {
            Tag = tag;
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
            return $"'{Contributor.Name} contributes to {Item.Tag}'";
        }
    }
}