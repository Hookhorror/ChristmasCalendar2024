using System;
using Jypeli;

public class StoryPlayer : LidContentInterface
{
    private readonly PhysicsGame _game;
    private readonly string _mediaUrl;
    private static readonly Image[] Images = Game.LoadImages("play", "pause", "stop", "radio");

    private bool _paused;
    private bool _playing;

    public StoryPlayer(PhysicsGame game, string url)
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
        // _game.Mouse.ListenOn(b, MouseButton.Left, ButtonState.Pressed, ButtonClicked, null);
        // _game.Mouse.Listen(MouseButton.Left, ButtonState.Pressed, () => Tools.PrintMousePosition(_game), null);
        Tools.PrintMousePositionOnClick(_game);

        // _game.Add(b);
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

    private void ButtonClicked()
    {
        if (_game.MediaPlayer.IsPlaying)
        {
            _game.MediaPlayer.Stop();
        }
        else
        {
            _game.MediaPlayer.Play(_mediaUrl);
        }
    }

    private void InitGame()
    {
        _game.ClearAll();
        _game.Camera.Reset();
        _game.Camera.Zoom(0.5);

        GameObject radio = new GameObject(1000, 1000);
        radio.Image = Images[3];
        _game.Add(radio, -1);
        MakeButtons();
    }
}