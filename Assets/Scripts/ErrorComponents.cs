using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorComponents : MonoBehaviour
{
    public Text errorTitle;
    public Text errorMessage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(){
        Destroy(this.gameObject);
    }
}
