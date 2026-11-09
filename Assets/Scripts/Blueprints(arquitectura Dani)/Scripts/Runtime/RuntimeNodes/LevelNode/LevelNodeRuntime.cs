namespace Blueprints_arquitectura_Dani_.Runtime.LevelNode
{
    public class LevelNodeRuntime : RuntimeNode
    {
        public string level { get; }
        public LevelNodeRuntime(string level)
        {
            this.level = level;
        }
    }
}