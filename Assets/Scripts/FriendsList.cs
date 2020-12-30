using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateFriendsList(){

    }

    public void LoadProfile(){
        
    }

    public void CheckOnline(){

    }

    public void PopulatePending(){

    }

    public void PopulateBlocked(){

    }

    public void AcceptRequest(){

    }

    public void DenyRequest(){

    }

    public void Block(){

    }

    public void Unblock(){

    }

    public void Visit(){

    }
}
