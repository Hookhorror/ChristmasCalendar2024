using System;
using Jypeli;

class Hexagon : GameObject
{
    private readonly PhysicsObject[] edges = new PhysicsObject[6];
    private readonly PhysicsObject[] vertices = new PhysicsObject[6];
    private double _radius;
    private double _edgeWidth;
    private Color _color;

    public Hexagon(double width, double height, double edgeWidth) : base(width, height)
    {
        Width = width;
        Height = height;
        _radius = width / 2;
        _edgeWidth = edgeWidth;
        IsVisible = false;
        _color = RandomGen.NextColor();

        CreateVertices();
        CreateEdges();
    }

    public void AddToGame(Game game)
    {
        foreach (PhysicsObject vertice in vertices)
        {
            game.Add(vertice);
        }
        foreach (PhysicsObject edge in edges)
        {
            if (edge != null && !edge.IsDestroyed)
            {
                game.Add(edge);
            }
        }
    }

    private void CreateVertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // Calculate the angle for each vertex (60-degree increments)
            double angle = i * 60 * Math.PI / 180 + Math.PI / 2;

            // Calculate the position of the vertex
            double x = Position.X + _radius * Math.Cos(angle);
            double y = Position.Y + _radius * Math.Sin(angle);
            Vector vertexPosition = new Vector(x, y);

            PhysicsObject vertex = new PhysicsObject(_edgeWidth, _edgeWidth);
            // vertex.Color = color;
            vertex.Shape = Shape.Circle;
            vertex.Position = vertexPosition;
            vertex.IgnoresCollisionResponse = true;
            vertex.Color = _color;
            vertices[i] = vertex;
        }
    }

    private void CreateEdges()
    {
        for (int i = 0; i < edges.Length; i++)
        {
            PhysicsObject start = vertices[i];
            PhysicsObject end = vertices[(i + 1) % 6];
            PhysicsObject edge = CreateEdge(start.Position, end.Position);
            edge.IgnoresCollisionResponse = true;
            edges[i] = edge;
        }
    }

    private PhysicsObject CreateEdge(Vector start, Vector end)
    {
        PhysicsObject edge = new PhysicsObject(_edgeWidth, (end - start).Magnitude);
        edge.Position = (start + end) / 2; // Midpoint of the edge
        // TODO clean formula
        edge.Angle = (end - start).Angle + Angle.FromDegrees(90);
        edge.Color = _color;

        return edge;
    }

    internal void Shrink(double speed, double amount, double until)
    {
        Timer shrinkTimer = new Timer(speed);
        shrinkTimer.Timeout += () => Shrink(amount, until);
        shrinkTimer.Start();

        Destroyed += () => shrinkTimer = null;
    }

    private void Shrink(double amount, double until)
    {
        if (Width < until)
        {
            DestroySelf();
            return;
        }
        foreach (PhysicsObject edge in edges)
        {
            if (edge != null && !edge.IsDestroyed)
            {
                edge.Height *= amount;
                edge.Position *= amount;
            }
        }
        foreach (PhysicsObject vertex in vertices)
        {
            vertex.Position *= amount;
        }
        Size *= amount;
    }

    private void DestroySelf()
    {
        foreach (PhysicsObject edge in edges)
        {
            edge.Destroy();
        }
        foreach (PhysicsObject vertex in vertices)
        {
            vertex.Destroy();
        }
    }

    internal void RemoveRandomEdge()
    {
        int edge = RandomGen.NextInt(edges.Length);
        edges[edge].Destroy();
    }
}