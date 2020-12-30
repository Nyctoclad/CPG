using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public Item item;
    public Player player;
    public Animal currentPet;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
        currentPet = GameObject.FindObjectOfType<PetNavigation>().currentPet;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Consume(){
        if(item.itemAmount - 1 >= 0)
            item.itemAmount--;
        
    }

    void UpdatePlayerInfo(){

    }
}
