using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public CanvasGroup im;
    bool doneFade = false;
    public GameObject mainMenu, options;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut(){
        im.alpha = 1f;
        StartCoroutine(fadeOutEnum());
    }

    IEnumerator fadeOutEnum(){
        if(im.alpha > 0f)
            im.alpha -= 0.01f;
        else
        {
            doneFade = true;
            Debug.Log("Done fade!");
            yield return null;
        }
        yield return null;
        //if(!doneFade)
            //StartCoroutine(fadeOutEnum());
    }

    public void FadeIn(){
        im.alpha = 0f;
        StartCoroutine(fadeInEnum());
    }

    IEnumerator fadeInEnum(){
        //Debug.Log("Fading in...");
        StopCoroutine(fadeOutEnum());
        if(im.alpha < .95){
            Debug.Log("Im alpha: " + im.alpha);
            im.alpha = im.alpha + 0.01f;
            Debug.Log("Im alpha 2: " + im.alpha);
        }
            
        else
        {
            doneFade = true;
            StopCoroutine(fadeInEnum());
            yield return null;
        }
        yield return null;
        //if(!doneFade)
           // StartCoroutine(fadeInEnum());
    }

    public void OptionsFade(){
        FadeOut();
        StartCoroutine(OptionsFadeEnum());
    }

    IEnumerator OptionsFadeEnum(){
        yield return null;
        if(!doneFade){
            StartCoroutine(OptionsFadeEnum());
            //Debug.Log("Working...");
        }
            
        else{
            StopCoroutine(fadeOutEnum());
            mainMenu.SetActive(false);
            options.SetActive(true);
            FadeIn();
            doneFade = false;
        }
        
    }

    public void MainMenuFade(){
        FadeOut();
        StartCoroutine(MainMenuFadeEnum());
    }

    IEnumerator MainMenuFadeEnum(){
        yield return null;
        if(!doneFade)
            StartCoroutine(MainMenuFadeEnum());
        else{
            mainMenu.SetActive(true);
            options.SetActive(false);
            FadeIn();
            doneFade = false;
        }
        
    }
}
