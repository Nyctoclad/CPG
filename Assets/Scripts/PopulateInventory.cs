using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateInventory : MonoBehaviour
{
    public GameObject slot, inventoryHolder;
    public Player player;
    
    // Start is called before the first frame update

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Populate(){
        foreach(Item item in player.items){
            GameObject itemSlot = GameObject.Instantiate(slot, transform.position, transform.rotation, inventoryHolder.transform);
            itemSlot.GetComponent<NumAccess>().number.text = "" + item.itemAmount;
            itemSlot.GetComponent<NumAccess>().icon.sprite = item.itemIcon;
            itemSlot.GetComponent<NumAccess>().item = item;
        }
    }
}
