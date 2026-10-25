
using UnityEngine;

public class Outliner : MonoBehaviour
{
    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(1f, 1.5f)]
    private float outlineWidth = 1.1f;

    private SpriteRenderer sourceRenderer;
    private SpriteRenderer outlineRenderer;
    private GameObject outlineObject;

    public Color OutlineColor
    {
        get => outlineColor;
        set
        {
            outlineColor = value;
            UpdateOutline();
        }
    }

    public float OutlineWidth
    {
        get => outlineWidth;
        set
        {
            outlineWidth = value;
            UpdateOutline();
        }
    }

    private void Awake()
    {
        sourceRenderer = GetComponentInChildren<SpriteRenderer>();
        if (sourceRenderer == null)
        {
            Debug.LogWarning("Outliner: no se encontro un SpriteRenderer en el objeto o sus hijos.", this);
            enabled = false;
            return;
        }

        // Creamos la copia del sprite.
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(sourceRenderer.transform, false);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one;

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();

        // Copiamos la configuración visual del sprite original.
        outlineRenderer.sprite = sourceRenderer.sprite;
        outlineRenderer.flipX = sourceRenderer.flipX;
        outlineRenderer.flipY = sourceRenderer.flipY;
        outlineRenderer.drawMode = sourceRenderer.drawMode;
        outlineRenderer.maskInteraction = sourceRenderer.maskInteraction;
        outlineRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = sourceRenderer.sortingOrder - 1;

        UpdateOutline();
    }

    private void LateUpdate()
    {
        if (sourceRenderer == null || outlineRenderer == null)
            return;

        // Actualizamos la copia si cambia el sprite original.
        outlineRenderer.sprite = sourceRenderer.sprite;
        outlineRenderer.flipX = sourceRenderer.flipX;
        outlineRenderer.flipY = sourceRenderer.flipY;
        outlineRenderer.enabled = sourceRenderer.enabled;

        outlineRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = sourceRenderer.sortingOrder - 1;

        UpdateOutline();
    }

    private void UpdateOutline()
    {
        if (outlineRenderer == null)
            return;

        outlineRenderer.color = outlineColor;

        outlineRenderer.transform.localScale =
            Vector3.one * Mathf.Max(1f, outlineWidth);
    }

    private void OnDestroy()
    {
        if (outlineObject != null)
            Destroy(outlineObject);
    }
    public void SwapOutline()
    {
        outlineRenderer.gameObject.SetActive(!outlineRenderer.gameObject.activeSelf);
    }
}
