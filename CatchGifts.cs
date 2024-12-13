using System;
using Jypeli;
using Jypeli.Widgets;

public class CatchGifts : LidContentInterface
{
    private const double DifficultyTime = 30;
    public const double DropTime = 2;
    private static readonly string HighScorePath = "CatchGiftsHighScore.xml";
    private static readonly double[] ItemSpeeds = { -250, -150, -175, -200, -225 };
    private static readonly int[,] DropRates = {{5,50,30,10,5}
                                               ,{10,40,25,15,10}
                                               ,{20,30,20,15,15}
                                               ,{30,25,15,15,15}
                                               ,{40,15,15,15,15}};
    private readonly IntMeter _points = new IntMeter(0);
    private ScoreList _highScore = new ScoreList(10, false, 0);
    private readonly ChristmasCalendar2024 _game;
    private PlatformCharacter _player;
    private int _difficulty;


    public CatchGifts(ChristmasCalendar2024 game)
    {
        _game = game;
    }

    public void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        _game.ClearAll();

        _difficulty = 1;
        AddLevel();
        AddPlayer();
        AddControls();
        AddTimers();
        AddUI();
    }

    private void AddUI()
    {
        _points.Value = 0;

        Label points = new Label();
        points.Title = "Pisteet: ";
        points.Position = new Vector(_game.Level.Right - 250, _game.Level.Center.Y + _game.Level.Height / 3);
        points.TextColor = Color.Black;
        points.BindTo(_points);

        _game.Add(points);
    }

    private void AddPlayer()
    {
        _player = new PlatformCharacter(100, 100);
        _player.Image = Resources.Toy;
        // _player.Shape = Resources.ToyShape;
        _player.Position = new Vector(_game.Level.Center.X, _game.Level.Bottom);
        _player.CanRotate = false;
        _player.Tag = "player";
        _player.Animation = new Animation(new Image[] { Resources.ToyImages[0], Resources.ToyImages[1], Resources.ToyImages[2], Resources.ToyImages[2] });
        _player.Animation.FPS = 5;
        _player.Animation.Start();

        _game.Add(_player);
    }

    private void AddLevel()
    {
        _game.Level.Background.Color = Color.SeaGreen;
        _game.Level.Size = new Vector(1024, 1024);

        PhysicsObject bottom = _game.Level.CreateBottomBorder();
        bottom.Color = Color.White;
        bottom.Tag = "BottomBorder";
        PhysicsObject leftBorder = _game.Level.CreateLeftBorder();
        leftBorder.Image = Resources.Pillar;
        PhysicsObject rightBorder = _game.Level.CreateRightBorder();
        rightBorder.Image = Resources.Pillar;

        _game.Camera.ZoomToLevel(50);
    }

    private void AddTimers()
    {
        Timer itemDrop = new Timer();
        itemDrop.Interval = DropTime;
        itemDrop.Timeout += AddItems;
        itemDrop.Start();

        Timer increaseDifficulty = new Timer();
        increaseDifficulty.Interval = DifficultyTime;
        increaseDifficulty.Timeout += RaiseDifficulty;
        increaseDifficulty.Start();
    }

    private void RaiseDifficulty()
    {
        if (_difficulty < DropRates.GetLength(0))
        {
            _difficulty++;
        }
    }

    private void AddItems()
    {
        int random = RandomGen.NextInt(_difficulty) + 1;

        for (int i = 0; i < random; i++)
        {
            AddItem();
        }
    }

    private void AddItem()
    {
        PhysicsObject item = CreateRandomItem();
        _game.AddCollisionHandler(item, ItemCollision);

        _game.Add(item);
    }

    private PhysicsObject CreateRandomItem()
    {
        PhysicsObject item = new PhysicsObject(50, 50);
        item.Y = _game.Level.Top + 100 + RandomGen.NextInt(300);
        item.X = RandomGen.NextDouble(_game.Level.Left + 50, _game.Level.Right - 50);
        item.Color = Color.White;
        item.Mass = 0.1;
        item.CollisionIgnoreGroup = 1;

        int random = RandomGen.NextInt(100);
        int chance = 0;
        for (int i = 0; i < DropRates.GetLength(1); i++)
        {
            chance += DropRates[_difficulty - 1, i];
            if (random < chance)
            {
                item.Image = Resources.GiftImages[i];
                item.Shape = Resources.GiftShapes[i];
                item.Velocity = Vector.UnitY * ItemSpeeds[i];

                if (i == 0) // Coal
                {
                    Timer followPlayer = new Timer();
                    followPlayer.Interval = 0.1;
                    followPlayer.Timeout += () => FollowPlayer(item);
                    followPlayer.Start();

                    item.Destroyed += () => followPlayer = null;
                }

                break;
            }
        }

        return item;
    }

    private void FollowPlayer(PhysicsObject follower)
    {
        Vector direction = _player.Position - follower.Position;
        int sign = Math.Sign(direction.X);
        follower.Velocity += Vector.UnitX * sign * _difficulty;
    }

    private void ItemCollision(PhysicsObject collider, PhysicsObject target)
    {
        if (target.Tag.ToString().Equals("player"))
        {
            AddPoints(collider);
        }
        collider.Destroy();
    }

    private void AddPoints(PhysicsObject collider)
    {
        for (int i = 0; i < Resources.GiftShapes.Length; i++)
        {
            if (collider.Image.Equals(Resources.GiftImages[i]))
            {
                if (i == 0)
                {
                    GameOver();
                    return;
                }
                _points.Value += i;
                Resources.CatchGiftsSounds[i - 1].Play();
                return;
            }
        }
    }

    private void GameOver()
    {
        _game.Keyboard.Disable(Key.A);
        _game.Keyboard.Disable(Key.D);
        _game.Keyboard.Disable(Key.Left);
        _game.Keyboard.Disable(Key.Right);

        Resources.PlayerDeath.Play();
        _game.MessageDisplay.Add("Hävisit pelin :(");
        _game.StopAll();
        _game.ClearTimers();
        NewHighScores();
    }

    private void AddControls()
    {
        _game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, OpenMenu, "Pausettaa pelin ja avaa menun");
        _game.Keyboard.Listen(Key.F1, ButtonState.Pressed, _game.ShowControlHelp, "Näyttää nämä ohjeet");

        _game.Keyboard.Listen(Key.A, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        _game.Keyboard.Listen(Key.A, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.D, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        _game.Keyboard.Listen(Key.D, ButtonState.Released, _player.Stop, null);

        _game.Keyboard.Listen(Key.Left, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        _game.Keyboard.Listen(Key.Left, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.Right, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        _game.Keyboard.Listen(Key.Right, ButtonState.Released, _player.Stop, null);
    }

    private void MovePlayer(Vector direction)
    {
        _player.Move(direction * 300);
    }

    private void NewHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, HighScorePath);

        HighScoreWindow w = new HighScoreWindow("Parhaat pisteet"
            , "Onneksi olkoon, pääsit listalle pisteillä %p! Syötä nimesi:", _highScore, _points);
        w.Closed += SaveHighScore;
        w.Closed += delegate { OpenMenu(); }; // TODO Maybe do both in single Closed event?
        _game.Add(w);
    }

    private void SaveHighScore(Window sender) => Game.DataStorage.Save<ScoreList>(_highScore, HighScorePath);

    private void OpenMenu()
    {
        if (_game.IsPaused)
        {
            _game.Pause();
            return;
        }
        _game.Pause();
        MultiSelectWindow pauseMenu = new MultiSelectWindow("Pause", "Jatka peliä", "Aloita alusta", "Parhaat pisteet", "Kalenteriin", "Lopeta");
        _game.Add(pauseMenu);

        pauseMenu.Closed += (handler) => _game.Pause();
        pauseMenu.AddItemHandler(1, InitGame);
        pauseMenu.AddItemHandler(2, ShowHighScores);
        pauseMenu.AddItemHandler(3, _game.InitCalendar);
        pauseMenu.AddItemHandler(4, _game.Exit);
    }


    private void ShowHighScores()
    {
        // TODO Game continues when opening high scores
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, HighScorePath);

        HighScoreWindow window = new HighScoreWindow("Parhaat pisteet", _highScore);
        window.Closed += delegate { OpenMenu(); };
        _game.Add(window);
    }
}