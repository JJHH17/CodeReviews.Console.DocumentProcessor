using DocumentProcessor.JJHH17.Models;
using ExcelDataReader;
using Spectre.Console;
using System.Data;

namespace Document.Processor.JJHH17.DataSeeding;

public class DataSeed
{
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
}