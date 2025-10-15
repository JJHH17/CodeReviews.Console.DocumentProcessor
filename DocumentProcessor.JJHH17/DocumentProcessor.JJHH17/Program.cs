using System.IO;
using DocumentProcessor.JJHH17.Models;
using Spectre.Console;
using CsvHelper;
using System.Data;
using ExcelDataReader;

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
                SeedOption();
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
                    CreateExportCSV();
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

    enum FileTypes
    {
        CSV,
        XLS,
        XLSX
    }

    public static void SeedOption()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold yellow]Seed Database via a given file type[/]");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<FileTypes>()
            .Title("Select a file type to import from:")
            .AddChoices(Enum.GetValues<FileTypes>()));

        switch (choice)
        {
            case FileTypes.CSV:
                SeedCSVData();
                break;
            case FileTypes.XLS:
                SeedXLSData();
                break;
            case FileTypes.XLSX:
                SeedXLSXData();
                break;
        }
    }

    public static List<string[]> ReadFile(string filePath)
    {
        List<string[]> rows = new List<string[]>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var values = line.Split(',');
                rows.Add(values);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading file: {ex.Message}[/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        return rows;
    }

    public static List<string[]> ReadExcel(string filePath)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataset = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        });

        var table = dataset.Tables[0];
        var rows = new List<string[]>();

        foreach (DataRow dr in table.Rows)
        {
            var name = dr["Name"].ToString()?.Trim();
            var email = dr["Email"].ToString()?.Trim();
            var phoneNumber = dr["PhoneNumber"].ToString()?.Trim();

            rows.Add(new string[] { name, email, phoneNumber });
        }

        return rows;
    }

    public static void SeedCSVData()
    {
        try
        {
            string csvFilePath = "Import Data - Sheet1.csv";
            List<string[]> csvData = ReadFile(csvFilePath);

            foreach (string[] row in csvData)
            {
                using (var context = new PhoneBookContext())
                {
                    var newEntry = new Phonebook
                    {
                        Name = row[0],
                        Email = row[1],
                        PhoneNumber = row[2]
                    };
                    context.Phonebooks.Add(newEntry);
                    context.SaveChanges();
                }
            }

            AnsiConsole.MarkupLine("[green]Database seeded successfully from CSV![/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading CSV file: {ex.Message}[/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }

    public static void SeedXLSData()
    {
        try
        {
            string xlsFilePath = "Import Data - Sheet1.xls";
            var xlsData = ReadExcel(xlsFilePath);
            using var context = new PhoneBookContext();
            foreach (var row in xlsData)
            {
                var newEntry = new Phonebook
                {
                    Name = row[0],
                    Email = row[1],
                    PhoneNumber = row[2]
                };
                context.Phonebooks.Add(newEntry);
            }
            context.SaveChanges();
            AnsiConsole.MarkupLine("[green]Database seeded successfully from XLS![/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading XLS file: {ex.Message}[/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }

    public static void SeedXLSXData()
    {
        try
        {
            string xlsxFilePath = "Import Data - Sheet1.xlsx";
            var xlsxData = ReadExcel(xlsxFilePath);

            using var context = new PhoneBookContext();
            foreach (var row in xlsxData)
            {
                var newEntry = new Phonebook
                {
                    Name = row[0],
                    Email = row[1],
                    PhoneNumber = row[2]
                };
                context.Phonebooks.Add(newEntry);
            }

            context.SaveChanges();
            AnsiConsole.MarkupLine("[green]Database seeded successfully from XLSX![/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        } 
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error reading XLSX file: {ex.Message}[/]");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }
    }

    public static void CreateExportCSV()
    {
        using (var context = new PhoneBookContext())
        {
            var entries = context.Phonebooks.ToList();
            using (var writer = new StreamWriter("ExportedPhonebook.pdf"))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(entries);
            }
        }
        AnsiConsole.MarkupLine("[green]Data exported to ExportedPhonebook.pdf successfully![/]");
        AnsiConsole.MarkupLine("[green]You can find the CSV in 'CodeReviews.Console.DocumentProcessor\\DocumentProcessor.JJHH17\\DocumentProcessor.JJHH17\\bin\\Debug\\net8.0\\ExportedPhonebook.pdf\'[/]");
    }
}