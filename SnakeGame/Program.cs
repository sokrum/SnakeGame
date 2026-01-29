using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static int width = 50;
    static int height = 20;

    static int x = width / 2;
    static int y = height / 2;

    static int dx = 1;
    static int dy = 0;

    static List<(int x, int y)> snake = new();

    static bool gameOver = false;

    static int foodX;
    static int foodY;
    static int score = 0;

    static Random random = new Random();

    static void Main()
    {
        Console.CursorVisible = false;
        Console.SetWindowSize(width, height);

        snake.Add((x, y));
        SpawnFood();

        while (!gameOver)
        {
            Input();
            Logic();
            Draw();
            Thread.Sleep(100);
        }

        Console.Clear();
        Console.SetCursorPosition(10, 10);
        Console.WriteLine("GAME OVER");
        Console.SetCursorPosition(10, 11);
        Console.WriteLine("Score: " + score);
        Console.ReadKey();
    }

    static void SpawnFood()
    {
        foodX = random.Next(1, width - 2);
        foodY = random.Next(1, height - 2);
    }

    static void Input()
    {
        if (!Console.KeyAvailable) return;

        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                dx = 0; dy = -1;
                break;
            case ConsoleKey.DownArrow:
                dx = 0; dy = 1;
                break;
            case ConsoleKey.LeftArrow:
                dx = -1; dy = 0;
                break;
            case ConsoleKey.RightArrow:
                dx = 1; dy = 0;
                break;
        }
    }

    static void Logic()
    {
        x += dx;
        y += dy;

        // ŚCIANY
        if (x <= 0 || x >= width - 1 || y <= 0 || y >= height - 1)
            gameOver = true;

        // KOLIZJA Z CIAŁEM
        foreach (var part in snake)
            if (part.x == x && part.y == y)
                gameOver = true;

        snake.Insert(0, (x, y));

        // JEDZENIE
        if (x == foodX && y == foodY)
        {
            score++;
            SpawnFood();
        }
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }
    }

    static void Draw()
    {
        Console.Clear();

        // ŚCIANY
        for (int i = 0; i < width; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
            Console.SetCursorPosition(i, height - 1);
            Console.Write("#");
        }

        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");
            Console.SetCursorPosition(width - 1, i);
            Console.Write("#");
        }

        // JEDZENIE
        Console.SetCursorPosition(foodX, foodY);
        Console.Write("*");

        // WĄŻ
        foreach (var part in snake)
        {
            Console.SetCursorPosition(part.x, part.y);
            Console.Write("O");
        }

        // SCORE
        Console.SetCursorPosition(2, height - 1);
        Console.Write("Score: " + score);

    }
}
