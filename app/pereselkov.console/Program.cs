using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    private static int n; // количество вершин
    private static string filePath = "map.txt";

    static void Main()
    {
        bool entervalue = true;

        // запись данных в файл
        Console.WriteLine("вводите строки вида: <начальная точк> <конечная точка> <расстояние>");
        Console.WriteLine("для завершения ввода введите 0 в качестве первой точки");

        while (entervalue)
        {
            string value = Console.ReadLine();
            string[] elements = value.Split(' ');

            if (elements.Length < 3)
            {
                Console.WriteLine("неправильный ввод. прочитайте условие");
                continue;
            }

            int.TryParse(elements[0], out int firstpoint);
            int.TryParse(elements[1], out int lastpoint);
            double destination;

            if (!double.TryParse(elements[2], out destination))
            {
                Console.WriteLine("некорректное расстояние. ведите число");
                continue;
            }

            if (firstpoint == 0)
            {
                Console.WriteLine("вывод завершён");
                entervalue = false;
                break;
            }

            File.AppendAllText(filePath, $"{firstpoint} {lastpoint} {destination}\n");
        }

        // чтение данных и построение матрицы смежности
        double[,] graph = ReadGraphFromFile();

        // применение алгоритма Флойда
        double[,] shortestPaths = Floyd(graph);

        Console.WriteLine("\nвведите три точки (A B C) для построения маршрута A > B > C");
        Console.WriteLine("для выхода введите 0 в качестве первой точки");

        while (true)
        {
            Console.Write("точки (A B C): ");
            string input = Console.ReadLine();
            string[] parts = input.Split(' ');

            if (parts.Length != 3)
            {
                Console.WriteLine("нужно ввести ровно три числа");
                continue;
            }

            int.TryParse(parts[0], out int A);
            int.TryParse(parts[1], out int B);
            int.TryParse(parts[2], out int C);

            if (A == 0)
            {
                Console.WriteLine("завершение программы");
                break;
            }

            // Проверка корректности номеров точек
            if (A < 1 || A > n || B < 1 || B > n || C < 1 || C > n)
            {
                Console.WriteLine("номера точек должны быть от 1 до " + n);
                continue;
            }

            //  маршрут A B C
            double pathAB = shortestPaths[A - 1, B - 1];
            double pathBC = shortestPaths[B - 1, C - 1];

            if (double.IsPositiveInfinity(pathAB) || double.IsPositiveInfinity(pathBC))
            {
                Console.WriteLine("маршрут невозможен (нет пути между точками)");
                continue;
            }

            double totalLength = pathAB + pathBC;

            Console.WriteLine($"маршрут: {A} > {B} > {C}");
            Console.WriteLine($"длина: {totalLength:F2}");
        }
    }

    private static double[,] ReadGraphFromFile()
    {
        string[] lines = File.ReadAllLines(filePath);
        var edges = new List<(int, int, double)>();

        foreach (string line in lines)
        {
            string[] parts = line.Split(' ');
            if (parts.Length >= 3)
            {
                int u = int.Parse(parts[0]);
                int v = int.Parse(parts[1]);
                double w = double.Parse(parts[2]);
                edges.Add((u, v, w));
            }
        }

        // количество вершин
        n = edges.Max(e => Math.Max(e.Item1, e.Item2));

        // матрица смежности (n x n)
        double[,] graph = new double[n, n];

        // матрица бесконечностью ребра отсутствуют
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                graph[i, j] = double.PositiveInfinity;

        // диагональ — нули
        for (int i = 0; i < n; i++)
            graph[i, i] = 0;

        // добавление рёбра
        foreach (var (u, v, w) in edges)
        {
            graph[u - 1, v - 1] = w;
            // если граф неориентированный, добавление обратного ребра
            // graph[v - 1, u - 1] = w;
        }

        return graph;
    }

    private static double[,] Floyd(double[,] a)
    {
        int n = a.GetLength(0);
        double[,] d = new double[n, n];
        d = (double[,])a.Clone();
        for (int i = 1; i <= n; i++)
            for (int j = 0; j <= n - 1; j++)
                for (int k = 0; k <= n - 1; k++)
                    if (d[j, k] > d[j, i - 1] + d[i - 1, k])
                        d[j, k] = d[j, i - 1] + d[i - 1, k];

        return d;
    }
}
