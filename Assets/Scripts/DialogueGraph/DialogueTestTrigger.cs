using Assets.Scripts.Camera;
using UnityEngine;

public class DialogueTestTrigger : MonoBehaviour
{
    [Header("Grafo de Prueba")]
    [SerializeField] private RuntimeDialogueGraph testDialogueGraph;
    [SerializeField] private Transform testDialogueTarget;

    [Header("Ajustes para Diseño (Visualizables)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1f, 0f); // Offset de la cámara
    [SerializeField] private float zoom = 3f; // Tamaño de la cámara
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private Vector2 dialogueTextOffset = new Vector2(0f, -200f); // Posición UI

    [Header("Tecla para Iniciar")]
    [SerializeField] private KeyCode testKey = KeyCode.T;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestStart();
        }
    }

    [ContextMenu("Test Start Dialogue")]
    public void TestStart()
    {
        if (testDialogueGraph != null)
        {
            DialogueData testData = new DialogueData();
            testData.targetPosition = testDialogueTarget;

            // Ahora leemos los valores que diseño ponga en el Inspector
            testData.cameraOffset = cameraOffset;
            testData.zoom = zoom;
            testData.smoothTime = smoothTime;
            testData.dialogueTextOffset = dialogueTextOffset;

            DialogueManager.Instance.StartDialogue(testDialogueGraph, testData);
        }
        else
        {
            Debug.LogError("¡Asigna un RuntimeDialogueGraph en el Inspector para probar!");
        }
    }

    // --- LA MAGIA PARA DISEÑO (Solo funciona en el Editor) ---
    private void OnDrawGizmosSelected()
    {
        if (testDialogueTarget == null) return;

        // 1. Centro de la cámara
        Vector3 cameraCenter = testDialogueTarget.position + cameraOffset;
        cameraCenter.z = 0; // Asumiendo un juego 2D

        // 2. Calcular tamaño del marco de la cámara
        float camHeight = zoom * 2f;
        // Intentamos coger la proporción de la pantalla real, si no, usamos 16:9 por defecto
        float aspect = Camera.main != null ? Camera.main.aspect : (16f / 9f);
        float camWidth = camHeight * aspect;

        // Dibujar el marco de lo que verá la cámara (Color Cian)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(cameraCenter, new Vector3(camWidth, camHeight, 0.1f));

        // 3. Calcular dónde caerá el panel de UI
        // Convertimos los píxeles del UI offset a unidades del mundo de Unity para dibujarlos
        float pixelsToUnits = camHeight / Screen.height;
        Vector3 uiWorldOffset = new Vector3(dialogueTextOffset.x * pixelsToUnits, dialogueTextOffset.y * pixelsToUnits, 0f);

        // Posición aproximada del centro del panel UI
        Vector3 uiCenter = cameraCenter + uiWorldOffset;

        // Dibujar un rectangulito simulando el texto de diálogo (Color Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(uiCenter, new Vector3(camWidth * 0.4f, camHeight * 0.2f, 0.1f));

        // Dibujar una línea desde la cabeza del NPC hasta el bocadillo para ver la relación
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.5f);
        Gizmos.DrawLine(testDialogueTarget.position, uiCenter);
    }
}