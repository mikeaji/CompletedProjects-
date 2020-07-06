using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    public GameObject resumbtn;//public reference to resume btn;

    // Use this for initialization
    void Start()
    {
        /// PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("isResumeBtnPressed"))//if records available
        {
            if (PlayerPrefs.GetInt("isResumeBtnPressed") == 1)//turning on resum btn if there are prvious records
            {
                resumbtn.SetActive(true);
            }
        }
        else//if records dsn't exists
        {
            resumbtn.SetActive(false);
            PlayerPrefs.SetInt("isResumeBtnPressed", 0);
        }

       


    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickplayBtn()
    {
        PlayerPrefs.DeleteAll();//reset all records

        if (!PlayerPrefs.HasKey("Name1"))//team 1 men color
        {
            PlayerPrefs.SetInt("Name1", -1);
        }
        if (!PlayerPrefs.HasKey("Name2")) //team 2 men color
        {
            PlayerPrefs.SetInt("Name2", -1);
        }
        SceneManager.LoadScene("playerSelection");//going to color selection screen

    }

    public void OnClikReSumeBtn()//if u have any records
    {
        SceneManager.LoadScene("Game");//go to game scene
    }

   

    public void Quit()//quit game
    {
        Application.Quit();//quit game
        Debug.Log("quitting game");
    }

}
