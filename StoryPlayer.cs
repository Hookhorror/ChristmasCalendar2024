using System;
using Jypeli;

public class StoryPlayer : LidContentInterface
{
    private readonly ChristmasCalendar2024 _game;
    private readonly string _mediaUrl;
    private static readonly Image[] Images = Game.LoadImages("play", "pause", "stop", "radio");

    private bool _paused;
    private bool _playing;

    public StoryPlayer(ChristmasCalendar2024 game, string url)
    {
        _game = game;
        _mediaUrl = url;
    }

    public void Start()
    {
        InitGame();
    }

    private void MakeButtons()
    {
        Vector position = new Vector(0, 0);

        MakeButton(new Vector(-396, -82), Images[0], Play);
        MakeButton(new Vector(-311, -82), Images[1], Pause);
        MakeButton(new Vector(-226, -82), Images[2], Stop);
    }

    private void MakeButton(Vector position, Image image, Action action)
    {
        double diameter = 65;
        GameObject b = new GameObject(diameter, diameter);
        b.Position = position;
        b.Image = image;
        _game.Mouse.ListenOn(b, MouseButton.Left, ButtonState.Pressed, action, null);

        _game.Add(b);
    }

    private void Play()
    {
        if (!_playing)
        {
            _game.MediaPlayer.Play(_mediaUrl);
            _playing = true;
        }
        else if (_paused)
        {
            _game.MediaPlayer.Resume();
            _paused = false;
        }
    }

    private void Stop()
    {
        _game.MediaPlayer.Stop();
        _playing = false;
    }

    private void Pause()
    {
        if (_playing)
        {
            _game.MediaPlayer.Pause();
            _paused = true;
        }
    }


    private void InitGame()
    {
        _game.ClearAll();
        _game.Camera.Reset();
        _game.Camera.Zoom(0.5);
        _game.Level.BackgroundColor = Color.BloodRed;

        GameObject radio = new GameObject(1000, 1000);
        radio.Image = Images[3];
        _game.Add(radio, -1);
        MakeButtons();

        Label title = new Label(_mediaUrl);
        title.Position = new Vector(0, 140);
        title.TextColor = Color.White;
        _game.Add(title);

        AddControls();
    }


    private void AddControls()
    {
        _game.Keyboard.Listen(Key.Escape, ButtonState.Pressed, OpenMenu, null);
    }


    private void OpenMenu()
    {
        if (_game.IsPaused)
        {
            _game.Pause();
            return;
        }
        _game.Pause();
        MultiSelectWindow pauseMenu = new MultiSelectWindow("Menu", "Palaa", "Kalenteriin", "Lopeta");
        _game.Add(pauseMenu);

        pauseMenu.Closed += (handler) => _game.Pause();
        pauseMenu.AddItemHandler(1, _game.InitCalendar); // TODO make better init calendar method
        pauseMenu.AddItemHandler(2, _game.Exit);
    }
}