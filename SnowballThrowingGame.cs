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
    private static readonly Animation _snowmanAnim = new Animation(_snowmanImages);
    private static readonly Shape _snowmanShape = Shape.FromImage(_snowmanImages[0]);
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
        AddControllers();
        AddInterface();
        AddTimers();
    }

    private void AddTimers()
    {
        Timer raiseDifficulty = new Timer(30, RaiseDifficulty);
        raiseDifficulty.Start();
        Timer spawnEnemies = new Timer(2, SpawnEnemies);
        spawnEnemies.Start();
        _game.MessageDisplay.MessageTime = new TimeSpan(10000);
    }


    private void RaiseDifficulty()
    {
        if (_difficulty <= 10)
        {
            _difficulty++;
        }
    }

    private void AddInterface()
    {
        Label labelPoints = new Label();
        labelPoints.Title = "Pisteet: ";
        labelPoints.Position = new Vector(_game.Level.Right - 100, _game.Level.Top - 50);
        labelPoints.BindTo(_points);
        _game.Add(labelPoints);
        // TODO resizing window doesn't recalculate label positions
    }

    private void AddPlayer()
    {
        _player = new PhysicsObject(50, 50);
        _player.IgnoresCollisionResponse = true;
        _game.AddCollisionHandler(_player, "enemy", GameOver);

        _game.Add(_player);
    }

    private void GameOver(PhysicsObject collider, PhysicsObject target)
    {
        collider.Destroy();
        _game.MessageDisplay.Add("Hävisit pelin :(");
        _game.StopAll();
        _game.ClearTimers();
        NewHighScores();
    }

    private void SpawnEnemies()
    {
        int howMany = RandomGen.NextInt(_difficulty) + 1;
        // game.MessageDisplay.Add($"Spawned {howMany} enemies");
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
        // TODO change facing of animation?
    }

    private Vector RandomSpawnPoint()
    {
        int direction = RandomGen.NextInt(4);
        double y = 0;
        double x = 0;
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

    private void AddControllers()
    {
        // TODO snowball throwing while holding left mouse button
        _game.Keyboard.Listen(Key.Space, ButtonState.Pressed, ThrowBall, "Heittää lumipallon");
        _game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, ThrowBall, "Heittää Lumipallon");
        _game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, OpenMenu, "Pausettaa pelin ja avaa menun");
        _game.Keyboard.Listen(Key.F1, ButtonState.Pressed, _game.ShowControlHelp, "Näyttää nämä ohjeet");

        _game.Keyboard.Listen(Key.W, ButtonState.Down, MovePlayer, "Liike ylös", new Vector(0, 1));
        _game.Keyboard.Listen(Key.W, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.S, ButtonState.Down, MovePlayer, "Liike alas", new Vector(0, -1));
        _game.Keyboard.Listen(Key.S, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.D, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        _game.Keyboard.Listen(Key.D, ButtonState.Released, _player.Stop, null);
        _game.Keyboard.Listen(Key.A, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        _game.Keyboard.Listen(Key.A, ButtonState.Released, _player.Stop, null);
    }

    private void MovePlayer(Vector direction)
    {
        double speed = 300;
        if (_game.Keyboard.IsKeyDown(Key.W) && _game.Keyboard.IsKeyDown(Key.A))
        {
            _player.Move(new Vector(-1, 1).Normalize() * speed);
            return;
        }
        if (_game.Keyboard.IsKeyDown(Key.S) && _game.Keyboard.IsKeyDown(Key.A))
        {
            _player.Move(new Vector(-1, -1).Normalize() * speed);
            return;
        }
        if (_game.Keyboard.IsKeyDown(Key.S) && _game.Keyboard.IsKeyDown(Key.D))
        {
            _player.Move(new Vector(1, -1).Normalize() * speed);
            return;
        }
        if (_game.Keyboard.IsKeyDown(Key.W) && _game.Keyboard.IsKeyDown(Key.D))
        {
            _player.Move(new Vector(1, 1).Normalize() * speed);
            return;
        }

        _player.Move(direction * speed);
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
        if (!_game.IsPaused)
        {
            // Vector mousePos = game.Mouse.PositionOnWorld;
            // Vector playerPos = player.Position;
            PhysicsObject ball = new PhysicsObject(20, 20, Shape.Circle);
            ball.Position = _player.Position;
            ball.LifetimeLeft = new TimeSpan(0, 0, 3);
            _game.AddCollisionHandler(ball, "enemy", DestroyEnemy);
            _game.Add(ball);

            Vector directionOfHit = (_game.Mouse.PositionOnWorld - ball.Position).Normalize();
            ball.Hit(directionOfHit * ball.Mass * 1000);
        }
    }


    private void DestroyEnemy(PhysicsObject collider, PhysicsObject target)
    {
        target.Destroy();
        collider.Destroy();
        _points.Value++;
    }

    private void ShowHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, "SnowmanDefenceHighScore.xml");

        HighScoreWindow w = new HighScoreWindow("Parhaat pisteet", _highScore);
        w.Closed += delegate { OpenMenu(); };
        _game.Add(w);
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