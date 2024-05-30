using System;
using System.IO;
using System.Globalization;

// namespace B3Alert
// {
    public class Program
    {
        public static void Main(string[] args)
        {
            // Console.WriteLine("Informe o ativo a ser monitorado: ");
            // string stock = Console.ReadLine();

            // Console.WriteLine("Informe o preço de referência para venda:");
            // decimal sellPrice = decimal.Parse(Console.ReadLine());

            // Console.WriteLine("Informe o preço de referência para compra:");
            // decimal buyPrice = decimal.Parse(Console.ReadLine());

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: dotnet run -- <stock> <sell-price> <buy-price>");
                return;
            }

            string stock = args[0];
            Console.WriteLine(stock);
            decimal sellPrice = decimal.Parse(args[1], CultureInfo.InvariantCulture); // InvariantCulture garante que o ponto será considerado como separador decimal
            decimal buyPrice = decimal.Parse(args[2], CultureInfo.InvariantCulture);

            Console.WriteLine(stock + sellPrice + buyPrice);
        }
    }
// }
