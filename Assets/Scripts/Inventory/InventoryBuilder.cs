using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.UIElements.Image;
using Image = UnityEngine.UI.Image;

public static class InventoryBuilder 
{
   public static GameObject Build(InventorySettings settings)
    {
        if (settings == null)
        {
            Debug.LogError("InventoryBuilder: there are no settings.");
            return null;
        }
 
        Canvas canvas = GetOrCreateCanvas();
        
        //UI del Canvas
        GameObject panel = NewUI(settings.inventoryName, canvas.transform, typeof(Image), typeof(GridLayoutGroup));
 
        //Rectangulo del Panel del inventario
        RectTransform panelRectangle = (RectTransform)panel.transform;
        panelRectangle.anchorMin = panelRectangle.anchorMax = panelRectangle.pivot = new Vector2(0.5f, 0.5f);
        panelRectangle.anchoredPosition = Vector2.zero;
        panelRectangle.sizeDelta = settings.panelSize;
        
        //Imagen background del Panel
        Image background = panel.GetComponent<Image>();
        background.sprite = settings.panelSprite;
        background.color = settings.panelColor;
        background.type = settings.panelSprite != null ? Image.Type.Sliced : Image.Type.Simple;
 
        //Grid del inventario
        GridLayoutGroup panelGrid = panel.GetComponent<GridLayoutGroup>();
        panelGrid.cellSize = settings.slotSize;
        panelGrid.spacing = settings.spacing;
        panelGrid.padding = new RectOffset(settings.padding, settings.padding, settings.padding, settings.padding);
        panelGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        panelGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
        panelGrid.childAlignment = TextAnchor.UpperLeft;
        panelGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        panelGrid.constraintCount = settings.columns;
 
        // Slots
        for (int row = 0; row < settings.rows; row++)
        {
            for (int colum = 0; colum < settings.columns; colum++)
            {
                GameObject slot = NewUI($"Slot_{row}_{colum}", panel.transform, typeof(Image));
                Image slotImg = slot.GetComponent<Image>();
                slotImg.sprite = settings.slotSprite;
                slotImg.color = settings.slotColor;
 
                // Hijo del slot donde se mostrará el icono del item (oculto hasta que haya uno)
                GameObject icon = NewUI("Icon", slot.transform, typeof(Image));
                RectTransform iconeRectangle = (RectTransform)icon.transform;
                iconeRectangle.anchorMin = Vector2.zero;
                iconeRectangle.anchorMax = Vector2.one;
                iconeRectangle.offsetMin = new Vector2(6, 6);
                iconeRectangle.offsetMax = new Vector2(-6, -6);
 
                Image iconImage = icon.GetComponent<Image>();
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;
                iconImage.enabled = true;
            }
        }
        Selection.activeGameObject = panel;
        return panel;
    }
   
 
    static GameObject NewUI(string name, Transform parent, params System.Type[] components)
    {
        var types = new System.Type[components.Length + 1];
        types[0] = typeof(RectTransform);
        components.CopyTo(types, 1);
 
        var go = new GameObject(name, types);
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go;
    }
 
    static Canvas GetOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            return canvas;
        }
 
        GameObject go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.layer = LayerMask.NameToLayer("UI");
 
        canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
 
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
 
        Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
        return canvas;
    }
}
