

using UnityEngine;

namespace Blueprints_arquitectura_Dani_.Runtime
{
    public interface IBlueprintNodeExecutor
    {
        int Execute(RuntimeNode node, BlueprintDirector ctx,GameObject obj);
        //Devuelve el puerto por el que va a continuar la ejecucion
        //En caso de solo tener 1 devuelve el 0 que es el primer puerto
        //Para pausar la ejecucion devuelve -1
    }
}