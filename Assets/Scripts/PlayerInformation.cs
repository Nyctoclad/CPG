using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public Player player;
    public AnimalList animalList;
    public FurnitureList furnitureList;
    public Messages messages;
    public SurfaceMaterials surfaceMaterials;
    public List<RoomSize> roomObjects = new List<RoomSize>();

    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("True is " + bool.TrueString);
        Debug.Log("False is " + bool.FalseString);

        if(!player){
            player = new Player();
            player.displayName = "test";
        }
        
        MessageComponents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MessageComponents(){
        Transform errorObject, inputObject, confirmObject;
        errorObject = messages.ErrorMessage.transform.GetChild(0).GetChild(0);
        inputObject = messages.InputMessage.transform.GetChild(0).GetChild(0);
        confirmObject = messages.ConfirmMessage.transform.GetChild(0).GetChild(0);

        foreach(Transform erroritem in errorObject.transform){
            if(erroritem.gameObject.tag == "title")
                messages.errorTitle = erroritem.gameObject.GetComponent<Text>();
            
            if(erroritem.gameObject.tag == "text")
                messages.errorText = erroritem.gameObject.GetComponent<Text>();
        }

        foreach(Transform inputitem in inputObject.transform){  
            if(inputitem.gameObject.tag == "submit")
                messages.inputButton = inputitem.gameObject.GetComponent<Button>();
            
            if(inputitem.gameObject.tag == "input")
                messages.input = inputitem.gameObject.GetComponent<InputField>();
            
        }

        foreach(Transform confirmitem in confirmObject.transform){
            if(confirmitem.gameObject.tag == "title")
                messages.confirmTitle = confirmitem.gameObject.GetComponent<Text>();
            
            if(confirmitem.gameObject.tag == "text")
                messages.confirmText = confirmitem.gameObject.GetComponent<Text>();
            
            if(confirmitem.gameObject.tag == "submit")
                messages.confirmButton = confirmitem.gameObject.GetComponent<Button>();
        }
    }

    public void AddNewPet(Animal pet, string petName){
        Animal newPet = new Animal();
        newPet.animalType = pet.animalType;
        newPet.physObj = pet.physObj;
        newPet.color = pet.color;
        newPet.face = pet.face;
        newPet.glow = pet.glow;
        newPet.special = pet.special;
        newPet.backAccessory = pet.backAccessory;
        newPet.faceAccessory = pet.faceAccessory;
        newPet.headAccessory = pet.headAccessory;
        newPet.neckAccessory = pet.neckAccessory;
        newPet.leftHand = pet.leftHand;
        newPet.rightHand = pet.rightHand;
        newPet.animalName = pet.animalName;
        newPet.petName = petName;
        newPet.clothing = pet.clothing;

        player.pets.Add(newPet);

        player.pets[player.pets.IndexOf(newPet)].petID = player.pets.IndexOf(newPet) + 1;

        if(player.pets.Count == 1)
            player.activePet = player.pets[0];
    }

    public GameObject RezzPet(Vector3 position, int petIndex){
        GameObject pet = GameObject.Instantiate(player.pets[petIndex].physObj,position, player.pets[petIndex].physObj.transform.rotation);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        body.material = player.pets[petIndex].color.animalMaterial;
        face.material = player.pets[petIndex].face.face;
        return pet;
    }

    public GameObject RezzPetPIP(Vector3 position, int petIndex, Transform parent){
        GameObject pet = GameObject.Instantiate(player.pets[petIndex].physObj,position, player.pets[petIndex].physObj.transform.rotation, parent);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        pet.transform.localPosition = position;
        pet.transform.localScale = new Vector3(730.4f, 730.4f, 2f);
        body.material = player.pets[petIndex].color.animalMaterial;
        face.material = player.pets[petIndex].face.face;
        return pet;
    }
}
