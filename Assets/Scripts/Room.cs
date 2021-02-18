using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public int roomSize, roomID;
    public bool active, defaultRoom;
    public Mat floor, wall1, wall2, wall3;
    public GameObject floorObject, wall1Object, wall2Object, wall3Object, physObj;
    public Vector3 location;
    public Quaternion rotation;
    public GameObject furnitureMover;

    MainLandController mlc;
    Player player;
    PlayerInformation playerInfo;
    Vector3 bounds1, bounds2, bounds3, bounds4, screenTouch, offset;
    Grid grid;

    Vector3 camOriginalPosition, camOriginalRotation;
    public List<string> furnitureTags = new List<string>(), roomTags = new List<string>();

    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
        playerInfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        player = playerInfo.player;
        grid = GameObject.FindGameObjectWithTag("scriptholder").GetComponent<Grid>();
        camOriginalPosition = Camera.main.transform.position;
        camOriginalRotation = Camera.main.transform.eulerAngles;
        Debug.Log("Furniture tag number 1 is " + playerInfo.furnitureTags.tags[0]);
        foreach(string tag in playerInfo.furnitureTags.tags){
            furnitureTags.Add(tag);
        }
        foreach(string tag in playerInfo.roomTags.tags){
            roomTags.Add(tag);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RezFurniture(Furniture furniture, Button button){
        GameObject rezzedFurniture = new GameObject();
        List<GameObject> pets = new List<GameObject>();
        Room rezzedRoom = new Room();
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            pets.Add(pet);
            pet.SetActive(false);
        }
        foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>()){
        	if(furnitureTags.Contains(g.tag) || roomTags.Contains(g.tag)){
                if(!g.GetComponentInChildren<MeshRenderer>().enabled){
                    Destroy(g);
                }
            }
        }   
        rezzedFurniture = GameObject.Instantiate(furniture.physObj);
        if(active){
            rezzedFurniture.transform.position = new Vector3(this.gameObject.transform.position.x, 4, this.gameObject.transform.position.z);
            rezzedRoom = this;
        }

        FurnitureMove fm = rezzedFurniture.AddComponent<FurnitureMove>();
        fm.bounds = rezzedRoom.floorObject.GetComponent<MeshRenderer>().bounds;
        fm.active = true;
        fm.furniture = furniture;

        mlc.SeeScreen();
        
        MoveObject(furniture, rezzedRoom, rezzedFurniture);
        
        mlc.furnitureCancel.SetActive(true);
        mlc.furnitureCancel.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureCancel.GetComponent<Button>().onClick.AddListener(() => CancelAfterRezz(rezzedFurniture, pets, button));
        mlc.furnitureRotate.SetActive(true);
        mlc.furnitureRotate.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureRotate.GetComponent<Button>().onClick.AddListener(() => Rotate(rezzedFurniture, rezzedRoom, furniture));
        mlc.furnitureSet.SetActive(true);
        mlc.furnitureSet.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureSet.GetComponent<Button>().onClick.AddListener(() => PlaceFunitureAfterRezz(rezzedFurniture, button, pets, furniture));

        mlc.crActive = true;
        rezzedRoom.furnitureMover.transform.position = rezzedFurniture.transform.position;
        button.enabled = false;
        Debug.Log("1. REZZED FURNITURE NAME: " + rezzedFurniture.name);
    }

    public void CancelAfterRezz(GameObject rf, List<GameObject> pets, Button button){

        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;

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


    	button.enabled = true;
        rf.GetComponent<FurnitureMove>().active = false;
    }

    public void PlaceFunitureAfterRezz(GameObject rf, Button button, List<GameObject> pets, Furniture furniture){
		Debug.Log("2. REZZED FURNITURE NAME: " + rf.name);
        FurnitureMove fm = rf.GetComponent<FurnitureMove>();
        bool found = false;
        Bounds bounds = rf.GetComponent<BoxCollider>().bounds;
        foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){
            if(furnitureTags.Contains(item.tag) || roomTags.Contains(item.tag)){
                if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                    Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
                    if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (this.gameObject != item)){
                        found = true;
                        break;
                    }
                }
            }
        }

        if(!found){
            foreach(Furniture f in player.furniture){
                if(f.uniqueFurnitureID == furniture.uniqueFurnitureID){
                    SQLFunctions sqf = playerInfo.gameObject.GetComponent<SQLFunctions>();

                    f.placed = true;
                    f.location = rf.transform.position;
                    f.rotation = rf.transform.eulerAngles;
                    f.roomID = roomID;

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
                    button.enabled = true;
                    
                    foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                        if(obj.name.Equals("New Game Object")){
                            Destroy(obj);
                        }
                    }
                    
                    string furnitureLocation = "" + f.location.x + ":" + f.location.y + ":" + f.location.z;
                    string furnitureRotation = "" + f.rotation.x + ":" + f.rotation.y + ":" + f.rotation.z;
                    sqf.SubmitFurniture(f.uniqueFurnitureID, f.furnitureID.ToString(), f.furnitureColorID.ToString(), furnitureLocation, furnitureRotation, f.roomID);
                    
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

    public void MoveFurniture(Furniture furniture, GameObject rezzedFurniture){
        List<GameObject> pets = new List<GameObject>();
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            pets.Add(pet);
            pet.SetActive(false);
        }

        foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>()){
        	foreach(MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>()){
                if(mr != null){
                    if(!mr.enabled && furnitureTags.Contains(g.tag)){
                        Destroy(g);
                    }
                }
            }
        }

        Room rezzedRoom = new Room();


        //Get active room and get active room floor
        if(active){
            rezzedRoom = this;
        }

        FurnitureMove fm = rezzedFurniture.GetComponent<FurnitureMove>();
        fm.active = true;

        MoveObject(furniture, rezzedRoom, rezzedFurniture);
        mlc.furnitureCancel.SetActive(true);
        mlc.furnitureCancel.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureCancel.GetComponent<Button>().onClick.AddListener(() => Cancel(rezzedFurniture, pets, furniture));
        mlc.furnitureRotate.SetActive(true);
        mlc.furnitureRotate.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureRotate.GetComponent<Button>().onClick.AddListener(() => Rotate(rezzedFurniture, rezzedRoom, furniture));
        mlc.furnitureSet.SetActive(true);
        mlc.furnitureSet.GetComponent<Button>().onClick.RemoveAllListeners();
        mlc.furnitureSet.GetComponent<Button>().onClick.AddListener(() => PlaceFuniture(rezzedFurniture, pets, furniture));
    }

    public void Cancel(GameObject rf, List<GameObject> pets, Furniture furniture){
    	Debug.Log("Clicked cancel");
        
        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;

    	
    	
    	mlc.furnitureCancel.SetActive(false);
    	mlc.furnitureRotate.SetActive(false);
        mlc.furnitureSet.SetActive(false);

        foreach(GameObject pet in pets){
            pet.SetActive(true);
        }

        pets.Clear();
        rf.GetComponent<FurnitureMove>().active = false;
    }

    public void PlaceFuniture(GameObject rf, List<GameObject> pets, Furniture furniture){
		FurnitureMove fm = rf.GetComponent<FurnitureMove>();
        bool found = false;
        Bounds bounds = rf.GetComponent<BoxCollider>().bounds;

        foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){
            if(furnitureTags.Contains(item.tag) || roomTags.Contains(item.tag)){
                if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                    Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
                    if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (this.gameObject != item)){
                        found = true;
                        break;
                    }
                }
            }
        }

        if(!found){
            foreach(Furniture f in player.furniture){
                if(f.uniqueFurnitureID == furniture.uniqueFurnitureID){
                    SQLFunctions sqf = playerInfo.gameObject.GetComponent<SQLFunctions>();

                    f.placed = true;
                    f.location = rf.transform.position;
                    f.rotation = rf.transform.eulerAngles;

                    Camera.main.transform.position = camOriginalPosition;
                    Camera.main.transform.eulerAngles = camOriginalRotation;

                    mlc.furnitureCancel.SetActive(false);
                    mlc.furnitureRotate.SetActive(false);
                    mlc.furnitureSet.SetActive(false);
                    
                    foreach(GameObject pet in pets){
                        pet.SetActive(true);
                    }

                    pets.Clear();
                    
                    foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                        if(obj.name.Equals("New Game Object")){
                            Destroy(obj);
                        }
                    }
                    
                    string furnitureLocation = "" + rf.transform.position.x + ":" + rf.transform.position.y + ":" + rf.transform.position.z;
                    string furnitureRotation = "" + rf.transform.eulerAngles.x + ":" + rf.transform.eulerAngles.y + ":" + rf.transform.eulerAngles.z;
                    Debug.Log("Location set as " + furnitureLocation);
                    sqf.SubmitFurniture(f.uniqueFurnitureID, f.furnitureID.ToString(), f.furnitureColorID.ToString(), furnitureLocation, furnitureRotation, f.roomID);
                    
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

    public void Rotate(GameObject rf, Room r, Furniture f){
    	Debug.Log("Clicked rotate button.");
    	Vector3 newRotations = rf.transform.localEulerAngles + new Vector3(0,90,0);
    	rf.transform.localEulerAngles = new Vector3(newRotations.x, newRotations.y % 360, newRotations.z);

    }

    public void MoveObject(Furniture f, Room r, GameObject rf){
        Bounds bounds = r.floorObject.GetComponent<MeshRenderer>().bounds;

        BoxCollider bc = rf.GetComponent<BoxCollider>();

        if(f.physObj.tag.Contains("floor")){
            if(r.roomSize == 4){
                Camera.main.transform.localPosition = new Vector3(bounds.center.x, 18, bounds.center.z);
                Camera.main.transform.eulerAngles = new Vector3(90,0,0);
            }
        }
        else if(f.physObj.tag.Contains("wall")){

        }
    }
}
