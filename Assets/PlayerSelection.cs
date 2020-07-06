using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("IsComputerPlaying"))//if wanna play with computer 
        {
            PlayerPrefs.SetInt("IsComputerPlaying", 0);

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClikSinglePlayerBtn()
    {
        PlayerPrefs.SetInt("IsComputerPlaying", 1);
        SceneManager.LoadScene("Selection");//going to color selection screen
    }
    public void OnClikMultiPlayerBtn()
    {
        PlayerPrefs.SetInt("IsComputerPlaying", 0);
        SceneManager.LoadScene("Selection");//going to color selection screen
    }
}
