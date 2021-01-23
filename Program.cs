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

            // Options
            var optionsCommand = new Command("options", "Custom options")
            {
                new Option<int>(
                    "--l",
                    getDefaultValue: () => 8,
                    description: "Length of password"),
                new Option<int>(
                    "--t",
                    getDefaultValue: () => 2,
                    description: $"1: only letters; 2: with numbers; 3: with {PasswordGenerator.SymbolChars}"),
            };
            optionsCommand.AddAlias("-o");
            optionsCommand.Handler = CommandHandler.Create<int, int>(OptionsCommandFunc);
            rootCommand.AddCommand(optionsCommand);

            // Run
            await rootCommand.InvokeAsync(args);
        }

        private static void OptionsCommandFunc(int l, int t)
        {
            var method = t.ToEnum<Method>();

            if (!method.HasValue)
            {
                method = Method.Strong;
                Console.WriteLine($"Cannot find value {t}");
            }

            var pg = new PasswordGenerator(l, method.Value);
            Console.WriteLine(pg.Generate());
        }

        private static T? ToEnum<T>(this int value) where T : struct
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }

        private static T? ToEnum<T>(this string value) where T : struct
        {
            if (Enum.TryParse(value, true, out T result))
                return result;

            return null;
        }
    }
}