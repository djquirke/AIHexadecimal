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
	//Use this list to store all available moves for the current turn
	//Needs to be cleared before the next turn
	List<Vector2> avail_moves;

	Team currentGo = Team.BLACK;

	public const int width = 8, height = 8;
	private int blackCount = 0, whiteCount = 0;
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
                if (board[i][j].Occupied())
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
        if (board[x][y].Occupied())
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
                if (board[x + i][y + j].Occupied() && OpposingCounter(board[x + i][y + j].Team(), team))
                {
                    enemyFound = true;
                }
            }
        }
        if (enemyFound == false)
            return false;

        return true;
    }

	//check if any moves are available for current player
	bool IsMoveAvailable()
	{
		bool ret = false;

		//loop through all grid spaces
		for (int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				//if tile is occupied - ignore
				if(board[i][j].Occupied())
					continue;
				//check if the tile could be taken by current player
				else
				{
					if(IsValidMove(i, j))
					{
						ret = true;
						break;
					}
				}
			}
		}

		return ret;
	}

	//Check if 2 counters are on the same team
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
		//make the move



		SwitchPlayerTurn();
        return false;
    }

	void SwitchPlayerTurn()
	{
		if (currentGo == Team.BLACK)
			currentGo = Team.WHITE;
		else
			currentGo = Team.BLACK;
	}

    void ShowValidMoves(bool team)
    {

    }

    //checks for game terminating conditions
    bool CheckEndGame()
    {
        //check if any moves available for current players go
		if (!IsMoveAvailable ())
			return true;


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //if not a space is not occupied then board is not full
                if (!board[i][j].Occupied())
				{
					return false;
				}
            }
        }

		return true;
    }

    bool EnemyInDirection()
    {
        return false;
    }
}
