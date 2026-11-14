using Blueprints_arquitectura_Dani_.Runtime;
using Blueprints_arquitectura_Dani_.Runtime.BaseClases;
using Blueprints_arquitectura_Dani_.Scripts.Runtime.Managers;
using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.PlayTimeLine
{
    public class PlayTimelineExecutor:BlueprintNodeExecutor<PlayTimelineRuntime>
    {
        protected override int OnExecute(PlayTimelineRuntime node, BlueprintDirector ctx, GameObject obj)
        {
            if (!string.IsNullOrEmpty(node.timeLine))
            {
                var director = TimeLineManager.Instance.GetDirector(node.timeLine);
                if (director != null)
                {
                    double duration = director.duration;
                    director.time = 0;
                    director.Play();
                    if (node.waitTimeLine)
                    {
                        ctx.PendingWait = (float)duration;
                    }
                    return 0;
                }
            }
            return -1;
        }
    }
}