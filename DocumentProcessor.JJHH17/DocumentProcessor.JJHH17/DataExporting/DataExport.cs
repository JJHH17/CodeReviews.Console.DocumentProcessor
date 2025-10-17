using CsvHelper;
using DocumentProcessor.JJHH17.Models;
using Spectre.Console;

namespace Document.Processor.JJHH17.DataExporting;

public class DataExport
{
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