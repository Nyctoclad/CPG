using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject target;
    public GameObject structure;
    Vector3 truePos;
    public float gridSize, xoffset, yoffset, zoffset;
    public Bounds bounds;
    public Vector3 up = new Vector3(0,0,0), down = new Vector3(0,0,0), left = new Vector3(0,0,0), right = new Vector3(0,0,0), screenTouch, offset;
    public bool test;
    
    
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
        //truePos = target.transform.position;
        if(bounds != null){
            //Debug.Log("Bounds: " + bounds);
            //Debug.Log("True pos: (" + truePos.x + ", " + truePos.y + ", " + truePos.z + ")");
            if(up != null && down != null && left != null && right != null){
                if(bounds.Contains(new Vector3(truePos.x, bounds.center.y, truePos.z))){
                    structure.transform.position = truePos;
                    bool found = false;
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:floor_bed")){
                      //  Debug.Log("Item name is " + item.name);
                        Debug.Log("Center bounds is " + structure.gameObject.GetComponent<BoxCollider>().bounds.center);
                        if(structure.gameObject.GetComponent<BoxCollider>().bounds.Contains(item.transform.localPosition) && (structure != item)){
                            structure.GetComponent<FurnitureMove>().placeable = false;
                            found = true;
                            Debug.Log("Found is " + found + ", Placeable is " + structure.GetComponent<FurnitureMove>().placeable + ", " + item.transform.position);
                            break;
                        }
                    }
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:wall")){
                        if(structure.gameObject.GetComponent<BoxCollider>().bounds.Contains(item.transform.localPosition) && (structure != item)){
                            structure.GetComponent<FurnitureMove>().placeable = false;
                            found = true;
                            break;
                        }
                    }
                    if(!found){
                        structure.GetComponent<FurnitureMove>().placeable = true;
                        structure.GetComponent<FurnitureMove>().placementLight.SetActive(false);
                    }
                    else if(found){
                        structure.GetComponent<FurnitureMove>().placementLight.SetActive(true);
                        structure.GetComponent<FurnitureMove>().placementRenderer.material.color = Color.red;
                    }
                }
            }
            
        }
        if(test)
            structure.transform.position = truePos;
        
    }
}
