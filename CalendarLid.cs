using System;
using Jypeli;

class CalendarLid : GameObject
{
    public readonly int lidNumber; // TODO: what is the right way in c# for getters
    // private readonly bool opened;

    public CalendarLid(double width, double height, int lidNumber) : base(width, height)
    {
        this.lidNumber = lidNumber;
        // opened = false;
        PlaceLidNumber();
    }

    private void PlaceLidNumber()
    {
        // GameObject lidNumber = new GameObject(Width, Height);
        Label lidText = new Label(Width / 2, Height / 2, lidNumber + "");
        this.Add(lidText);
    }
}