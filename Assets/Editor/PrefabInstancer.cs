using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditorInternal;
using System;
using Unity.VisualScripting;
using TMPro;
using System.IO;
using log4net.Util;
using System.Linq;

public class PrefabInstancer : EditorWindow
{

    public GameObject prefab;
    public GameObject productButtonPrefab;
    public string nombreInstancia;

    public PrincipalProductInfo productInfo;
    public List<OtherProductInfo> otherProducts = new List<OtherProductInfo>();

    

    private Vector2 scrollPosition1, scrollPosition2;

    [MenuItem("Tools/Prefab Instancer")]
    static void Init()
    {
        PrefabInstancer window = (PrefabInstancer)GetWindow(typeof(PrefabInstancer));
        window.Show();
    }
    private void OnEnable()
    {
        //Inicializamos la Lista de Otros Productos
        if(otherProducts.Count == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                otherProducts.Add(new OtherProductInfo());
            }
        }
    }

    private void OnGUI()
    {

        GUILayout.Label("Color Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), false);

        productButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Product Prefab:", productButtonPrefab, typeof(GameObject), false);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();

        scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1);

        productInfo = PrincipalProductField(productInfo);

        GUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical();

        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);

        GUILayout.Label("Otros Productos", EditorStyles.boldLabel);

        //Le damos el nombre de Objeto a cada Otro Producto y Mostramos todos los Otros Productos existentes
        GUILayout.Space(2);
        for (int i = 0; i < otherProducts.Count; i++)
        {
            string objectName = "Other Products Button (" + i + ")";
            otherProducts[i] = OtherProductField(otherProducts[i], objectName, productInfo);
 

        }

        //Boton para agregar Otro Producto
        GUILayout.Space(2);
        if (GUILayout.Button("Agregar Producto"))
        {
            otherProducts.Add(new OtherProductInfo());


        }

        //Boton para limpiar la lista de Otros Productos
        GUILayout.Space(2);
        if (GUILayout.Button("Limpiar Lista"))
        {
            otherProducts.Clear();
        }

        
        GUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        nombreInstancia = EditorGUILayout.TextField("Nombre Instancia", nombreInstancia);

        GUILayout.Space(2);
        if (GUILayout.Button("Crear Instancia"))
        {
            CrearNuevaInstancia(productInfo, otherProducts);
        }
        GUILayout.Space(5);
    }

    void CrearNuevaInstancia(PrincipalProductInfo principalProduct, List<OtherProductInfo> otherProducts)
    {
        

        if (prefab == null)
        {
            Debug.LogError("No se ha asignado un prefab.");
            return;
        }

        GameObject nuevaInstancia = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        Variables urlOpener = null;
        Variables dataSheetManager = null; 
        Variables bubbleVariables = null;
        
        Variables[] tempVariables = FindObjectsOfType<Variables>();

        for (int i = 0; i < tempVariables.Length; i++)
        {
            if(tempVariables[i].transform.name == "URL Opener")
            {
                urlOpener = tempVariables[i];
            }
            else if(tempVariables[i].transform.name == "Data Sheet Manager")
            {
                dataSheetManager = tempVariables[i];
            }
            else if (tempVariables[i].transform.name == "Bubble")
            {
                bubbleVariables = tempVariables[i];
            }
            
        }  

        RectTransform canvas = SearchFirstChild("Canvas", nuevaInstancia).GetComponent<RectTransform>();
        //RectTransform content = FindAnyObjectByType<VerticalLayoutGroup>().gameObject.GetComponent<RectTransform>() ?? ;

        RectTransform content = FindObjectsOfType<RectTransform>().FirstOrDefault(rt => rt.name == "Content");

        List<RectTransform> otherProductsButtons = new List<RectTransform>();

        for (int i = 0; i < content.childCount; i++)
        {

            otherProductsButtons.Add(content.GetChild(i).GetComponent<RectTransform>());

        }


        #region Producto Principal
        //Seccion Producto Principal

        #region Informacion del Producto Principal
        //Informacion Del Producto Principal

        RectTransform[] tempRect = RectTransform.FindObjectsOfType<RectTransform>();

        for (int i = 0; i < tempRect.Length; i++)
        {
            switch (tempRect[i].transform.name)
            {
                case "Product Image":

                    tempRect[i].GetComponent<Image>().sprite = principalProduct.productImage;

                    continue;

                case "Product Name":

                    tempRect[i].GetComponent<TextMeshProUGUI>().text = principalProduct.productName;

                    continue;

                case "Product Price":

                    tempRect[i].GetComponent<TextMeshProUGUI>().text = "$" + principalProduct.productPrice;

                    continue;

                case "Visual Help Image":

                    tempRect[i].GetComponentInChildren<Image>().sprite = principalProduct.visualHelpImage;

                    continue;
                
            }
            
        }


        //SearchFirstRectChild("Product Image", canvas).GetComponent<Image>().sprite = principalProduct.productImage;
        //SearchFirstRectChild("Product Name", canvas).GetComponent<TextMeshProUGUI>().text = principalProduct.productName;
        //SearchFirstRectChild("Product Price", canvas).GetComponent<TextMeshProUGUI>().text = "$"+principalProduct.productPrice;

        Variables.Object(urlOpener).Set("shopURL", principalProduct.productLink);

        Variables.Object(bubbleVariables).Set("AchievementID", principalProduct.achievementID);

        #endregion

        #region Imagenes de Caracteristicas 
        //Seccion de Imagenes de Caracteristicas

        //SearchFirstRectChild("Visual Help Image", canvas).GetComponentInChildren<Image>().sprite = principalProduct.visualHelpImage;

        Variables.Object(dataSheetManager).Set("energyImage", principalProduct.energyImage);
        Variables.Object(dataSheetManager).Set("technologyImage", principalProduct.technologyImage);
        Variables.Object(dataSheetManager).Set("durabilityImage", principalProduct.durabilityImage);

        #endregion

        #region Audios del Producto
        //Seccion Para Los Audios Del Producto Principal

        AudioSource[] tempAudios = GameObject.FindObjectsOfType<AudioSource>();

        for (int i = 0; i < tempAudios.Length; i++)
        {
            switch (tempAudios[i].transform.name)
            {
                case "Warranty Audio":
                    tempAudios[i].clip = principalProduct.warrantyAudio;
                    continue;

                case "Energy Audio":
                    tempAudios[i].clip = principalProduct.energyAudio;
                    continue ;

                case "Technology Audio":
                    tempAudios[i].clip = principalProduct.technologyAudio;
                    continue;

                case "Durability Audio":
                    tempAudios[i].clip = principalProduct.durabilityAudio;
                    break;
                
            }
           
        }
        //SearchFirstChild("Warranty Audio", nuevaInstancia).GetComponent<AudioSource>().clip = principalProduct.warrantyAudio;
        //SearchFirstChild("Energy Audio", nuevaInstancia).GetComponent<AudioSource>().clip = principalProduct.energyAudio;
        //SearchFirstChild("Technology Audio", nuevaInstancia).GetComponent<AudioSource>().clip = principalProduct.technologyAudio;
        //SearchFirstChild("Durability Audio", nuevaInstancia).GetComponent<AudioSource>().clip = principalProduct.durabilityAudio;

        #endregion

        #endregion



        #region Otros Productos
        //Seccion para instanciar los botones para los otros productos

        if (otherProducts.Count > otherProductsButtons.Count)
        {
            
            for (int i = 0; i < (otherProducts.Count - otherProductsButtons.Count)+1; i++)
            {
                otherProductsButtons.Add(productButtonPrefab.GetComponent<RectTransform>());
            }          

        }


        //Recorremos la lista de botones existentes
        for (int i = 0; i < otherProductsButtons.Count; i++)
        {       

            //Recorremos la lista de otros productos
            for (int j = 0; j < otherProducts.Count; j++)
            {         

                //Si el nombre del boton es el mismo al del producto
                if (otherProductsButtons[i].name == otherProducts[j].objectName)
                {

                    //Asignamos los valores de sus hijos
                    FindObjectsOfType<RectTransform>()
                        .FirstOrDefault(
                        rt => rt.name == "Image" && rt.IsChildOf(otherProductsButtons[i])
                        ).GetComponent<Image>().sprite = otherProducts[j].productImage;

                    FindObjectsOfType<RectTransform>()
                        .FirstOrDefault(
                        rt => rt.name == "Text" && rt.IsChildOf(otherProductsButtons[i])
                        ).GetComponent<TextMeshProUGUI>().text = otherProducts[j].productName;

                    FindObjectsOfType<RectTransform>()
                        .FirstOrDefault(
                        rt => rt.name == "Price" && rt.IsChildOf(otherProductsButtons[i])
                        ).GetComponent<TextMeshProUGUI>().text = "$"+otherProducts[j].productPrice;

                    //SearchFirstRectChild("Text", otherProductsButtons[i]).GetComponent<TextMeshProUGUI>().text = otherProducts[j].productName;
                    //SearchFirstRectChild("Price", otherProductsButtons[i]).GetComponent<TextMeshProUGUI>().text = otherProducts[j].productPrice;


                    //Le damos el nombre variable al cual este corresponde en el URLOpener
                    string productVariableName = "productURL" + j.ToString();

                    //Asignamos la URL a la variable
                    Variables.Object(urlOpener).Set(productVariableName, otherProducts[j].productLink);

                    //Lo activamos
                    otherProductsButtons[i].gameObject.SetActive(otherProducts[j].isActive);

                }

               
                

            }

            

        }

        #endregion

        var invalidButtons = otherProductsButtons
                .Where(x => !otherProducts.Any(y => y.objectName == x.name))
                .ToList();

        foreach (var button in invalidButtons)  button.gameObject.SetActive(false); 


        nuevaInstancia.name = nombreInstancia;

        string localPath = "Assets/Prefabs/Objects/Panafoto/" + nuevaInstancia.name + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;

        PrefabUtility.SaveAsPrefabAsset(nuevaInstancia, localPath, out prefabSuccess);

       if (prefabSuccess == true)
        {
            Debug.Log("Prefab was saved successfully");
            DestroyImmediate(nuevaInstancia);
        }
        else
            Debug.Log("Prefab failed to save" + prefabSuccess);
       
        
    }




    [Serializable]
    public struct OtherProductInfo
    {
        public bool isActive;
        public string objectName;
        public Sprite productImage;
        public string productName;
        public string productPrice;
        public string productLink;
    }

    [Serializable]
    public struct PrincipalProductInfo
    {
        public Sprite productImage;
        public string productName;
        public string productPrice;
        public string productLink;
        public string achievementID;

        public Sprite visualHelpImage;
        public Sprite energyImage;
        public Sprite technologyImage;
        public Sprite durabilityImage;

        public AudioClip warrantyAudio;
        public AudioClip energyAudio;
        public AudioClip technologyAudio; 
        public AudioClip durabilityAudio;

    }

    public OtherProductInfo OtherProductField(OtherProductInfo otherProduct, string objectName, PrincipalProductInfo principalProduct)
    {
        otherProduct.objectName = objectName;

        GUILayout.Label(objectName, EditorStyles.boldLabel);
        GUILayout.Space(5);
        otherProduct.isActive = EditorGUILayout.Toggle("It's Active?", otherProduct.isActive);

        otherProduct.productImage = (Sprite)EditorGUILayout.ObjectField("Imagen Producto:", otherProduct.productImage, typeof(Sprite), false);
        otherProduct.productName = EditorGUILayout.TextField("Nombre Producto:", otherProduct.productName);
        otherProduct.productPrice = EditorGUILayout.TextField("Precio Producto:", otherProduct.productPrice);
        otherProduct.productLink = EditorGUILayout.TextField("Link Producto:", otherProduct.productLink);
        
        GUILayout.Space(10);

        return otherProduct;

    }

    public PrincipalProductInfo PrincipalProductField(PrincipalProductInfo principalProduct)
    {

        GUILayout.Label("Producto Principal", EditorStyles.boldLabel);
        GUILayout.Space(2);
        principalProduct.productImage = (Sprite)EditorGUILayout.ObjectField("Imagen Producto:", principalProduct.productImage, typeof(Sprite), false);
        principalProduct.productName = EditorGUILayout.TextField("Nombre Producto:", principalProduct.productName);
        principalProduct.productPrice = EditorGUILayout.TextField("Precio Producto:", principalProduct.productPrice);
        principalProduct.productLink = EditorGUILayout.TextField("Link Producto:", principalProduct.productLink);
        GUILayout.Space(2);
        principalProduct.achievementID = EditorGUILayout.TextField("ID Logro:", principalProduct.achievementID);
        GUILayout.Space(5);

        GUILayout.Label("Imagenes Caracteristicas", EditorStyles.boldLabel);
        GUILayout.Space(2);
        principalProduct.visualHelpImage = (Sprite)EditorGUILayout.ObjectField("Visual Help Image:", principalProduct.visualHelpImage, typeof(Sprite), false);
        principalProduct.energyImage = (Sprite)EditorGUILayout.ObjectField("Imagen Energia:", principalProduct.energyImage, typeof(Sprite), false);
        principalProduct.technologyImage = (Sprite)EditorGUILayout.ObjectField("Imagen Tecnologia:", principalProduct.technologyImage, typeof(Sprite), false);
        principalProduct.durabilityImage = (Sprite)EditorGUILayout.ObjectField("Imagen Durabilidad:", principalProduct.durabilityImage, typeof(Sprite), false);
        GUILayout.Space(5);

        GUILayout.Label("Audios Producto", EditorStyles.boldLabel);
        GUILayout.Space(2);
        principalProduct.warrantyAudio = (AudioClip)EditorGUILayout.ObjectField("Audio Garantia:", principalProduct.warrantyAudio, typeof(AudioClip), false);
        principalProduct.energyAudio = (AudioClip)EditorGUILayout.ObjectField("Audio Energia:", principalProduct.energyAudio, typeof(AudioClip), false);
        principalProduct.technologyAudio = (AudioClip)EditorGUILayout.ObjectField("Audio Tecnologia:", principalProduct.technologyAudio, typeof(AudioClip), false);
        principalProduct.durabilityAudio = (AudioClip)EditorGUILayout.ObjectField("Audio Durabilidad:", principalProduct.durabilityAudio, typeof(AudioClip), false);

        GUILayout.Space(5);

        return principalProduct;
    }

    private GameObject SearchFirstChild(string name, GameObject parent)
    {
       
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            if(parent.transform.GetChild(i).name == name) 
                return parent.transform.GetChild(i).gameObject;

        }

        return null;
    }

    private RectTransform SearchFirstRectChild(string name, RectTransform parent)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == name)
                return parent.GetChild(i).GetComponent<RectTransform>();

        }

        return null;
    }



}