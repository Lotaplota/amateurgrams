internal class Program
{
    private static void Main()
    {
        List<Person> people = [];
        List<Link> links = [];
        List<Item> items = [];

        PopulatePeople();
        PopulateItems();

        for (int i = 0; i < people.Count; i++)
        {
            DisplayInfo(people[i]);
        }
        DisplayAllInfo();

        for (int i = 0; i < people.Count; i++)
        {
            DisplayInfo(people[i]);
        }
        DisplayAllInfo();

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

        void PopulateItems()
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

        void AddPerson()
        {
            // IMPLEMENT
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

        List<Item> GetItemsFrom(Person person)
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
        Person GetPerson(string name)
        {
            for (int i = 0; i < people.Count; i++)
            {
                if (people[i].Name == name)
                {
                    return people[i];
                }

            }

            // If no person is found, returns a dummy person named 'null'
            return new("null");
        }

        string? GetString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        float GetFloat(string prompt)
        {
            Console.Write(prompt);
            return Convert.ToSingle(Console.ReadLine());
        }
    }

    public interface ICommand
    {
        public abstract void Run();
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