using Jypeli;


public class ChristmasCalendar2024 : PhysicsGame
{
    private readonly CalendarLid[] calendarLids = new CalendarLid[24];
    private readonly LidContentInterface[] content = new LidContentInterface[24];

    public override void Begin()
    {
        InitCalendar();
    }

    public void InitCalendar()
    {
        ClearAll();
        MasterVolume = 0.5;
        Level.BackgroundColor = Color.Black;
        Level.Background.Image = LoadImage("ChristmasTree.jpg");

        content[0] = new SnowballThrowingGame(this);
        content[1] = new StoryPlayer(this, "Kalju metsämies");
        content[2] = new StoryPlayer(this, "Mehiläistarhuri");
        content[3] = new StoryPlayer(this, "Linnut, villipedot ja lepakko");
        content[4] = new StoryPlayer(this, "Nokkava matkamies");
        content[5] = new StoryPlayer(this, "Pojan uimaretki");
        content[6] = new ClickingGame(this);
        // content[7] = new StoryPlayer(this, "Poika ja pähkinät");
        // content[8] = new StoryPlayer(this, "Poika ja susi");

        double sideLength = 100;
        for (int i = 0; i < calendarLids.Length; i++)
        {
            CalendarLid cl = new CalendarLid(sideLength, sideLength, i + 1);
            calendarLids[i] = cl;
            if (content[i] != null)
            {
                LidContentInterface game = content[i];

                Mouse.ListenOn(calendarLids[i], MouseButton.Left, ButtonState.Pressed, () => StartGame(game, cl), null);
            }
            Mouse.ListenOn(cl, MouseButton.Left, ButtonState.Pressed, cl.Open, null);
        }

        SuffleLids();
        AddLids(sideLength);

        AddControls();
        Camera.ZoomToAllObjects(50);
    }

    private void StartGame(LidContentInterface game, CalendarLid cl)
    {
        if (cl.Opened)
        {
            game.Start();
        }
    }

    private void AddLids(double side)
    {
        int columns = 8;
        int rows = 3;
        int gap = 50;
        double centerX = (columns / 2.0) * (side + gap) - side / 2;
        double centerY = (rows / 2.0) * (side + gap) - gap - side / 2;
        for (int i = 0; i < calendarLids.Length; i++)
        {
            CalendarLid cl = calendarLids[i];
            cl.X = (i % columns) * (gap + side) - centerX;
            cl.Y = (i / columns) * -(gap + side) + centerY;
            Add(cl);
        }
    }

    private void SuffleLids()
    {
        // CalendarLid[] suffledLids = new CalendarLid[calendarLids.Length];
        for (int i = 0; i < calendarLids.Length; i++)
        {
            int r = RandomGen.NextInt(calendarLids.Length);
            CalendarLid tempWhere = calendarLids[r];
            CalendarLid tempFrom = calendarLids[i];
            calendarLids[r] = tempFrom;
            calendarLids[i] = tempWhere;
        }
    }


    private void AddControls()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }



}

