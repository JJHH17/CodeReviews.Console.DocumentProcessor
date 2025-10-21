using CsvHelper;
using DocumentProcessor.JJHH17.DataExporting;
using DocumentProcessor.JJHH17.Models;
using Spectre.Console;
using Azure.Storage.Blobs;
using System.Configuration;
using Azure.Storage.Blobs.Models;

namespace Document.Processor.JJHH17.DataExporting;

public class DataExport
{
    enum ExportMenuOptions
    {
        PDF,
        CSV,
        AzureBlobStorage
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

            case ExportMenuOptions.AzureBlobStorage:
                CreateAzureBlobExport();
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

    public static async Task CreateAzureBlobExport()
    {
        var localCsv = "ExportedPhonebook.csv";
        if (!File.Exists(localCsv))
        {
            AnsiConsole.MarkupLine("[yellow]CSV file not found. Creating CSV file first...[/]");
            CreateExportCsv();
        }

        var connectionString = ConfigurationManager.AppSettings["AzureBlobConnectionString"];
        var containerName = ConfigurationManager.AppSettings["ContainerName"];

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
        {
            AnsiConsole.MarkupLine("[red]Azure Blob Storage connection string or container name is not configured properly. Please check in the app.config file.[/]");
            return;
        }

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient("ExportedPhonebook.csv");
        using var fileStream = File.OpenRead(localCsv);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = "text/csv" }
        };

        await blobClient.UploadAsync(fileStream, options);
        AnsiConsole.MarkupLine("[green]CSV file uploaded to Azure Blob Storage successfully![/]");
    }
}