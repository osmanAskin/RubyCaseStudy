using UnityEngine;

namespace RubyCase.Game
{
    public class DirectionChanger : MonoBehaviour
    {
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
}
