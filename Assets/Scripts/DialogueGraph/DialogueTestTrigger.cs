using UnityEngine;

public class DialogueTestTrigger : MonoBehaviour
{
    [Header("Grafo de Prueba")]
    [SerializeField] private RuntimeDialogueGraph testDialogueGraph;

    [Header("Tecla para Iniciar")]
    [SerializeField] private KeyCode testKey = KeyCode.T;

    private void Update()
    {
        // Al pulsar la tecla 'T' en Play Mode, inicia el diálogo
        if (Input.GetKeyDown(testKey))
        {
            TestStart();
        }
    }

    [ContextMenu("Test Start Dialogue")] // Permite probarlo haciendo clic derecho en el Inspector
    public void TestStart()
    {
        if (testDialogueGraph != null)
        {
            DialogueManager.Instance.StartDialogue(testDialogueGraph);
        }
        else
        {
            Debug.LogError("¡Asigna un RuntimeDialogueGraph en el Inspector para probar!");
        }
    }
}