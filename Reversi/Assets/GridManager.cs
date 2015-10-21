using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum Team
{
    BLACK,
    WHITE,
    NONE
}

struct Tile
{
    Team team;
    bool occupied;

    //public Tile()
    //{
    //   team = false;
    //   occupied = false;
    //}

    public Tile(Team team, bool occupied)
    {
        this.team = team;
        this.occupied = occupied;
    }

    public Team Team()
    {
        return team;
    }

    public void Team(Team team)
    {
        this.team = team;
    }

    public bool Occupied()
    {
        return occupied;
    }
    
    public void Occupied(bool occupied)
    {
        this.occupied = occupied;
    }
}

public class GridManager : MonoBehaviour
{
    enum Direction
	{
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
	};

    List<List<Tile>> board = new List<List<Tile>>();
    public GameObject tileHighlight;
    public GameObject counterInst;

    public int width = 8, height = 8, blackCount = 0, whiteCount = 0;
    Tile white = new Tile(Team.WHITE, true);
    Tile black = new Tile(Team.BLACK, true);

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
        if (CheckEndGame())
        {
            //display win/lose screen
        }

        Debug.Log("Black tiles: " + blackCount + ", white tiles " + whiteCount + ".");
        UpdateBoard();
    }

    void CreateBoard()
    {
        for (int i = 0; i < height; i++)
        {
            List<Tile> tempList = new List<Tile>();
            for (int j = 0; j < width; j++)
            {
                Tile tempTile = new Tile(Team.NONE, false);
                tempList.Add(tempTile);
            }
            board.Add(tempList);
        }
    }

    void SetUpBoard()
    {
        int wsmaller = (width / 2) - 1;
        int wlarger = (width / 2);
        int lsmaller = (height / 2) - 1;
        int llarger = (height / 2);
        board[wsmaller][lsmaller] = white;
        board[wlarger][llarger] = white;
        board[wsmaller][llarger] = black;
        board[wlarger][lsmaller] = black;
        whiteCount += 2;
        blackCount += 2;
    }

    void UpdateBoard()
    {
        foreach (var counter in GameObject.FindGameObjectsWithTag("Counter"))
        {
            GameObject.Destroy(counter);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (board[i][j].Occupied() == true)
                {
                    //Set tile location
                    float locX = i - (height - 1) / 2.0f;
                    float locY = j - (width - 1) / 2.0f;
                    //Scale up location
                    locX *= 10.0f;
                    locY *= 10.0f;
                    GameObject counter = (GameObject)Instantiate(counterInst, new Vector3(locX, 1.5f, locY), Quaternion.Euler(Vector3.zero));
                    if (board[i][j].Team() == Team.BLACK)
                    {
                        counter.transform.rotation = Quaternion.Euler(new Vector3(180.0f, 0.0f, 0.0f));
                    }
                }
            }
        }
    }

    bool IsValidMove(int x, int y, Team team)
    {
        if (x >= width || y >= height || x < 0 || y < 0)
            return false;
        if (board[x][y].Occupied() == true)
            return false;
        bool enemyFound = false;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 & j == 0)
                {
                    continue;
                }
                if (board[x + i][y + j].Occupied() == true && OpposingCounter(board[x + i][y + j].Team(), team))
                {
                    enemyFound = true;
                }
            }
        }
        if (enemyFound == false)
            return false;

        return true;
    }

    bool OpposingCounter(Team first, Team second)
    {
        if ((first == Team.WHITE && second == Team.BLACK) || (first == Team.BLACK && second == Team.WHITE))
        {
            return true;
        }
        return false;
    }

    bool MakeMove(int x, int y, bool team)
    {

        return false;
    }

    void ShowValidMoves(bool team)
    {

    }

    //checks for game terminating conditions
    bool CheckEndGame()
    {
        bool ret = false;

        //check if any moves available for current players go
        //if moves are available - ret = false
        //no moves - ret = true

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //if not a space is not occupied then board is not full
                if (board[i][j].Occupied() == false)
                    ret = false;
            }
        }

        return ret;
    }

    bool EnemyInDirection()
    {
        return false;
    }
}
