using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Furniture")]
public class Furniture : ScriptableObject
{
    public float furnitureID;
    public string furnitureName;
    public float furnitureColorID;
    public string colorName;
    public int furnitureType;
    public Material material;
    public GameObject physObj;
    public GameObject icon;
    public Vector3 location, rotation;
    public bool placed;
    public int count;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
