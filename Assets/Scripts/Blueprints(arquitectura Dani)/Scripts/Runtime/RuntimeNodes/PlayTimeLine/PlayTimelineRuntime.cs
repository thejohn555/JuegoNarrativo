using Blueprints_arquitectura_Dani_.Runtime;

namespace Blueprints_arquitectura_Dani_.Scripts.Runtime.RuntimeNodes.PlayTimeLine
{
    public class PlayTimelineRuntime:RuntimeNode
    {
        public string timeLine;
        public bool waitTimeLine;

        public PlayTimelineRuntime(string timeLine,bool waitTimeLine)
        {
            this.timeLine = timeLine;
            this.waitTimeLine = waitTimeLine;
        }
    }
}