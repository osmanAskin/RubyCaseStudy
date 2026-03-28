using System;
using UnityEngine;

public class DirectionChanger : MonoBehaviour
{
    public ShooterDirection ShooterNextDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shooter"))
        {
            var shooter = other.GetComponentInParent<Shooter>();
            if (shooter == null) return;

            shooter.SetDirection(ShooterNextDirection);
        }
    }
}
