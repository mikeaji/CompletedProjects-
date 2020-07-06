using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectionScreen : MonoBehaviour {

    public GameObject ChoosSomeOther;//dialouge box choos someother color
    public GameObject Startbtn;//play game btn
    public GameObject colorContainer;//color men grids
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //if p1 and p2 has picked there colors
             if(GameController.instance.FirstTeamName != -1 && GameController.instance.secondTeamName != -1)
             {
                 Startbtn.SetActive(true);//turn on start game btn
             }

           
        
    }
    
    public void OnClickStartBtn()//when start btn is pressed
    {
        if (GameController.instance.FirstTeamName != GameController.instance.secondTeamName)//if colors are not same
        {
            if (GameController.instance.FirstTeamName != -1 && GameController.instance.secondTeamName != -1)//both have picked there colors
            {

                SceneManager.LoadScene("Game");//start game
            }
            
        }
        else//otherwise choose someother color dialouge will appear
        {
           ChoosSomeOther.SetActive(true);
           ChoosSomeOther.GetComponent<Animator>().Rebind();//reseting the animation of the box
          ChoosSomeOther.GetComponent<Animator>().enabled = true;
        }
    }
}
