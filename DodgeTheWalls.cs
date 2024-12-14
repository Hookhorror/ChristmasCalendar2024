using System;
using Jypeli;
using Jypeli.Widgets;

class DodgeTheWalls : LidContentInterface
{
    private ChristmasCalendar2024 _game;
    private PhysicsObject _player;
    private ScoreList _highScore = new ScoreList(10, false, 0);
    private Timer _time;
    private Timer hexTimer;
    private bool _gameOver;
    private string _highScorePath = "DodgeTheWallsHighScore.xml";
    private double _difficulty = 0;
    private const double SpawnTime = 5.0;
    private const double MaxDifficult = 2.0;

    // TODO More hexagons

    public DodgeTheWalls(ChristmasCalendar2024 game)
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
        _gameOver = false;
        _game.Level.BackgroundColor = Color.Black;


        AddPlayer();
        CreateHexagon();
        AddControls();
        StartTimers();
    }

    private void StartTimers()
    {
        _time = new Timer(1.0);
        Label timeLabel = new Label();
        timeLabel.TextColor = Color.White;
        timeLabel.Position = new Vector(_game.Level.Center.X, _game.Level.Top - 100);
        timeLabel.DecimalPlaces = 2;
        timeLabel.BindTo(_time.SecondCounter);
        _game.Add(timeLabel);
        _time.Start();

        hexTimer = new Timer(SpawnTime, CreateHexagon);
        hexTimer.Start();

        Timer increaseDifficulty = new Timer(6);
        increaseDifficulty.Timeout += () => IncreaseDifficulty(hexTimer);
        increaseDifficulty.Start();
    }

    private void IncreaseDifficulty(Timer hexTimer)
    {
        if (_difficulty < MaxDifficult)
        {
            _difficulty += 0.05;
            hexTimer.Interval = SpawnTime - _difficulty;
        }
    }

    private void AddPlayer()
    {
        _player = new PhysicsObject(20, 20);
        _player.Shape = Shape.Circle;
        _player.Color = Color.White;
        _player.Collided += Collision;

        _game.Add(_player);
    }

    private void Collision(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        if (!_gameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _gameOver = true;

        _game.Keyboard.Disable(Key.W);
        _game.Keyboard.Disable(Key.A);
        _game.Keyboard.Disable(Key.S);
        _game.Keyboard.Disable(Key.D);
        _game.Keyboard.Disable(Key.Left);
        _game.Keyboard.Disable(Key.Up);
        _game.Keyboard.Disable(Key.Down);
        _game.Keyboard.Disable(Key.Right);
        _game.Mouse.Disable(MouseButton.Left);

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

        // TODO moving with mouse
        _game.Mouse.Listen(MouseButton.Left, ButtonState.Down, MovePlayer, "Liike kohti hiiren osoitinta", Vector.Zero);
        _game.Mouse.Listen(MouseButton.Left, ButtonState.Released, _player.Stop, null);
    }

    private Vector CalculateDirection()
    {
        Vector mousePos = _game.Mouse.PositionOnWorld;
        Vector playerPos = _player.Position;
        Vector direction = mousePos - playerPos;

        if (direction.Magnitude < 5)
        {
            return Vector.Zero;
        }

        return direction.Normalize();
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
        pauseMenu.AddItemHandler(3, _game.InitCalendar);
        pauseMenu.AddItemHandler(4, _game.Exit);
    }

    private void ShowHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, _highScorePath);

        HighScoreWindow window = new HighScoreWindow("Parhaat pisteet", _highScore);
        window.Closed += delegate { OpenMenu(); };
        _game.Add(window);
    }

    // TODO Game over
    private void NewHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, _highScorePath);

        HighScoreWindow w = new HighScoreWindow("Parhaat pisteet"
            , "Onneksi olkoon, pääsit listalle pisteillä %p! Syötä nimesi:", _highScore, _time.SecondCounter.Value);
        w.Closed += SaveHighScore;
        w.Closed += delegate { OpenMenu(); }; // TODO Maybe do both in single Closed event?
        _game.Add(w);
    }

    private void SaveHighScore(Window sender) => Game.DataStorage.Save<ScoreList>(_highScore, _highScorePath);


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
        if (_game.Mouse.CurrentState.LeftButton)
        {
            _player.Velocity = CalculateDirection() * speed;
            return;
        }

        _player.Velocity = direction * speed;
    }

    /// <summary>
    /// Creates a hexagon using PhysicsObjects.
    /// </summary>
    /// <param name="center">The center of the hexagon.</param>
    /// <param name="radius">The radius of the hexagon (distance from center to vertices).</param>
    /// <param name="vertexSize">The size of each vertex object.</param>
    /// <param name="color">The color of the hexagon vertices and edges.</param>
    public void CreateHexagon()
    {
        double radius = 1000;
        Hexagon hexagon = new Hexagon(radius * 2, radius * 2, 5);

        hexagon.Shrink(0.1, 0.99, _player.Width);
        hexagon.RemoveRandomEdge();
        hexagon.AddToGame(_game);
    }
}