using Common;

namespace Hyper;
internal class CommandInterpreter
{
    private readonly Window _window;

    public CommandInterpreter(Window window)
    {
        _window = window;
    }

    public void ParseLine(string line, ref Game game, out bool exitCli)
    {
        var args = line.Split(' ').Select(x => x.Trim().ToLower()).ToArray();

        if (args.Length == 0)
        {
            exitCli = true;
            return;
        }

        switch (args[0])
        {
            case "help":
                ShowHelp();
                break;

            case "resume":
                exitCli = true;
                return;

            case "exit":
                exitCli = true;
                _window.Close();
                return;

            case "load":
                if (args.Length < 2)
                    throw ExpectedArgumentFor("load");
                if (!SaveManager.GetSaves().Contains(args[1]))
                    throw new FileNotFoundException("This save does not exist.");
                if (game.IsRunning)
                    game.SaveAndClose();

                game = new Game(_window.Size.X, _window.Size.Y, new WindowHelper(_window), args[1], GeometryType.Euclidean);
                _window.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;
                break;

            case "delete":
                if (args[1] == "*")
                {
                    SaveManager.DeleteAllSaves();
                    break;
                }
                else if (args.Length >= 2)
                {
                    SaveManager.DeleteSaves(args[1..]);
                }
                else
                    throw ExpectedArgumentFor("delete");
                break;

            case "show":
                if (args.Length < 2)
                    throw ExpectedArgumentFor("show");
                if (args[1] == "saves")
                {
                    foreach (var file in SaveManager.GetSaves())
                    {
                        Console.WriteLine(file);
                    }
                }
                else
                    throw new ArgumentException("Unknown option");
                break;

            case "new":
                if (args.Length < 2)
                    throw ExpectedArgumentFor("new");
                if (Enum.TryParse<GeometryType>(args[1], ignoreCase: true, out var geometryType))
                {
                    string saveName;
                    if (args.Length == 3)
                        saveName = args[2];
                    else
                        saveName = DefaultSaveName();
                    game = new Game(_window.Size.X, _window.Size.Y, new WindowHelper(_window), saveName, geometryType);
                }
                else
                    throw new ArgumentException("Unknown geometry type");
                break;
            default:
                throw new ArgumentException("Unknown command");
        }

        exitCli = false;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("""
            Available commands:
                resume
                exit
                load <save name>
                delete (* | <save name>...)
                show saves
                new (euclidean | hyperbolic | spherical) <save name>?
                help <command>?
            """);
    }

    private static string DefaultSaveName()
        => DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");

    ArgumentException ExpectedArgumentFor(string commandName)
        => new("Expected argument for " + commandName);
}
