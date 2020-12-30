using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Animal List")]
public class AnimalList : ScriptableObject
{
    public List<Animal> animals;
    public List<AnimalColor> colors;
    public List<AnimalFaces> faces;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
