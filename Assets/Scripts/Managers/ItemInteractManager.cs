using System.Collections.Generic;
using Interactives;

namespace Managers
{
    public class ItemInteractManager:Singleton<ItemInteractManager>
    {
        List<Interactive> objectsInteractives = new ();

        public void PublishItem(Interactive item)
        {
            objectsInteractives.Add(item);
        }

        public void UnPublishItem(Interactive item)
        {
            objectsInteractives.Remove(item);
        }

        public void SwapItems()
        {
            foreach (var obj in objectsInteractives)
            {
                obj.UseOutliner(true);
            }
        }
    }
}