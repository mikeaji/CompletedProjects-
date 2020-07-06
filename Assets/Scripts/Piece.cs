using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    //class to discribe men (piece) behaviour
    public bool isWhite;//bool p1's or p2's men
    public bool isKing;//bool king 
    public static Piece instance;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

    }

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)//checking move is valid or not
    {
        if (board[x2, y2] != null)      //if enddragis null
            return false;
        
        int deltaMove = Mathf.Abs(x1 - x2);//difference btw start and end drag in x
        int deltaMovey = y2 - y1;//difference btw start and end drag in y
        if (isWhite || isKing)//for p1
        {
            if (deltaMove == 1)
            {//simpleJump
                if (deltaMovey == 1)
                    return true;
            }
            else if (deltaMove == 2)
            {//killjump
                if (deltaMovey == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                   
                }
            }
        }


        if (!isWhite || isKing)//for p2
        {
            if (deltaMove == 1)
            {
                if (deltaMovey == -1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMovey == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                       return true;
                    
                }
            }
        }
        return false;
    }
        
    public bool IsForceToMove(Piece[,]board,int x ,int y)//if opponent's men can be killed so this move is only valid
    {
        if (isWhite || isKing)
        {
            //top left
            if (x >= 2 && y <= 5)
            {
                Piece p = board[x - 1, y + 1];
                //if there is a piece and it is not the same color as urs
                if (p != null && p.isWhite != isWhite)
                {
                    //check if its possible to land after the jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
            //top right
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];
                //if there is a piece and it is not the same color as urs
                if (p != null && p.isWhite != isWhite)
                {
                    //check if its possible to land after the jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if(!isWhite || isKing)
        {
            //bottom left
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                //if there is a piece and it is not the same color as urs
                if (p != null && p.isWhite != isWhite)
                {
                    //check if its possible to land after the jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
            //bottom right
            if (x <= 5 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                //if there is a piece and it is not the same color as urs
                if (p != null && p.isWhite != isWhite)
                {
                    //check if its possible to land after the jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;        
    }
}
