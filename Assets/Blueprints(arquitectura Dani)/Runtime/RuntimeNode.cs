using System;
using System.Collections.Generic;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    [Serializable]
    public abstract class RuntimeNode
    {
        public List<int> NextNodeIndex = new ();
    }
}