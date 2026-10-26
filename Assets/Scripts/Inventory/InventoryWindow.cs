using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class InventoryWindown : EditorWindow
{
    //Ruta donde se guarda el archivo de los ajustes del invetario
    const string settingsFolder = "Assets/Scripts/Inventory";
    const string settingsPath = settingsFolder + "/InventorySettings.asset";
    
    InventorySettings settings; //Guarda el archivo que estamos creando
    Editor settingsWindow; //Es lo que escribe los campos en la ventana
    Vector2 scroll; //La barra vertical para desplazarnos por la ventana
    
    [MenuItem("Tools/Inventory/Editor")]
    static void Open()
    {
        InventoryWindown window = GetWindow<InventoryWindown>("Inventory");
        window.maxSize = new Vector2(1000, 800);
        window.minSize = new Vector2(300, 200);
    }

    void OnEnable() => settings = LoadSettings();
    void OnDisable() => DestroyImmediate(settingsWindow);
    
    public void OnGUI()  
    {
        EditorGUILayout.LabelField("Inventory Settings", EditorStyles.boldLabel); 
        
        settings = (InventorySettings)EditorGUILayout.ObjectField("Settings asset", settings, typeof(InventorySettings), false);
 
        EditorGUILayout.Space();
 
        // Los campos se dibujan con el inspector del asset: respeta [Header], [Tooltip], [Min]...
        if (settingsWindow == null || settingsWindow.target != settings)
        {
            DestroyImmediate(settingsWindow);
            if (settings != null)
            {
                settingsWindow = Editor.CreateEditor(settings);
            }
        }
 
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (settingsWindow != null)
        {
            settingsWindow.OnInspectorGUI();
        }
        EditorGUILayout.EndScrollView();

        if (settings == null)
        {
            EditorGUILayout.HelpBox(
                $"No hay un asset de InventorySettings asignado. Asignalo arriba o crea uno en {settingsFolder}.",
                MessageType.Warning);
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.Button("Create", GUILayout.Height(30));
            }
            return;
        }
        
        Vector2 size = settings.panelSize;
        
        EditorGUILayout.HelpBox($"{settings.rows} rows x {settings.columns} columns = {settings.SlotCount} slots\n" + $"Panel Size: {size.x:0} x {size.y:0}", MessageType.Info); //Informacion en la parte baja de la venta
 
        if (GUILayout.Button("Create", GUILayout.Height(30)))
        {
            AssetDatabase.SaveAssets();
            
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.settings = settings;
                InventoryManager.Instance.InitializeInventory();
            }
            else
            {
                InventoryBuilder.Build(settings);
            }
        }
    }
    
    static InventorySettings LoadSettings() //Comprueba si se ha cargado el archivo de settings del inventario
    {
        return AssetDatabase.LoadAssetAtPath<InventorySettings>(settingsPath);
    }
}
