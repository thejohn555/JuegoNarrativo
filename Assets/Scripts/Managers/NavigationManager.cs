using System;
using UnityEngine;

namespace Managers
{
    public class NavigationManager:Singleton<NavigationManager>
    {
        public Pathfinding2D Pathfinding { get; private set; } = new Pathfinding2D();

        public void InitializeGrid( Grid2D grid)
        {
            if (grid == null)
            {
                Debug.LogError("NavigationManager cannot initialize a null Grid2D.", this);
                return;
            }

            // Keep the same Pathfinding2D instance so characters that cached it
            // continue using the active scene's grid after a scene change.
            Pathfinding.Grid = grid;
            grid.CreateGrid();
        }
    }
}
