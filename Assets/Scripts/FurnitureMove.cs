using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureMove : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    public Bounds bounds;
    public GameObject placementLight;
    public MeshRenderer placementRenderer;
    public Furniture furniture;
    MainLandController mlc;
    Room room = new Room();


    public bool placeable = true, active = false;
    
    // Start is called before the first frame update
    void Start()
    {
        mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
        placementLight = this.gameObject.transform.Find("PlacementLight").gameObject;
        placementRenderer = placementLight.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
 
 void OnMouseDown()
 {
   mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
   foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
        if(obj.name.Equals("New Game Object")){
            Destroy(obj);
        }
    }
   foreach(GameObject r in GameObject.FindGameObjectsWithTag("roombox")){
       if(r.GetComponent<Room>().active){
            room = r.GetComponent<Room>();
            bounds = room.floorObject.GetComponent<MeshRenderer>().bounds;
            break;
       }
   }

   if(mlc.furnitureScreen.activeSelf && !active){

        bool found = false;
        foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
            if(room.furnitureTags.Contains(obj.tag)){
                if(obj.GetComponent<FurnitureMove>().active){
                    found = true;
                    break;
                }
            }
        }

        if(!found){
            active = true;
            room.MoveFurniture(furniture, this.gameObject);
        }
    }
    
    if(mlc.furnitureScreen.activeSelf && active){
        Debug.Log("On mouse down, I am active!");
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }
 }
 
 void OnMouseDrag()
 {
     if(mlc.furnitureScreen.activeSelf && active){
         Debug.Log("I'm active!");
        foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
            if(obj.name.Equals("New Game Object")){
                Destroy(obj);
            }
        }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
 
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        if(bounds != null){
            Debug.Log("Out bounds exists!");
            if(bounds.Contains(new Vector3(curPosition.x, bounds.center.y, curPosition.z))){
                Debug.Log("Within out bounds!");
                transform.position = new Vector3(curPosition.x, transform.position.y, curPosition.z);
                bool found = false;
                Bounds inBounds = this.gameObject.GetComponent<BoxCollider>().bounds;
                foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){

                    if(room.furnitureTags.Contains(item.tag) || room.roomTags.Contains(item.tag)){
                        //Debug.Log("Any items? " + room.furnitureTags.Contains(item.tag));
                        if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                            Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
                            if((inBounds.Contains(item.transform.localPosition) || inBounds.Intersects(itemBounds)) && (this.gameObject != item)){
                                placeable = false;
                                found = true;
                                break;
                            }
                        }
                    }
                }

                

                if(!found){
                    Color color = new Color();
                    placeable = true;
                    color.a = 0f;
                    placementRenderer.material.color = color;
                    //placementLight.SetActive(false);
                    GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().furnitureSet.GetComponent<Button>().enabled = true;
                    
                }
                else if(found){
                    placementLight.SetActive(true);
                    placementRenderer.material.color = Color.red;
                }
            }
        }
        else{
            Debug.Log("No bounds on this object!");
        }
     }

 }

}
