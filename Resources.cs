using Jypeli;

public class Resources
{
    public static readonly SoundEffect[] ClickingGameSounds = Game.LoadSoundEffects("Tap", "Spring", "Pig", "Sheep");
    public static readonly SoundEffect[] CatchGiftsSounds = Game.LoadSoundEffects("Plop", "CoinDrop", "MagicSparkle", "JingleBell");
    public static readonly SoundEffect PlayerDeath = Game.LoadSoundEffect("Fail");
    public static readonly Image Toy = Game.LoadImage("Toy1");
    public static readonly Shape ToyShape = Shape.FromImage(Toy);
    public static readonly Image[] ToyImages = Game.LoadImages("Toy1", "Toy2", "Toy3");
    public static readonly Image[] GiftImages = Game.LoadImages("Coal", "Gift1", "Gift2", "Gift3", "Gift4");
    public static readonly Shape[] GiftShapes = Tools.ShapesFromImages(GiftImages);
    public static readonly Image Pillar = Game.LoadImage("Pillar");
    public static readonly Image ChristmasTree = Game.LoadImage("ChristmasTree");
    // TODO Add more resources
}