using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Widgets;

class ClickingGame : LidContentInterface
{
    private ChristmasCalendar2024 _game;
    private IntMeter _points;
    private const int MaxBlocks = 5;
    private IntMeter _life = new IntMeter(3);
    private int _blocksLeft;
    private int _ramping = 1;
    private bool _gameOver;
    private string _highScorePath = "ClickingGameHighScore.xml";
    private ScoreList _highScore = new ScoreList(10, false, 0);

    private static readonly List<PhysicsObject> _blocks = new List<PhysicsObject>();

    public ClickingGame(ChristmasCalendar2024 game)
    {
        _game = game;
        _points = new IntMeter(0);
    }

    private void AddUI()
    {
        Label points = new Label();
        points.Title = "Pisteet: ";
        points.Position = new Vector(_game.Level.Right - 200, _game.Level.Top - 50);
        points.TextColor = Color.White;
        points.BindTo(_points);

        _game.Add(points);

        Label life = new Label();
        life.Title = "Elämät: ";
        life.Position = new Vector(_game.Level.Left + 200, _game.Level.Top - 50);
        life.TextColor = Color.White;
        life.BindTo(_life);
        _game.Add(life);
    }

    public void Start()
    {
        InitGame();
    }

    private void StartTimers()
    {
        double timeout = 5.0;

        Timer blockSpawner = new Timer(timeout);
        blockSpawner.Timeout += DestroyAndCreateBlocks;
        blockSpawner.Start();

        Timer increaseDifficulty = new Timer(timeout);
        increaseDifficulty.Timeout += () => IncreaseDifficulty(blockSpawner);
        increaseDifficulty.Start();
    }

    private void DestroyAndCreateBlocks()
    {
        DestroyBlocks();
        CreateBlocks();
    }

    private void IncreaseDifficulty(Timer spawner)
    {
        if (spawner.Interval >= 2.5)
        {
            spawner.Interval -= 0.1;
        }
    }

    private void DestroyBlocks()
    {
        _life.Value -= _blocksLeft;
        if (_life <= 0)
        {
            GameOver();
            return;
        }

        foreach (PhysicsObject block in _blocks)
        {
            block.Destroy();
        }
        _blocks.Clear();
    }

    private void GameOver()
    {
        _game.Mouse.Disable(MouseButton.Left);

        _gameOver = true;

        // TODO sound
        // DeathSounds[3].Play();
        _game.MessageDisplay.Add("Hävisit pelin :(");
        _game.StopAll();
        _game.ClearTimers();
        NewHighScores();
    }

    private void NewHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, _highScorePath);

        HighScoreWindow w = new HighScoreWindow("Parhaat pisteet"
            , "Onneksi olkoon, pääsit listalle pisteillä %p! Syötä nimesi:", _highScore, _points);
        w.Closed += SaveHighScore;
        w.Closed += delegate { OpenMenu(); }; // TODO Maybe do both in single Closed event?
        _game.Add(w);
    }

    private void SaveHighScore(Window sender) => Game.DataStorage.Save<ScoreList>(_highScore, _highScorePath);

    private void InitGame()
    {
        _blocksLeft = 0;
        _ramping = 1;
        _points.Value = 0;
        _life.Value = 3;
        _gameOver = false;
        _game.Level.BackgroundColor = Color.Black;
        _game.ClearAll();
        _game.Level.Size = new Vector(Game.Screen.Width - 100, Game.Screen.Height - 100);
        AddUI();
        // CreateBlocks(); // TODO instructions
        AddInstructions();
        StartTimers();
        AddControls();
    }

    private void AddInstructions()
    {
        Label instructions = new Label("Klikkaa paketteja niin saat pisteitä ja elämiä!");
        instructions.TextColor = Color.White;

        _game.Add(instructions);

        Timer.SingleShot(4.5, instructions.Destroy);
    }

    private void AddControls()
    {
        _game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, OpenMenu, "Pausettaa pelin ja avaa menun");
        _game.Keyboard.Listen(Key.F1, ButtonState.Pressed, _game.ShowControlHelp, "Näyttää nämä ohjeet");
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
        pauseMenu.AddItemHandler(3, _game.InitCalendar); // TODO make better init calendar method
        pauseMenu.AddItemHandler(4, _game.Exit);
    }

    private void ShowHighScores()
    {
        _highScore = Game.DataStorage.TryLoad<ScoreList>(_highScore, _highScorePath);

        HighScoreWindow window = new HighScoreWindow("Parhaat pisteet", _highScore);
        window.Closed += delegate { OpenMenu(); };
        _game.Add(window);
    }

    private void CreateBlocks()
    {
        if (!_gameOver)
        {

            _blocksLeft = 0;
            for (int i = 0; i < _ramping; i++)
            {
                CreateRandomBlock();
            }

            if (_ramping < MaxBlocks)
            {
                _ramping++;
            }
        }
    }

    private void CreateRandomBlock()
    {
        double height = RandomGen.NextDouble(25, 200);
        double width = RandomGen.NextDouble(25, 200);
        Vector position = _game.Level.GetRandomFreePosition(100);

        GameObject block = CreateBlock(height, width, position);
        DecorateBlock(block);
        block.Color = RandomGen.NextColor();
        block.Angle = RandomGen.NextAngle();

        _game.Add(block);
    }

    private static void DecorateBlock(GameObject block)
    {
        Color randomColor = RandomGen.NextColor();

        GameObject ribbonWidth = new GameObject(block.Width, block.Height / 4);
        ribbonWidth.Position = block.Position;
        ribbonWidth.Color = randomColor;
        block.Add(ribbonWidth);

        GameObject ribbonHeight = new GameObject(ribbonWidth.Height, block.Height);
        ribbonHeight.Position = block.Position;
        ribbonHeight.Color = randomColor;
        block.Add(ribbonHeight);
    }


    private PhysicsObject CreateBlock(double height, double width, Vector position)
    {
        PhysicsObject block = new PhysicsObject(height, width);
        block.Position = position;
        _blocks.Add(block);
        _blocksLeft++;

        _game.Mouse.ListenOn(block, MouseButton.Left, ButtonState.Pressed, HandleClick, null, block);

        return block;
    }

    private void HandleClick(GameObject clicked)
    {
        if (!_gameOver)
        {
            _points.Value++;
            _blocksLeft--;
            clicked.Destroy();
            PlaySound();

            if (_blocksLeft <= 0)
            {
                _life.Value++;
            }
        }
    }

    private void PlaySound()
    {
        int max = 1000;
        int r = RandomGen.NextInt(max);
        if (r == max - 1)
        {
            Resources.ClickingGameSounds[3].Play();
            return;
        }
        if (r >= 990)
        {
            Resources.ClickingGameSounds[2].Play();
            return;
        }
        if (r >= 900)
        {
            Resources.ClickingGameSounds[1].Play();
            return;
        }
        Resources.ClickingGameSounds[0].Play();
    }
}