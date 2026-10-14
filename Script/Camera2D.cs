using System;
using Microsoft.Xna.Framework;

namespace Game;

public class Camera2D
{
    public Vector2 Position { get; private set; } = Vector2.Zero;

    private readonly float _deadZoneWidth;
    private readonly float _deadZoneHeight;
    private readonly float _lerpFactor;

    public Camera2D(float deadZoneWidth, float deadZoneHeight, float lerpFactor = 0.15f)
    {
        _deadZoneWidth = deadZoneWidth;
        _deadZoneHeight = deadZoneHeight;
        _lerpFactor = lerpFactor;
    }

    public void Update(Vector2 targetPosition)
    {
        float diffX = targetPosition.X - Position.X;
        float diffY = targetPosition.Y - Position.Y;

        Vector2 desiredPosition = Position;

        float halfDeadWidth = _deadZoneWidth / 2f;
        float halfDeadHeight = _deadZoneHeight / 2f;

        if (MathF.Abs(diffX) > halfDeadWidth)
        {
            desiredPosition.X = targetPosition.X - MathF.Sign(diffX) * halfDeadWidth;
        }

        if (MathF.Abs(diffY) > halfDeadHeight)
        {
            desiredPosition.Y = targetPosition.Y - MathF.Sign(diffY) * halfDeadHeight;
        }

        Position += (desiredPosition - Position) * _lerpFactor;
    }

    public Matrix GetViewMatrix(float screenWidth, float screenHeight)
    {
        return
        Matrix.CreateTranslation(-Position.X, -Position.Y, 0f) *
        Matrix.CreateTranslation(screenWidth / 2f, screenHeight / 2f, 0f);
    }

    public void SnapTo(Vector2 targetPosition)
    {
        Position = targetPosition;
    }
}