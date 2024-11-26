using Jypeli;

public class Tools
{

    /// <summary>
    /// Print mouse position on world in messagedisplay and console in format: "x, y"
    /// </summary>
    /// <param name="game"></param>
    public static void PrintMousePosition(Game game)
    {
        Vector mousePos = game.Mouse.PositionOnWorld;
        string mousePosStr = $"{mousePos.X}, {mousePos.Y}";
        System.Console.WriteLine(mousePosStr);
        game.MessageDisplay.Add(mousePosStr);
    }

    public static void PrintMousePositionOnClick(Game game)
    {
        string mousePos = FormatMousePosition(game);
        game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, () => PrintMousePosition(game), null);
    }

    public static string FormatMousePosition(Game game)
    {
        Vector mousePos = game.Mouse.PositionOnWorld;
        string mousePosStr = $"{mousePos.X}, {mousePos.Y}";

        return mousePosStr;
    }

    public static Image[] LoadAnimation(string name, int frames)
    {
        Image[] images = new Image[frames];
        for (int i = 0; i < frames; i++)
        {
            string fullName = $"{name}{i:D2}";
            images[i] = Game.LoadImage(fullName);
        }

        return images;
    }
}