using System;
using System.Collections.Generic;
using System.Net;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace ChristmasCalendar2024;

public class ChristmanCalendar2024 : PhysicsGame
{
    private readonly CalendarLid[] calendarLids = new CalendarLid[24];
    private readonly LidContentInterface[] content = new LidContentInterface[24];

    // TODO Lid opening restriction

    public override void Begin()
    {
        ClearAll();
        MasterVolume = 0.5;
        // Level.Background.CreateGradient(Color.Green, Color.Red);
        // Level.Background.Image = LoadImage("ChristmasTree.jpg");
        content[0] = new SnowballThrowingGame(this);
        content[1] = new StoryPlayer(this, "example");
        content[2] = new ClickingGame(this);

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

        // MessageDisplay.MessageTime = new TimeSpan(0, 0, 3);


        SuffleLids();
        AddLids(sideLength);

        AddControls();
        Camera.ZoomToAllObjects(50);



        // game.StartGame();
        // game.Run();

        // Camera.Follow(pelaaja1);
        // Camera.ZoomFactor = 1.2;
        // Camera.StayInLevel = true;

        // MasterVolume = 0.5;
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
        for (int i = 0; i < calendarLids.Length; i++)
        {
            CalendarLid cl = calendarLids[i];
            cl.X = (i % 8) * (50 + side);
            cl.Y = (i / 8) * -(50 + side);
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

