using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Tile
{
    public bool team;
    public bool occupied;

    //public Tile()
    //{
    //   team = false;
    //   occupied = false;
    //}
    public Tile(bool team)
    {
        this.team = team;
        occupied = true;
    }
}

public class GridManager : MonoBehaviour
{

    List<List<Tile>> board = new List<List<Tile>>();
    public GameObject tileHighlight;
    public GameObject counterInst;

    public int length = 8, width = 8;
    Tile white = new Tile(false);
    Tile black = new Tile(true);

    // Use this for initialization
    void Start()
    {
        CreateBoard();
        SetUpBoard();
        UpdateBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateBoard()
    {
        for (int i = 0; i < length; i++)
        {
            List<Tile> tempList = new List<Tile>();
            for (int j = 0; j < width; j++)
            {
                Tile tempTile = new Tile();
                tempTile.team = false;
                tempTile.occupied = false;
                tempList.Add(tempTile);
            }
            board.Add(tempList);
        }
    }

    void SetUpBoard()
    {
        int wsmaller = (width / 2) - 1;
        int wlarger = (width / 2);
        int lsmaller = (length / 2) - 1;
        int llarger = (length / 2);
        board[wsmaller][lsmaller] = white;
        board[wlarger][llarger] = white;
        board[wsmaller][llarger] = black;
        board[wlarger][lsmaller] = black;
    }

    void UpdateBoard()
    {
        foreach (var counter in GameObject.FindGameObjectsWithTag("Counter"))
        {
            GameObject.Destroy(counter);
        }

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i][j].occupied == true)
                {
                    //Set tile location
                    float locX = i - (length - 1) / 2.0f;
                    float locY = j - (width - 1) / 2.0f;
                    //Scale up location
                    locX *= 10.0f;
                    locY *= 10.0f;
                    GameObject counter = (GameObject)Instantiate(counterInst, new Vector3(locX, 1.5f, locY), Quaternion.Euler(Vector3.zero));
                    if (board[i][j].team)
                    {
                        counter.transform.rotation = Quaternion.Euler(new Vector3(180.0f, 0.0f, 0.0f));
                    }
                }
            }
        }
    }

    bool IsValidMove(int x, int y, bool team)
    {

        return false;
    }

    bool MakeMove(int x, int y, bool team)
    {
        return false;
    }

    void ShowValidMoves(bool team)
    {

    }

    bool CheckVictory(bool team)
    {
        return false;
    }
}
