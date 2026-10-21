using UnityEngine;

namespace Navigation
{
    public class NodeGrid
    {
        public Vector2 WorldPosition { get; }
        public bool Walkable { get; }

        public int GridX { get; }
        public int GridY { get; }

        public NodeGrid(
            Vector2 worldPosition,
            bool walkable,
            int gridX,
            int gridY)
        {
            WorldPosition = worldPosition;
            Walkable = walkable;

            GridX = gridX;
            GridY = gridY;
        }
    }
}