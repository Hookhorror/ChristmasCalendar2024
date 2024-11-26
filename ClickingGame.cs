using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Jypeli;

class ClickingGame : LidContentInterface
{
    private PhysicsGame _game;
    private IntMeter _points;
    private const int HowManyBlocks = 5;
    private int _life = 3;
    private int _blocksLeft;
    private double _spawnTime = 5;
    private static readonly List<PhysicsObject> _blocks = new List<PhysicsObject>();
    private static readonly SoundEffect[] sounds = Game.LoadSoundEffects("Tap", "Spring", "Pig", "Sheep");

    public ClickingGame(PhysicsGame game)
    {
        _game = game;
        _points = new IntMeter(0);
    }

    private void AddUI()
    {
        Label l = new Label();
        l.Title = "Pisteet: ";
        l.Position = new Vector(_game.Level.Right - 200, _game.Level.Top - 50);
        l.BindTo(_points);

        _game.Add(l);
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
            // TODO Too hard
            spawner.Interval -= 0.2;
        }
    }

    private void DestroyBlocks()
    {
        if (_blocksLeft <= 0)
            _life++;
        else
            _life -= _blocksLeft;
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
        for (int i = 0; i < HowManyBlocks; i++)
        {
            CreateRandomBlock();
        }
    }

    private void CreateRandomBlock()
    {
        double height = RandomGen.NextDouble(25, 200);
        double width = RandomGen.NextDouble(25, 200);
        Vector position = _game.Level.GetRandomFreePosition(100);

        GameObject block = CreateBlock(height, width, position);
        block.Angle = RandomGen.NextAngle();

        _game.Add(block);
    }

    private PhysicsObject CreateBlock(double height, double width, Vector position)
    {
        PhysicsObject block = new PhysicsObject(height, width);
        block.Position = position;
        // block.LifetimeLeft = new TimeSpan(0, 0, 5);
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