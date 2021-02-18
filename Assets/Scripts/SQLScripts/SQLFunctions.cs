using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


public class SQLFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    string url = "http://localhost/CPG/PHPs/";
    double lastID;
    
    Player player;
    PlayerInformation playerinfo;
    bool friendsRetrieved;
    SurfaceMaterials sf;
    List<RoomSize> roomSizes;
    
    void Start()
    {
        playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        player = playerinfo.player;
        sf = playerinfo.surfaceMaterials;
        roomSizes = playerinfo.roomObjects;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateUser(){
        StartCoroutine(CheckUser());
    }

    /*public bool CreateUserPlayfab(string playfabID){
        
    }

    bool SetUserData(string playfabID) {
        bool success = false;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                {"", "Arthur"},
                {"Successor", "Fred"}
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }*/

    public void SubmitNewUserInfo(){
        StartCoroutine(SubmitNewUserInfoEnum());
    }

    public void RetrieveUser(){
        StartCoroutine(RetrieveUserEnum());
    }

    public void RetrievePet(){
        StartCoroutine(RetrievePetEnum());
    }

    public void RetriveFriends(){
        StartCoroutine(RetrieveFriendsEnum());
    }

    public void RetriveFriendsUserInfo(){
        StartCoroutine(RetrieveFriendsUserInfoEnum());
    }

    public void UpdateFriends(double friendID, int pendingUTF, int pendingFTU, int friends, int blockUTF, int blockFTU){
        StartCoroutine(UpdateFriendsEnum(friendID, pendingUTF, pendingFTU, friends, blockUTF, blockFTU));
    }

    public void RetrieveFurniture(int choice = 0){
        if(choice > 0){
            player.furniture.Clear();
            StartCoroutine(RetrieveFurnitureNoListEnum());
        }
        else{
            StartCoroutine(RetrieveFurnitureEnum());
        }
    }

    public void RetriveRooms(){
        StartCoroutine(RetrieveRoomsEnum());
    }

    public void SubmitFurniture(double uniqueFurnitureID, string furnitureID, string furnitureColorID, string location, string rotation, int roomID){
        StartCoroutine(SubmitFurnitureEnum(uniqueFurnitureID, furnitureID, furnitureColorID, location, rotation, roomID));
    }

    IEnumerator SubmitFurnitureEnum(double uniqueFurnitureID, string furnitureID, string furnitureColorID, string location, string rotation, int roomID){
        WWWForm form = new WWWForm();
        UnityWebRequest wwwSubmitFurniture;
        Debug.Log("Passing location as: " + location);
        form.AddField("userid", player.userID.ToString());
        form.AddField("uniquefurnitureid", uniqueFurnitureID.ToString());
        form.AddField("furnitureid", furnitureID);
        form.AddField("furniturecolorid", furnitureColorID);
        form.AddField("location", location);
        form.AddField("rotation", rotation);
        form.AddField("roomid", roomID);

        using(wwwSubmitFurniture = UnityWebRequest.Post(url + "SubmitFurniture.php", form)){
            yield return wwwSubmitFurniture.SendWebRequest();
            if(wwwSubmitFurniture.isNetworkError || wwwSubmitFurniture.isHttpError){
                Debug.Log(wwwSubmitFurniture.error);
                playerinfo.messages.ErrorMessage.SetActive(true);
                playerinfo.messages.errorTitle.text = "Network Error";
                playerinfo.messages.errorText.text = "Cannot save furniture location.";
            }
                
            else{
               Debug.Log(wwwSubmitFurniture.downloadHandler.text);
            }
        }
    }

    IEnumerator RetrieveRoomsEnum(){
        UnityWebRequest wwwRetrieveRooms;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrieveRooms = UnityWebRequest.Post(url + "RetrieveRooms.php", form)){
            yield return wwwRetrieveRooms.SendWebRequest();
            if(wwwRetrieveRooms.isNetworkError){
                Debug.Log(wwwRetrieveRooms.error);
                ShowError(wwwRetrieveRooms.error);
                }
            else{
                Debug.Log(wwwRetrieveRooms.downloadHandler.text);

                String[] lines = wwwRetrieveRooms.downloadHandler.text.Split(';');

                foreach(string line in lines){
                    if(!(String.IsNullOrEmpty(line) || String.IsNullOrWhiteSpace(line))){
                        Room room = new Room();
                    
                        Debug.Log("ROOMSIZE: " + int.Parse(line.Split(',')[0]));

                        room.roomSize = int.Parse(line.Split(',')[0]);
                        foreach(RoomSize rs in roomSizes){
                            if(rs.roomSize == room.roomSize){
                                room.floorObject = rs.floorObject;
                                room.wall1Object = rs.wall1Object;
                                room.wall2Object = rs.wall2Object;
                                room.wall3Object = rs.wall3Object;
                                room.physObj = rs.physObj;
                                break;
                            }
                        }


                        room.defaultRoom = bool.Parse(line.Split(',')[1]);
                        foreach(Mat mat in sf.materials){
                            if(mat.num == int.Parse(line.Split(',')[2])){
                                room.floor = mat;
                            }
                            if(mat.num == int.Parse(line.Split(',')[3])){
                                room.wall1 = mat;
                            }
                            if(mat.num == int.Parse(line.Split(',')[4])){
                                room.wall2 = mat;
                            }
                            if(mat.num == int.Parse(line.Split(',')[5])){
                                room.wall3 = mat;
                            }
                        }
                        String loc = line.Split(',')[6];
                        String rot = line.Split(',')[7];

                        room.rotation = Quaternion.Euler(float.Parse(rot.Split(':')[0]), float.Parse(rot.Split(':')[1]), float.Parse(rot.Split(':')[2]));

                        Debug.Log("(" + loc.Split(':')[0] + ", " + loc.Split(':')[1] + ", " + loc.Split(':')[2] + ")");
                        
                        float xloc = float.Parse(loc.Split(':')[0]);
                        Debug.Log("XLOC: " + xloc);
                        
                        float yloc = float.Parse(loc.Split(':')[1]);
                        Debug.Log("YLOC: " + yloc);
                        
                        float zloc = float.Parse(loc.Split(':')[2]);
                        Debug.Log("ZLOC: " + zloc);
                        
                        room.location = new Vector3(xloc, yloc, zloc);
                        room.roomID = int.Parse(line.Split(',')[8]);
                        player.rooms.Add(room);
                    }

                }
                

                wwwRetrieveRooms.Dispose();
                StopCoroutine(RetrieveUserEnum());
        
                
            }
        }
    }

    IEnumerator RetrieveFurnitureNoListEnum(){
        PlayerInformation playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        FurnitureList furnitureList = playerinfo.furnitureList;
        UnityWebRequest wwwRetrieveFurniture;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrieveFurniture = UnityWebRequest.Post(url + "RetrieveFurniture.php", form)){
            yield return wwwRetrieveFurniture.SendWebRequest();
            if(wwwRetrieveFurniture.isNetworkError){
                Debug.Log(wwwRetrieveFurniture.error);
                ShowError(wwwRetrieveFurniture.error);
            }
            else{
                Debug.Log(wwwRetrieveFurniture.downloadHandler.text);
                string[] lines = wwwRetrieveFurniture.downloadHandler.text.Split(';');
                foreach(string line in lines){
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        Furniture furniture = new Furniture();
                        furniture.furnitureID = int.Parse(line.Split(',')[0]);
                        furniture.uniqueFurnitureID = double.Parse(line.Split(',')[4]);
                        furniture.furnitureColorID = int.Parse(line.Split(',')[1]);
                        foreach(Furniture f in furnitureList.furniture){
                            if(f.furnitureID == furniture.furnitureID){
                                if(f.furnitureColorID == furniture.furnitureColorID){
                                    furniture.colorName = f.colorName;
                                    furniture.furnitureName = f.furnitureName;
                                    furniture.physObj = f.physObj;
                                    furniture.furnitureType = f.furnitureType;
                                    furniture.icon = f.icon;
                                    if(f.material)
                                        furniture.material = f.material;
                                    break;
                                }

                            }
                        }
                        string furnitureLocation = line.Split(',')[2];
                        if(!furnitureLocation.Contains("n")){
                            Vector3 loc = new Vector3(float.Parse(furnitureLocation.Split(':')[0]),float.Parse(furnitureLocation.Split(':')[1]),float.Parse(furnitureLocation.Split(':')[2]));
                            furniture.location = loc;
                            furniture.placed = true;
                            string furnitureRotation = line.Split(',')[3];
                            furniture.rotation = new Vector3(float.Parse(furnitureRotation.Split(':')[0]), float.Parse(furnitureRotation.Split(':')[1]), float.Parse(furnitureRotation.Split(':')[2]));
                        }
                        if(furnitureLocation.Contains("n")){
                            furniture.placed = false;
                            furniture.location = new Vector3();
                            furniture.rotation = new Vector3();
                        }

                        

                        player.furniture.Add(furniture);
                    }
                }
                
                GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().PutDownFurniture();
                wwwRetrieveFurniture.Dispose();
                StopCoroutine(RetrieveUserEnum());
            }
        }
    }

    IEnumerator RetrieveFurnitureEnum(){
        PlayerInformation playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        FurnitureList furnitureList = playerinfo.furnitureList;
        UnityWebRequest wwwRetrieveFurniture;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrieveFurniture = UnityWebRequest.Post(url + "RetrieveFurniture.php", form)){
            yield return wwwRetrieveFurniture.SendWebRequest();
            if(wwwRetrieveFurniture.isNetworkError){
                Debug.Log(wwwRetrieveFurniture.error);
                ShowError(wwwRetrieveFurniture.error);
            }
            else{
                Debug.Log(wwwRetrieveFurniture.downloadHandler.text);
                string[] lines = wwwRetrieveFurniture.downloadHandler.text.Split(';');
                foreach(string line in lines){
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        Furniture furniture = new Furniture();
                        furniture.uniqueFurnitureID = double.Parse(line.Split(',')[4]);
                        furniture.furnitureID = int.Parse(line.Split(',')[0]);
                        furniture.furnitureColorID = int.Parse(line.Split(',')[1]);
                        foreach(Furniture f in furnitureList.furniture){
                            if(f.furnitureID == furniture.furnitureID){
                                if(f.furnitureColorID == furniture.furnitureColorID){
                                    furniture.colorName = f.colorName;
                                    furniture.furnitureName = f.furnitureName;
                                    furniture.physObj = f.physObj;
                                    furniture.furnitureType = f.furnitureType;
                                    furniture.icon = f.icon;
                                    if(f.material)
                                        furniture.material = f.material;
                                    break;
                                }

                            }
                        }
                        string furnitureLocation = line.Split(',')[2];
                        if(!furnitureLocation.Contains("n")){
                            Vector3 loc = new Vector3(float.Parse(furnitureLocation.Split(':')[0]),float.Parse(furnitureLocation.Split(':')[1]),float.Parse(furnitureLocation.Split(':')[2]));
                            furniture.location = loc;
                            furniture.placed = true;
                            string furnitureRotation = line.Split(',')[3];
                            furniture.rotation = new Vector3(float.Parse(furnitureRotation.Split(':')[0]), float.Parse(furnitureRotation.Split(':')[1]), float.Parse(furnitureRotation.Split(':')[2]));
                        }
                        if(furnitureLocation.Contains("n")){
                            furniture.placed = false;
                            furniture.location = new Vector3();
                        }
                        player.furniture.Add(furniture);
                         }
                }
                

                wwwRetrieveFurniture.Dispose();
                GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().SetFurnitureList();
                StopCoroutine(RetrieveUserEnum());
            }
        }
    }

    IEnumerator UpdateFriendsEnum(double friendID, int pendingUTF, int pendingFTU, int friends, int blockUTF, int blockFTU){
        UnityWebRequest wwwUpdateFriends;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());
        form.AddField("friendid", friendID.ToString());
        form.AddField("pendingutf", pendingUTF);
        form.AddField("pendingftu", pendingFTU);
        form.AddField("friends", friends);
        form.AddField("blockutf", blockUTF);
        form.AddField("blockftu", blockFTU);

        using(wwwUpdateFriends = UnityWebRequest.Post(url + "UpdateFriends.php", form)){
            yield return wwwUpdateFriends.SendWebRequest();
            if(wwwUpdateFriends.isNetworkError){
                Debug.Log(wwwUpdateFriends.error);
                ShowError(wwwUpdateFriends.error);
            }
            else{
               // Debug.Log(wwwUpdateFriends.downloadHandler.text);

                String line = wwwUpdateFriends.downloadHandler.text.Split(';')[0];

                

                wwwUpdateFriends.Dispose();
                StopCoroutine(RetrieveUserEnum());
        
                
            }
        }
    }
    
    IEnumerator RetrieveFriendsUserInfoEnum(){
        UnityWebRequest wwwRetrieveFriendsUserInfo;
        WWWForm form = new WWWForm();
        foreach(Player friend in player.friends){
           // Debug.Log("User ID is: " + friend.userID);
            form.AddField("userid", friend.userID.ToString());

            using(wwwRetrieveFriendsUserInfo = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
                yield return wwwRetrieveFriendsUserInfo.SendWebRequest();
                if(wwwRetrieveFriendsUserInfo.isNetworkError){
                    Debug.Log(wwwRetrieveFriendsUserInfo.error);
                    ShowError(wwwRetrieveFriendsUserInfo.error);
                }
                else{
                  //  Debug.Log(wwwRetrieveFriendsUserInfo.downloadHandler.text);

                    String line = wwwRetrieveFriendsUserInfo.downloadHandler.text.Split(';')[0];
                    friend.displayName = line.Split(',')[0];
                    

                    wwwRetrieveFriendsUserInfo.Dispose();


                    UnityWebRequest wwwRetrievePet;
                    using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
                        yield return wwwRetrievePet.SendWebRequest();
                        if(wwwRetrievePet.isNetworkError){
                            Debug.Log(wwwRetrievePet.error);
                            ShowError(wwwRetrievePet.error);
                        }
                        else{
                   //         Debug.Log(wwwRetrievePet.downloadHandler.text);

                            String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                            foreach(String line2 in lines){
                                if(!string.IsNullOrEmpty(line2) || !string.IsNullOrWhiteSpace(line2)){
                                    Animal pet = new Animal();
                                    pet.petName = line2.Split(',')[1];
                                    pet.petID = int.Parse(line2.Split(',')[2]);
                                    pet.animalType = int.Parse(line2.Split(',')[3]);
                                    
                                    //Get name of animal and other animal information!
                                    foreach(Animal animal in playerinfo.animalList.animals){
                                        if(animal.animalType == pet.animalType){
                                            pet.animalName = animal.animalName;
                                            pet.physObj = animal.physObj;

                                            //TEMPORARY ACCESSORY
                                            pet.backAccessory = animal.backAccessory;
                                            pet.clothing = animal.clothing;
                                            pet.faceAccessory = animal.faceAccessory;
                                            pet.headAccessory = animal.headAccessory;
                                            pet.leftHand = animal.leftHand;
                                            pet.neckAccessory = animal.neckAccessory;
                                            pet.rightHand = animal.rightHand;
                                        }
                                    }

                                    pet.color = new AnimalColor();
                                    pet.color.color = int.Parse(line2.Split(',')[4]);
                                    
                                    //Get pet color names
                                    foreach(AnimalColor colors in playerinfo.animalList.colors){
                                        if((colors.color == pet.color.color) && (colors.animalID == pet.animalType)){
                                            pet.physObj.GetComponent<PetInformation>().body.material = colors.animalMaterial;
                                            pet.color.colorName = colors.colorName;
                                            pet.color.animalID = colors.animalID;
                                            pet.color.animalMaterial = colors.animalMaterial;
                                            break;
                                        }
                                    }

                                    pet.glow = int.Parse(line2.Split(',')[5]);
                                    pet.special = int.Parse(line2.Split(',')[6]);
                                    
                                    pet.face = new AnimalFaces();
                                    pet.face.faceID = int.Parse(line2.Split(',')[7]);
                                    
                                    foreach(AnimalFaces faces in playerinfo.animalList.faces){
                                        if(faces.faceID == pet.face.faceID){
                                            pet.physObj.GetComponent<PetInformation>().face.material = faces.face;
                                            pet.face.eligibleAnimals = faces.eligibleAnimals;
                                            pet.face.face = faces.face;
                                            break;
                                        }
                                    }
                                    //Debug.Log("ADDING PET " + pet.petName);
                                    friend.pets.Add(pet);
                                    if(int.Parse(line2.Split(',')[15]) > 0){
                                        friend.activePet = friend.pets[friend.pets.IndexOf(pet)];
                                    }

                                    
                                }
                            }
                            

                            wwwRetrievePet.Dispose();                           
                        }
                    }

                    
            
                    
                }
            }
        }
        foreach(Player friend in player.friendsPendingIn){
           // Debug.Log("User ID is: " + friend.userID);
            form.AddField("userid", friend.userID.ToString());

            using(wwwRetrieveFriendsUserInfo = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
                yield return wwwRetrieveFriendsUserInfo.SendWebRequest();
                if(wwwRetrieveFriendsUserInfo.isNetworkError){
                    Debug.Log(wwwRetrieveFriendsUserInfo.error);
                    ShowError(wwwRetrieveFriendsUserInfo.error);
                }
                else{
                  //  Debug.Log(wwwRetrieveFriendsUserInfo.downloadHandler.text);

                    String line = wwwRetrieveFriendsUserInfo.downloadHandler.text.Split(';')[0];
                    friend.displayName = line.Split(',')[0];
                    

                    wwwRetrieveFriendsUserInfo.Dispose();


                    UnityWebRequest wwwRetrievePet;
                    using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
                        yield return wwwRetrievePet.SendWebRequest();
                        if(wwwRetrievePet.isNetworkError){
                            Debug.Log(wwwRetrievePet.error);
                            ShowError(wwwRetrievePet.error);
                        }
                        else{
                           // Debug.Log(wwwRetrievePet.downloadHandler.text);

                            String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                            foreach(String line2 in lines){
                                if(!string.IsNullOrEmpty(line2) || !string.IsNullOrWhiteSpace(line2)){
                                    Animal pet = new Animal();
                                    pet.petName = line2.Split(',')[1];
                                    pet.petID = int.Parse(line2.Split(',')[2]);
                                    pet.animalType = int.Parse(line2.Split(',')[3]);
                                    
                                    //Get name of animal and other animal information!
                                    foreach(Animal animal in playerinfo.animalList.animals){
                                        if(animal.animalType == pet.animalType){
                                            pet.animalName = animal.animalName;
                                            pet.physObj = animal.physObj;

                                            //TEMPORARY ACCESSORY
                                            pet.backAccessory = animal.backAccessory;
                                            pet.clothing = animal.clothing;
                                            pet.faceAccessory = animal.faceAccessory;
                                            pet.headAccessory = animal.headAccessory;
                                            pet.leftHand = animal.leftHand;
                                            pet.neckAccessory = animal.neckAccessory;
                                            pet.rightHand = animal.rightHand;
                                        }
                                    }

                                    pet.color = new AnimalColor();
                                    pet.color.color = int.Parse(line2.Split(',')[4]);
                                    
                                    //Get pet color names
                                    foreach(AnimalColor colors in playerinfo.animalList.colors){
                                        if((colors.color == pet.color.color) && (colors.animalID == pet.animalType)){
                                            pet.physObj.GetComponent<PetInformation>().body.material = colors.animalMaterial;
                                            pet.color.colorName = colors.colorName;
                                            pet.color.animalID = colors.animalID;
                                            pet.color.animalMaterial = colors.animalMaterial;
                                            break;
                                        }
                                    }

                                    pet.glow = int.Parse(line2.Split(',')[5]);
                                    pet.special = int.Parse(line2.Split(',')[6]);
                                    
                                    pet.face = new AnimalFaces();
                                    pet.face.faceID = int.Parse(line2.Split(',')[7]);
                                    
                                    foreach(AnimalFaces faces in playerinfo.animalList.faces){
                                        if(faces.faceID == pet.face.faceID){
                                            pet.physObj.GetComponent<PetInformation>().face.material = faces.face;
                                            pet.face.eligibleAnimals = faces.eligibleAnimals;
                                            pet.face.face = faces.face;
                                            break;
                                        }
                                    }

                                    //Debug.Log("ADDING PET " + pet.petName);
                                    friend.pets.Add(pet);
                                    if(int.Parse(line2.Split(',')[15]) > 0){
                                        
                                        friend.activePet = friend.pets[friend.pets.IndexOf(pet)];
                                    }

                                    
                                }
                            }
                            

                            wwwRetrievePet.Dispose();                           
                        }
                    }

                    
            
                    
                }
            }
        }
        foreach(Player friend in player.friendsPendingOut){
            Debug.Log("User ID is: " + friend.userID);
            form.AddField("userid", friend.userID.ToString());

            using(wwwRetrieveFriendsUserInfo = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
                yield return wwwRetrieveFriendsUserInfo.SendWebRequest();
                if(wwwRetrieveFriendsUserInfo.isNetworkError){
                    Debug.Log(wwwRetrieveFriendsUserInfo.error);
                    ShowError(wwwRetrieveFriendsUserInfo.error);
                }
                else{
                   // Debug.Log(wwwRetrieveFriendsUserInfo.downloadHandler.text);

                    String line = wwwRetrieveFriendsUserInfo.downloadHandler.text.Split(';')[0];
                    friend.displayName = line.Split(',')[0];
                    

                    wwwRetrieveFriendsUserInfo.Dispose();


                    UnityWebRequest wwwRetrievePet;
                    using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
                        yield return wwwRetrievePet.SendWebRequest();
                        if(wwwRetrievePet.isNetworkError){
                            Debug.Log(wwwRetrievePet.error);
                            ShowError(wwwRetrievePet.error);
                        }
                        else{
                          //  Debug.Log(wwwRetrievePet.downloadHandler.text);

                            String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                            foreach(String line2 in lines){
                                if(!string.IsNullOrEmpty(line2) || !string.IsNullOrWhiteSpace(line2)){
                                    Animal pet = new Animal();
                                    pet.petName = line2.Split(',')[1];
                                    pet.petID = int.Parse(line2.Split(',')[2]);
                                    pet.animalType = int.Parse(line2.Split(',')[3]);
                                    
                                    //Get name of animal and other animal information!
                                    foreach(Animal animal in playerinfo.animalList.animals){
                                        if(animal.animalType == pet.animalType){
                                            pet.animalName = animal.animalName;
                                            pet.physObj = animal.physObj;

                                            //TEMPORARY ACCESSORY
                                            pet.backAccessory = animal.backAccessory;
                                            pet.clothing = animal.clothing;
                                            pet.faceAccessory = animal.faceAccessory;
                                            pet.headAccessory = animal.headAccessory;
                                            pet.leftHand = animal.leftHand;
                                            pet.neckAccessory = animal.neckAccessory;
                                            pet.rightHand = animal.rightHand;
                                        }
                                    }

                                    pet.color = new AnimalColor();
                                    pet.color.color = int.Parse(line2.Split(',')[4]);
                                    
                                    //Get pet color names
                                    foreach(AnimalColor colors in playerinfo.animalList.colors){
                                        if((colors.color == pet.color.color) && (colors.animalID == pet.animalType)){
                                            pet.physObj.GetComponent<PetInformation>().body.material = colors.animalMaterial;
                                            pet.color.colorName = colors.colorName;
                                            pet.color.animalID = colors.animalID;
                                            pet.color.animalMaterial = colors.animalMaterial;
                                            break;
                                        }
                                    }

                                    pet.glow = int.Parse(line2.Split(',')[5]);
                                    pet.special = int.Parse(line2.Split(',')[6]);
                                    
                                    pet.face = new AnimalFaces();
                                    pet.face.faceID = int.Parse(line2.Split(',')[7]);
                                    
                                    foreach(AnimalFaces faces in playerinfo.animalList.faces){
                                        if(faces.faceID == pet.face.faceID){
                                            pet.physObj.GetComponent<PetInformation>().face.material = faces.face;
                                            pet.face.eligibleAnimals = faces.eligibleAnimals;
                                            pet.face.face = faces.face;
                                            break;
                                        }
                                    }
                                    //Debug.Log("ADDING PET " + pet.petName);
                                    friend.pets.Add(pet);
                                    if(int.Parse(line2.Split(',')[15]) > 0){
                                        friend.activePet = friend.pets[friend.pets.IndexOf(pet)];
                                    }

                                    
                                }
                            }
                            

                            wwwRetrievePet.Dispose();                           
                        }
                    }

                    
            
                    
                }
            }
        }
        foreach(Player friend in player.blockedOut){
            Debug.Log("User ID is: " + friend.userID);
            form.AddField("userid", friend.userID.ToString());

            using(wwwRetrieveFriendsUserInfo = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
                yield return wwwRetrieveFriendsUserInfo.SendWebRequest();
                if(wwwRetrieveFriendsUserInfo.isNetworkError){
                    Debug.Log(wwwRetrieveFriendsUserInfo.error);
                    ShowError(wwwRetrieveFriendsUserInfo.error);
                }
                else{
                   // Debug.Log(wwwRetrieveFriendsUserInfo.downloadHandler.text);

                    String line = wwwRetrieveFriendsUserInfo.downloadHandler.text.Split(';')[0];
                    friend.displayName = line.Split(',')[0];
                    

                    wwwRetrieveFriendsUserInfo.Dispose();

                }
            }
        }
        
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "MainScene"){
            if(player.friends.Count > 0 || player.friendsPendingIn.Count > 0 || player.friendsPendingOut.Count > 0 || player.blockedOut.Count > 0){
                GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().SetFriendsList();
            }
        }

        StopCoroutine(RetrieveFriendsUserInfoEnum());

    }

    IEnumerator RetrieveFriendsEnum(){
        UnityWebRequest wwwRetrieveFriends;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrieveFriends = UnityWebRequest.Post(url + "RetrieveFriends.php", form)){
            yield return wwwRetrieveFriends.SendWebRequest();
            if(wwwRetrieveFriends.isNetworkError){
                Debug.Log(wwwRetrieveFriends.error);
                ShowError(wwwRetrieveFriends.error);
            }
            else{
                //Debug.Log(wwwRetrieveFriends.downloadHandler.text);

                foreach(String line in wwwRetrieveFriends.downloadHandler.text.Split(';')){
                    //If you are the user!
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        if(double.Parse(line.Split(',')[0]) == player.userID){
                            Player friend = new Player();
                            double friendNum = double.Parse(line.Split(',')[1]);
                            //Debug.Log("FriendNum is: " + friendNum);
                            friend.userID = friendNum;
                            if(int.Parse(line.Split(',')[2]) == 1){
                                player.friendsPendingOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[3]) == 1){
                                player.friendsPendingIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[4]) == 1){
                                player.friends.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[5]) == 1){
                                player.blockedOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[6]) == 1){
                                player.blockedIn.Add(friend);
                            }

                           // Debug.Log("Friend user ID is: " + friend.userID);

                        }
                        else if(double.Parse(line.Split(',')[1]) == player.userID){
                            Player friend = new Player();
                            double friendNum = double.Parse(line.Split(',')[0]);
                            friend.userID = friendNum;
                           // Debug.Log("Friendnum is " + friendNum);
                            if(int.Parse(line.Split(',')[2]) == 1){
                                player.friendsPendingIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[3]) == 1){
                                player.friendsPendingOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[4]) == 1){
                                player.friends.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[5]) == 1){
                                player.blockedIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[6]) == 1){
                                player.blockedOut.Add(friend);
                            }

                           // Debug.Log("Friend user ID is: " + friend.userID);
                        }
                    }
                    //If you are the friend!
                }
                 wwwRetrieveFriends.Dispose();
                
                StartCoroutine(RetrieveFriendsUserInfoEnum());
                StopCoroutine(RetrieveFriendsEnum());
        
                
            }
        }
    }

    IEnumerator RetrievePetEnum(){
        UnityWebRequest wwwRetrievePet;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
            yield return wwwRetrievePet.SendWebRequest();
            if(wwwRetrievePet.isNetworkError){
                Debug.Log(wwwRetrievePet.error);
                ShowError(wwwRetrievePet.error);
            }
            else{
               // Debug.Log(wwwRetrievePet.downloadHandler.text);

                String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                
                foreach(String line in lines){
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        Animal pet = new Animal();
                        pet.petName = line.Split(',')[1];
                        pet.petID = int.Parse(line.Split(',')[2]);
                        pet.animalType = int.Parse(line.Split(',')[3]);
                        
                        //Get name of animal and other animal information!
                        foreach(Animal animal in playerinfo.animalList.animals){
                            if(animal.animalType == pet.animalType){
                                pet.animalName = animal.animalName;
                                pet.physObj = animal.physObj;

                                //TEMPORARY ACCESSORY
                                pet.backAccessory = animal.backAccessory;
                                pet.clothing = animal.clothing;
                                pet.faceAccessory = animal.faceAccessory;
                                pet.headAccessory = animal.headAccessory;
                                pet.leftHand = animal.leftHand;
                                pet.neckAccessory = animal.neckAccessory;
                                pet.rightHand = animal.rightHand;
                            }
                        }

                        pet.color = new AnimalColor();
                        pet.color.color = int.Parse(line.Split(',')[4]);
                        
                        //Get pet color names
                        foreach(AnimalColor colors in playerinfo.animalList.colors){
                            if((colors.color == pet.color.color) && (colors.animalID == pet.animalType)){
                                pet.physObj.GetComponent<PetInformation>().body.material = colors.animalMaterial;
                                pet.color.colorName = colors.colorName;
                                pet.color.animalID = colors.animalID;
                                pet.color.animalMaterial = colors.animalMaterial;
                                break;
                            }
                        }

                        pet.glow = int.Parse(line.Split(',')[5]);
                        pet.special = int.Parse(line.Split(',')[6]);
                        
                        pet.face = new AnimalFaces();
                        pet.face.faceID = int.Parse(line.Split(',')[7]);
                        
                        foreach(AnimalFaces faces in playerinfo.animalList.faces){
                            if(faces.faceID == pet.face.faceID){
                                pet.physObj.GetComponent<PetInformation>().face.material = faces.face;
                                pet.face.eligibleAnimals = faces.eligibleAnimals;
                                pet.face.face = faces.face;
                                break;
                            }
                        }
                        //Debug.Log("ADDING PET " + pet.petName);
                        player.pets.Add(pet);
                        if(int.Parse(line.Split(',')[15]) > 0){
                            player.activePet = player.pets[player.pets.IndexOf(pet)];
                        }

                        
                    }
                }
                

                wwwRetrievePet.Dispose();
                StopCoroutine(RetrievePetEnum());
        
                
            }
        }
    }

    IEnumerator RetrieveUserEnum(){
        UnityWebRequest wwwRetrieveUser;
        WWWForm form = new WWWForm();
        form.AddField("userid", player.userID.ToString());

        using(wwwRetrieveUser = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
            yield return wwwRetrieveUser.SendWebRequest();
            if(wwwRetrieveUser.isNetworkError){
                Debug.Log(wwwRetrieveUser.error);
                ShowError(wwwRetrieveUser.error);
            }
            else{
                Debug.Log(wwwRetrieveUser.downloadHandler.text);

                String line = wwwRetrieveUser.downloadHandler.text.Split(';')[0];
                player.displayName = line.Split(',')[0];
                

                wwwRetrieveUser.Dispose();
                StopCoroutine(RetrieveUserEnum());
        
                
            }
        }
    }

    IEnumerator CheckUser(){
        UnityWebRequest wwwUserCheck;
        WWWForm form = new WWWForm();

        using(wwwUserCheck = UnityWebRequest.Post(url + "CheckUser.php", form)){
            yield return wwwUserCheck.SendWebRequest();
            if(wwwUserCheck.isNetworkError){
                Debug.Log(wwwUserCheck.error);
                ShowError(wwwUserCheck.error);
            }
            else{
                //Debug.Log(wwwUserCheck.downloadHandler.text);

                player.userID = double.Parse(wwwUserCheck.downloadHandler.text) + 1;
                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
                    Data = new Dictionary<string, string>() {
                        {"userid", player.userID.ToString()},
                        {"loggedin", bool.FalseString},
                        {"active", "1"}
                    }
                    
                },
                result => Debug.Log("Successfully updated user data"),
                error => {
                    ShowError("Error creating account.");
                    Debug.Log("Error creating account.");
                    Debug.Log(error.GenerateErrorReport());
                });
                

                wwwUserCheck.Dispose();
                StopCoroutine(CheckUser());
        
                
            }
        }
    }

    IEnumerator SubmitNewUserInfoEnum(){
        WWWForm form = new WWWForm();
        UnityWebRequest wwwSubmitUserInfo;

        form.AddField("username", player.displayName);
        form.AddField("userid", player.userID.ToString());
        form.AddField("active", "1");

        using(wwwSubmitUserInfo = UnityWebRequest.Post(url + "InsertNewUser.php", form)){
            yield return wwwSubmitUserInfo.SendWebRequest();
            if(wwwSubmitUserInfo.isNetworkError){
                Debug.Log(wwwSubmitUserInfo.error);
                playerinfo.messages.ErrorMessage.SetActive(true);
                playerinfo.messages.errorTitle.text = "Network Error";
                playerinfo.messages.errorText.text = "Unable to create account. Please try again later.";
            }
                
            else{
               // Debug.Log(wwwSubmitUserInfo.downloadHandler.text);
                StartCoroutine(SubmitNewPetInfoEnum());
                
                //Submit information on player and first pet!


                wwwSubmitUserInfo.Dispose();
                StopCoroutine(SubmitNewUserInfoEnum());
        
                
            }
        }
    }

    IEnumerator SubmitNewPetInfoEnum(){
        //Debug.Log("Pet being submitted.");
        foreach(Animal pet in player.pets){
           // Debug.Log("Pet name: " + pet.petName);
            WWWForm form = new WWWForm();
            UnityWebRequest wwwSubmitPetInfo;

            //Debug.Log("Created unity web request.");

            form.AddField("userid", player.userID.ToString());
            form.AddField("petname", pet.petName);
            form.AddField("petid", pet.petID);
            form.AddField("animal", pet.animalType);
            form.AddField("color", pet.color.color);
            form.AddField("glow", pet.glow);
            form.AddField("special", pet.special);
            form.AddField("face", pet.face.faceID);
            form.AddField("faceaccessories", pet.faceAccessory.accessoryID);
            form.AddField("backaccessories", pet.backAccessory.accessoryID);
            form.AddField("headaccessories", pet.headAccessory.accessoryID);
            form.AddField("lefthand", pet.leftHand.accessoryID);
            form.AddField("righthand", pet.rightHand.accessoryID);
            form.AddField("clothing", pet.clothing.clothingID);
            form.AddField("neckaccessories", pet.neckAccessory.accessoryID);
            Debug.Log("Active pet not set yet.");
            if(pet == player.activePet)
                form.AddField("active", "1");
            if(pet != player.activePet)
                form.AddField("active", "0");

           // Debug.Log("Form info created.");

            using(wwwSubmitPetInfo = UnityWebRequest.Post(url + "InsertNewPet.php", form)){
                yield return wwwSubmitPetInfo.SendWebRequest();
               // Debug.Log("Connecting to database.");
                if(wwwSubmitPetInfo.isNetworkError){
                    Debug.Log(wwwSubmitPetInfo.error);
                    playerinfo.messages.ErrorMessage.SetActive(true);
                    playerinfo.messages.errorTitle.text = "Network Error";
                    playerinfo.messages.errorText.text = "Unable to create account. Please try again later.";
                }
                    
                else{
                   // Debug.Log(wwwSubmitPetInfo.downloadHandler.text);
                   // Debug.Log("Completed pet info submission for " + pet.petName);
                    player.loggedIn = true;
                    
                    //Submit information on player and first pet!


                    wwwSubmitPetInfo.Dispose();
                    StopCoroutine(SubmitNewPetInfoEnum());
            
                    
                }
            }
        }
    }

    public void ShowError(string errorMessage, string errorTitle = "Error"){
        GameObject errorObject = GameObject.Instantiate(playerinfo.messages.ErrorMessage);
        playerinfo.messages.ErrorMessage.SetActive(true);
        playerinfo.messages.errorTitle.text = "Error";
        playerinfo.messages.errorText.text = errorMessage;
    }

    public void CheckConnection(){
        StartCoroutine(test());
    }

    IEnumerator test(){
        WWWForm form = new WWWForm();
        LogIn login = GameObject.FindGameObjectWithTag("scriptholder").GetComponent<LogIn>();
        using(UnityWebRequest www = UnityWebRequest.Post(url + "TestConnection.php", form)){
            yield return www.SendWebRequest();
            if(www.isNetworkError){
                Debug.Log(www.error);
                ShowError(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
                if(www.downloadHandler.text.Contains("Warning")){
                    Debug.Log(www.downloadHandler.text);
                    ShowError("Could not connect to server. Please try again later.");
                    login.mainMenu.SetActive(true);
                }
                else
                    login.SignIn();
            }
        }
    }
}
