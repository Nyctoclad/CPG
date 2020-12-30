using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class LogIn : MonoBehaviour
{
    public Player player;
    public GameObject displayNameContainer, errorContainer, confirmContainer;
    public InputField dnInput;
    public Text errorText;

    public TemporaryAccount tempAccount;


    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("The player object is " + GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player.displayName);

        player = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<PlayerInformation>().player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TempCreateAccount(){
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "DD12B"; 
        }
        
        var request = new RegisterPlayFabUserRequest{Username = tempAccount.tempUsername, Password = tempAccount.tempPassword, RequireBothUsernameAndEmail = false};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnTempRegisterSuccess, OnTempRegisterFailure);
    }

    private void OnTempRegisterSuccess(RegisterPlayFabUserResult result){
        Debug.Log("Success!");
    }

    private void OnTempRegisterFailure(PlayFabError error){
        //Attempt to login user
        Debug.Log("Failure!");
        Debug.Log(error);
    }

    public void SignIn(){
        
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "DD12B"; 
        }
        
        var request = new RegisterPlayFabUserRequest{Username = tempAccount.tempUsername, Password = tempAccount.tempPassword, RequireBothUsernameAndEmail = false};
      //  var request = new LoginWithPlayFabRequest{TitleId = PlayFabSettings.TitleId, Username = tempAccount.tempUsername, Password = tempAccount.tempPassword};
     //   LoginWithCustomIDRequest { CustomId = player.userID.ToString(), CreateAccount = true};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    

    private void OnRegisterSuccess(RegisterPlayFabUserResult result){
        Debug.Log("Account successfully created.");
        GameObject.FindGameObjectWithTag("playerinfo").GetComponent<SQLFunctions>().CreateUser();
        displayNameContainer.SetActive(true);
        player.displayName = "";
        player.loggedIn = false;
        player.activePet = new Animal();
        player.pets = new List<Animal>();
        player.friends = new List<Player>();
        player.items = new List<Item>();
    }

    private void OnRegisterFailure(PlayFabError error){
        //Attempt to login user
        Debug.Log("Failed to register user.");
        Debug.Log(error);

        var request = new LoginWithPlayFabRequest{TitleId = PlayFabSettings.TitleId, Username = tempAccount.tempUsername, Password = tempAccount.tempPassword};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        GetUserData(result.PlayFabId);
        
   
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    void GetUserData(string ID) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            PlayFabId = ID,
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("userid")) {
                errorText.text = "Account does not exist.";
                errorContainer.SetActive(true);
            }
            else {
                //Debug.Log("Ancestor: "+result.Data["Ancestor"].Value);
                if(int.Parse(result.Data["active"].Value) == 0){
                    errorText.text = "Account not active. Please contact support.";
                    errorContainer.SetActive(true);
                }
                else{
                    player.userID = double.Parse(result.Data["userid"].Value);
                    player.loggedIn = bool.Parse(result.Data["loggedin"].Value);
                    
                    if(!player.loggedIn){
                        Debug.Log("Player has never logged in!");
                        displayNameContainer.SetActive(true);
                        player.displayName = "";
                        player.loggedIn = false;
                        player.activePet = new Animal();
                        player.pets = new List<Animal>();
                        player.friends = new List<Player>();
                        player.items = new List<Item>();
                    }
                    else if(player.loggedIn){
                        Debug.Log("Player has logged in before");
                        SQLFunctions sqf = GameObject.FindGameObjectWithTag("playerinfo").GetComponent<SQLFunctions>();
                        sqf.RetrieveUser();
                        sqf.RetrievePet();
                        sqf.RetriveRooms();
                        //sqf.RetriveFriends();
                        SceneManager.LoadSceneAsync("MainScene");
                    }
                }
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void OnLoginFailure(PlayFabError error)
    {
        errorText.text = "There was an error trying to login. Please try again later.";
        errorContainer.SetActive(true);
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    
    }

    public void SubmitName(){
        if(!System.Text.RegularExpressions.Regex.IsMatch(dnInput.text, @"^[a-zA-Z]+$")){
            errorText.text = "Display names must use letters only.";
            errorContainer.SetActive(true);
        }
        else{
            confirmContainer.SetActive(true);
        }
    }

    public void ConfirmName(){
        player.displayName = dnInput.text;
        SceneManager.LoadSceneAsync("MainScene");
    }

}
