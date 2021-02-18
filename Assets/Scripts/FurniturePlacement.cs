using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using BuildSystem;
public class FurniturePlacement : MonoBehaviour
{
    public Furniture furniture;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FurniturePlace(){
        Room room = new Room();
        foreach(GameObject r in GameObject.FindGameObjectsWithTag("roombox")){
            if(r.GetComponent<Room>().active){
                room = r.GetComponent<Room>();
                break;
            }
        }

        room.RezFurniture(furniture, button);
    }

    
  /*public void RezFurniture(){
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            pets.Add(pet);
            pet.SetActive(false);
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("furniture:floor_bed")){
        	if(!g.GetComponentInChildren<MeshRenderer>().enabled){
        		Destroy(g);
        	}
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("furniture:wall")){
        	if(!g.GetComponentInChildren<MeshRenderer>().enabled){
        		Destroy(g);
        	}
        }
        while(GameObject.Find("New GameObject") != null){
            Destroy(GameObject.Find("New GameObject"));
        }

        
        
        rezzedFurniture = GameObject.Instantiate(furniture.physObj);
        Room rezzedRoom = new Room();

        
        //rezzedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
        //Get active room and get active room floor
        foreach(Room room in mlc.roomBoxes){
            if(room.active){
                rezzedFurniture.transform.position = new Vector3(room.gameObject.transform.position.x, 4, room.gameObject.transform.position.z);
                rezzedRoom = room;
                break;
            }
        }

        FurnitureMove fm = rezzedFurniture.AddComponent<FurnitureMove>();
        fm.bounds = rezzedRoom.floorObject.GetComponent<MeshRenderer>().bounds;
        fm.active = true;
        //rezzedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
        Debug.Log("Furniture rotation is (" + furniture.location.x + ", " + furniture.location.y + ", " + furniture.location.z + ")");
        mlc.SeeScreen();
        MoveObject(furniture, rezzedRoom, rezzedFurniture);
        mlc.furnitureCancel.SetActive(true);
        mlc.furnitureCancel.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureCancel.GetComponent<Button>().onClick.AddListener(() => Cancel(rezzedFurniture));
        mlc.furnitureRotate.SetActive(true);
        mlc.furnitureRotate.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureRotate.GetComponent<Button>().onClick.AddListener(() => Rotate(rezzedFurniture, rezzedRoom, furniture));
        mlc.furnitureSet.SetActive(true);
        mlc.furnitureSet.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureSet.GetComponent<Button>().onClick.AddListener(() => PlaceFuniture(rezzedFurniture));

        mlc.crActive = true;
        rezzedRoom.furnitureMover.transform.position = rezzedFurniture.transform.position;
        this.gameObject.GetComponent<Button>().enabled = false;
        
    }*/

    /*public void MoveObject(Furniture f, Room r, GameObject rf){
        r.furnitureMover.transform.position = new Vector3(rf.transform.position.x, r.furnitureMover.transform.position.y, rf.transform.position.z);
        grid.target = r.furnitureMover;
        grid.structure = rf;
        Bounds bounds = r.floorObject.GetComponent<MeshRenderer>().bounds;
        grid.bounds = bounds;

        BoxCollider bc = rf.GetComponent<BoxCollider>();
        //grid.up = new Vector3(0,0,bc.size.z/2);
        //grid.down = new Vector3(0,0,-bc.size.z/2);
        //grid.left = new Vector3(-bc.size.x/2,0,0);
        //grid.right = new Vector3(bc.size.x/2,0,0);
        if(f.physObj.tag.Contains("floor")){
            if(r.roomSize == 4){
                Camera.main.transform.localPosition = new Vector3(bounds.center.x, 18, bounds.center.z);
                Camera.main.transform.eulerAngles = new Vector3(90,0,0);
            }
        }
        else if(f.physObj.tag.Contains("wall")){

        }

        //BuildItem item = new BuildItem();
        //item.Prefab = rf;
        //item.ghostCache = rf;
        //item.Name = f.furnitureName;
        //op.objectToPlace = rf;
        //op.Toggle();
    }*/

    /*public void PlaceFuniture(GameObject rf){
        Debug.Log("Click!");
		FurnitureMove fm = rf.GetComponent<FurnitureMove>();
        bool found = false;
        Bounds bounds = rf.GetComponent<BoxCollider>().bounds;
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:floor_bed")){
            //  Debug.Log("Item name is " + item.name);
            //Debug.Log("Center bounds is " + structure.gameObject.GetComponent<BoxCollider>().bounds.center);
            Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
            if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (rf != item)){
                //fm.placeable = false;
                found = true;
                // Debug.Log("Found is " + found + ", Placeable is " + structure.GetComponent<FurnitureMove>().placeable + ", " + item.transform.position);
                break;
            }
        }
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("wall")){
            //  Debug.Log("Item name is " + item.name);
            //Debug.Log("Center bounds is " + structure.gameObject.GetComponent<BoxCollider>().bounds.center);
            Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
            if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (rf != item)){
               // fm.placeable = false;
                found = true;
                // Debug.Log("Found is " + found + ", Placeable is " + structure.GetComponent<FurnitureMove>().placeable + ", " + item.transform.position);
                break;
            }
        }
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("furniture:wall")){
            if((bounds.Contains(item.transform.localPosition)) && (rf != item)){
                //fm.placeable = false;
                found = true;
                break;
            }
        }
        if(!found){
           // fm.placeable = true;
            foreach(Furniture f in player.furniture){
                if(f.uniqueFurnitureID == furniture.uniqueFurnitureID){
                    SQLFunctions sqf = playerInfo.gameObject.GetComponent<SQLFunctions>();
                    f.placed = true;
                    f.location = rf.transform.position;
                    f.rotation = rf.transform.eulerAngles;
                    grid.structure = new GameObject();
                    Camera.main.transform.position = camOriginalPosition;
                    Camera.main.transform.eulerAngles = camOriginalRotation;
                    mlc.HideScreen();
                    mlc.furnitureCancel.SetActive(false);
                    mlc.furnitureRotate.SetActive(false);
                    mlc.furnitureSet.SetActive(false);
                    mlc.crActive = false;
                    foreach(GameObject pet in pets){
                        pet.SetActive(true);
                    }

                    pets.Clear();
                    this.gameObject.GetComponent<Button>().enabled = true;
                    
                    foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                        if(obj.name.Equals("New Game Object")){
                            Destroy(obj);
                        }
                    }
                    
                    string furnitureLocation = "" + f.location.x + ":" + f.location.y + ":" + f.location.z;
                    string furnitureRotation = "" + f.rotation.x + ":" + f.rotation.y + ":" + f.rotation.z;
                    sqf.SubmitFurniture(f.uniqueFurnitureID, f.furnitureID.ToString(), f.furnitureColorID.ToString(), furnitureLocation, furnitureRotation);
                    
                    rf.GetComponent<FurnitureMove>().active = false;
                    
                    mlc.SetFurnitureList();
                }
            }
            fm.placementLight.SetActive(false);
        }
        else if(found){
            playerInfo.messages.errorText.text = "Cannot place; furniture blocked.";
            playerInfo.messages.errorTitle.text = "Error";
            playerInfo.messages.ErrorMessage.SetActive(true);
            fm.placementLight.SetActive(true);
            fm.placementRenderer.material.color = Color.red;
        }
    }
    
    public void Cancel(GameObject rf){
    	Debug.Log("Clicked cancel");
        grid.structure = new GameObject();
        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;
    	//Destroy(rf);
    //	rf.SetActive(false);
    	foreach(Transform child in rf.transform){
    		if(child.gameObject.GetComponent<MeshRenderer>() != null){
    			child.gameObject.GetComponent<MeshRenderer>().enabled = false;
    		}
    	}
    	mlc.HideScreen();
    	mlc.furnitureCancel.SetActive(false);
    	mlc.furnitureRotate.SetActive(false);
        mlc.furnitureSet.SetActive(false);
        mlc.crActive = false;
        foreach(GameObject pet in pets){
            pet.SetActive(true);
        }

        pets.Clear();
    	this.gameObject.GetComponent<Button>().enabled = true;
        rf.GetComponent<FurnitureMove>().active = false;
    }
    
    public void Rotate(GameObject rf, Room r, Furniture f){
    	Debug.Log("Clicked rotate button.");
    	Vector3 newRotations = rf.transform.localEulerAngles + new Vector3(0,90,0);
    	rf.transform.localEulerAngles = new Vector3(newRotations.x, newRotations.y % 360, newRotations.z);

        //MoveObject(f, r, rf);
        
    }*/
}