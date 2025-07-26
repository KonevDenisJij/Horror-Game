using Godot;

public partial class ReplyOptionButton : Button
{
    private Reply.ReplyOptions _reply;
    private float _speed;
    private float _offset;

    private double d;

    public void SetReply(Reply.ReplyOptions reply, float speed, float offset)
    {
        _reply = reply;
        _speed = speed;
        _offset = offset;
        Text = reply.TextKey;
        d = offset * Mathf.Pi * 2 / _speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_reply == null)
        {
            return;
        }

        if (ButtonPressed)
        {
            SignalsHelper.ParseSignalName(_reply.SignalName);
            return;
        }

        InfinityPatternMovement(delta);
    }

    private void InfinityPatternMovement(double delta)
    {
        if (d > (Mathf.Pi * 2 / _speed) + (_offset * Mathf.Pi * 2 / _speed))
        {
            d -= Mathf.Pi * 2 / _speed;
        }

        d += delta;

        var scale = (700 - Size.X - 30) / (3 - Mathf.Cos(2 * d * _speed));
        var x = scale * Mathf.Cos(d * _speed) - (Size.X) / 2;
        var y = (scale * Mathf.Sin(d * _speed) + 800) / 2 - (Size.Y * 2) / 3;

        var scale2 = 15 / (3 - Mathf.Cos(2 * d * 10));
        var x2 = scale2 * Mathf.Cos(d * 10);
        var y2 = scale2 * Mathf.Sin(2 * d * 10);

        Position = new Vector2((float)x + (float)x2, -(float)y - (float)y2);
    }
}
