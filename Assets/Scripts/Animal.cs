using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Custom Assets/Animal")]
public class Animal : ScriptableObject
{
    public string petName;
    public int petID;
    public GameObject physObj;
    public int animalType;
    public string animalName;
    public AnimalFaces face;
    public AnimalColor color;
    public Accessory headAccessory;
    public Accessory faceAccessory;
    public Accessory backAccessory;
    public Accessory neckAccessory;
    public int glow;
    public int special;
    public Accessory leftHand;
    public Accessory rightHand;
    public Clothing clothing;
    public List<PetAffection> affections;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
