namespace RubyCase.Game
{
    public class CollectableBoxNode : Node
    {
        public bool IsFrontNode()
        {
            return GridPosition.y == _gridSystem.gridHeight - 1;
        }
    }
}
