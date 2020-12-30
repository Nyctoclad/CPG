using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempPickScripts : MonoBehaviour
{
    public bool onlyonce = false;
    Tutorial tutorial;
    public Transform chosenObject;
    
    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.FindGameObjectWithTag("scriptholder").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!onlyonce){
            ClickOn();
        }
    }

    void ClickOn(){
        if((Input.touchCount > 0 || Input.GetMouseButton(0))){
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if(Physics.Raycast (ray, out hit))
            {
                if(hit.transform.tag == "pet")
                {
                    
                    tutorial.chosenObject = hit.transform.gameObject;
                    tutorial.confirmMessage.SetActive(true);
                    onlyonce = true;
                }
                else {
                    
                }
            }
        }
    }
}
