using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class checkerBoard : MonoBehaviour
{

    // white stands for p1
    // black stands for p2

  
    public static checkerBoard instance;//declearing public instance of this class
    //public references
    public Piece[,] pieces = new Piece[8, 8];//getting checker's boards rows and columns 8*8
    public GameObject whitePiecePrefab;//p1's men prefab
    public GameObject blackPiecePrefab;//p2's men prefab
    public Vector3 boardOffSet = new Vector3(-4.0f, 0, -4.0f);//for accurate pos of men
    public Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);//for accurate pos of men
    public Vector3 posvector;//position vector.
    public List<GameObject> gotis = new List<GameObject>();//list of men as gamobjects
    public List<Vector2> enddragofPiece = new List<Vector2>(25);//position of men
    public string tag_of_obj;//tag of men//white(p1) or black(p2)
    public Color[] colors;//colors to give men
    public BoxCollider BoardCollider;//board collider
    public Text Turn; //whose turn p1 or p2
    public Text timer;//refrence to timer in game
    public bool isWhite;//men is of p1 or not
    public Text p1score;
    public Text p2Score;
    public Text winpopupScore1;
    public Text winpopupScore2;
    //private variables
    bool isWhiteTurn;// whose turn
    Piece selectedpiece;//selected men
    public List<Piece> forcedPieces;//forces move list
    Vector2 mouseOver;//where is fingure touching
    Vector2 stratDrag;//strating position of touch/move
    Vector2 endDrag;//ending psiiton of touch/move
    bool hasKilled;//did men killed opponent's men
    int turn_Number;//even(p2's turn) or odd(p1's turn)
    int whiteCount;//men count of p1
    int blackCount;//men count of p2
    bool turnContinued;//if u can kill 2 men in one row
    int gotinumber;//men number
    int time;//time
    int p1_score;
    int p2_score;


    bool TurnEnded;

    //AI section
    public List<GameObject> moveanble_AIGottis = new List<GameObject>();
    public List<int> saftey_Steps_ai = new List<int>();
    public bool isComputersPlaying;
    public List<int> max_safeMoves = new List<int>();
    public int movegottiOfIndex = 0;
    public bool iscoumputers_turn;
    public bool forcedToMoveAiGotti;
    public int DontMoveThisGotti;


    private void Awake()
    {
        instance = this;//initializing instance
    }
    void Start()
    {

        //initializing variables
        Debug.Log(PlayerPrefs.GetInt("IsComputerPlaying"));
        if (PlayerPrefs.GetInt("IsComputerPlaying") == 1)
        {
            timer.enabled = false;
            isComputersPlaying = true;

        }
        else
        {
            iscoumputers_turn = false;
            timer.enabled = true;
            isComputersPlaying = false;

        }
        p1_score = 0;
        p2_score = 0;
        gotinumber = 0;
        time = 20;
        forcedToMoveAiGotti = false;
        StartCoroutine(TimeInSeconds());//starting timer
        blackPiecePrefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial.color = colors[PlayerPrefs.GetInt("Name2")];//change color of p2's men as seleted
        whitePiecePrefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial.color = colors[PlayerPrefs.GetInt("Name1")];//change color of p1's men as seleted
        if (PlayerPrefs.GetInt("isResumeBtnPressed") == 1)//if there were any record
        {
            SetGottis();//set player's
            selectedpiece = null;
            endDrag = Vector2.zero;
        }
        else//if no records set board from zero
        {
            GenerateBorad();
            whiteCount = 12;
            blackCount = 12;
        }

        turnContinued = true;
        forcedPieces = new List<Piece>();

        if (!PlayerPrefs.HasKey("TurnPlayer"))//chehcking which player's the turn was
        {
            PlayerPrefs.SetInt("TurnPlayer", 1);
            turn_Number = 1;

            isWhiteTurn = true;
        }
        else
        {
            turn_Number = PlayerPrefs.GetInt("TurnPlayer");
            if (turn_Number % 2 != 0)//if p1's
            {
                isWhiteTurn = true;
            }
            else//if p2's turn
            {
                isWhiteTurn = false;
                isWhite = false;
                GameController.instance.OnResumeCameraTurn();
                Turn.text = "Player 2's Turn";
                Turn.transform.parent.gameObject.SetActive(true);
                if (isComputersPlaying)
                {
                    Ai_CheckAvailableGottis();
                }

            }
        }

        PlayerPrefs.SetInt("isResumeBtnPressed", 1);//starting records
       

    }

    void Update()
    {
        //will run every frame

        if (time <= 0)//if turn's time is up
        {
            StopAllCoroutines();//stop timer
            time = 1;
            timer.text = "00:00";
            EndTurn();//end the turn
        }
        else if (GameController.instance.IsRotationallowed && !GameController.instance.SkipBtnWasPressed)//if camera is rotating stop the timer
        {
            StopAllCoroutines();
        }

     
        UpdateMouseOver();//getting pos of mouse
    


   

        if (!GameController.instance.NotAllowed)//if playing is allowed
        {
           
            if ((isWhite) ? isWhiteTurn : !isWhiteTurn)//decide whose turn is this
            {
                //takin mouse position
                if (!iscoumputers_turn)
                {
                   //  Debug.Log("turn:"+ isWhiteTurn);
                  //  Debug.Log("iscoumputers_turn: " + iscoumputers_turn);
                    int x = (int)mouseOver.x;
                    int y = (int)mouseOver.y;
                    if (selectedpiece != null)//if didnt selected a piece yet
                    {
                        UpdatePieceDrag(selectedpiece);
                    }

                    if (Input.GetMouseButtonDown(0))//when clicked aur touch beggins
                    {
                        Selectpiece(x, y);//select the piece
                    }

                    if (Input.GetMouseButtonUp(0))//when click btn is release or touch ended
                    {
                        TryMove((int)stratDrag.x, (int)stratDrag.y, x, y);//try to move the men for start to enddrag position
                    }
                }

            }

        }

        
    }

    void UpdateMouseOver()
    {
        
        if (!Camera.main )//if there is no camera
        {
            Debug.Log("No Camera Found");
            return;
        }

        RaycastHit hit;
        //<summary>
        /*raycast emits a ray from camera(anygameobject) to any Gameobject and detect colliders*/
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")) )//cast a ray from camera to board
        {
            //if detected board's collider
            mouseOver.x = (int)(hit.point.x - boardOffSet.x);//get pos x
            mouseOver.y = (int)(hit.point.z - boardOffSet.z);//get pos y
        }
        else//if didn't detected board's collider
        {
            mouseOver.x = -1;//
            mouseOver.y = -1;
        }

    }

    void GenerateBorad()//calls when game is loaded from start
    {
        //Generate white_Piece(p1)
        for (int y = 0; y < 3; y++)
        {
            bool oddrow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddrow) ? x : x + 1, y);//generate men at certain position
            }
        }
        //Generate black_Piece(p2)
        for (int y = 7; y > 4; y--)
        {
            bool oddrow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddrow) ? x : x + 1, y);//generate men at certain position
            }
        }

    }

    void GeneratePiece(int x, int y)//generate men at certain position
    {
        bool IsWhite = ((y > 3) ? false : true);//if p1 or p2
        GameObject obj = Instantiate((IsWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;//generate p1 or p2
        obj.transform.SetParent(transform);//set board as this men parent 
        obj.name = gotinumber.ToString();//set men's name
        gotinumber++;
        Piece p = obj.GetComponent<Piece>();//manipulate men in piece array
        pieces[x, y] = p;
        enddragofPiece[(int.Parse(obj.name))] = new Vector2(x, y);//get intial position of men as its final position for records
        MovePiece(p, x, y);//set men in place;

    }

    public void MovePiece(Piece p, int x, int y)//set men at exact pos
    {

        if (isComputersPlaying && iscoumputers_turn)//if this is computers turn
        {       
            Vector3 p_target = (Vector3.right * x) + (Vector3.forward * y) + boardOffSet + pieceOffset;         
            StartCoroutine(MoveOverSpeed(p.transform.gameObject, p_target, 2f));//MoveAiGotti gotti
          
        }
        else //if this is players turn
        {
            p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffSet + pieceOffset;//set position of men
        }

    }

    void Selectpiece(int x, int y)//when some men is selected
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)//if out of board's bound
            return;

        Piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)//if selected p is not null and checking its turn
        {
            if (forcedPieces.Count == 0)//if tehre are no force moves
            {
                selectedpiece = p;
                stratDrag = mouseOver;
            }
            else
            {
                if (forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }

            }
            selectedpiece = p;
            stratDrag = mouseOver;
        }
    }

    void TryMove(int x1, int y1, int x2, int y2)//try to move selected piece form x1,y1 to x2,y2
    {
        forcedPieces = ScanforPossibleMove();//scan for possible moves
        //Debug.Log("should_move a gotti in try");
        //multiplayer_Support
        stratDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedpiece = pieces[x1, y1];
       
        //out of bounds
        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)//if out bounds// for avoiding errors
        {
            if (selectedpiece != null)
            {
                MovePiece(selectedpiece, x1, y1);
            }
            stratDrag = Vector2.zero;
            selectedpiece = null;
            return;
        }

        if (selectedpiece != null)
        {
            //if it has not moved
            if (endDrag == stratDrag)
            {
                MovePiece(selectedpiece, x1, y1);
                stratDrag = Vector2.zero;
                selectedpiece = null;
                return;
            }
            //check if its a valid move
            if (selectedpiece.ValidMove(pieces, x1, y1, x2, y2))
            {

                //did we kill someone
                if (Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        p.gameObject.SetActive(false);
                        if (p.gameObject.tag == "white")
                        {
                            whiteCount--;//p1's men dead 
                            p2_score += 10;

                            p2Score.text = "P2/" + p2_score.ToString();
                        }
                        else
                        {
                            p1_score += 10;
                            p1score.text = "P1/" + p1_score.ToString();
                            blackCount--;//p2's men dead 
                        }
                       // Debug.Log("dead");
                        p.tag = "dead";
                        hasKilled = true;
                    }
                }
                //were we suppose to kill anything??
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    //resetting variables
                    MovePiece(selectedpiece, x1, y1);
                    stratDrag = Vector2.zero;
                    selectedpiece = null;
                    return;
                }
                //
              //  Debug.Log("moving");
                pieces[x2, y2] = selectedpiece;
               
                pieces[x1, y1] = null;
                MovePiece(selectedpiece, x2, y2);//move gotti

                if (!iscoumputers_turn)//if this isnt players turn
                {
                  //  Debug.Log("ending_");
                    EndTurn();//end the player's turn
                }

            }
            else
            {
                //resetting variables
                MovePiece(selectedpiece, x1, y1);
                stratDrag = Vector2.zero;
                selectedpiece = null;
                return;
            }
        }
    }

    void EndTurn()//End turn
    {
        //TurnEnded = true;
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;
        //promotions
        if (selectedpiece != null)
        {
            //  Debug.Log("Name:"+selectedpiece.name);
            enddragofPiece[(int.Parse(selectedpiece.name))] = new Vector2(x, y);
            if (selectedpiece.isWhite && !selectedpiece.isKing && y == 7)
            {
                selectedpiece.isKing = true;
                selectedpiece.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);//turn on crown

            }
            else if (!selectedpiece.isWhite && !selectedpiece.isKing && y == 0)
            {
                selectedpiece.isKing = true;

                selectedpiece.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);//turn on crown

            }
        }
        selectedpiece = null;
        stratDrag = Vector2.zero;
      //  Debug.Log("EndTurn");
        //swapTurn
        if (ScanforPossibleMove(selectedpiece, x, y).Count != 0 && hasKilled)
            return;

       
        hasKilled = false;
        
        isWhiteTurn = !isWhiteTurn;//changing turns
        isWhite = !isWhite;//changing turns

        Turn.transform.parent.gameObject.SetActive(false);//setting turn dialouge
        turn_Number++;

        if (turn_Number % 2 == 0)//p2's Turn
        {
            PlayerPrefs.SetInt("TurnPlayer", 2);
            GameController.instance.isWhiteTurn = false;
           

            if (GameController.instance.SkipBtnWasPressed)//if camera rotation is not allowed//if camera was pressed
            {
                Turn.text = "Player 2's Turn";//turning text on
                Turn.transform.parent.gameObject.SetActive(true);
                GameController.instance.test = false;
                time = 20;//setting timer.
                StopAllCoroutines();
                StartCoroutine(TimeInSeconds());//starting timer.
            }
            if (isComputersPlaying)
            {
                timer.enabled = false;
                iscoumputers_turn = true;
            }
        }
        else//p1's Turn
        {

            PlayerPrefs.SetInt("TurnPlayer", 1);
            GameController.instance.isWhiteTurn = true;
           
            if (GameController.instance.SkipBtnWasPressed)//if camera rotation is not allowed//if camera was pressed 
            {

                GameController.instance.test = true;
                Turn.transform.parent.gameObject.SetActive(true);
                Turn.text = "player 1's Turn";//turning text on
                Invoke("ActiveTips", 1f);
                time = 20;//setting timer.
                StopAllCoroutines();
                StartCoroutine(TimeInSeconds());//starting timer
            }
            if (PlayerPrefs.GetInt("IsComputerPlaying") == 1)
            {
                iscoumputers_turn = false;
                timer.enabled = false;
            }
        }
       // TurnEnded = false;
        if (!GameController.instance.SkipBtnWasPressed)//if camera rotation is allowed
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            GameController.instance.IsRotationallowed = true;
            if (PlayerPrefs.GetInt("IsComputerPlaying") == 0)
            {
                Invoke("SetOnCamera", 0.5f);

            }
            else
            {
                Invoke("SetOnCamera", 0.2f);
            }
        }
        else if (GameController.instance.SkipBtnWasPressed)// if camera rotation is not allowed//if camera was pressed 
        {
            GameController.instance.IsRotationallowed = true;
            Invoke("SetOnCamera", 0.2f);

        }

        SetAnimatorsOfGotti();
          
        CheckVictory();//check if someone wins
    }

    void SetOnCamera()//changing camera
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        if (GameController.instance.SkipBtnWasPressed)
        {
            GameController.instance.IsRotationallowed = false;
            gameObject.GetComponent<BoxCollider>().enabled = true;
            if (isComputersPlaying)
            {
                Ai_CheckAvailableGottis();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("IsComputerPlaying") == 0)
            {
                Invoke("SetCameraFollowing", 4.5f);

            }
            else
            {
                Invoke("SetCameraFollowing", 0.5f);
            }
           

        }

    }

    void CheckVictory()
    {
        // white stands for p1
        // black stands for p2
        if (blackCount == 0 || whiteCount == 0)
        {
            if (blackCount > whiteCount)
            {
                Debug.Log("Team Black won");
                GameController.instance.winningText = "PLAYER 2 WIN !!!";
            }
            else
            {
                Debug.Log("Team white won");
                StopAllCoroutines();
                GameController.instance.winningText = "PLAYER 1 WIN !!!";
            }
            GameController.instance.SetPopUp();

            PlayerPrefs.SetInt("isResumeBtnPressed", 0);
            winpopupScore1.text = "Player1 Score: " + p1_score.ToString();
            winpopupScore2.text = "Player2 Score: " + p2_score.ToString();
            StopEveryThing();
            timer.text = "00:00";
        }

    }

    private List<Piece> ScanforPossibleMove()//look for moves
    {
        forcedPieces = new List<Piece>();
        //check all pieces
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                {
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                    {
                        forcedPieces.Add(pieces[i, j]);
                        SetAnimatorsOfGotti();
                     //   StartCoroutine(BlinkGotti(forcedPieces[0].transform.GetChild(0).gameObject));
                      //  forcedPieces[0].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
                        forcedPieces[0].transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
                    }
                }
            }
        }

        return forcedPieces;
    }

    void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("No Camera Found");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + new Vector3(0, 0.2f, 0);
        }

    }

    private List<Piece> ScanforPossibleMove(Piece p, int x, int y)//look for moves
    {
        forcedPieces = new List<Piece>();
        if (time != 1)
        {
        if (pieces[x, y].IsForceToMove(pieces, x, y))
        {
            SetAnimatorsOfGotti();
            forcedPieces.Add(pieces[x, y]);
            //StartCoroutine(BlinkGotti(forcedPieces[0].transform.GetChild(0).gameObject));
               transform.GetChild(int.Parse(pieces[x, y].gameObject.name)).transform.GetChild(0).gameObject.GetComponent<Animator> ().enabled = true;
        }
        }

        return forcedPieces;

    }

    void SetCameraFollowing()//set cameras
    {
        if (turn_Number % 2 == 0)
        {
            Turn.text = "Player 2's Turn";
            if (isComputersPlaying)
            {
                Ai_CheckAvailableGottis();
            }
            PlayerPrefs.SetInt("TurnPlayer", 2);

        }
        else
        {
            PlayerPrefs.SetInt("TurnPlayer", 1);
           
            Turn.transform.parent.gameObject.SetActive(true);
            Turn.text = "player 1's Turn";
            Invoke("ActiveTips", 1f);
            //GameController.instance.tipsContainer.SetActive(true);

        }
        GameController.instance.IsRotationallowed = false;
        Turn.transform.parent.gameObject.SetActive(true);
        StopAllCoroutines();
        if (!isComputersPlaying)
        {
            time = 20;
            StartCoroutine(TimeInSeconds());
        }
        gameObject.GetComponent<BoxCollider>().enabled = true;


    }

    public void IfSkipBtnPressed()//if camera btn is pressed
    {
        CancelInvoke();

    }

    IEnumerator TimeInSeconds()//timer 
    {

        while (time >= 0)
        {
            if (time < 10)
            {
                timer.text = "00:0" + time;

            }
            else
            {
                timer.text = "00:" + time;
            }
            time--;
            yield return new WaitForSeconds(1f);
        }

    }

    private void OnApplicationQuit()//if user kills the game suddenly
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject obj = gameObject.transform.GetChild(i).gameObject;
            Vector2 piecePos = enddragofPiece[i];
            //Output the status of the GameObject's active state in the console
            int king;
            if (gameObject.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.activeInHierarchy)
            {
                king = 1;
                Debug.Log("was King");
            }
            else
            {
                king = 0;
            }
            PlayerPrefs.SetString("goti" + i + 1, obj.transform.position.x + "," + obj.transform.position.y + "," + obj.transform.position.z + "," + obj.tag + "," + (int)piecePos.x + "," + (int)piecePos.y + "," + king + ";");
            //saving men's data in player prefes for resume functionality
        }
        PlayerPrefs.SetInt("p1_score", p1_score);
        PlayerPrefs.SetInt("p2_score", p2_score);
    }

    public void OnClikResumBtn()//saving records
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject obj = gameObject.transform.GetChild(i).gameObject;
            Vector2 piecePos = enddragofPiece[i];
            //Output the status of the GameObject's active state in the console
            int king;
            if (gameObject.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.activeInHierarchy)
            {
                king = 1;
                Debug.Log("was King");
            }
            else
            {
                king = 0;
            }
            PlayerPrefs.SetString("goti" + i + 1, obj.transform.position.x + "," + obj.transform.position.y + "," + obj.transform.position.z + "," + obj.tag + "," + (int)piecePos.x + "," + (int)piecePos.y + "," + king + ";");

        }
    }

    public void StopEveryThing()
    {
        if (!isComputersPlaying)
        {

        StopAllCoroutines();
        }
        time = 20;
    }

    int number = 0;
    public void SetGottis()//set men's in case of resum
    {
        for (int i = 0; i < 25; i++)
        {
            if (PlayerPrefs.HasKey("goti" + i + 1))
            {

                SetGotti("goti" + i + 1);
                number++;
            }
        }
        //PlayerPrefs.DeleteAll();
    }

    void SetGotti(string gotiName)//set men's data in case of resume btn pressed
    {
        string s = PlayerPrefs.GetString(gotiName);

        int j = 1;
        string first = "";
        string second = "";
        string third = "";
        string fourth = "";
        string xpiecepos = "";
        string ypiecepos = "";
        string was_king = "";
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ',')
            {
                j++;
            }
            else if (s[i] == ';')
            {
                break;
            }
            else
            {
                if (j == 1)
                {
                    first = first + s[i];

                }
                else if (j == 2)
                {
                    second = second + s[i];
                }
                else if (j == 3)
                {
                    third = third + s[i];
                }
                else if (j == 4)
                {
                    fourth = fourth + s[i];

                }
                else if (j == 5)
                {
                    xpiecepos = xpiecepos + s[i];
                }
                else if (j == 6)
                {
                    ypiecepos = ypiecepos + s[i];

                }
                else if (j == 7)
                {
                    was_king = was_king + s[i];

                }
            }

        }
        float x = float.Parse(first, System.Globalization.CultureInfo.InvariantCulture);
        float y = float.Parse(second, System.Globalization.CultureInfo.InvariantCulture);
        float z = float.Parse(third, System.Globalization.CultureInfo.InvariantCulture);
        tag_of_obj = fourth;
        posvector = new Vector3(x, y, z);
        int xvalue = int.Parse(xpiecepos);
        int yvalue = int.Parse(ypiecepos);
        enddragofPiece[number] = new Vector2(xvalue + 0.0f, yvalue + 0.0f);
        if (tag_of_obj == "white")//generating p1's men
        {
            GameObject obj = Instantiate(whitePiecePrefab) as GameObject;
            obj.transform.SetParent(transform);
            Piece p = obj.GetComponent<Piece>();
            obj.transform.position = posvector;

            pieces[xvalue, yvalue] = p;
            MovePiece(p, xvalue, yvalue);
            obj.name = number.ToString();
            whiteCount++;
            if (int.Parse(was_king) == 1)
            {
                obj.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                obj.GetComponent<Piece>().isKing = true;
            }
        }
        else if (tag_of_obj == "black")//generating p2's men
        {
            GameObject obj = Instantiate(blackPiecePrefab) as GameObject;
            obj.transform.SetParent(transform);
            Piece p = obj.GetComponent<Piece>();
            obj.transform.position = posvector;
            pieces[xvalue, yvalue] = p;
            obj.name = number.ToString();
            MovePiece(p, xvalue, yvalue);
            blackCount++;
            if (int.Parse(was_king) == 1)
            {
                obj.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                obj.GetComponent<Piece>().isKing = true;

            }


        }
        p1_score = PlayerPrefs.GetInt("p1_score");
        p1score.text = "P1/" + p1_score.ToString();
        p2_score = PlayerPrefs.GetInt("p2_score");
        p2Score.text = "P2/" + p2_score.ToString();
    }
    
    void Ai_CheckAvailableGottis()
    {
        forcedPieces = ScanforPossibleMove();
        if (forcedPieces.Count != 0) //if forced to move
        {
            Debug.Log("adding force pieces");
            forcedToMoveAiGotti = true;
            string name = forcedPieces[0].name;
            float x = enddragofPiece[(int.Parse(name))].x;
            float y = enddragofPiece[(int.Parse(name))].y;
            movegottiOfIndex = int.Parse(name);
            SetAnimatorsOfGotti();
          //  StartCoroutine(BlinkGotti(forcedPieces[0].transform.GetChild(0).gameObject));
           // forcedPieces[0].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0f);
            transform.GetChild(movegottiOfIndex).transform.GetChild(0).gameObject.GetComponent<Animator> ().enabled = true;
            Invoke("MoveSpecificGottiOfAI_forced", 3);

        }
        else
        {
            //if not forced to move
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).tag == "black" && transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    string name = transform.GetChild(i).name;
                    float x = enddragofPiece[(int.Parse(name))].x;
                    float y = enddragofPiece[(int.Parse(name))].y;
                    GetMoveAbleGottis(transform.GetChild(i).gameObject, x, y);
                }
            }
            if (moveanble_AIGottis.Count < 1)
            {
                StopTheGame();
            }
            ShouldMoveTheseAiGotti();
        }

    }

    void GetMoveAbleGottis(GameObject currentgotti, float xfloat, float yfloat)
    {
        int x = (int)xfloat;
        int y = (int)yfloat;
        selectedpiece = pieces[x, y];
        //check all directions
        bool canMoved = false;

        if (x + 1 < 8 && y - 1 > -1)
        {
            if (selectedpiece.ValidMove(pieces, x, y, x + 1, y - 1)) //check downward
            {
                canMoved = true;
            }
        }

        if (x - 1 > -1 && y - 1 > -1)
        {
            if (selectedpiece.ValidMove(pieces, x, y, x - 1, y - 1))
            {
                canMoved = true;
            }
        }

        if (currentgotti.gameObject.GetComponent<Piece>().isKing)//check upward only if it is king
        {
            if (x + 1 < 8 && y + 1 < 8)
            {
                if (selectedpiece.ValidMove(pieces, x, y, x + 1, y + 1))
                {
                    canMoved = true;
                }
            }
            if (x - 1 > -1 && y + 1 < 8)
            {
                if (selectedpiece.ValidMove(pieces, x, y, x - 1, y + 1))
                {
                    canMoved = true;
                }
            }
        }

        if (canMoved)
        {
            //add into movable gottis ai

           // Debug.Log(currentgotti.name);
            moveanble_AIGottis.Add(currentgotti);

        }
    }

    void ShouldMoveTheseAiGotti()
    {
        int x;
        int y;
        int max_safe = 0;
        for (int i = 0; i < moveanble_AIGottis.Count; i++)
        {
            x = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[i].name))].x);
            y = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[i].name))].y);

         
            if (x + 2 < 8 && y - 2 > -1) //check bounds
            {
                Piece p1 = pieces[x + 2, y - 2];
                Piece p_ = pieces[x + 1, y - 1];
                
                    if (p1 == null && p_ != isWhite)
                {
                    max_safe++;
                }
            }

            if (x - 2 > -1 && y - 2 > -1)
            {
                Piece p2 = pieces[x - 2, y - 2];
                Piece p_ = pieces[x - 1, y - 1];
                if (p2 == null && p_ != isWhite)
                {
                    max_safe++;
                }
            }

           

            if (moveanble_AIGottis[i].gameObject.GetComponent<Piece>().isKing)//check upward only if it is king
            {
                if (x + 2 < 8 && y + 2 < 8)
                {
                    Piece p = pieces[x + 2, y + 2];
                    Piece p_ = pieces[x + 1, y + 1];
                    if (p == null && p_ != isWhite)
                    {
                        max_safe++;
                    }
                }

                if (x - 2 > -1 && y + 2 < 8)
                {
                    Piece p3 = pieces[x - 2, y + 2];
                    Piece p_ = pieces[x - 1, y + 1];
                    if (p3 == null && p_ != isWhite)
                    {
                        max_safe++;
                    }
                }
            }

            if (y - 2 > -1)
            {
                Piece p1 = pieces[x, y - 2];
                if (p1!=null && p1 == isWhite)
                {
                max_safe = 0;
                }
               
            }
            if (y + 2 < 8)
            {
                Piece p1 = pieces[x, y + 2];
                if (p1 != null && p1 == isWhite)
                {
                    max_safe = 0;
                }
            }


            max_safeMoves.Add(max_safe);
            max_safe = 0;
        }

        for (int i = 1; i < max_safeMoves.Count; i++) //get safest moveable Ai gotti index
        {
            movegottiOfIndex = max_safeMoves[0];
            if (max_safeMoves.Count > 0 && moveanble_AIGottis.Count>1)
            {
                if (DontMoveThisGotti == max_safeMoves[i])
                {
                    max_safeMoves[i] = max_safeMoves[i] - 1;
                }

                if (max_safeMoves[i] > movegottiOfIndex)
                {

                    movegottiOfIndex = i;
                }
            }
           
        }


        Invoke("MoveSpecificGottiOfAI", 4f);
        max_safeMoves.Clear();

    }

    void MoveSpecificGottiOfAI() 
    {
        int index = movegottiOfIndex;
        int Start_x = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].x);
        int Start_y = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].y);
      //  Debug.Log("startx" + Start_x + " -starty" + Start_y);
        
        Piece p_ = pieces[Start_x, Start_y];
       // UpdatePieceDrag(p_);
       // Selectpiece(Start_x, Start_y);
        int end_x = -1;
        int end_y = -1;
        int factor = 1;

        if (Start_x + 2 > 7 || Start_y - 2 < 0 || Start_x - 2 < 0 || Start_y + 2 > 7)//check out of bounds
        {
            factor = 1;
        }
        else
        {
            factor = 2;
        }

        if (Start_x + factor < 8 && Start_y - factor > -1) //check left downward
        {
            Piece p1 = pieces[Start_x + factor, Start_y - factor];
            if ((p1 == null || (factor == 2 && p1 != isWhite)))
            {
                end_x = Start_x + 1;
                end_y = Start_y - 1;

            }

        }
        if (Start_x - factor > -1 && Start_y - factor > -1)//check right downward
        {

            Piece p2 = pieces[Start_x - factor, Start_y - factor];
            if (p2 == null || (factor == 2 && p2 != isWhite))
            {

                end_x = Start_x - 1;
                end_y = Start_y - 1;

            }
        }


        if (moveanble_AIGottis[index].gameObject.GetComponent<Piece>().isKing)//check upward only if it is king
        {
            if (Start_x + factor < 8 && Start_y + factor < 8)//check left upward
            {
                Piece p = pieces[Start_x + factor, Start_y + factor];
                if (p == null || (factor == 2 && p != isWhite))
                {

                    end_x = Start_x + 1;
                    end_y = Start_y + 1;
                }
            }

            if (Start_x - factor > -1 && Start_y + factor < 8)//check right upward
            {
                Piece p3 = pieces[Start_x - factor, Start_y + factor];
                if (p3 == null || (factor == 2 && p3 != isWhite))
                {

                    end_x = Start_x - 1;
                    end_y = Start_y + 1;


                }
            }
        }



        //Selectpiece(Start_x, Start_y);
        //Debug.Log("startx: " + Start_x);
        //Debug.Log("starty: " + Start_y);
        //Debug.Log("end_x: " + end_x);
        //Debug.Log("end_y: " + end_y);
        Piece pCheck = pieces[end_x, end_y];
        if (max_safeMoves.Count > 2) //double check for safe moves
        {
           if (pCheck != null || factor == 1 || DontMoveThisGotti != -1)
            {
                //   Debug.Log("checkinggotti again");
                if (end_x > Start_x)
                {
                    if (end_x + 1 < 8 && end_y - 1 < -1)//checking donwleft
                    {
                        Piece p_1 = pieces[end_x + 1, end_y - 1];
                        if (!p_1.isWhite)
                        {
                            //   Debug.Log("calllinf again");
                            ShouldMoveTheseAiGotti();
                        }
                    }
                }

                if (end_x < Start_x)
                {
                    if (end_x - 1 > -1 && end_y - 1 < -1)//checking donwright
                    {
                        Piece p_1 = pieces[end_x - 1, end_y - 1];
                        if (!p_1.isWhite)
                        {
                            //   Debug.Log("calllinf again");
                            ShouldMoveTheseAiGotti();
                        }
                    }
                }


                if (end_y > Start_y)
                {
                    if (end_x + 1 < 8 && end_y + 1 > 8)//checking donwleft
                    {
                        Piece p_1 = pieces[end_x + 1, end_y + 1];
                        if (!p_1.isWhite)
                        {
                            //   Debug.Log("calllinf again");
                            ShouldMoveTheseAiGotti();
                        }
                    }
                }

                if (end_x < Start_x)
                {
                    if (end_x - 1 > -1 && end_y - 1 > 8)//checking donwright
                    {
                        Piece p_1 = pieces[end_x - 1, end_y + 1];
                        if (!p_1.isWhite)
                        {
                            //   Debug.Log("calllinf again");
                            ShouldMoveTheseAiGotti();
                        }
                    }
                }

            }
        }

        TryMove(Start_x, Start_y, end_x, end_y);

        //Debug.Log("should_move a gotti");
        moveanble_AIGottis.Clear();
        DontMoveThisGotti = -1;
        index = 0;
    }

    void MoveSpecificGottiOfAI_forced()
    {
        int index = movegottiOfIndex;
        int Start_x = (int)(enddragofPiece[index].x);
        int Start_y = (int)(enddragofPiece[index].y);
        Piece p_ = pieces[Start_x, Start_y];
        //UpdatePieceDrag(p_);
        //Selectpiece(Start_x, Start_y);
        int end_x = -1;
        int end_y = -1;

        if (Start_x + 2 < 8 && Start_y - 2 > -1) //check left side downward
        {
            Piece p1 = pieces[Start_x + 2, Start_y - 2];
            Piece p_0 = pieces[Start_x + 1, Start_y - 1];

            if (p1 == null && p_0 != isWhite)
            {
                end_x = Start_x + 2;
                end_y = Start_y - 2;
            }
            Debug.Log("1there");
        }

        if (Start_x - 2 > -1 && Start_y - 2 > -1) //check right side downward
        {
            Piece p2 = pieces[Start_x - 2, Start_y - 2];

            Piece p_0 = pieces[Start_x - 1, Start_y - 1];
            if (p2 == null && p_0 != isWhite)
            {
                end_x = Start_x - 2;
                end_y = Start_y - 2;
            }
            Debug.Log("2there");
        }


        if (transform.GetChild(index).gameObject.GetComponent<Piece>().isKing)//check upward only if it is king
        {
            if (Start_x + 2 < 8 && Start_y + 2 < 8) //check left side upward
            {
                Piece p = pieces[Start_x + 2, Start_y + 2];
                Piece p_0 = pieces[Start_x + 1, Start_y + 1];
                if (p == null && p_0 != isWhite)
                {
                    end_x = Start_x + 2;
                    end_y = Start_y + 2;
                }
                Debug.Log("there3");
            }

            if (Start_x - 2 > -1 && Start_y + 2 < 8) //check right side upward
            {
                Piece p3 = pieces[Start_x - 2, Start_y + 2];

                Piece p_0 = pieces[Start_x - 1, Start_y + 1];
                if (p3 == null && p_0 != isWhite)
                {
                    end_x = Start_x - 2;
                    end_y = Start_y + 2;
                }
                Debug.Log("there4");
            }
        }
        Debug.Log("startx: " + Start_x);
        Debug.Log("starty: " + Start_y);
        Debug.Log("end_x: " + end_x);
        Debug.Log("end_y: " + end_y);
       
        TryMove(Start_x, Start_y, end_x, end_y);
        moveanble_AIGottis.Clear();
        //if (forcedPieces.Count != 0)
        //{
        //    string name = forcedPieces[0].name;
        //    movegottiOfIndex = int.Parse(name);
        //    MoveSpecificGottiOfAI_forced();
        //}
        Invoke("CheckDoubleCrossGotti", 2f);
    }
    
    void CheckDoubleCrossGotti() //if Ai can kill double gottis
    {
        if (forcedPieces.Count != 0)
        {
            string name = forcedPieces[0].name;
            movegottiOfIndex = int.Parse(name);
            MoveSpecificGottiOfAI_forced();
        }
    }
    
    void StopTheGame() //if no more valid moves are left
    {
        blackCount = 0;
        CheckVictory();
    }
    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed) //moving ai gotti
    {
        //GameObject  obj= objectToMove;
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {

            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        EndTurn();
    }
    void ActiveTips() //active tip container
    {
        GameController.instance.tipsContainer.SetActive(true);
    }

    void SetAnimatorsOfGotti()
    {
         for (int i = 0; i < transform.childCount; i++)
            {
            //if (transform.GetChild(i).tag=="white")
            //whitePiecePrefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial.color = colors[PlayerPrefs.GetInt("Name1")];
            //else
            //blackPiecePrefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial.color = colors[PlayerPrefs.GetInt("Name2")];
            transform.GetChild(i).transform.GetChild(0).GetComponent<Animator>().enabled = false;
            }
       
    }

    //public IEnumerator BlinkGotti(GameObject obj) //moving ai gotti
    //{
    //    //GameObject  obj= objectToMove;
    //    // speed should be 1 unit per second
    //    Color originalColor = obj.gameObject.GetComponent<MeshRenderer>().material.color;
    //    Color changeColor = new Color(1f, 1f, 1f, 0f);
    //    bool isoriginal=true;
    //    while (!TurnEnded)
    //    {
    //        if (isoriginal)
    //        {
    //            obj.gameObject.GetComponent<MeshRenderer>().material.color = changeColor;
    //            isoriginal = false;
    //        }
    //        else
    //        {
    //        obj.gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
    //        isoriginal = true;

    //        }

    //        yield return new WaitForSeconds(0.07f);
    //    }
    //    SetAnimatorsOfGotti();
    //    //obj.gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
    //    //objectToMove.transform.position = end;

    //}


    //    enum Direction {UpLeft, UpRight,DownLeft, DownRight};


    //    Direction GottiDirection;




    //Direction GiveSafestDirectionToGotti(Direction dir)
    //{
    //    List<string> AvailableDirForThisGotti = new List<string>(); 
    //    GottiDirection = Direction.UpLeft;
    //    int index = movegottiOfIndex;
    //    int Start_x = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].x);
    //    int Start_y = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].y);
    //    Debug.Log("startx" + Start_x + " -starty" + Start_y);
    //    //
    //    //checkBoundires



    //            if (Start_y == 0)//if at top row
    //            {
    //                  if (Start_x == 0)
    //                  {
    //                    GottiDirection = Direction.UpLeft;//must +
    //                  }
    //                  else if (Start_x == 7)
    //                  {
    //                    GottiDirection = Direction.UpRight;//must +
    //                  }
    //                  else
    //                  {
    //                    Piece p_firstcheck = pieces[Start_x, Start_y];
    //                    if(Start_y)
    //                  }
    //            AvailableDirForThisGotti.Add(GottiDirection.ToString());
    //            }
    //            else if (Start_y == 7)
    //            {
    //                 if (Start_x == 0)
    //                 {
    //                     GottiDirection = Direction.DownLeft;//must -
    //        }
    //                 else if (Start_x == 7)
    //                 {
    //                     GottiDirection = Direction.DownRight;//must -
    //        }
    //            }



    //    if (moveanble_AIGottis[index].gameObject.GetComponent<Piece>().isKing)
    //    {
    //        if (Start_y == 0)//if at top row
    //        {
    //            GottiDirection = Direction.DownLeft;
    //        }
    //        else
    //        {
    //            if (Start_y == 7)//if at bottom row
    //            {
    //                GottiDirection = Direction.UpLeft;
    //            }
    //        }

    //    }//check both downward and upward
    //    else
    //    {

    //        if (Start_y == 7)//if at bottom row
    //        {
    //            GottiDirection = Direction.UpLeft;

    //        }
    //    }







    //    if (Start_x + factor < 8 && Start_y - factor > -1) //check left downward
    //    {
    //        Piece p1 = pieces[Start_x + factor, Start_y - factor];
    //        if ((p1 == null || (factor == 2 && p1 != isWhite)))
    //        {
    //            end_x = Start_x + 1;
    //            end_y = Start_y - 1;

    //        }

    //    }
    //    if (Start_x - factor > -1 && Start_y - factor > -1)//check right downward
    //    {

    //        Piece p2 = pieces[Start_x - factor, Start_y - factor];
    //        if (p2 == null || (factor == 2 && p2 != isWhite))
    //        {

    //            end_x = Start_x - 1;
    //            end_y = Start_y - 1;

    //        }
    //    }


    //    if (moveanble_AIGottis[index].gameObject.GetComponent<Piece>().isKing)//check upward only if it is king
    //    {
    //        if (Start_x + factor < 8 && Start_y + factor < 8)//check left upward
    //        {
    //            Piece p = pieces[Start_x + factor, Start_y + factor];
    //            if (p == null || (factor == 2 && p != isWhite))
    //            {

    //                end_x = Start_x + 1;
    //                end_y = Start_y + 1;
    //            }
    //        }

    //        if (Start_x - factor > -1 && Start_y + factor < 8)//check right upward
    //        {
    //            Piece p3 = pieces[Start_x - factor, Start_y + factor];
    //            if (p3 == null || (factor == 2 && p3 != isWhite))
    //            {

    //                end_x = Start_x - 1;
    //                end_y = Start_y + 1;


    //            }
    //        }
    //    }

    //    dir = Direction.UpLeft;


    //    return dir;
    //}

    //void MoveSpecificGottiOfAI_test()
    //{
    //    GottiDirection = Direction.UpLeft;
    //    int index = movegottiOfIndex;
    //    int Start_x = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].x);
    //    int Start_y = (int)(enddragofPiece[(int.Parse(moveanble_AIGottis[movegottiOfIndex].name))].y);
    //    Debug.Log("startx" + Start_x + " -starty" + Start_y);

    //    Piece p_ = pieces[Start_x, Start_y];
    //    // UpdatePieceDrag(p_);
    //    // Selectpiece(Start_x, Start_y);
    //    int end_x = -1;
    //    int end_y = -1;
    //    int factor = 1;

    //    if (Start_x + 2 > 7 || Start_y - 2 < 0 || Start_x - 2 < 0 || Start_y + 2 > 7)//check out of bounds
    //    {
    //        factor = 1;
    //    }
    //    else
    //    {
    //        factor = 2;
    //    }




    //    //Selectpiece(Start_x, Start_y);
    //    //Debug.Log("startx: " + Start_x);
    //    //Debug.Log("starty: " + Start_y);
    //    //Debug.Log("end_x: " + end_x);
    //    //Debug.Log("end_y: " + end_y);
    //    Piece pCheck = pieces[end_x, end_y];
    //    if (max_safeMoves.Count > 0) //double check for safe moves
    //    {
    //        if (pCheck != null || factor == 1 || DontMoveThisGotti != -1)
    //        {
    //            //   Debug.Log("checkinggotti again");
    //            if (end_x > Start_x)
    //            {
    //                if (end_x + 1 < 8 && end_y - 1 < -1)//checking donwleft
    //                {
    //                    Piece p_1 = pieces[end_x + 1, end_y - 1];
    //                    if (!p_1.isWhite)
    //                    {
    //                        //   Debug.Log("calllinf again");
    //                        ShouldMoveTheseAiGotti();
    //                    }
    //                }
    //            }

    //            if (end_x < Start_x)
    //            {
    //                if (end_x - 1 > -1 && end_y - 1 < -1)//checking donwright
    //                {
    //                    Piece p_1 = pieces[end_x - 1, end_y - 1];
    //                    if (!p_1.isWhite)
    //                    {
    //                        //   Debug.Log("calllinf again");
    //                        ShouldMoveTheseAiGotti();
    //                    }
    //                }
    //            }


    //            if (end_y > Start_y)
    //            {
    //                if (end_x + 1 < 8 && end_y + 1 > 8)//checking donwleft
    //                {
    //                    Piece p_1 = pieces[end_x + 1, end_y + 1];
    //                    if (!p_1.isWhite)
    //                    {
    //                        //   Debug.Log("calllinf again");
    //                        ShouldMoveTheseAiGotti();
    //                    }
    //                }
    //            }

    //            if (end_x < Start_x)
    //            {
    //                if (end_x - 1 > -1 && end_y - 1 > 8)//checking donwright
    //                {
    //                    Piece p_1 = pieces[end_x - 1, end_y + 1];
    //                    if (!p_1.isWhite)
    //                    {
    //                        //   Debug.Log("calllinf again");
    //                        ShouldMoveTheseAiGotti();
    //                    }
    //                }
    //            }

    //        }
    //    }

    //    TryMove(Start_x, Start_y, end_x, end_y);

    //    //Debug.Log("should_move a gotti");
    //    moveanble_AIGottis.Clear();
    //    DontMoveThisGotti = -1;
    //    index = 0;
    //}
}

