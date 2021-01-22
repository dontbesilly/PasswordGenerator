using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;

namespace PasswordGenerator
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                var pg = new PasswordGenerator();
                Console.WriteLine(pg.Generate());
            }
            else
            {
                await CliCommand(args);
            }
        }

        private static async Task CliCommand(string[] args)
        {
            var rootCommand = new RootCommand("Password generator cli")
            {
                Handler = CommandHandler.Create(() => { Console.WriteLine("For help -h"); })
            };

            // Version (without arguments)
            var versionCommand = new Command("version", "Assembly version");
            versionCommand.AddAlias("-v");
            versionCommand.AddAlias("--version");
            versionCommand.Handler = CommandHandler.Create(() =>
            {
                var vers = Assembly.GetEntryAssembly()?.GetName().Version;
                Console.WriteLine(vers);
            });
            rootCommand.AddCommand(versionCommand);

            // Length command (only 1 argument)
            var lengthCommand = new Command("length", "Password length");
            lengthCommand.AddAlias("-l");
            lengthCommand.AddArgument(new Argument<int>("length"));
            lengthCommand.Handler = CommandHandler.Create<int>(length =>
            {
                var pg = new PasswordGenerator(length);
                Console.WriteLine(pg.Generate());
            });
            rootCommand.AddCommand(lengthCommand);

            // Run
            await rootCommand.InvokeAsync(args);
        }
    }
}