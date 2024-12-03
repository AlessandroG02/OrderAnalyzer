using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using System.Runtime.CompilerServices;
using System.Formats.Asn1;

class Program
{
    static void Main()
    {
        // Recupera il percorso della directory sorgente
        string sourcePath = GetSourceDirectory();

        // Percorso relativo del file CSV rispetto alla directory sorgente
        string filePath = Path.Combine(sourcePath, "orders.csv");

        Console.WriteLine($"Percorso del file CSV: {filePath}");

        // Controllo se il file esiste
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Errore: Il file {filePath} non esiste.");
            return;
        }

        try
        {
            // Leggi i dati dal file CSV
            var orders = ReadOrders(filePath);

            // Trova gli ordini richiesti
            var highestTotalOrder = GetOrderWithHighestTotal(orders);
            var highestQuantityOrder = GetOrderWithHighestQuantity(orders);
            var maxDiscountDifferenceOrder = GetOrderWithMaxDiscountDifference(orders);

            // Stampa i risultati
            Console.WriteLine("\nOrdine con l'importo totale più alto:");
            PrintOrder(highestTotalOrder);

            Console.WriteLine("\nOrdine con la quantità più alta:");
            PrintOrder(highestQuantityOrder);

            Console.WriteLine("\nOrdine con la differenza maggiore tra totale e totale scontato:");
            PrintOrder(maxDiscountDifferenceOrder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'elaborazione: {ex.Message}");
        }
    }

    static string GetSourceDirectory([CallerFilePath] string sourceFilePath = "")
    {
        return Path.GetDirectoryName(sourceFilePath);
    }
    static List<Order> ReadOrders(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        try
        {
            // Converte le righe del CSV in oggetti Order
            return new List<Order>(csv.GetRecords<Order>());
        }
        catch
        {
            Console.WriteLine("Errore: Il file CSV non è formattato correttamente.");
            throw;
        }
    }

    static Order GetOrderWithHighestTotal(List<Order> orders)
    {
        Order highest = null;
        foreach (var order in orders)
        {
            if (highest == null || order.GetTotal() > highest.GetTotal())
            {
                highest = order;
            }
        }
        return highest;
    }

    static Order GetOrderWithHighestQuantity(List<Order> orders)
    {
        Order highest = null;
        foreach (var order in orders)
        {
            if (highest == null || order.Quantity > highest.Quantity)
            {
                highest = order;
            }
        }
        return highest;
    }

    static Order GetOrderWithMaxDiscountDifference(List<Order> orders)
    {
        Order highest = null;
        foreach (var order in orders)
        {
            var discountDifference = order.GetTotal() - order.GetTotalWithDiscount();
            if (highest == null || discountDifference > (highest.GetTotal() - highest.GetTotalWithDiscount()))
            {
                highest = order;
            }
        }
        return highest;
    }

    static void PrintOrder(Order order)
    {
        if (order == null)
        {
            Console.WriteLine("Nessun ordine trovato.");
            return;
        }

        Console.WriteLine($"ID: {order.Id}");
        Console.WriteLine($"Articolo: {order.ArticleName}");
        Console.WriteLine($"Quantità: {order.Quantity}");
        Console.WriteLine($"Prezzo unitario: {order.UnitPrice}");
        Console.WriteLine($"Sconto (%): {order.PercentageDiscount}");
        Console.WriteLine($"Acquirente: {order.Buyer}");
        Console.WriteLine($"Totale: {order.GetTotal():C}");
        Console.WriteLine($"Totale con sconto: {order.GetTotalWithDiscount():C}");
    }
}

class Order
{
    public int Id { get; set; }
    public string ArticleName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PercentageDiscount { get; set; }
    public string Buyer { get; set; }

    public decimal GetTotal()
    {
        return Quantity * UnitPrice;
    }

    public decimal GetTotalWithDiscount()
    {
        return GetTotal() * (1 - PercentageDiscount / 100);
    }
}
