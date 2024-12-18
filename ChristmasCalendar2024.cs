using System;
using Jypeli;


public class ChristmasCalendar2024 : PhysicsGame
{
    private readonly CalendarLid[] calendarLids = new CalendarLid[24];
    private readonly LidContentInterface[] content = new LidContentInterface[24];
    private readonly bool[] OpenedLids = new bool[24];

    public override void Begin()
    {
        InitCalendar();
    }

    public void InitCalendar()
    {
        ClearAll();
        MediaPlayer.Stop();
        MasterVolume = 0.5;
        Level.BackgroundColor = Color.Black;
        Level.Background.Image = Resources.ChristmasTree;

        content[0] = new SnowballThrowingGame(this);
        content[1] = new StoryPlayer(this, "Kalju metsämies");
        content[2] = new StoryPlayer(this, "Mehiläistarhuri");
        content[3] = new StoryPlayer(this, "Linnut, villipedot ja lepakko");
        content[4] = new StoryPlayer(this, "Nokkava matkamies");
        content[5] = new StoryPlayer(this, "Pojan uimaretki");
        content[6] = new ClickingGame(this);
        content[7] = new StoryPlayer(this, "Poika ja pähkinät");
        content[8] = new StoryPlayer(this, "Poika ja susi");
        content[9] = new StoryPlayer(this, "Häkkilintu ja lepakko");
        content[10] = new StoryPlayer(this, "Rapuäiti ja sen poika");
        content[11] = new StoryPlayer(this, "Varis ja vesikannu");
        content[12] = new StoryPlayer(this, "Koira ja sen peilikuva");
        content[13] = new CatchGifts(this);
        content[14] = new StoryPlayer(this, "Tähtitieteilijä");
        content[15] = new StoryPlayer(this, "Ennustaja");
        content[16] = new StoryPlayer(this, "Kettu ja piikkipensas");
        content[17] = new StoryPlayer(this, "Kettu ja korppi");
        content[18] = new StoryPlayer(this, "Kettu ja rypäleet");
        content[19] = new StoryPlayer(this, "Kettu ja haikara");
        content[20] = new StoryPlayer(this, "Heinäsirkka ja muurahaiset");
        content[21] = new StoryPlayer(this, "Jänis ja kilpikonna");
        content[22] = new StoryPlayer(this, "Hevonen ja ratsastaja");
        content[23] = new DodgeTheWalls(this);

        double sideLength = 100;
        for (int i = 0; i < calendarLids.Length; i++)
        {
            CalendarLid cl = new CalendarLid(sideLength, sideLength, i + 1);
            calendarLids[i] = cl;

            if (OpenedLids[i]) cl.Open(true);

            if (content[i] != null)
            {
                LidContentInterface game = content[i];

                Mouse.ListenOn(calendarLids[i], MouseButton.Left, ButtonState.Pressed, () => StartGame(game, cl), null);
            }
            Mouse.ListenOn(cl, MouseButton.Left, ButtonState.Pressed, () => HandleOpen(cl), null);
        }

        SuffleLids();
        AddLids(sideLength);

        AddControls();
        Camera.ZoomToAllObjects(50);
    }

    private void HandleOpen(CalendarLid cl)
    {
        cl.Open();
        OpenedLids[cl.lidNumber - 1] = true;
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

