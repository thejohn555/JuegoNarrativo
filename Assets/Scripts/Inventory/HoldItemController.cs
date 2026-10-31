using System;
using Blueprints_arquitectura_Dani_.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeldItemController : MonoBehaviour
{
    public static HeldItemController Instance { get; private set; }

    [SerializeField] private Canvas canvas; // opcional, si no se asigna se busca solo
    [SerializeField] private Vector2 iconSize = new Vector2(64, 64);

    public event Action<string> OnItemHeld;
    public event Action<string> OnItemReleased;

    private int heldSlotIndex = -1;
    private int pickupFrame = -1;
    private string heldItemName;
    private GameObject heldItemPrefab; //Nuevo
    private GameObject iconObject;
    private RectTransform iconRect;

    public bool HasHeldItem => heldSlotIndex != -1;
    public string HeldItemName => heldItemName;
    public GameObject HeldItemPrefab => heldItemPrefab; //Nuevo

    public bool IsHolding(GameObject prefab)
    {
        Debug.Log($"Prefab: {prefab}");
        Debug.Log($"HasheldItem: {HasHeldItem}");
        Debug.Log($"heldItemPrefab: {heldItemPrefab}");
         return HasHeldItem && prefab != null && heldItemPrefab == prefab; //Nuevo
    }

    public bool IsHolding(string itemName) => HasHeldItem && heldItemName == itemName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() //Nuevo
    {
        if (BlueprintsEventManager.Instance != null)
            BlueprintsEventManager.Instance.IsHoldingPrefab = IsHolding;
    }

    // Coge el objeto del slot con el cursor
    public void Hold(int slotIndex)
    {
        InventoryManager inventory = InventoryManager.Instance;

        if (inventory == null || !inventory.TryGetItem(slotIndex, out string itemName,
                out Sprite icon,out GameObject prefab)) //Nuevo
        {
            return;
        }

        if (HasHeldItem)
        {
            Release();
        }

        heldSlotIndex = slotIndex;
        heldItemName = itemName;
        heldItemPrefab = prefab;
        pickupFrame = Time.frameCount;

        inventory.HeldSlotIndex = slotIndex;
        inventory.UpdateUI();

        CreateIcon(icon);
        OnItemHeld?.Invoke(itemName);
    }

    // Devuelve el objeto al inventario
    public void Release()
    {
        if (!HasHeldItem)
        {
            return;
        }

        OnItemReleased?.Invoke(heldItemName);
        ClearState();
    }

    // Gasta el objeto (ej. la llave abre la puerta)
    public bool Consume()
    {
        if (!HasHeldItem)
        {
            return false;
        }
        int index = heldSlotIndex;
        ClearState();
        InventoryManager.Instance.ClearSlot(index);
        //OnItemReleased?.Invoke("");
        return true;
    }
    public void ClearState()
    {
        heldSlotIndex = -1;
        heldItemName = null;
        heldItemPrefab = null; //Nuevo

        if (iconObject != null)
        {
            Destroy(iconObject);
            iconObject = null;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.HeldSlotIndex = -1;
            InventoryManager.Instance.UpdateUI();
        }
    }

    private void CreateIcon(Sprite sprite)
    {
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            return;
        }

        canvas = canvas.rootCanvas;

        iconObject = new GameObject("HeldItemCursor", typeof(RectTransform));
        iconObject.layer = canvas.gameObject.layer;
        iconObject.transform.SetParent(canvas.transform, false);
        iconObject.transform.SetAsLastSibling();

        Image img = iconObject.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false; // no bloquea clics

        iconRect = (RectTransform)iconObject.transform;
        iconRect.sizeDelta = iconSize;
        iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0.5f, 0.5f);

        MoveIcon();
    }

    private void MoveIcon()
    {
        if (iconRect == null || Mouse.current == null)
        {
            return;
        }

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 mouse = Mouse.current.position.ReadValue();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)canvas.transform, mouse, cam, out Vector3 worldPos))
        {
            iconRect.position = worldPos;
        }
    }

    private void Update()
    {
        if (iconObject != null)
        {
            MoveIcon();
        }
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnItemReleased?.Invoke(heldItemName);
            //Release();
        }
    }

    // LateUpdate: así la puerta puede consumir el objeto en su Update antes de que el click lo suelte
    private void LateUpdate()
    {
        if (!HasHeldItem || Time.frameCount == pickupFrame)
        {
            return;
        }

        
    }
}
