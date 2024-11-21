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
    private int missedBlocks;
    private static readonly List<PhysicsObject> _blocks = new List<PhysicsObject>();

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
        Timer blockSpawner = new Timer(5, DestroyAndCreateBlocks);
        blockSpawner.Start();
    }

    private void DestroyAndCreateBlocks()
    {
        DestroyBlocks();
        CreateBlocks();
    }

    private void DestroyBlocks()
    {
        if (missedBlocks >= 3) GameOver();

        foreach (PhysicsObject block in _blocks)
        {
            block.Destroy();
        }
        _blocks.Clear();
    }

    private void GameOver()
    {
        _game.MessageDisplay.Add("Peli päättynyt :(");
    }

    private void InitGame()
    {
        _game.ClearAll();
        AddUI();
        CreateBlocks();
        StartTimers();
    }

    private void CreateBlocks()
    {
        for (int i = 0; i < HowManyBlocks; i++)
        {
            CreateRandomBlock();
        }
    }

    private void CreateRandomBlock()
    {
        double height = RandomGen.NextDouble(10, 200);
        double width = RandomGen.NextDouble(10, 200);
        Vector position = _game.Level.GetRandomFreePosition(10);

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
        missedBlocks++;

        _game.Mouse.ListenOn(block, MouseButton.Left, ButtonState.Pressed, HandleClick, null, block);

        return block;
    }

    private void HandleClick(GameObject clicked)
    {
        _game.MessageDisplay.Add("Tuhosit laatikon");
        _points.Value++;
        missedBlocks--;
        clicked.Destroy();
    }
}