using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class MainLandController : MonoBehaviour
{
    Player player;
    Vector3 middlePosition;
    PlayerInformation playerinfo;
    SwipeManager sm;
    public bool press;
    bool traveling = false, travelingUp = false, travelingFront = false, travelingLeft = false, travelingRight = false;
    public GameObject petScreen, landScreen, currentPet, activeMark, relationshipsScreen, friendsListSeparator, friendsScreen, furnitureScreen, fcb, pcb, bcb, confirmBox, friendsPetInfo, petHolder, backSection;
    public Text petName, type, color, friendPetName, friendPetType, friendPetColor;
    public List<GameObject> friendButtons;
    public Button friendsRequest, pendingFriends, friends, acceptAll, declineAll;
    int i = 0, n = 0;
    public double pbuserID = 0;
    SQLFunctions sqf;
    Vector3 originalFriendScreenLocation = new Vector3(), originalFurnitureScreenLocation = new Vector3(), newFurnitureScreenLocation = new Vector3(), travelposition = new Vector3();
    public InputField friendsSearchBar;
    public List<Transform> furnitureButtonHolders;
    public GameObject furnitureButton, materialButton;
    RectTransform furnitureScreenPosition;
    HeightCheck heightCheck;
    public Vector3 frontLocation, topLocation, leftLocation, rightLocation;
    public List<Room> roomBoxes = new List<Room>();
    Room currentRoom = new Room();
    
    //string optionCast = "none";


    // Start is called before the first frame update
    void Start()
    {
        sqf = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<SQLFunctions>();
        playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        player = playerinfo.player;
        middlePosition = new Vector3(8.96f, 4.05f, 7.29f);
        if(player.loggedIn){
            currentPet = playerinfo.RezzPet(middlePosition, player.pets.IndexOf(player.activePet));
            Debug.Log("Player has " + player.rooms.Count + " ROOMS.");
            foreach(Room room in player.rooms){
                Debug.Log("Room number " + room.roomSize);
                GameObject rezzedRoom = GameObject.Instantiate(room.physObj);
                rezzedRoom.transform.localPosition = room.location;
                rezzedRoom.transform.localRotation = room.rotation;
                GameObject tempWall1 = new GameObject(), tempWall2 = new GameObject(), tempWall3 = new GameObject(), tempFloor = new GameObject();
                foreach(Transform child in rezzedRoom.transform){
                    if(child.gameObject.name == "GridFloor"){
                        child.gameObject.GetComponent<MeshRenderer>().material = room.floor.material;
                        tempFloor = child.gameObject;
                    }
                    else if(child.gameObject.name == "Wall1"){
                        child.gameObject.GetComponent<MeshRenderer>().material = room.wall1.material;
                        tempWall1 = child.gameObject;
                    }
                    else if(child.gameObject.name == "Wall2"){
                        child.gameObject.GetComponent<MeshRenderer>().material = room.wall2.material;
                        tempWall2 = child.gameObject;
                    }
                    else if(child.gameObject.name == "Wall3"){
                        child.gameObject.GetComponent<MeshRenderer>().material = room.wall3.material;
                        tempWall3 = child.gameObject;
                    }
                    else if(child.gameObject.name == "RoomInfo"){
                        Room r = child.gameObject.GetComponent<Room>();
                        if(r == null)
                            Debug.Log("R is null. No R there.");
                        else   Debug.Log("R is not null");
                        r.defaultRoom = room.defaultRoom;
                        r.floor = room.floor;
                        r.floorObject = room.floorObject;
                        r.location = room.location;
                        r.physObj = room.physObj;
                        r.roomSize = room.roomSize;
                        r.wall1 = room.wall1;
                        r.wall2 = room.wall2;
                        r.wall3 = room.wall3;
                        r.floorObject = tempFloor;
                        r.wall1Object = tempWall1;
                        r.wall2Object = tempWall2;
                        r.wall3Object = tempWall3;
                        if(r.defaultRoom){
                            r.active = true;
                        }
                        roomBoxes.Add(r);
                    }
                }

                
            }
        }
        sm = GameObject.FindGameObjectWithTag("swiper").GetComponent<SwipeManager>();
        originalFriendScreenLocation = friendsScreen.GetComponent<RectTransform>().localPosition;

        heightCheck = furnitureScreen.GetComponentInChildren<HeightCheck>();
        originalFurnitureScreenLocation = heightCheck.gameObject.GetComponent<RectTransform>().localPosition;
        newFurnitureScreenLocation = originalFriendScreenLocation - new Vector3(0,430,0);
        furnitureScreenPosition = heightCheck.gameObject.GetComponent<RectTransform>();
        BoxCollider2D bc2D = friendsScreen.GetComponentInChildren<BoxCollider2D>();
        sqf.RetrieveFurniture(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(press){
            SwipeCheck();
        }

        if(friendsSearchBar.isFocused){
            SearchUpdate(friendsSearchBar.text);
        }

        if(furnitureScreen.activeSelf && Input.GetMouseButton(0) && !traveling){
            TouchToLowerCheck();
        }

        if(traveling){
            int i = 0;
            //Going down
            if(travelposition.y < 0){
                i = -n;
            }
            //Going up
            if(travelposition.y >= 0)
                i = -(0-n);

            if(((furnitureScreenPosition.localPosition.y > travelposition.y) && heightCheck.down) || ((furnitureScreenPosition.localPosition.y < travelposition.y) && !heightCheck.down)){
                furnitureScreenPosition.localPosition += new Vector3(0,i,0);
                
               // Debug.Log("Position furniture = " + furnitureScreenPosition.localPosition.y + " travelposition: " + travelposition.y);
                n++;
            }
            else{//if(furnitureScreenPosition.localPosition.y <= travelposition.y){
                n = 0;
                traveling = false;
            }
        }

        //if()
    }

    public void CameraChange(string direction){
        if(direction == "default"){
            Camera.main.orthographic = false;
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
        else{
            Camera.main.orthographic = true;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
            

            if(direction == "up"){
                
            }
            else if(direction == "front"){

            }
            else if(direction == "left"){

            }
            else if(direction == "right"){

            }
        }
        
    }

    public void PutDownFurniture(){
        foreach(Furniture furniture in player.furniture){
            if(furniture.placed){
                GameObject placedFurniture = GameObject.Instantiate(furniture.physObj, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                placedFurniture.transform.localPosition = furniture.location;
                placedFurniture.transform.rotation = new Quaternion(0,0,0,0);
                placedFurniture.transform.localRotation = new Quaternion(0,0,0,0);
                placedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
            }
        }
    }

    void TouchToLowerCheck(){
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit)){
            GameObject tempObject;
            if(hit.collider != null){
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                tempObject = hit.collider.gameObject;
                if(tempObject.name == "BackSwipeSection" && !heightCheck.down){
                    //Debug.Log("Touched back swipe section.");
                    //StartCoroutine(travel(newFurnitureScreenLocation));
                    travelposition = newFurnitureScreenLocation;
                    heightCheck.down = true;
                    traveling = true;
                    backSection.SetActive(false);
                }
                else if(tempObject.tag == "downswipe" && heightCheck.down){
                    Debug.Log("Touched furniture section.");
                    //StartCoroutine(travel(originalFurnitureScreenLocation));
                    backSection.SetActive(true);
                    travelposition = originalFurnitureScreenLocation;
                    heightCheck.down = false;
                    traveling = true;
                }
            }

            
        }
    }

    public void SeeScreen(){
        travelposition = newFurnitureScreenLocation;
        heightCheck.down = true;
        traveling = true;
        backSection.SetActive(false);
    }

    void SwipeCheck(){
        if(!relationshipsScreen.activeSelf){
            if(sm.SwipeLeft)
                SwipeLeft();
            if(sm.SwipeRight)
                SwipeRight();
        }
    }

    void SwipeLeft(){
        Debug.Log("Swiped left.");
        if(petScreen.activeSelf){
            Destroy(currentPet);
            if(i + 1 >= player.pets.Count){
                i = 0;
            }
            else i++;
            currentPet = playerinfo.RezzPetPIP(new Vector3(0,0,0), i, GameObject.FindGameObjectWithTag("petholder").transform);
            CheckActivePet();
        }
    }
    
    void SwipeRight(){
        Debug.Log("Swiped right.");
        if(petScreen.activeSelf){
            Destroy(currentPet);
            if(i - 1 < 0){
                i = player.pets.Count - 1;
            }
            else i--;

            currentPet = playerinfo.RezzPetPIP(new Vector3(0,0,0), i, GameObject.FindGameObjectWithTag("petholder").transform);
            CheckActivePet();
        }
    }

    void CheckActivePet(){
        //Debug.Log("Checking for current pet. Active pet id is " + player.activePet.petID + " and current petID is " + player.pets[i].petID);

        if(player.activePet.petID == player.pets[i].petID){
            activeMark.SetActive(false);
        }
        else{
            activeMark.SetActive(true);
        }

        petName.text = player.pets[i].petName;
        color.text = player.pets[i].color.colorName;
        type.text = player.pets[i].animalName;
    }

    public void Press(){
        press = !press;
        if(press){
            Destroy(currentPet);
            foreach(Animal pet in player.pets){
                if(pet == player.activePet){
                    break;
                }
                else   i++;
            }
            currentPet = playerinfo.RezzPetPIP(new Vector3(0,0,0), player.pets.IndexOf(player.activePet), GameObject.FindGameObjectWithTag("petholder").transform);
            //GameObject.FindGameObjectWithTag("pet");
            CheckActivePet();
        }
        if(!press){
            i = 0;
            CheckActivePet();
            Destroy(currentPet);
            currentPet = playerinfo.RezzPet(middlePosition, player.pets.IndexOf(player.activePet));
        }
    }

    public void SummonPet(){
        player.activePet = player.pets[i];
        CheckActivePet();
    }

    public void CreateFriendsList(){
        fcb.SetActive(false);
        Debug.Log("Setting friendScreen disable.");
        player.friends.Clear();
        player.friendsPendingIn.Clear();
        player.friendsPendingOut.Clear();
        player.blockedIn.Clear();
        player.blockedOut.Clear();
        if(friendButtons.Count > 0){
            foreach(GameObject button in friendButtons){
                Destroy(button);
            }
            friendButtons.Clear();
        }

        //StartCoroutine(Wait(1f));
  
        Debug.Log("Creating friends list.");

        sqf.RetriveFriends();

    }

    public void SetFriendsList(){
        
        Vector3 lastLocation, firstLocation = new Vector3(256,-96,0);

        lastLocation = firstLocation;
        if(player.friendsPendingIn.Count > 0){
            
            acceptAll.gameObject.SetActive(true);
            declineAll.gameObject.SetActive(true);
            for(int l = 0; l < player.friendsPendingIn.Count; l++){
                if(player.friendsPendingIn[l].userID == pbuserID){
                    player.friendsPendingIn.RemoveAt(l);
                    break;
                }
            }

            foreach(Player friend in player.friendsPendingIn){
                Debug.Log("PENDING " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), pcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friendsPendingIn.IndexOf(friend);
                pend.tag = "fpin";
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.confirmBox = confirmBox;

                Debug.Log("ADDING FRIEND INFO STUFF.");
                pbi.friend = friend;
                pbi.playerName = friend.displayName;
                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();

                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }

            Debug.Log("RESETTING PENDING FRIEND'S LIST.");
            
        }

        if(player.friendsPendingOut.Count > 0){
            Debug.Log("This person has asked someone to be friends.");
            lastLocation = firstLocation;
            acceptAll.gameObject.SetActive(false);
            declineAll.gameObject.SetActive(false);
            foreach(Player friend in player.friendsPendingOut){
                Debug.Log("PENDING OUT " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friendsPendingOut.IndexOf(friend);
                pend.tag = "fpout";
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }
        }

        if(player.friends.Count > 0){
            lastLocation = firstLocation;
            foreach(Player friend in player.friends){

                Debug.Log("FRIEND " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friends.IndexOf(friend);
                pend.tag = "friends";
                
                
                Debug.Log("ADDING FRIEND INFO STUFF.");
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.friend = friend;
                pbi.playerName = friend.displayName;
                Debug.Log("Added username " + pbi.username.text);
                pend.GetComponent<Button>().onClick.AddListener(()=> pbi.ShowFriendsPetInfo());
                pbi.petName = friendPetName;
                pbi.petType = friendPetType;
                pbi.petColor = friendPetColor;
                pbi.friendsPetInfo = friendsPetInfo;

                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }

            fcb.SetActive(true);
        }

        if(player.blockedOut.Count > 0){
            lastLocation = firstLocation;
            for(int l = 0; l < player.blockedOut.Count; l++){
                if(player.blockedOut[l].userID == pbuserID){
                    player.blockedOut.RemoveAt(l);
                    break;
                }
            }
            foreach(Player friend in player.blockedOut){
                Debug.Log("BLOCKED " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.blockedOut.IndexOf(friend);
                pend.tag = "bout";

                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.confirmBox = confirmBox;

                

                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }

        }
        friendsScreen.SetActive(true);

        ResetUI(0.5f);
        pbuserID = 0;
    }

    public void CreateFurnitureList(){
        player.furniture.Clear();
        sqf.RetrieveFurniture();
    }

    public void SetFurnitureList(){
        furnitureScreen.SetActive(true);

        foreach(Furniture furniture in player.furniture){
            Transform holder;
            Debug.Log("Furniture is " + furniture.furnitureName + " and type is " + furniture.furnitureType);
            
            if(!furniture.placed){
                if(furniture.furnitureType == 1){
                    //Walls
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("walls")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(materialButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.material, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("MaterialHolder"));
                            break;
                        }
                    }

                    

                }
                else if(furniture.furnitureType == 2){
                    //Floors
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("floor")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(materialButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.material, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("MaterialHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 3){
                    //Tables
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("table")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 4){
                    //Chairs
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("chair")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 5){
                    //Beds
                    Debug.Log("THIS IS A BED.");
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        Debug.Log("This holder holds " + furnitureHolder.parent.gameObject.name);
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("bed")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, -60);
                            button.gameObject.GetComponent<FurniturePlacement>().furniture = furniture;
                            GameObject icon = GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            icon.transform.localPosition = new Vector3(0,0,0);
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 6){
                    //Storage
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("storage")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 7){
                    //Bath
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("bath")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 8){
                    //Kitchen
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("kitchen")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 9){
                    //Electronics
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("electronics")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 10){
                    //Lighting
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("light")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
                else if(furniture.furnitureType == 11){
                    //Wall Decorations
                    foreach(Transform furnitureHolder in furnitureButtonHolders){
                        if(furnitureHolder.parent.gameObject.name.ToLower().Contains("wall deco")){
                            //Use a material button
                            holder = furnitureHolder;
                            GameObject button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                            GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                            break;
                        }
                    }
                }
            }
        }
    }

    public void ResetUI(float seconds = 1f){
        pcb.SetActive(false);
        bcb.SetActive(false);
        StartCoroutine(WaitForReset(seconds));
        
        //UPDATE `friends` SET `pending_friend_to_user`='1',`friends`='0'WHERE `user` = '2' AND `friend` = '3' OR `friend` = '2' AND `user` = '3'
    }

    public void SendBackward(){
        friendsScreen.transform.localPosition = originalFriendScreenLocation + new Vector3(0,0,400);
    }

    public void SendForward(){
        friendsScreen.transform.localPosition = originalFriendScreenLocation;
        foreach(Transform child in petHolder.transform){
            Destroy(child.gameObject);
        }
    }

    public void SearchUpdate(string searchText){

        Debug.Log("Updating search. Search text: " + searchText.ToLower());
        Vector3 lastLocation, firstLocation = new Vector3(256,-96,0);
        lastLocation = firstLocation;
        if(!(string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))){
            foreach(GameObject pend in friendButtons){
                if(pend.tag == "friends" && pend.GetComponent<PlayerButtonInfo>().playerName.ToLower().StartsWith(searchText.ToLower())){
                    pend.SetActive(true);
                    pend.GetComponent<RectTransform>().localPosition = lastLocation;
                    lastLocation = lastLocation + new Vector3(0,-152,0);
                }
                else{
                    pend.SetActive(false);
                }
            }
        }
        else{
            foreach(GameObject pend in friendButtons){
                if(pend.tag == "friends"){
                    pend.GetComponent<RectTransform>().localPosition = lastLocation;
                    lastLocation = lastLocation + new Vector3(0,-152,0);
                    pend.SetActive(true);
                }
            }
        }
    }

    IEnumerator WaitForReset(float seconds){

        yield return new WaitForSeconds(seconds);
        Canvas.ForceUpdateCanvases();
        pcb.SetActive(true);
        pcb.SetActive(true);

        Debug.Log("Finished RESET.");
        
    }

    public IEnumerator Wait(float seconds){
        yield return new WaitForSeconds(seconds);
    }


}
