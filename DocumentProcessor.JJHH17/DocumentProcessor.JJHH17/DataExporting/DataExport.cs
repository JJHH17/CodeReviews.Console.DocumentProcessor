using CsvHelper;
using DocumentProcessor.JJHH17.Models;
using Spectre.Console;

namespace Document.Processor.JJHH17.DataExporting;

public class DataExport
{
    enum ExportMenuOptions
    {
        PDF,
        CSV
    }

    public static void ExportMenu()
    {
        Console.Clear();

        AnsiConsole.MarkupLine("[bold yellow]Export Data to a given file type[/]");
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<ExportMenuOptions>()
            .Title("Select a file type to export to:")
            .AddChoices(Enum.GetValues<ExportMenuOptions>()));
        switch (choice)
        {
            case ExportMenuOptions.PDF:
                CreateExportPDF();
                break;

            case ExportMenuOptions.CSV:
                CreateExportCsv();
                break;
        }
    }

    public static void CreateExportPDF()
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

    public static void CreateExportCsv()
    {
        using (var context = new PhoneBookContext())
        {
            var entries = context.Phonebooks.ToList();
            using (var writer = new StreamWriter("ExportedPhonebook.csv"))
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(entries);
            }
        }

        AnsiConsole.MarkupLine("[green]Data exported to ExportedPhonebook.csv successfully![/]");
        AnsiConsole.MarkupLine("[green]You can find the CSV in 'CodeReviews.Console.DocumentProcessor\\DocumentProcessor.JJHH17\\DocumentProcessor.JJHH17\\bin\\Debug\\net8.0\\ExportedPhonebook.csv\'[/]");
    }
}