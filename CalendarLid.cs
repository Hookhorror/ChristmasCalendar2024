using System;
using Jypeli;

class CalendarLid : GameObject
{
    public readonly int lidNumber;
    private static readonly Image[] LidOpeningImages = Tools.LoadAnimation("LidOpening", 13);
    private static readonly SoundEffect CreakingDoor = Game.LoadSoundEffect("CreakingDoor.wav");
    public Animation LidOpening = new Animation(LidOpeningImages);
    public bool Opened { get; private set; }

    public CalendarLid(double width, double height, int lidNumber) : base(width, height)
    {
        this.lidNumber = lidNumber;
        // opened = false;
        PlaceLidNumber();
        // this.Image = LidOpeningImages[0];
        this.Animation = LidOpening;
        this.Animation.StopOnLastFrame = true;
        this.Animation.Played += this.Animation.Stop;
        LidOpening.FPS = 10;
        // LidOpening.Start();
    }

    private void PlaceLidNumber()
    {
        // GameObject lidNumber = new GameObject(Width, Height);
        Label lidText = new Label(Width / 2, Height / 2, lidNumber + "");
        this.Add(lidText);
    }

    public void Open()
    {
        if (!Opened)
        {
            Opened = true;
            LidOpening.Start();
            CreakingDoor.Play();
        }
        // TODO Image for game
    }
}