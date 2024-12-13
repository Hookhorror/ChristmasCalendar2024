using System;
using Jypeli;

class CalendarLid : GameObject
{
    public readonly int lidNumber;
    private static readonly Image[] LidOpeningImages = Tools.LoadNumberedImages("LidOpening", 13);
    private static readonly SoundEffect CreakingDoor = Game.LoadSoundEffect("CreakingDoor.wav");
    public Animation LidOpening = new Animation(LidOpeningImages);
    public bool Opened { get; private set; }

    public CalendarLid(double width, double height, int lidNumber) : base(width, height)
    {
        this.lidNumber = lidNumber;
        PlaceLidNumber();
        this.Animation = LidOpening;
        this.Animation.StopOnLastFrame = true;
        this.Animation.Played += this.Animation.Stop;
        LidOpening.FPS = 10;
    }

    private void PlaceLidNumber()
    {
        Label lidText = new Label(Width / 2, Height / 2, lidNumber + "");
        this.Add(lidText);
    }

    public void Open(bool silently = false)
    {
        if (CanYouOpen())
        {

            if (!Opened)
            {
                Opened = true;
                LidOpening.Start();
                if (!silently)
                {
                    CreakingDoor.Play();
                }
            }
        }
        // TODO Image for game
        // TODO Keep track of opened lids
    }

    private bool CanYouOpen()
    {
        DateTime currentDay = DateTime.Now;

        // TODO Put these back
        // if (currentDay.Month == 12 || currentDay.Year > 2024)
        {
            // if (currentDay.Day >= lidNumber || currentDay.Year > 2024)
            {
                return true;
            }
            return false;
        }

        return false;
    }
}