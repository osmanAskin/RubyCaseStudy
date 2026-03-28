using System;
using UnityEngine;

public class DirectionChanger : MonoBehaviour
{
    //TODO: Shooterlarin isimlerini box olarak degistir hem prefab hem scriptlerde
    public CollectableBoxDirection CollectableBoxNextDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollectableBox"))
        {
            var collectableBox = other.GetComponentInParent<CollectableBox>();
            if (collectableBox == null) return;

            collectableBox.SetDirection(CollectableBoxNextDirection);
        }
    }
}
