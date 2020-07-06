using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour {
    public static GameController instance;//variable via which class(attributes and methods) can be excess from other classes
                                          //declearing instance of this class
    public GameObject WinPopUp;//public refrence.holding winpopup
    public GameObject tipsContainer;
    public Text PopUpText;////public refrence. holding a text
    public string winningText;//public to get the winning team's info
    public int secondTeamName;//will identify player2's men color
    public int FirstTeamName;//will identify player1's men color
    public bool IsRotationallowed;//will tell camera to rotate or not
    public bool isWhiteTurn;//is this white (player1's) turn
    public GameObject[] Cameras;//getting cameras.
    public string sceneName;//getting scene name
    public GameObject rotatingCamera;//getting rotating camera
    public bool SkipBtnWasPressed;//bool for camera button press 
    public bool test;
    public GameObject[] cameraBtn;//public refrence to get camera button icons
    public GameObject[] AllColorsPlayer1;//getting colors of men for p1
    public GameObject[] AllColorsPlayer2;//getting colors of men for p1
    public GameObject PausePop;//public refrence to pause popup
    public GameObject light;//getting roatting camera's light
    public bool resumedAtTur2;//bool for for p2's turn after resume button pressed
    public bool NotAllowed;//bool for camera rotation
    public bool stopTurns;
    public GameObject PauseBtn;
    float alpha = 0.5f; Color current_Color;// local variable for colors of men at color selection
    // Use this for initialization
    private void Awake()//initializing instance of this class
    {
        instance = this;
    }


    void Start () {
        //initializing variables

        if (PlayerPrefs.GetInt("IsComputerPlaying")==1 && cameraBtn.Length>0)
        {
            cameraBtn[0].gameObject.GetComponent<Image>().enabled = false;
            cameraBtn[1].gameObject.GetComponent<Image>().enabled = false;
        }
        test = true;
        resumedAtTur2 = false;
        stopTurns = false;
        NotAllowed = false;
        secondTeamName = -1;//to dont get errors
        FirstTeamName = -1;//to dont get errors
        IsRotationallowed = false;
        isWhiteTurn = true;
        SkipBtnWasPressed = false;
    }
	
	// Update is called once per frame
	void Update () {
        //summary//
        /*
         all this code is managing cameras.
        if camera button is pressed rotatingCamera should be off or vice versa.
        if p1's turn,turn on camera[1].
        if p2's turn,turn on camera[0].
        giving tag's to camera and untagging
        this code will run every frame
        */


        if (sceneName == "Game")//if this is game scene
        {
            if (IsRotationallowed)//if camera roattion is allowed
            {
              
                cameraBtn[0].SetActive(false);//camera for player2
                cameraBtn[1].SetActive(false);//camera for player1
            }
            else
            {
                if (SkipBtnWasPressed)//if camera btn was pressed
                {
                    cameraBtn[0].SetActive(true);
                }
                else
                {
                    cameraBtn[1].SetActive(true);

                }
            }


            if (!SkipBtnWasPressed)//if camera btn was not pressed
            {
                if (isWhiteTurn && !IsRotationallowed)//p1's turn
                {
                    PauseBtn.SetActive(true);
                    Cameras[0].SetActive(false);
                    Cameras[1].SetActive(true);
                    rotatingCamera.gameObject.tag = "Untagged";//because two camera's cant work at the same time with same tag
                    Cameras[1].tag = "MainCamera";
                    NotAllowed = false;
                }
                else if (!isWhiteTurn && !IsRotationallowed)//p2's turn
                {
                    PauseBtn.SetActive(true);
                    Cameras[0].SetActive(true);
                    Cameras[1].SetActive(false);
                    rotatingCamera.gameObject.tag = "Untagged";
                    Cameras[0].tag = "MainCamera";
                    NotAllowed = false;
                }
                else if (IsRotationallowed)//when rotatingcamera is on. turn off p1 and p2's camera
                {
                    if(PlayerPrefs.GetInt("IsComputerPlaying") == 0)
                    {
                        PauseBtn.SetActive(false);
                        rotatingCamera.tag = "MainCamera";
                        Cameras[0].SetActive(false);
                        Cameras[1].SetActive(false);
                        NotAllowed = true;
                    }
                    else{
                        light.SetActive(false);
                    }
                 

                }
            }
            else
            {
                if (IsRotationallowed)
                {
                    if (test)
                    {
                        Cameras[0].SetActive(false);
                        Cameras[1].SetActive(true);
                        rotatingCamera.gameObject.tag = "Untagged";
                        Cameras[1].tag = "MainCamera";
                    }
                    else if (!test)
                    {
                        Cameras[0].SetActive(true);
                        Cameras[1].SetActive(false);
                        rotatingCamera.gameObject.tag = "Untagged";
                        Cameras[0].tag = "MainCamera";
                    }
                }
            }



            if (!PausePop.gameObject.activeInHierarchy)
            {
                Time.timeScale = 1;
            }
        }
        
    }

    public void SetPopUp() //turning on the WiningPopUp
    {
       
        //giving text(which player wins) in popuptext 
        PopUpText.text = winningText;
        WinPopUp.SetActive(true);
    }

    public void OnClikResetBtn()//if reset button is presses
    {
        //again load the game scene 
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
      
		SceneManager.LoadScene("playerSelection");
    }
    public void OnClikHomeBtn()//if home button is pressed
    {
		Time.timeScale = 1;
       // checkerBoard.instance.OnClikResumBtn();//save player's men info.
        SceneManager.LoadScene("MainMenu");
    }
    public void OnClickBackBtn()//if back button is pressed
    {
        //resetting the variables
        //closing pause menu

		Time.timeScale = 1;
		if ((checkerBoard.instance.isComputersPlaying && !checkerBoard.instance.iscoumputers_turn) || (!checkerBoard.instance.isComputersPlaying)) {
			//IsRotationallowed = true;
			NotAllowed = false;
			stopTurns = false;
		}
		PausePop.SetActive(false);
        
       
    }
    public void OnClickPauseBtn()//if pause button is pressed
    {

		PausePop.SetActive(true);
		Invoke ("Pause_Clicked_", 1f);
        //pausing everthing
        
    }

	void Pause_Clicked_(){
		
			

		if ((checkerBoard.instance.isComputersPlaying && !checkerBoard.instance.iscoumputers_turn) || (!checkerBoard.instance.isComputersPlaying)) {
			IsRotationallowed = false;
			NotAllowed = true;
			stopTurns = true;

			checkerBoard.instance.StopEveryThing ();
			Time.timeScale = 0;
		} 

		checkerBoard.instance.OnClikResumBtn ();

	}


    public void Player1Color(GameObject obj)//player one choose his color
    {
       
        FirstTeamName = int.Parse(obj.name);
        PlayerPrefs.SetInt("Name1", FirstTeamName);//saving it to use again
        for (int i = 0; i < AllColorsPlayer1.Length; i++)//blur every color
        {
            alpha = 0.5f;
            current_Color = AllColorsPlayer1[i].gameObject.GetComponent<Image>().color;
            current_Color.a = alpha;
            AllColorsPlayer1[i].gameObject.GetComponent<Image>().color = current_Color;
        }

        alpha = 1; current_Color.a = alpha;
        AllColorsPlayer1[FirstTeamName].gameObject.GetComponent<Image>().color = current_Color;//highlight the current selected color
    }
    public void Player2Color(GameObject obj)//player one choose his color
    {
        
        secondTeamName = int.Parse(obj.name);
        PlayerPrefs.SetInt("Name2", secondTeamName);//saving it to use again
        for (int i = 0; i < AllColorsPlayer1.Length; i++)//blur every color
        {
            alpha = 0.5f;
            current_Color = AllColorsPlayer2[i].gameObject.GetComponent<Image>().color;
            current_Color.a = alpha;
            AllColorsPlayer2[i].gameObject.GetComponent<Image>().color = current_Color;
            
        }
        alpha = 1; current_Color.a = alpha;
        AllColorsPlayer2[secondTeamName].gameObject.GetComponent<Image>().color = current_Color;//highlight the current selected color
    }

    public void Onclicktab()//if camera btn is clicked
    {
        //switching cameras
        if (!SkipBtnWasPressed)//if it was pressed
        {
            
            SkipBtnWasPressed = true;
            cameraBtn[0].SetActive(true);
            cameraBtn[1].SetActive(false);
            light.SetActive(false);
         

        }
        else//if it was pressed
        {
            SkipBtnWasPressed = false;
            cameraBtn[0].SetActive(false);
            cameraBtn[1].SetActive(true);
        }
        IsRotationallowed = false;
        if (isWhiteTurn)//if p1's turn
        {
             Cameras[0].SetActive(false);
             Cameras[1].SetActive(true);
        }
        else//if p2's turn
        {
            Cameras[0].SetActive(true);
            Cameras[1].SetActive(false);
        }

        checkerBoard.instance.IfSkipBtnPressed();
    }

    public void OnResumeCameraTurn()//if it was 2nd playe's turn and game is resumed handle camera
    {
		//Time.timeScale = 1;
        //switching camera
        isWhiteTurn = false;
        Cameras[1].SetActive(true);
        Cameras[0].SetActive(false);
        Cameras[0].tag = "MainCamera";
        resumedAtTur2 = true;
    }

}
