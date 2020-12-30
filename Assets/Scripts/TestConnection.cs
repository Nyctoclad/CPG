using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestConnection : MonoBehaviour
{
    private string secretKey="ym835d8XTqmB50xU"; 
    string url = "http://localhost/CPG/PHPs/";
    
    WWWForm form;

    /// 
    // Start is called before the first frame update
    void Start()
    {
        form = new WWWForm();
        StartCoroutine(test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator test(){
        using(UnityWebRequest www = UnityWebRequest.Post(url + "TestConnection.php", form)){
            yield return www.SendWebRequest();
            if(www.isNetworkError)
                Debug.Log(www.error);
            else{
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
