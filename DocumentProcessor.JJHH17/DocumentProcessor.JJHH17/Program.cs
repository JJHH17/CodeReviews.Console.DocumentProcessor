using System;
using DocumentProcessor.JJHH17.Models;
using Spectre.Console;

namespace DocumentProcessor.JJHH17;

public class Program 
{
    public static void Main(string[] args)
    {
        Menu();
    }

    enum MenuOptions
    {
        AddEntry,
        Read,
        Delete,
        Update,
        Exit
    }

    public static void Menu()
    {
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
                    Console.WriteLine("Read selected.");
                    Console.ReadKey();
                    break;

                case MenuOptions.Delete:
                    Console.WriteLine("Delete selected.");
                    Console.ReadKey();
                    break;

                case MenuOptions.Update:
                    Console.WriteLine("Update selected.");
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
            Console.WriteLine("Enter phohne number (must be 11 digits):");
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