using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static int width = 50;
    static int height = 20; // plansza: wiersze 0..height-1, linia score: height

    // Snake 1
    static int x1 = width / 2;
    static int y1 = height / 2;
    static int dx1 = 1;
    static int dy1 = 0;
    static List<(int x, int y)> snake1 = new();

    // Snake 2
    static int x2 = width / 2;
    static int y2 = height / 2 + 3;
    static int dx2 = -1;
    static int dy2 = 0;
    static List<(int x, int y)> snake2 = new();

    static int foodX;
    static int foodY;
    static int score = 0;

    static bool gameOver = false;
    static Random random = new Random();

    static void Main()
    {
        Console.CursorVisible = false;

        // +1 linia na score
        Console.SetWindowSize(width, height + 1);
        Console.SetBufferSize(width, height + 1);

        snake1.Add((x1, y1));
        snake2.Add((x2, y2));

        SpawnFood();

        while (!gameOver)
        {
            Input();
            Logic();
            Draw();
            Thread.Sleep(90);
        }

        Console.Clear();
        Console.SetCursorPosition(10, 8);
        Console.WriteLine("GAME OVER");
        Console.SetCursorPosition(10, 9);
        Console.WriteLine("Score: " + score);
        Console.ReadKey(true);
    }

    static void SpawnFood()
    {
        while (true)
        {
            int fx = random.Next(1, width - 2);
            int fy = random.Next(1, height - 2);

            bool onSnake1 = snake1.Any(p => p.x == fx && p.y == fy);
            bool onSnake2 = snake2.Any(p => p.x == fx && p.y == fy);

            if (!onSnake1 && !onSnake2)
            {
                foodX = fx;
                foodY = fy;
                return;
            }
        }
    }

    static void Input()
    {
        // czyścimy kolejkę klawiszy, żeby reagowało szybko
        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;

            // Snake 1: strzałki
            if (key == ConsoleKey.UpArrow && dy1 != 1) { dx1 = 0; dy1 = -1; }
            if (key == ConsoleKey.DownArrow && dy1 != -1) { dx1 = 0; dy1 = 1; }
            if (key == ConsoleKey.LeftArrow && dx1 != 1) { dx1 = -1; dy1 = 0; }
            if (key == ConsoleKey.RightArrow && dx1 != -1) { dx1 = 1; dy1 = 0; }

            // Snake 2: WASD
            if (key == ConsoleKey.W && dy2 != 1) { dx2 = 0; dy2 = -1; }
            if (key == ConsoleKey.S && dy2 != -1) { dx2 = 0; dy2 = 1; }
            if (key == ConsoleKey.A && dx2 != 1) { dx2 = -1; dy2 = 0; }
            if (key == ConsoleKey.D && dx2 != -1) { dx2 = 1; dy2 = 0; }
        }
    }

    static void Logic()
    {
        // następna pozycja głowy
        int nx1 = x1 + dx1;
        int ny1 = y1 + dy1;

        int nx2 = x2 + dx2;
        int ny2 = y2 + dy2;

        // ściany
        if (HitWall(nx1, ny1) || HitWall(nx2, ny2))
        {
            gameOver = true;
            return;
        }

        // kolizja z własnym ciałem (sprawdzamy przyszłą głowę)
        if (snake1.Any(p => p.x == nx1 && p.y == ny1) || snake2.Any(p => p.x == nx2 && p.y == ny2))
        {
            gameOver = true;
            return;
        }

        // kolizja między wężami
        // głowa 1 w ciało 2 / głowa 2 w ciało 1
        if (snake2.Any(p => p.x == nx1 && p.y == ny1) || snake1.Any(p => p.x == nx2 && p.y == ny2))
        {
            gameOver = true;
            return;
        }

        // głowa-głowa
        if (nx1 == nx2 && ny1 == ny2)
        {
            gameOver = true;
            return;
        }

        bool eat1 = (nx1 == foodX && ny1 == foodY);
        bool eat2 = (nx2 == foodX && ny2 == foodY);

        // wykonaj ruch
        x1 = nx1; y1 = ny1;
        x2 = nx2; y2 = ny2;

        snake1.Insert(0, (x1, y1));
        snake2.Insert(0, (x2, y2));

        // jedzenie
        if (eat1 || eat2)
        {
            score++;
            SpawnFood();
        }
        else
        {
            // nie rosną -> ucinamy ogon
            snake1.RemoveAt(snake1.Count - 1);
            snake2.RemoveAt(snake2.Count - 1);
        }
    }

    static bool HitWall(int x, int y)
    {
        return x <= 0 || x >= width - 1 || y <= 0 || y >= height - 1;
    }

    static void Draw()
    {
        Console.Clear();

        // ściany
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

        // jedzenie
        Console.SetCursorPosition(foodX, foodY);
        Console.Write("*");

        // snake 1
        foreach (var part in snake1)
        {
            Console.SetCursorPosition(part.x, part.y);
            Console.Write("O");
        }

        // snake 2 (inny znak)
        foreach (var part in snake2)
        {
            Console.SetCursorPosition(part.x, part.y);
            Console.Write("X");
        }

        // score - ostatnia linia
        Console.SetCursorPosition(2, height);
        Console.Write("Score: " + score + "   |  P1: arrows  P2: WASD");
    }
}
