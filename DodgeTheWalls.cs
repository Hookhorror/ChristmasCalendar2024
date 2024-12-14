using System;
using Jypeli;
using Jypeli.Widgets;

class DodgeTheWalls : LidContentInterface
{
    private ChristmasCalendar2024 _game;
    private PhysicsObject _player;
    private ScoreList _highScore;
    private double _points;

    // TODO More hexagons
    // TODO Add points
    // TODO Shrink hexagons
    // TODO collision with hexagons

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

        CreateHexagon();
        AddPlayer();
        AddControls();
    }

    private void AddPlayer()
    {
        _player = new PhysicsObject(50, 50);
        _player.Shape = Shape.Circle;
        _player.Color = Color.Red;
        _game.Add(_player);
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
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, "SnowmanDefenceHighScore.xml");

        HighScoreWindow window = new HighScoreWindow("Parhaat pisteet", _highScore);
        window.Closed += delegate { OpenMenu(); };
        _game.Add(window);
    }

    // TODO Game over
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

    /// <summary>
    /// Creates a hexagon using PhysicsObjects.
    /// </summary>
    /// <param name="center">The center of the hexagon.</param>
    /// <param name="radius">The radius of the hexagon (distance from center to vertices).</param>
    /// <param name="vertexSize">The size of each vertex object.</param>
    /// <param name="color">The color of the hexagon vertices and edges.</param>
    public void CreateHexagon()
    {
        // TODO make hexagon object
        double radius = 500;
        GameObject hexagon = new GameObject(radius * 2, radius * 2);
        hexagon.Color = Color.White;
        PhysicsObject[] vertices = new PhysicsObject[6];
        Vector center = Vector.Zero;
        double vertexSize = 5;
        Color color = RandomGen.NextColor();
        int sides = 6;

        for (int i = 0; i < sides; i++)
        {
            // Calculate the angle for each vertex (60-degree increments)
            double angle = i * 60 * Math.PI / 180;

            // Calculate the position of the vertex
            double x = center.X + radius * Math.Cos(angle);
            double y = center.Y + radius * Math.Sin(angle);
            Vector vertexPosition = new Vector(x, y);

            PhysicsObject vertex = new PhysicsObject(vertexSize, vertexSize);
            vertex.Color = color;
            vertex.Shape = Shape.Circle;
            vertex.Position = vertexPosition;
            vertex.IgnoresCollisionResponse = true;

            hexagon.Add(vertex);
            vertices[i] = vertex;
        }

        // Connect the vertices with lines to form edges
        for (int i = 0; i < sides - 1; i++)
        {
            PhysicsObject start = vertices[i];
            PhysicsObject end = vertices[(i + 1) % 6];
            PhysicsObject edge = CreateEdge(start.Position, end.Position, color, vertexSize);
            edge.IgnoresCollisionResponse = true;
            hexagon.Add(edge);
        }
        // hexagon.IgnoresCollisionResponse = true;
        ShrinkTimer(hexagon);
        _game.Add(hexagon);
    }

    private void ShrinkTimer(GameObject hexagon)
    {
        Timer shrinkTimer = new Timer(0.1);
        shrinkTimer.Timeout += () => ShrinkHexagon(hexagon);
        shrinkTimer.Start();

        hexagon.Destroyed += () => shrinkTimer = null;
    }

    private void ShrinkHexagon(GameObject hexagon)
    {
        // hexagon.Width = 200;
        // hexagon.Height = 200;
        hexagon.Size *= 0.99;
        foreach (var item in hexagon.GetChildObjectList)
        {
            item.Size *= 0.99;
            item.Position *= 0.99;
        }

        if (hexagon.Width < 100)
        {
            hexagon.Destroy();
        }
    }

    /// <summary>
    /// Creates an edge between two points using a visual line.
    /// </summary>
    /// <param name="start">The starting position of the edge.</param>
    /// <param name="end">The ending position of the edge.</param>
    /// <param name="color">The color of the edge.</param>
    private PhysicsObject CreateEdge(Vector start, Vector end, Color color, double thickness)
    {
        PhysicsObject edge = new PhysicsObject(thickness, (end - start).Magnitude);
        edge.Color = color;
        edge.Position = (start + end) / 2; // Midpoint of the edge
        // TODO clean formula
        edge.Angle = (end - start).Angle + Angle.FromDegrees(90);

        return edge;
    }
}