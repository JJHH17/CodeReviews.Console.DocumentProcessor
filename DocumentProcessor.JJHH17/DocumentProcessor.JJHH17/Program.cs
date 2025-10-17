using DocumentProcessor.JJHH17.UserInterface;
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
        UserInterface.UserInterface.Menu();
    }
}