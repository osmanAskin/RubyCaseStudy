using System;
using UnityEngine;

public class DirectionChanger : MonoBehaviour
{
    //TODO: Shooterlarin isimlerini box olarak degistir hem prefab hem scriptlerde
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
