using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePlacement : MonoBehaviour
{
    public Furniture furniture;
    MainLandController mlc;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
        player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RezFurniture(){
        GameObject rezzedFurniture = GameObject.Instantiate(furniture.physObj);
        //rezzedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
        foreach(Room room in mlc.roomBoxes){
            if(room.active){
                rezzedFurniture.transform.position = room.gameObject.transform.position;
                break;
            }
        }
        //rezzedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
        Debug.Log("Furniture rotation is (" + furniture.location.x + ", " + furniture.location.y + ", " + furniture.location.z + ")");
        mlc.SeeScreen();
        foreach(Furniture f in player.furniture){
            if(f.placed){
                
            }
        }
    }

    public void PlaceFuniture(){

    }
}
