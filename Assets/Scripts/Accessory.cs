using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Accessory")]
public class Accessory : ScriptableObject
{
    public string accessoryName;
    public int accessoryID;
    public Sprite accessoryIcon;
    public int accessoryLocation;
    public Material accessoryMaterial;
    public GameObject physObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
