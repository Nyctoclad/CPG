using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Custom Assets/Messages")]
public class Messages : ScriptableObject
{
    public GameObject ErrorMessage, ConfirmMessage, InputMessage;
    public Text errorTitle, errorText, confirmTitle, confirmText;
    public InputField input;
    public Button confirmButton, inputButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
