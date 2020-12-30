using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScrollSideways : MonoBehaviour
{
    public List<HorizontalScrollSnap> scrolls = new List<HorizontalScrollSnap>();
    public HorizontalScrollSnap itemPages, mainPage;
    public SwipeManager sm;
    public HeightCheck hc;
    //public GameObject furniturePage;

    
    // Start is called before the first frame update
    void Start()
    {
        //furniturePage = new GameObject();

    }

    // Update is called once per frame
    void Update()
    {
        if(!hc.down){
            NextPage();
        }
    }

    void NextPage(){
        if(sm.SwipeLeft){
            Debug.Log("Swiped!");
            foreach(HorizontalScrollSnap scroll in scrolls){
                
                if(scroll.CurrentPage + 1 >= scrolls.Count){
                    scroll.ChangePage(0);
                }
                else{
                    scroll.ChangePage(scroll.CurrentPage+1);
                }
            }
        }
        else if(sm.SwipeRight){
            foreach(HorizontalScrollSnap scroll in scrolls){
                if(scroll.CurrentPage - 1 < 0){
                    scroll.ChangePage(scrolls.Count - 1);
                }
                else{
                    scroll.ChangePage(scroll.CurrentPage-1);
                }
            }
        }

        UpdatePages();
    }

    public void PageChange(int num){
        num = num - 1;
        int i = num - 5;
        if(i < 0)
            i = 11 + i;

        foreach(HorizontalScrollSnap scroll in scrolls){ 
            scroll.ChangePage(i);
            
            if(i + 1 >= 11)
                i = 0;
            else
                i++;
            

            
        }

        UpdatePages();

    }

    void UpdatePages(){
        itemPages.ChangePage(mainPage.CurrentPage);
    }
}

