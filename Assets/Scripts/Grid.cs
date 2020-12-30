using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject target;
    public GameObject structure;
    Vector3 truePos;
    public float gridSize, xoffset, yoffset, zoffset;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        truePos.x = Mathf.Floor(target.transform.position.x/gridSize)*gridSize + xoffset;
        truePos.y = Mathf.Floor(target.transform.position.y/gridSize)*gridSize + yoffset;
        truePos.z = Mathf.Floor(target.transform.position.z/gridSize)*gridSize + zoffset;
        structure.transform.position = truePos;
        
    }
}
