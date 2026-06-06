using System;

namespace UserManager;

class Authentication
{
    public static async Task Run()
    {
        int cols = Console.WindowWidth;
        int rows = Console.WindowHeight;

        Console.Clear();

        Console.SetCursorPosition(cols/2 - 5, rows /2 - 1);
        Console.Write("Username: ");

        string username = Console.ReadLine();

        Console.SetCursorPosition(cols/2 - 5, rows /2);
        Console.Write("Password: ");

        string password = "";

        while(true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            char c = keyInfo.KeyChar;

            if(keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            } else if(keyInfo.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
            } else {
                password += c;
                Console.Write("*");
            }
        }

        Console.WriteLine(password);
    }
}

