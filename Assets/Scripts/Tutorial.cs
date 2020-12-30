using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Tutorial : MonoBehaviour
{
    public bool pause;
    public string[] tutorialWords;
    public Text words,namewords;
    public GameObject confirmMessage, namingMessage, chosenObject, mainMenu, tutorialObject, tutorialPetObject;
    GameObject pet;
    Player player;
    
    Vector3 petPosition = new Vector3(8.9691f,4.041695f,7.391274f), defaultRoomPosition = new Vector3(9.196397f, 5.672013f, 7.483993f);

    int i = 0;
    public int[] pauseNumbers;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInformation playerInfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
        player = playerInfo.player;
        if(!player.loggedIn){
            foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
                pet.GetComponent<TempPickScripts>().onlyonce = true;
            }
            
            if(tutorialWords.Length > 0)
                words.text = tutorialWords[i];
            //Create a new room for this new player
            if(player.rooms.Count < 1){
                PlayerInformation playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
                SurfaceMaterials sf = playerinfo.surfaceMaterials;
                Room room = new Room();
                room.active = true;
                room.defaultRoom = true;
                foreach(Mat mat in sf.materials){
                    if(mat.num == 0)
                        room.floor = sf.materials[0];
                    else if(mat.num == 1){
                        room.wall1 = sf.materials[1];
                        room.wall2 = sf.materials[1];
                        room.wall3 = sf.materials[1];
                    }
                }
                
                room.roomSize = 4;
                room.rotation = Quaternion.Euler(0,0,0);
                room.location = defaultRoomPosition;
                foreach(RoomSize rs in playerInfo.roomObjects){
                    if(rs.roomSize == room.roomSize){
                        room.floorObject = rs.floorObject;
                        room.wall1Object = rs.wall1Object;
                        room.wall2Object = rs.wall2Object;
                        room.wall3Object = rs.wall3Object;
                    }
                }
                player.rooms.Add(room);

            }
            if(player.rooms.Count > 0){
                //Rezz room
                Room room = player.rooms[0];
                GameObject rezzedRoom = GameObject.Instantiate(room.physObj, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                rezzedRoom.transform.localPosition = room.location;
                rezzedRoom.transform.localRotation = room.rotation;
                room.floorObject.GetComponent<MeshRenderer>().material = room.floor.material;
                room.wall1Object.GetComponent<MeshRenderer>().material = room.wall1.material;
                room.wall2Object.GetComponent<MeshRenderer>().material = room.wall2.material;
                room.wall3Object.GetComponent<MeshRenderer>().material = room.wall3.material;

            }
        }
        else{
            mainMenu.SetActive(true);
            tutorialObject.SetActive(false);
            tutorialPetObject.SetActive(false);
            Tutorial tutorial = this;
            tutorial.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        NextLine();
    }

    public void SubmitAnimalChoice(){
        chosenObject.transform.position = petPosition;
        
        confirmMessage.SetActive(false);
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            if(pet != chosenObject)
                pet.SetActive(false);
        }
        
        namingMessage.SetActive(true);
    }

    public void SubmitAnimalName(){
        if(System.Text.RegularExpressions.Regex.IsMatch(namewords.text, @"^[a-zA-Z]+$")){
            chosenObject.GetComponent<TempPickScripts>().onlyonce = true;
            pause = false;
            if(tutorialWords.Length > 0 && i < tutorialWords.Length){
                i++;
                Debug.Log(tutorialWords[i]);
                words.text = tutorialWords[i];
                pause = true;
                StartCoroutine(Wait(.5f));
                
            }

            PlayerInformation pi = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
            PetInformation petInfo = chosenObject.GetComponent<PetInformation>();

            pi.AddNewPet(petInfo.animal,namewords.text);
            pi.gameObject.GetComponent<SQLFunctions>().SubmitNewUserInfo();
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
                Data = new Dictionary<string, string>() {
                    {"loggedin", bool.TrueString},
                }
            },
            result => Debug.Log("Successfully updated user data"),
            error => {
                Debug.Log("Got error setting user data Ancestor to Arthur");
                Debug.Log(error.GenerateErrorReport());
            });
            namingMessage.SetActive(false);
        }
        else{
            PlayerInformation pi = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
            pi.messages.ErrorMessage.SetActive(true);
            pi.messages.errorText.text = "Pet names must only use letters.";
        }
        
    }


    public void Exit(){
        chosenObject.GetComponent<TempPickScripts>().onlyonce = false;
    }

    public void NextLine(){
        if(((Input.touchCount > 0) || Input.GetMouseButton(0)) && !pause){
            if(tutorialWords.Length > 0 && i < tutorialWords.Length){
                i++;
                Debug.Log(tutorialWords[i]);
                words.text = tutorialWords[i];
                pause = true;
                StartCoroutine(Wait(.5f));
                
            }
            else if(i > 0){
                mainMenu.SetActive(true);
                tutorialObject.SetActive(false);
                tutorialPetObject.SetActive(false);
                PlayerInformation playerinfo = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>();
                pet = playerinfo.RezzPet(petPosition, playerinfo.player.pets.IndexOf(playerinfo.player.activePet));
                GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().currentPet = pet;
                Tutorial tutorial = this;
                tutorial.enabled = false;
            }

        }
    }

    IEnumerator Wait(float seconds){
        yield return new WaitForSeconds(seconds);
        foreach(int num in pauseNumbers){
            if(i == num){
                if(i == 2)
                    foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
                        pet.GetComponent<TempPickScripts>().onlyonce = false;
                    }
                break;
            }
            else pause = false;
        }
    }
}
