using System;
using Jypeli;
using Jypeli.Widgets;

class SnowballThrowingGame : LidContentInterface
{
    private readonly PhysicsGame _game;
    private int _difficulty = 1;
    private PhysicsObject _player;
    private readonly IntMeter _points = new IntMeter(0);
    private static readonly Image[] _snowmanImages = Game.LoadImages("Lumiukko-1.png", "Lumiukko-2.png", "Lumiukko-3.png", "Lumiukko-2.png");
    private static readonly Image[] BackGroundImages = Game.LoadImages("Snow1", "Snow2", "Snow3", "Snow4");
    private static readonly Animation _snowmanAnim = new Animation(_snowmanImages);
    private static readonly Animation ToyAnimRight = new Animation(Game.LoadImages("Toy1", "Toy2", "Toy3", "Toy2"));
    private static readonly SoundEffect[] ThrowSounds = Game.LoadSoundEffects("Whoosh1", "Whoosh2", "Whoosh3");
    private static readonly SoundEffect[] DeathSounds = Game.LoadSoundEffects("Jippii", "Wii", "WilhelmScream", "Fail");
    private static readonly Shape _snowmanShape = Shape.FromImage(_snowmanImages[0]);
    private static readonly Shape PlayerShape = Shape.FromImage(Game.LoadImage("Toy1"));
    private ScoreList _highScore = new ScoreList(10, false, 0);

    public SnowballThrowingGame(PhysicsGame game)
    {
        this._game = game;
    }

    public void Start()
    {
        InitGame();
    }


    private void InitGame()
    {
        _game.ClearAll();
        _game.Camera.Reset();
        _game.Camera.Zoom(0.5);
        _points.Value = 0;
        AddPlayer();
        InitMap();
        AddControls();
        AddUI();
        AddTimers();
    }

    private void InitMap()
    {
        _game.Level.Size = new Vector(1024, 1024);
        PhysicsObject[] borders = { _game.Level.CreateLeftBorder(), _game.Level.CreateTopBorder()
            , _game.Level.CreateRightBorder(), _game.Level.CreateBottomBorder() };

        foreach (PhysicsObject border in borders)
        {
            border.IsVisible = false;
            _game.Add(border);
        }

        _game.Level.BackgroundColor = Color.FromHexCode("#003300");
        // _game.Level.Background.Image = Game.LoadImage("Snow1.png");
        // _game.Level.Background.TileToLevel();
        AddBackGround();
    }

    private void AddBackGround()
    {
        int rows = 20;
        int columns = 20;
        double width = _game.Level.Width / columns;
        double height = _game.Level.Height / rows;
        // Vector position = 
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {

                GameObject tile = new GameObject(width, height);
                tile.Position = new Vector(_game.Level.Left + (width / 2) + width * row, _game.Level.Top - (height / 2) - height * column);
                tile.Image = RandomGen.SelectOne(BackGroundImages);

                _game.Add(tile, -3);
            }
        }
    }

    private void AddTimers()
    {
        Timer raiseDifficulty = new Timer(30, RaiseDifficulty);
        raiseDifficulty.Start();
        Timer spawnEnemies = new Timer(2, SpawnEnemies);
        spawnEnemies.Start();
    }


    private void RaiseDifficulty()
    {
        if (_difficulty <= 10)
        {
            _difficulty++;
        }
    }

    private void AddUI()
    {
        Label labelPoints = new Label();
        labelPoints.Title = "Pisteet: ";
        labelPoints.Position = new Vector(_game.Level.Right - 100, _game.Level.Top + 50);
        labelPoints.TextScale = new Vector(2, 2);
        labelPoints.TextColor = Color.White;
        labelPoints.BindTo(_points);
        _game.Add(labelPoints, 3);
        // TODO resizing window doesn't recalculate label positions
        // game.WindowSizeChanged
    }

    private void AddPlayer()
    {
        _player = new PhysicsObject(75, 75);
        _player.CanRotate = false;
        _player.Shape = PlayerShape;
        _game.AddCollisionHandler(_player, "enemy", PlayerHitsEnemy);
        AnimatePlayer(ToyAnimRight);

        _game.Add(_player);
    }

    private void AnimatePlayer(Animation anim)
    {
        _player.Animation = anim;
        _player.Animation.FPS = 3;
        _player.Animation.Start();
    }

    private void PlayerHitsEnemy(PhysicsObject collider, PhysicsObject target)
    {
        collider.Destroy();
        GameOver();
    }

    private void GameOver()
    {
        _game.Mouse.Disable(MouseButton.Left);
        _game.Keyboard.Disable(Key.W);
        _game.Keyboard.Disable(Key.A);
        _game.Keyboard.Disable(Key.S);
        _game.Keyboard.Disable(Key.D);
        _game.Keyboard.Disable(Key.Left);
        _game.Keyboard.Disable(Key.Up);
        _game.Keyboard.Disable(Key.Down);
        _game.Keyboard.Disable(Key.Right);

        DeathSounds[3].Play();
        _game.MessageDisplay.Add("Hävisit pelin :(");
        _game.StopAll();
        _game.ClearTimers();
        NewHighScores();
    }

    private void SpawnEnemies()
    {
        int howMany = RandomGen.NextInt(_difficulty) + 1;
        for (int i = 0; i < howMany; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        PhysicsObject enemy = new PhysicsObject(100, 100, Shape.Hexagon);
        enemy.Position = RandomSpawnPoint();
        enemy.Tag = "enemy";
        enemy.Shape = _snowmanShape;
        enemy.CanRotate = false;
        FollowerBrain brain = new FollowerBrain(_player);
        brain.Speed = RandomGen.NextDouble(25, 200);
        enemy.Brain = brain;
        enemy.Animation = _snowmanAnim;
        enemy.Animation.FPS = 4;
        enemy.Animation.Start();
        _game.Add(enemy);
    }

    private Vector RandomSpawnPoint()
    {
        int direction = RandomGen.NextInt(4);
        double y;
        double x;
        switch (direction)
        {
            case 0:
                y = RandomGen.NextDouble(_game.Level.Bottom, _game.Level.Top);
                return new Vector(_game.Level.Right, y);
            case 1:
                x = RandomGen.NextDouble(_game.Level.Left, _game.Level.Right);
                return new Vector(x, _game.Level.Top);
            case 2:
                y = RandomGen.NextDouble(_game.Level.Bottom, _game.Level.Top);
                return new Vector(_game.Level.Left, y);
            case 3:
                x = RandomGen.NextDouble(_game.Level.Left, _game.Level.Right);
                return new Vector(x, _game.Level.Bottom);
            default:
                return Vector.Zero;
        }
    }

    private void AddControls()
    {
        // TODO snowball throwing while holding left mouse button
        _game.Keyboard.Listen(Key.Space, ButtonState.Pressed, ThrowBall, "Heittää lumipallon");
        _game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, ThrowBall, "Heittää Lumipallon");
        _game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, OpenMenu, "Pausettaa pelin ja avaa menun");
        _game.Keyboard.Listen(Key.F1, ButtonState.Pressed, _game.ShowControlHelp, "Näyttää nämä ohjeet");

        _game.Keyboard.Listen(Key.W, ButtonState.Down, MovePlayer, "Liike ylös", new Vector(0, 1));
        _game.Keyboard.Listen(Key.W, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.Up, ButtonState.Down, MovePlayer, "Liike ylös", new Vector(0, 1));
        _game.Keyboard.Listen(Key.Up, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.S, ButtonState.Down, MovePlayer, "Liike alas", new Vector(0, -1));
        _game.Keyboard.Listen(Key.S, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.Down, ButtonState.Down, MovePlayer, "Liike alas", new Vector(0, -1));
        _game.Keyboard.Listen(Key.Down, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.D, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        _game.Keyboard.Listen(Key.D, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.Right, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        _game.Keyboard.Listen(Key.Right, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.A, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        _game.Keyboard.Listen(Key.A, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.Left, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        _game.Keyboard.Listen(Key.Left, ButtonState.Released, _player.Stop, null);
    }


    private void MovePlayer(Vector direction)
    {
        double speed = 300;

        if ((_game.Keyboard.IsKeyDown(Key.W) || _game.Keyboard.IsKeyDown(Key.Up)) && (_game.Keyboard.IsKeyDown(Key.A) || _game.Keyboard.IsKeyDown(Key.Left)))
        {
            _player.Velocity = new Vector(-1, 1).Normalize() * speed;
            return;
        }
        if ((_game.Keyboard.IsKeyDown(Key.S) || _game.Keyboard.IsKeyDown(Key.Down)) && (_game.Keyboard.IsKeyDown(Key.A) || _game.Keyboard.IsKeyDown(Key.Left)))
        {
            _player.Velocity = new Vector(-1, -1).Normalize() * speed;
            return;
        }
        if ((_game.Keyboard.IsKeyDown(Key.S) || _game.Keyboard.IsKeyDown(Key.Down)) && (_game.Keyboard.IsKeyDown(Key.D) || _game.Keyboard.IsKeyDown(Key.Right)))
        {
            _player.Velocity = new Vector(1, -1).Normalize() * speed;
            return;
        }
        if ((_game.Keyboard.IsKeyDown(Key.W) || _game.Keyboard.IsKeyDown(Key.Up)) && (_game.Keyboard.IsKeyDown(Key.D) || _game.Keyboard.IsKeyDown(Key.Right)))
        {
            _player.Velocity = new Vector(1, 1).Normalize() * speed;
            return;
        }

        _player.Velocity = direction * speed;
    }

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
        pauseMenu.AddItemHandler(3, _game.Begin); // TODO make better init calendar method
        pauseMenu.AddItemHandler(4, _game.Exit);
    }

    private void ThrowBall()
    {
        // TODO continuous throwing
        if (!_game.IsPaused)
        {
            PhysicsObject ball = new PhysicsObject(20, 20, Shape.Circle);
            Vector directionOfHit = (_game.Mouse.PositionOnWorld - _player.Position).Normalize();
            ball.Position = _player.Position + directionOfHit * (_player.Width / 1.5);
            ball.LifetimeLeft = new TimeSpan(0, 0, 3);
            _game.AddCollisionHandler(ball, BallHits);
            _game.Add(ball);

            ball.Hit(directionOfHit * ball.Mass * 1000);

            RandomGen.SelectOne(ThrowSounds).Play();
        }
    }


    private void BallHits(PhysicsObject collider, PhysicsObject target)
    {
        if (target != _player)
        {
            collider.Destroy();

            if (target.Tag.ToString().Equals("enemy"))
            {
                target.Destroy();
                _points.Value++;
                int max = 1000;
                int specialSound = RandomGen.NextInt(max);
                if (specialSound == max - 1)
                {
                    DeathSounds[2].Play();
                    return;
                }
                int normalSound = RandomGen.NextInt(2);
                DeathSounds[normalSound].Play();
            }
        }
    }

    private void ShowHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, "SnowmanDefenceHighScore.xml");

        HighScoreWindow window = new HighScoreWindow("Parhaat pisteet", _highScore);
        window.Closed += delegate { OpenMenu(); };
        _game.Add(window);
    }

    private void NewHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, "SnowmanDefenceHighScore.xml");

        HighScoreWindow w = new HighScoreWindow("Parhaat pisteet"
            , "Onneksi olkoon, pääsit listalle pisteillä %p! Syötä nimesi:", _highScore, _points);
        w.Closed += SaveHighScore;
        w.Closed += delegate { OpenMenu(); }; // TODO Maybe do both in single Closed event?
        _game.Add(w);
    }

    private void SaveHighScore(Window sender) => Game.DataStorage.Save<ScoreList>(_highScore, "SnowmanDefenceHighScore.xml");
}