using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomSize;
    public bool active, defaultRoom;
    public Mat floor, wall1, wall2, wall3;
    public GameObject floorObject, wall1Object, wall2Object, wall3Object, physObj;
    public Vector3 location;
    public Quaternion rotation;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
