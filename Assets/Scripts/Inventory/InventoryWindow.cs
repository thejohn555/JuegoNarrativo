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
            settingsWindow = Editor.CreateEditor(settings);
        }
 
        scroll = EditorGUILayout.BeginScrollView(scroll);
        settingsWindow.OnInspectorGUI();
        EditorGUILayout.EndScrollView();
        
        Vector2 size = settings.panelSize;
        
        EditorGUILayout.HelpBox($"{settings.rows} filas x {settings.columns} columnas = {settings.SlotCount} slots\n" + $"Tamaño del panel: {size.x:0} x {size.y:0}", MessageType.Info); //Informacion en la parte baja de la venta
 
        if (GUILayout.Button("Create", GUILayout.Height(30)))
        {
            AssetDatabase.SaveAssets();  // guarda los settings antes de usarlos
            InventoryBuilder.Build(settings);
        }
    }
    
    static InventorySettings LoadSettings() //Comprueba si se ha cargado el archivo de settings del inventario
    {
        InventorySettings alreadyCreated = AssetDatabase.LoadAssetAtPath<InventorySettings>(settingsPath);
        
        if (alreadyCreated == null)
        {
           Debug.Log("No se ha encontrado el archivo de InventorySettings");
        }

        return alreadyCreated;
    }
}
