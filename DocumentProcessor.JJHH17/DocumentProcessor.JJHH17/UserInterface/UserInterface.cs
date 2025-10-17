using DocumentProcessor.JJHH17.Models;
using Spectre.Console;
using Document.Processor.JJHH17.DataSeeding;
using Document.Processor.JJHH17.DataExporting;

namespace DocumentProcessor.JJHH17.UserInterface;

public class UserInterface
{
    enum MenuOptions
    {
        AddEntry,
        Read,
        Delete,
        Export,
        Exit
    }

    public static void Menu()
    {
        // Seeds data if database is empty
        using (var context = new PhoneBookContext())
        {
            if (!context.Phonebooks.Any())
            {
                DataSeed.SeedOption();
            }
        }


        bool running = true;
        while (running)
        {
            AnsiConsole.MarkupLine("[bold yellow]Document Processor Menu[/]");
            Console.Clear();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOptions>()
                    .Title("Select an option:")
                    .AddChoices(Enum.GetValues<MenuOptions>()));

            switch (choice)
            {
                case MenuOptions.AddEntry:
                    Console.Clear();
                    AddEntry();
                    break;

                case MenuOptions.Read:
                    Console.Clear();
                    ReadEntries();
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                    break;

                case MenuOptions.Delete:
                    Console.Clear();
                    DeleteEntry();
                    break;

                case MenuOptions.Export:
                    Console.Clear();
                    DataExport.ExportMenu();
                    Console.WriteLine("Enter any key to return to the menu...");
                    Console.ReadKey();
                    break;

                case MenuOptions.Exit:
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    running = false;
                    break;
            }
        }
    }

    public static void AddEntry()
    {
        AnsiConsole.MarkupLine("[bold yellow]Add Entry[/]");
        Console.WriteLine("Enter name:");
        string name = Console.ReadLine();
        string email = EmailInput();
        string phoneNumber = PhoneNumberInput();

        using (var context = new PhoneBookContext())
        {
            var newEntry = new Phonebook
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };

            context.Phonebooks.Add(newEntry);
            context.SaveChanges();
        }

        AnsiConsole.MarkupLine("[green]Entry added successfully![/]");
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }

    public static void ReadEntries()
    {
        using (var context = new PhoneBookContext())
        {
            var query = context.Phonebooks.ToList();

            if (query.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No entries were found[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Email");
                table.AddColumn("Phone Number");

                foreach (var entry in query)
                {
                    table.AddRow(entry.Id.ToString(), entry.Name, entry.Email, entry.PhoneNumber);
                }
                AnsiConsole.Write(table);
            }
        }
    }

    public static void DeleteEntry()
    {
        using (var context = new PhoneBookContext())
        {
            AnsiConsole.MarkupLine("[bold yellow]Delete Entry[/]");
            ReadEntries();
            Console.WriteLine("Enter the ID of the entry to delete:");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var entry = context.Phonebooks.Find(id);
                if (entry != null)
                {
                    context.Phonebooks.Remove(entry);
                    context.SaveChanges();
                    AnsiConsole.MarkupLine("[green]Entry deleted successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Entry not found.[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid ID format.[/]");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }

    public static string EmailInput()
    {
        string email;
        while (true)
        {
            Console.WriteLine("Enter email address:");
            email = Console.ReadLine();

            if (email.Contains("@") && email.Contains("."))
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid email format. Please try again.[/]");
            }
        }

        return email;
    }

    public static string PhoneNumberInput()
    {
        string phoneNumber;
        while (true)
        {
            Console.WriteLine("Enter phone number (must be 11 digits):");
            phoneNumber = Console.ReadLine();

            if (phoneNumber.Length == 11 && long.TryParse(phoneNumber, out _))
            {
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid phone number. Please enter exactly 11 digits.[/]");
            }
        }

        return phoneNumber;
    }

}