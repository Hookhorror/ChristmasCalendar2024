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


    /// <summary>
    /// Prints current mouse world position in Jypeli Messagedisplay and console
    /// </summary>
    /// <param name="game"></param>
    public static void PrintMousePositionOnClick(Game game)
    {
        string mousePos = FormatMousePosition(game);
        game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, () => PrintMousePosition(game), null);
    }


    /// <summary>
    /// Fetches current mouseposition in game and formats it
    /// </summary>
    /// <param name="game"></param>
    /// <returns>Mouseposition in format "x, y"</returns>
    public static string FormatMousePosition(Game game)
    {
        Vector mousePos = game.Mouse.PositionOnWorld;
        string mousePosStr = $"{mousePos.X}, {mousePos.Y}";

        return mousePosStr;
    }


    /// <summary>
    /// Loads png images numbered from 01 to nn
    /// </summary>
    /// <param name="name">Name of images without numbering</param>
    /// <param name="frames">Number of images</param>
    /// <returns>Array of Images</returns>
    public static Image[] LoadNumberedImages(string name, int frames)
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