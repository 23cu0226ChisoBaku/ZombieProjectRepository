using UnityEngine;

public interface IExternalForceAffectable2D
{
    void SetForce(Vector2 force);
    void SetForceAtPosition(Vector2 force, Vector2 position);
}