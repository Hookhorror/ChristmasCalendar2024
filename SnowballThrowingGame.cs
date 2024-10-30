using System;
using Jypeli;

class SnowballThrowingGame : LidContentInterface
{
    private PhysicsGame game;
    private int difficulty = 1;
    private PhysicsObject player;
    private readonly IntMeter points = new IntMeter(0);

    public SnowballThrowingGame(PhysicsGame game)
    {
        this.game = game;
    }

    public void Start()
    {
        InitGame();
    }


    private void InitGame()
    {
        game.ClearAll();
        game.Camera.Reset();
        game.Camera.Zoom(0.5);
        AddPlayer();
        AddMap();
        AddControllers();
        AddInterface();
        AddTimers();
    }

    private void AddMap()
    {
        game.Level.CreateBorders();
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
        if (difficulty <= 10)
        {
            difficulty++;
        }
    }

    private void AddInterface()
    {
        Label labelPoints = new Label();
        labelPoints.Title = "Pisteet: ";
        labelPoints.Position = new Vector(game.Level.Right - 100, game.Level.Top - 50);
        labelPoints.BindTo(points);
        game.Add(labelPoints);
        // TODO resizing window doesn't recalculate label positions
        // game.WindowSizeChanged
    }

    private void AddPlayer()
    {
        player = new PhysicsObject(50, 50);
        player.IgnoresCollisionResponse = true;
        game.Add(player);
    }

    private void SpawnEnemies()
    {
        int howMany = RandomGen.NextInt(difficulty) + 1;
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
        enemy.Color = Color.Black;
        FollowerBrain brain = new FollowerBrain(player);
        brain.Speed = 100;
        enemy.Brain = brain;
        game.Add(enemy);
    }

    private Vector RandomSpawnPoint()
    {
        int direction = RandomGen.NextInt(4);
        double y = 0;
        double x = 0;
        switch (direction)
        {
            case 0:
                y = RandomGen.NextDouble(game.Level.Bottom, game.Level.Top);
                return new Vector(game.Level.Right, y);
            case 1:
                x = RandomGen.NextDouble(game.Level.Left, game.Level.Right);
                return new Vector(x, game.Level.Top);
            case 2:
                y = RandomGen.NextDouble(game.Level.Bottom, game.Level.Top);
                return new Vector(game.Level.Left, y);
            case 3:
                x = RandomGen.NextDouble(game.Level.Left, game.Level.Right);
                return new Vector(x, game.Level.Bottom);
            default:
                return Vector.Zero;
        }
    }

    private void AddControllers()
    {
        game.Keyboard.Listen(Key.Space, ButtonState.Pressed, ThrowBall, "Heittää lumipallon");
        game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, ThrowBall, "Heittää Lumipallon");
        game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, EnterMenu, "Pausettaa pelin ja avaa menun");
        game.Keyboard.Listen(Key.F1, ButtonState.Pressed, game.ShowControlHelp, "Näyttää nämä ohjeet");

        game.Keyboard.Listen(Key.W, ButtonState.Down, MovePlayer, "Liike ylös", new Vector(0, 1));
        game.Keyboard.Listen(Key.W, ButtonState.Released, player.Stop, null);
        game.Keyboard.Listen(Key.S, ButtonState.Down, MovePlayer, "Liike alas", new Vector(0, -1));
        game.Keyboard.Listen(Key.S, ButtonState.Released, player.Stop, null);
        game.Keyboard.Listen(Key.D, ButtonState.Down, MovePlayer, "Liike oikealle", new Vector(1, 0));
        game.Keyboard.Listen(Key.D, ButtonState.Released, player.Stop, null);
        game.Keyboard.Listen(Key.A, ButtonState.Down, MovePlayer, "Liike vasemmalle", new Vector(-1, 0));
        game.Keyboard.Listen(Key.A, ButtonState.Released, player.Stop, null);
    }

    private void MovePlayer(Vector direction)
    {
        double speed = 300;
        if (game.Keyboard.IsKeyDown(Key.W) && game.Keyboard.IsKeyDown(Key.A))
        {
            player.Move(new Vector(-1, 1).Normalize() * speed);
            return;
        }
        if (game.Keyboard.IsKeyDown(Key.S) && game.Keyboard.IsKeyDown(Key.A))
        {
            player.Move(new Vector(-1, -1).Normalize() * speed);
            return;
        }
        if (game.Keyboard.IsKeyDown(Key.S) && game.Keyboard.IsKeyDown(Key.D))
        {
            player.Move(new Vector(1, -1).Normalize() * speed);
            return;
        }
        if (game.Keyboard.IsKeyDown(Key.W) && game.Keyboard.IsKeyDown(Key.D))
        {
            player.Move(new Vector(1, 1).Normalize() * speed);
            return;
        }

        player.Move(direction * speed);
    }

    private void EnterMenu()
    {
        if (game.IsPaused)
        {
            game.Pause();
            return;
        }
        game.Pause();
        MultiSelectWindow pauseMenu = new MultiSelectWindow("Pause", "Jatka peliä", "Aloita alusta", "Kalenteriin", "Lopeta");
        game.Add(pauseMenu);

        pauseMenu.Closed += (handler) => game.Pause();
        pauseMenu.AddItemHandler(1, InitGame);
        pauseMenu.AddItemHandler(2, game.Begin); // TODO make better init calendar method
        pauseMenu.AddItemHandler(3, game.Exit);
    }

    private void ThrowBall()
    {
        if (!game.IsPaused)
        {
            // Vector mousePos = game.Mouse.PositionOnWorld;
            // Vector playerPos = player.Position;
            PhysicsObject ball = new PhysicsObject(20, 20, Shape.Circle);
            ball.Position = player.Position;
            ball.LifetimeLeft = new TimeSpan(0, 0, 3);
            game.AddCollisionHandler(ball, "enemy", DestroyEnemy);
            game.Add(ball);

            Vector directionOfHit = (game.Mouse.PositionOnWorld - ball.Position).Normalize();
            ball.Hit(directionOfHit * ball.Mass * 1000);
        }
    }


    private void DestroyEnemy(PhysicsObject collider, PhysicsObject target)
    {
        target.Destroy();
        collider.Destroy();
        points.Value++;
    }
}