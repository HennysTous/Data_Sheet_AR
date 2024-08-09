using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditorInternal;

public class ColorToolWindow : EditorWindow
{
    private Color primaryColor = Color.white;
    private Color secondaryColor = Color.white;
    private Color tertiaryColor = Color.white;

    private ReorderableList primaryMaterialsList;
    private ReorderableList secondaryMaterialsList;
    private ReorderableList tertiaryMaterialsList;

    private ReorderableList primaryImagesList;
    private ReorderableList secondaryImagesList;
    private ReorderableList tertiaryImagesList;

    private List<Material> primaryMaterials = new List<Material>();
    private List<Material> secondaryMaterials = new List<Material>();
    private List<Material> tertiaryMaterials = new List<Material>();

    private List<Image> primaryImages = new List<Image>();
    private List<Image> secondaryImages = new List<Image>();
    private List<Image> tertiaryImages = new List<Image>();

    private Vector2 scrollPosition;

    [MenuItem("Tools/Color Tool")]
    public static void ShowWindow() => GetWindow<ColorToolWindow>("Color Tool");

    private void OnEnable()
    {
        primaryColor = LoadColor("primaryColor", Color.white);
        secondaryColor = LoadColor("secondaryColor", Color.white);
        tertiaryColor = LoadColor("tertiaryColor", Color.white);

        primaryMaterials = LoadList<Material>("primaryMaterials");
        secondaryMaterials = LoadList<Material>("secondaryMaterials");
        tertiaryMaterials = LoadList<Material>("tertiaryMaterials");

        primaryImages = LoadList<Image>("primaryImages");
        secondaryImages = LoadList<Image>("secondaryImages");
        tertiaryImages = LoadList<Image>("tertiaryImages");

        primaryMaterialsList = CreateReorderableList("Primary Materials", primaryMaterials);
        secondaryMaterialsList = CreateReorderableList("Secondary Materials", secondaryMaterials);
        tertiaryMaterialsList = CreateReorderableList("Tertiary Materials", tertiaryMaterials);

        primaryImagesList = CreateReorderableList("Primary Images", primaryImages);
        secondaryImagesList = CreateReorderableList("Secondary Images", secondaryImages);
        tertiaryImagesList = CreateReorderableList("Tertiary Images", tertiaryImages);
    }

    private ReorderableList CreateReorderableList<T>(string header, List<T> list) where T : UnityEngine.Object
    {
        return new ReorderableList(list, typeof(T), true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, header),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                list[index] = (T)EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), list[index], typeof(T), true);
            },
            onAddCallback = (ReorderableList l) =>
            {
                l.list.Add(null);
            }
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Color Tool", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        GUILayout.Label("Primary Color", EditorStyles.boldLabel);
        primaryColor = EditorGUILayout.ColorField("Color", primaryColor);
        primaryMaterialsList.DoLayoutList();
        primaryImagesList.DoLayoutList();

        GUILayout.Space(20);

        GUILayout.Label("Secondary Color", EditorStyles.boldLabel);
        secondaryColor = EditorGUILayout.ColorField("Color", secondaryColor);
        secondaryMaterialsList.DoLayoutList();
        secondaryImagesList.DoLayoutList();

        GUILayout.Space(20);

        GUILayout.Label("Tertiary Color", EditorStyles.boldLabel);
        tertiaryColor = EditorGUILayout.ColorField("Color", tertiaryColor);
        tertiaryMaterialsList.DoLayoutList();
        tertiaryImagesList.DoLayoutList();

        GUILayout.Space(20);

        if (GUILayout.Button("Apply Colors", GUILayout.Height(50)))
            ApplyColors();

        EditorGUILayout.EndScrollView();

        SaveColor("primaryColor", primaryColor);
        SaveColor("secondaryColor", secondaryColor);
        SaveColor("tertiaryColor", tertiaryColor);

        SaveList("primaryMaterials", primaryMaterials);
        SaveList("secondaryMaterials", secondaryMaterials);
        SaveList("tertiaryMaterials", tertiaryMaterials);

        SaveList("primaryImages", primaryImages);
        SaveList("secondaryImages", secondaryImages);
        SaveList("tertiaryImages", tertiaryImages);
    }

    private void ApplyColors()
    {
        ApplyColorToMaterials(primaryColor, primaryMaterials);
        ApplyColorToMaterials(secondaryColor, secondaryMaterials);
        ApplyColorToMaterials(tertiaryColor, tertiaryMaterials);

        ApplyColorToImages(primaryColor, primaryImages);
        ApplyColorToImages(secondaryColor, secondaryImages);
        ApplyColorToImages(tertiaryColor, tertiaryImages);
    }

    private void ApplyColorToMaterials(Color color, List<Material> materials)
    {
        foreach (Material material in materials)
        {
            if (material != null)
                material.color = color;
        }
    }

    private void ApplyColorToImages(Color color, List<Image> images)
    {
        foreach (Image image in images)
        {
            if (image != null)
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(image.gameObject);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab != null)
                {
                    Image prefabImage = prefab.GetComponent<Image>();

                    if (prefabImage != null)
                    {
                        prefabImage.color = color;
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
    }

    private void SaveColor(string key, Color color)
    {
        EditorPrefs.SetFloat($"{key}_r", color.r);
        EditorPrefs.SetFloat($"{key}_g", color.g);
        EditorPrefs.SetFloat($"{key}_b", color.b);
        EditorPrefs.SetFloat($"{key}_a", color.a);
    }

    private Color LoadColor(string key, Color defaultValue)
    {
        float r = EditorPrefs.GetFloat($"{key}_r", defaultValue.r);
        float g = EditorPrefs.GetFloat($"{key}_g", defaultValue.g);
        float b = EditorPrefs.GetFloat($"{key}_b", defaultValue.b);
        float a = EditorPrefs.GetFloat($"{key}_a", defaultValue.a);

        return new Color(r, g, b, a);
    }

    private void SaveList<T>(string key, List<T> list) where T : UnityEngine.Object
    {
        for (int i = 0; i < list.Count; i++)
        {
            string itemKey = $"{key}_{i}";

            if (list[i] != null)
                EditorPrefs.SetString(itemKey, AssetDatabase.GetAssetPath(list[i]));
            else
                EditorPrefs.DeleteKey(itemKey);
        }
    }

    private List<T> LoadList<T>(string key) where T : UnityEngine.Object
    {
        List<T> list = new List<T>();
        int i = 0;

        while (EditorPrefs.HasKey($"{key}_{i}"))
        {
            string itemKey = $"{key}_{i}";
            string assetPath = EditorPrefs.GetString(itemKey);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            list.Add(asset);
            i++;
        }

        return list;
    }
}
