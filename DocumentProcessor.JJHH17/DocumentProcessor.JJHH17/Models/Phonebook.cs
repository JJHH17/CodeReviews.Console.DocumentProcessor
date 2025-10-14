using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DocumentProcessor.JJHH17.Models;

public class Phonebook
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class PhoneBookContext : DbContext
{
    private static readonly string server = ConfigurationManager.AppSettings["Server"];
    private static readonly string databaseInstance = ConfigurationManager.AppSettings["DatabaseName"];
    public static string connectionString = $@"Server=({server})\{databaseInstance};Integrated Security=true;";

    public DbSet<Phonebook> Phonebooks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }
}