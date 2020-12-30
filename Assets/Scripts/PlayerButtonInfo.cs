using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerButtonInfo : MonoBehaviour
{
    public Text username, petName, petType, petColor;
    public Image userIcon;
    public GameObject online, offline, confirmBox, infoBox, friendsPetInfo;
    
    public Player friend;
    public string playerName;
    SQLFunctions sqf;
    List<string[]> changeList = new List<string[]>();
    Transform original;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Online(){
        online.SetActive(true);
        offline.SetActive(false);
    }

    public void Offline(){
        offline.SetActive(true);
        offline.SetActive(false);
    }

    public void Info(){
        string playerType = this.gameObject.tag;
        sqf = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<SQLFunctions>();
        
        
        if(playerType == "fpin"){
            if(confirmBox != null){
                ConfirmBoxInfo cbi = confirmBox.GetComponent<ConfirmBoxInfo>();
                cbi.title.text = "Add Friend";
                cbi.descriptionText.text = "Add " + playerName + " as a friend?";
                cbi.confirm.onClick.AddListener(()=> AddFriend());
                confirmBox.SetActive(true);
            }
           
        }
        else if(playerType == "fpout"){
            infoBox.SetActive(true);

        }
        else if(playerType == "friends"){
            
        }
        else if(playerType == "bout"){
            if(confirmBox != null){
                ConfirmBoxInfo cbi = confirmBox.GetComponent<ConfirmBoxInfo>();
                cbi.title.text = "Unblock";
                cbi.descriptionText.text = "Unblock " + playerName + "?";
                cbi.confirm.onClick.AddListener(()=> Unblock());
            }
        }
    }

    public void ShowFriendsPetInfo(){
        GameObject petholder = new GameObject();
        Debug.Log("Friend first pet is " + friend.pets[0].petName);

        petName.text = friend.activePet.petName;
        petType.text = friend.activePet.animalName;
        petColor.text = friend.activePet.color.colorName;

        foreach(Transform child in friendsPetInfo.transform){
            if(child.gameObject.tag == "petholder"){
                petholder = child.gameObject;
                break;
            }
        }

        GameObject pet = GameObject.Instantiate(friend.activePet.physObj,new Vector3(0,0,0), friend.activePet.physObj.transform.rotation, petholder.transform);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        pet.transform.localPosition = new Vector3(0,0,0);
        pet.transform.localScale = new Vector3(730.4f, 730.4f, 2f);
        body.material = friend.activePet.color.animalMaterial;
        face.material = friend.activePet.face.face;
        GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().SendBackward();
        friendsPetInfo.SetActive(true);
    }


    public void AddFriend(){
        sqf.UpdateFriends(friend.userID, 0, 0, 1, 0, 0);
        MainLandController mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
        mlc.pbuserID = friend.userID;
        mlc.CreateFriendsList();
        confirmBox.SetActive(false);
    }

    public void Unblock(){
        sqf.UpdateFriends(friend.userID, 0,0,0,0,0);
        MainLandController mlc = GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>();
        mlc.pbuserID = friend.userID;
        mlc.CreateFriendsList();
        confirmBox.SetActive(false);
    }
    
    

   /* IEnumerator CheckForChange(){
        bool changed = false;
        List<string[]> check = new List<string[]>();
        Player player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
        foreach(Player f in player.friendsPendingIn){
            string[] friendInfo = new string[2];
            friendInfo[0] = "pin";
            friendInfo[1] = f.userID.ToString();
            check.Add(friendInfo);
        }
        foreach(Player f in player.friendsPendingOut){
            string[] friendInfo = new string[2];
            friendInfo[0] = "pout";
            friendInfo[1] = f.userID.ToString();
            check.Add(friendInfo);
        }
        foreach(Player f in player.friends){
            string[] friendInfo = new string[2];
            friendInfo[0] = "f";
            friendInfo[1] = f.userID.ToString();
            check.Add(friendInfo);
        }
        foreach(Player f in player.blockedOut){
            string[] friendInfo = new string[2];
            friendInfo[0] = "bout";
            friendInfo[1] = f.userID.ToString();
            check.Add(friendInfo);
        }
        Debug.Log("Check is " + check[0][0] + ": " + check[0][1] + " and original is " + changeList[0][0] + ": " + changeList[0][1]);
        GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().CreateFriendsList();
        yield return new WaitForSeconds(2f);
        
        if(Enumerable.SequenceEqual(check, changeList) && i < 3){
            GameObject.FindGameObjectWithTag("landinfo").GetComponent<MainLandController>().ResetUI();
            Debug.Log("No change yet.");
            
            i++;
            StartCoroutine(CheckForChange());
        }
        else{
            Debug.Log("Exiting change check.");
            StopCoroutine(CheckForChange());
            
        }
        
    }*/

}
