using System;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            for (int i = 0; i < 50; i++)
            {
                var pg = new PasswordGenerator(12, Method.Special);
                Console.WriteLine(pg.Generate());
            }
        }
    }
}