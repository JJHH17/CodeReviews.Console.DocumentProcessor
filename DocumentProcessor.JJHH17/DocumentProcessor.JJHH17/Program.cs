using System;
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
                    Console.WriteLine("Add Entry selected.");
                    Console.ReadKey();
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
}