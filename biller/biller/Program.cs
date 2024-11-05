internal class Program
{
    // Lists to store the people, items, and links between those two
    static List<Person> people = [];
    static List<Link> links = [];
    static List<Item> items = [];

    static void Main()
    {
        PopulatePeople();
        AddItems();
        MainMenu();
    }

    // Shows the user a list of available options, then prompts them for an input
    static void MainMenu()
    {
        string? input = "start";
        
        while (input != "exit")
        {
            Console.Clear();
            
            // Prints the main menu, showing the user all of his options
            Console.WriteLine("----MAIN MENU----\n"
            + "1. Display the lists of people, items, or links\n"
            + "2. Edit people or item list\n"
            + "3. Change data of a person or item\n"
            + "4. Print all info\n");

            input = GetString("Enter a 'number' or 'exit' to quit the application: ");

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
        Console.Clear();

        string? input;

        while (true)
        {
            input = GetString("Enter the 'name' of each person\n");

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
        Console.Clear();

        string? input;

        while (true)
        {
            ListItems();
            Item newItem;
            // float itemPrice = 0; inlining? DONKEY

            // Prompts for the new item name and price, then separates the input into an array
            Console.WriteLine("Enter the item's prices and contributors\n"
                + "If you don't specify anyone, everyone will contribute\n");
            input = GetString("Enter 'name price [person...]' or 'done' to continue\n");
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
                links.Remove(links[i]);
            }
        }
    }

    static void RemoveLink(Person person, Item item)
    {
        string name = person.Name;
        string tag = item.Tag;

        // Loops through the list in reverse order
        for (int i = links.Count - 1; i >= 0; i--)
        {
            string contributor = links[i].Contributor.Name;
            string linkedTag = links[i].Item.Tag;

            // Removes the link that links the person to the item
            if (contributor == name && linkedTag == tag)
            {
                links.Remove(links[i]);
            }
        }
    }

    // Edits the list of people by comparing the names to user input
    // If the entered person is already on the list, removes the person
    // If the entered person is not on the list, adds the person
    static void EditPersonList()
    {
        Console.Clear();
        
        // Prints, in one line, the name of each person on the list
        ListPeople();

        Console.WriteLine("Now type in a list of people\n"
        + "People who match the names in the list will be removed\n"
        + "People that aren't on the list will be added");
        string? input = GetString("Enter the list of 'names' or 'done' to go back\n");
        string[] names = input!.Split();

        if (input == "done" || input == "")
        {
            return;
        }

        Console.Clear();

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
        "If you want to add more items, type 'add'\n");

        string? input = GetString("Enter an option or 'done' to go back\n");
        string[] tags = input!.Split();

        // Allows the user to add more items to the list
        if (input == "add")
        {
            AddItems();
            return;
        }

        Console.Clear();

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
        Console.Clear();
        
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

            finalValue = (float)Math.Round(finalValue, 2);
            
            Printy($"Owing a total of {owedValue}", ConsoleColor.White, ConsoleColor.Black);
            Console.WriteLine("\n");
        }

        // Prints the total price of the items
        Printy("The final bill amounts to ", ConsoleColor.Black, ConsoleColor.Green);
        Printy($"{finalValue}", ConsoleColor.Green, ConsoleColor.Black);
        Console.WriteLine("");

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

    static bool AreLinked(Person person, Item item)
    {
        foreach(Link link in links)
        {
            if (link.Contributor.Name == person.Name && link.Item.Tag == item.Tag)
            {
                return true;
            }
        }

        return false;
    }

    static string? GetString(string prompt)
    {
        Printy(prompt, ConsoleColor.DarkYellow);
        return Console.ReadLine();
    }

    static void Printy(string text, ConsoleColor f)
    {
        Console.ForegroundColor = f;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }

    static void Printy(string text, ConsoleColor b, ConsoleColor f)
    {
        Console.BackgroundColor = b; Console.ForegroundColor = f;
        Console.Write(text);
        Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor= ConsoleColor.White;
    }

    static void UpdatePerson(string name) // TODO use this on the edit branch!
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
        Console.Clear(); 
        
        Console.WriteLine("People:");
        for (int i = 0; i < people.Count; i++)
        {
            Console.WriteLine($"{people[i].Name}");
        }

        Console.WriteLine();
    }

    static void ListItems()
    {
        Console.Clear();
        
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
        Console.Clear();
        
        foreach ( Person person in people)
        {
            foreach (Link link in links)
            {
                if (link.Contributor.Name == person.Name)
                Console.WriteLine(link);
            }
            Console.WriteLine();
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
            Console.Clear();
            
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
        Console.Write("\nPress any key to continue... ");
        Console.ReadKey();

        Console.Clear();
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
            Console.Clear();
            

            while (true)
            {
                Console.WriteLine("Edit the list of\n" +
                "1. People\n" +
                "2. Items\n");

                string? input = GetString("Type in a 'number' or 'cancel' to go back: ");

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
                    BadPrompt("Invalid input");
                }
            }
        }
    }

    public class ChangeBranch : IBranch
    {
        public void Go()
        {
            // Initializing branch
            IBranch? nextBranch = null;

            while (nextBranch == null) // MAYBE changing this condition to check if the branch is null
            {
                Console.Clear();

                Console.WriteLine($"What would you like to change?\n"
                + "1. A person\n"
                + "2. An item\n");

                string? input = GetString("Choose a 'number' or 'cancel' to go back: ");

                nextBranch = input switch
                {
                    "1" => new ChangePersonBranch(),
                    "2" => new ChangeItemBranch(),
                    "cancel" => new VoidBranch(),
                    "" => new VoidBranch(),
                    _ => null
                };

                if (nextBranch == null)
                {
                    if (input == "")
                    {
                        return;
                    }
                    
                    BadPrompt("Invalid input");
                }
                else
                {
                    nextBranch.Go();
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

                Person person = GetPerson(input!);

                if (GetPerson(input!) == null)
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
            Console.Clear();

            Console.WriteLine($"Current person: {person}\n\n"
            + $"What would you like to change about this person?\n"
            + "1. Name\n"
            + "2. Contributions\n");
            string? input = GetString("Enter a 'number' or 'cancel' to go back: ");

            if (input == "cancel" || input == "")
            {
                return;
            }

            if (input == "1")
            {
                ChangeName(person);
                break;
            }
            else if (input == "2")
            {
                ChangeContributions(person);
                break;
            }
            else
            {
                BadPrompt("Invalid input");
            }
        }
    }

    static void ChangeName(Person person)
    {
        while (true)
        {
            Console.Clear();

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

    static void ChangeContributions(Person person)
    {
        Console.Clear();

        Console.WriteLine("Items which this person contributes:");
        foreach (Item item in items)
        {
            if (AreLinked(person, item))
            {
                Printy(item.Tag! + "\n", ConsoleColor.Blue);
            }
            else
            {
                Console.WriteLine(item.Tag);
            }
        }

        Console.WriteLine("\nTyping in an item alternates their statuses on the contributor's list");

        string? input = GetString("Enter a 'list of items' or 'cancel' to go back\n");
        string[] tags = input!.Split();

        if (input == "cancel" || input == "")
        {
            return;
        }

        Console.Clear();

        foreach (string tag in tags)
        {
            Item currentItem = GetItem(tag);

            if (currentItem == null)
            {
                Console.WriteLine($"{tag} is not a valid item!");
            }
            else if (AreLinked(person, currentItem))
            {
                Console.WriteLine($"{person.Name} stopped contributing to {currentItem.Tag}");
                RemoveLink(person, currentItem);
            }
            else
            {
                Console.WriteLine($"{person} now contributes to {currentItem.Tag}");
                links.Add(new(person, currentItem));
            }
        }

        HoldForKey();
    }

    static void Edit(Item item)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine($"Current item: {item}\n\n"
            + $"What would you like to change about this item?\n"
            + "1. Tag\n"
            + "2. Price\n"
            + "3. Contributors\n");
            string? input = GetString("Enter a 'number' or 'cancel' to go back: ");

            if (input == "cancel" || input == "")
            {
                return;
            }

            if (input == "1")
            {
                ChangeTag(item);
                break;
            }
            else if (input == "2")
            {
                ChangePrice(item);
                break;
            }
            else if (input == "3")
            {
                ChangeContributors(item);
                break;
            }
            else
            {
                BadPrompt("Invalid input");
            }
        }
    }

    static void ChangeTag(Item item)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine($"Current tag: {item.Tag}\n");
            string? input = GetString("Enter a new 'tag' or 'cancel' to go back: ");

            if (input == "cancel" || input == "")
            {
                return;
            }

            if (input!.Split().Length > 1)
            {
                BadPrompt("New tag must be a single word");
            }
            else
            {
                Console.WriteLine($"\nYou changed {item.Tag} to {input}");
                item.Tag = input;

                HoldForKey();
                break;
            }
        }
    }

    static void ChangePrice(Item item)
    {
        Console.Clear();

        while (true)
        {
            Console.WriteLine($"Current price: {item.Price}\n");
            string? input = GetString("Enter a new 'price' or 'cancel' to go back: ");

            if (input == "cancel" || input == "")
            {
                return;
            }

            if (input!.Split().Length > 1)
            {
                BadPrompt("New price must not contain whitespaces");
            }
            else if (!float.TryParse(input, out float newPrice))
            {
                BadPrompt("New price must be numeric");
            }
            else
            {
                Console.WriteLine($"Changed {item.Price} to {newPrice}");
                item.Price = newPrice;

                HoldForKey();
                break;
            }
        }
    }

    static void ChangeContributors(Item item)
    {
        // TODO copy the structure from ChangeContributions()
    }

    public class ChangeItemBranch : IBranch
    {
        public void Go()
        {
            while (true)
            {
                ListItems();

                Console.WriteLine("Which item would you like to access?");
                string? input = GetString("Enter a 'tag' or 'cancel' to go back: ");

                if (input == "cancel" || input == "")
                {
                    return;
                }

                Item item = GetItem(input!);

                if (GetItem(input!) == null)
                {
                    BadPrompt(input + " is not on the list");
                }
                else
                {
                    Edit(item);
                    break;
                }
            }
        }
    }

    public class InfoBranch : IBranch
    {
        public void Go()
        {
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
        public string Tag {get; set;}
        public float Price {get; set;}

        public Item(string tag, float price)
        {
            Tag = tag;
            Price = price;
        }

        public override string ToString()
        {
            return $"{Tag} for {Price}";
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