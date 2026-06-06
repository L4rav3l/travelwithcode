using System;
using UserManager;

class Program
{
    public static async Task Main()
    {
        await Authentication.Run();
    }
}