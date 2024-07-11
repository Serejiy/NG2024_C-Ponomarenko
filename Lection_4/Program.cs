using System;

namespace PhoneBookApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PhoneBook phoneBook = new PhoneBook();
            string command;

            Console.WriteLine("Phone Book Application");
            Console.WriteLine("Commands: Q - Exit, A - Add, GP - Get by phone, GN - Get by name, GA - Get all, D - Delete by phone");

            do
            {
                Console.Write("\nEnter command: ");
                command = Console.ReadLine().ToUpper();

                switch (command)
                {
                    case "A":
                        AddPerson(phoneBook);
                        break;
                    case "GP":
                        GetPersonByPhone(phoneBook);
                        break;
                    case "GN":
                        GetPersonsByName(phoneBook);
                        break;
                    case "GA":
                        GetAllPersons(phoneBook);
                        break;
                    case "D":
                        DeletePersonByPhone(phoneBook);
                        break;
                    case "Q":
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Unknown command. Please try again.");
                        break;
                }

            } while (command != "Q");
        }

        static void AddPerson(PhoneBook phoneBook)
        {
            Console.Write("Enter full name: ");
            string fullName = Console.ReadLine();

            Console.Write("Enter phone number: ");
            string phoneNumber = Console.ReadLine();

            try
            {
                phoneBook.AddPerson(new Person(fullName, phoneNumber));
                Console.WriteLine("Person added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void GetPersonByPhone(PhoneBook phoneBook)
        {
            Console.Write("Enter phone number: ");
            string phoneNumber = Console.ReadLine();

            try
            {
                Person person = phoneBook.GetPersonByPhone(phoneNumber);
                Console.WriteLine($"Full Name: {person.FullName}, Phone Number: {person.PhoneNumber}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void GetPersonsByName(PhoneBook phoneBook)
        {
            Console.Write("Enter full name: ");
            string fullName = Console.ReadLine();

            try
            {
                var persons = phoneBook.GetPersonsByName(fullName);
                foreach (var person in persons)
                {
                    Console.WriteLine($"Full Name: {person.FullName}, Phone Number: {person.PhoneNumber}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void GetAllPersons(PhoneBook phoneBook)
        {
            var persons = phoneBook.GetAllPersons();
            foreach (var person in persons)
            {
                Console.WriteLine($"Full Name: {person.FullName}, Phone Number: {person.PhoneNumber}");
            }
        }

        static void DeletePersonByPhone(PhoneBook phoneBook)
        {
            Console.Write("Enter phone number: ");
            string phoneNumber = Console.ReadLine();

            try
            {
                phoneBook.DeletePersonByPhone(phoneNumber);
                Console.WriteLine("Person deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
