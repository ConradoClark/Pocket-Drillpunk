using Licht.Unity.Objects;
using UnityEngine;

public class ConstantRotation : BaseGameObject
{
    public float Speed;

    private void Update()
    {
        transform.Rotate(Vector3.forward, (float) GameTimer.UpdatedTimeInMilliseconds * Speed * 0.1f);
    }
}
