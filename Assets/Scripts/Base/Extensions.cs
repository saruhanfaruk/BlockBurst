using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
    public static Direction GetOppositeDirection(Direction direction)
    {
        if (direction == Direction.Up)
            direction = Direction.Down;
        else if (direction == Direction.Down)
            direction = Direction.Up;
        else if (direction == Direction.Left)
            direction = Direction.Right;
        else
            direction = Direction.Left;
        return direction;
    }
}