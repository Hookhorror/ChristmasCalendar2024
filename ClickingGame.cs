using System;
using System.Collections.Generic;
using Jypeli;

class ClickingGame : LidContentInterface
{
    private PhysicsGame _game;
    private IntMeter _points;
    private const int MaxBlocks = 5;
    private IntMeter _life = new IntMeter(3);
    private int _blocksLeft;
    private int _ramping = 1;
    private static readonly List<PhysicsObject> _blocks = new List<PhysicsObject>();
    private static readonly SoundEffect[] sounds = Game.LoadSoundEffects("Tap", "Spring", "Pig", "Sheep");

    public ClickingGame(PhysicsGame game)
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
        // TODO Show lives

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
        // IncreaseDifficulty(spawner);
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
        // if (_blocksLeft <= 0)
        //     _life.Value++;
        // else
        _life.Value -= _blocksLeft;
        if (_life <= 0) GameOver();

        foreach (PhysicsObject block in _blocks)
        {
            block.Destroy();
        }
        _blocks.Clear();
    }

    private void GameOver()
    {
        _game.MessageDisplay.Add("Peli päättynyt :(");
        // TODO ending
    }

    private void InitGame()
    {
        _game.ClearAll();
        _game.Level.Size = new Vector(Game.Screen.Width - 100, Game.Screen.Height - 100);
        AddUI();
        CreateBlocks(); // TODO instructions
        StartTimers();
    }

    private void CreateBlocks()
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
        _points.Value++;
        _blocksLeft--;
        clicked.Destroy();
        PlaySound();

        if (_blocksLeft <= 0)
        {
            _life.Value++;
        }
    }

    private void PlaySound()
    {
        int max = 1000;
        int r = RandomGen.NextInt(max);
        if (r == max - 1)
        {
            sounds[3].Play();
            return;
        }
        if (r >= 990)
        {
            sounds[2].Play();
            return;
        }
        if (r >= 900)
        {
            sounds[1].Play();
            return;
        }
        sounds[0].Play();
    }
}