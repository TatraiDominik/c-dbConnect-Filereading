using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace FileReadWriteToClass
{
    class Program
    {
        static List<Series> serieses = new List<Series>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ReadFile();

            Console.WriteLine("\nBeolvasott sorozatok:");
            DisplaySeries();

            serieses.Add(new Series
            {
                Name = "Stranger Things",
                Released = "2016",
                Streaming = "Netflix"
            });

            Console.WriteLine("\nÚj sorozat hozzáadása után:");
            DisplaySeries();

            WriteFile();
            Console.WriteLine("\nAdatok fájlba írva!");
            Console.ReadKey();
        }

        private static void ReadFile()
        {
            if (!File.Exists("sorozatok.txt"))
            {
                Console.WriteLine("A sorozatok.txt fájl nem található!");
                return;
            }

            StreamReader readFile = new StreamReader("sorozatok.txt");
            while (!readFile.EndOfStream)
            {
                string line = readFile.ReadLine();
                string[] data = line.Split(';');
                serieses.Add(new Series
                {
                    Name = data[0],
                    Released = data[1],
                    Streaming = data[2]
                });
            }
            readFile.Close();
        }

        private static void WriteFile()
        {
            StreamWriter writeFile = new StreamWriter("sorozatok.txt");
            foreach (Series series in serieses)
            {
                string line = series.Name + ";" + series.Released + ";" + series.Streaming;
                writeFile.WriteLine(line);
            }
            writeFile.Close();
        }

        private static void DisplaySeries()
        {
            foreach (Series series in serieses)
            {
                Console.WriteLine($"Név: {series.Name}, Megjelent: {series.Released}, Platform: {series.Streaming}");
            }
        }
    }

    internal class Series
    {
        public string Name { get; set; }
        public string Released { get; set; }
        public string Streaming { get; set; }
    }
}