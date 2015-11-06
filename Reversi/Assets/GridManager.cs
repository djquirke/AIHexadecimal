using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Team
{
    BLACK,
    WHITE,
    NONE
}
public class Tile
{
    public Team team { get; set; }
    public bool occupied { get; set; }

    public Tile(Team team, bool occupied)
    {
        this.team = team;
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

    Team playerTeam = Team.BLACK;
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
        ShowValidMoves(currentGo);
    }

    // Update is called once per frame
    void Update()
    {
        //if (CheckEndGame())
        //{
        //    //display win/lose screen
        //}

        //Debug.Log("Black tiles: " + blackCount + ", white tiles " + whiteCount + ".");
        //UpdateBoard();
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
                if (board[i][j].occupied)
                {
                    //Set tile location
                    float locX = i - (height - 1) / 2.0f;
                    float locY = j - (width - 1) / 2.0f;
                    //Scale up location
                    locX *= 10.0f;
                    locY *= 10.0f;
                    GameObject counter = (GameObject)Instantiate(counterInst, new Vector3(locX, 1.5f, locY), Quaternion.Euler(Vector3.zero));
                    if (board[i][j].team == Team.BLACK)
                    {
                        counter.transform.rotation = Quaternion.Euler(new Vector3(180.0f, 0.0f, 0.0f));
                    }
                    else
                    {
                        Debug.Log("WHITE FOUND");
                    }
                }
            }
        }
    }

    void NewTurn()
    {
        SwitchPlayerTurn();
        UpdateBoard();
        if(!IsMoveAvailable(currentGo))
        {
            Debug.Log("WINNER");
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Marker"))
        {
            Destroy(item);
        }
        ShowValidMoves(currentGo);

    }

    bool IsValidMove(int x, int y, Team team)
    {
        //Return false if out of bounds
        if (x >= width || y >= width || x < 0 || y < 0)
            return false;
        //Return false if tile is occupied
        if (board[x][y].occupied == true)
            return false;

        //Get a list of directions that there is an enemy in
        bool enemyFound = false;
        List<Direction> enemyDirections = new List<Direction>();
        for (int i = 0; i < 10; i++)
        {
            if (EnemyInDirection((Direction)i, team, x, y))
            {
                enemyFound = true;
                enemyDirections.Add((Direction)i);
            }
        }

        //Check if a valid move could be made from every direction there is an enemy
        bool validMove = false;
        if (enemyFound == false)
            return false;
        else
            foreach (var direction in enemyDirections)
            {
                if (ValidMoveRecursion(x, y, team, direction))
                {
                    validMove = true;
                }
            }
        return validMove;
    }

    bool ValidMoveRecursion(int x, int y, Team team, Direction direction)
    {
        //Generate new coordinates based on direction
        int newX = GetCoordinatesFromDirection(direction)[0] + x;
        int newY = GetCoordinatesFromDirection(direction)[1] + y;

        //Call the function again if there is an enemy
        if (EnemyInDirection(direction, team, x, y))
        {
            return ValidMoveRecursion(newX, newY, team, direction);
        }

        //If there isn't an enemy in the tile, run the checks to see if it's a valid move
        if (newX < 0 || newX >= width || newY < 0 || newY >= height)
        {
            return false;
        }
        if (board[newX][newY].team == team)
        {
            return true;

        }
        return false;
    }
    //check if any moves are available for current player
    bool IsMoveAvailable(Team team)
    {
        bool ret = false;

        //loop through all grid spaces
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if tile is occupied - ignore
                if (board[i][j].occupied)
                    continue;
                //check if the tile could be taken by current player
                else
                {
                    if (IsValidMove(i, j, team))
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

    void SwitchPlayerTurn()
    {
        if (currentGo == Team.BLACK)
            currentGo = Team.WHITE;
        else
            currentGo = Team.BLACK;
    }

    void ShowValidMoves(Team team)
    {

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (IsValidMove(i, j, team))
                {
                    //Set tile location
                    float locX = i - (height - 1) / 2.0f;
                    float locY = j - (width - 1) / 2.0f;
                    //Scale up location
                    locX *= 10.0f;
                    locY *= 10.0f;
                    GameObject counter = (GameObject)Instantiate(tileHighlight, new Vector3(locX, 1.5f, locY), tileHighlight.transform.rotation);
                    counter.GetComponent<MoveMarkerManager>().x = i;
                    counter.GetComponent<MoveMarkerManager>().y = j;
                }
            }
        }
    }

    //checks for game terminating conditions
    bool CheckEndGame()
    {
        //check if any moves available for current players go
        if (!IsMoveAvailable(currentGo))
            return true;


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //if not a space is not occupied then board is not full
                if (!board[i][j].occupied)
                {
                    return false;
                }
            }
        }

        return true;
    }

    static int[] GetCoordinatesFromDirection(Direction direction)
    {
        int[] coordinates = new int[2] { 0, 0 };
        switch (direction)
        {
            case Direction.North:
                coordinates = new int[] { 0, 1 };
                break;
            case Direction.NorthEast:
                coordinates = new int[] { 1, 1 };
                break;
            case Direction.East:
                coordinates = new int[] { 1, 0 };
                break;
            case Direction.SouthEast:
                coordinates = new int[] { 1, -1 };
                break;
            case Direction.South:
                coordinates = new int[] { 0, -1 };
                break;
            case Direction.SouthWest:
                coordinates = new int[] { -1, -1 };
                break;
            case Direction.West:
                coordinates = new int[] { -1, 0 };
                break;
            case Direction.NorthWest:
                coordinates = new int[] { -1, 1 };
                break;
            default:
                break;
        }
        return coordinates;
    }

    bool EnemyInDirection(Direction direction, Team team, int x, int y)
    {
        switch (direction)
        {
            case Direction.North:
                if (y >= height - 1)
                {
                    return false;
                }
                if (board[x][y + 1].occupied == true && board[x][y + 1].team != team)
                {
                    return true;
                }
                break;
            case Direction.NorthEast:
                if (y >= height - 1 || x >= width - 1)
                {
                    return false;
                }
                if (board[x + 1][y + 1].occupied == true && board[x + 1][y + 1].team != team)
                {
                    return true;
                }
                break;
            case Direction.East:
                if (x >= width - 1)
                {
                    return false;
                }
                if (board[x + 1][y].occupied == true && board[x + 1][y].team != team)
                {
                    return true;
                }
                break;
            case Direction.SouthEast:
                if (y <= 0 || x >= width - 1)
                {
                    return false;
                }
                if (board[x + 1][y - 1].occupied == true && board[x + 1][y - 1].team != team)
                {
                    return true;
                }
                break;
            case Direction.South:
                if (y <= 0)
                {
                    return false;
                }
                if (board[x][y - 1].occupied == true && board[x][y - 1].team != team)
                {
                    return true;
                }
                break;
            case Direction.SouthWest:
                if (y <= 0 || x <= 0)
                {
                    return false;
                }
                if (board[x - 1][y - 1].occupied == true && board[x - 1][y - 1].team != team)
                {
                    return true;
                }
                break;
            case Direction.West:
                if (x <= 0)
                {
                    return false;
                }
                if (board[x - 1][y].occupied == true && board[x - 1][y].team != team)
                {
                    return true;
                }
                break;
            case Direction.NorthWest:
                if (y >= height - 1 || x <= 0)
                {
                    return false;
                }
                if (board[x - 1][y + 1].occupied == true && board[x - 1][y + 1].team != team)
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }

    bool SetTilesOnCapture(int x, int y, Team team, Direction direction)
    {
        //Generate new coordinates based on direction
        int newX = GetCoordinatesFromDirection(direction)[0] + x;
        int newY = GetCoordinatesFromDirection(direction)[1] + y;
        //If the direction contains a friendly tile, return that a move can be made in that direction
        if(board[newX][newY].team == team)
        {
            return true;
        }
        //If enemy tile, check the next tile to see if it is friendly
        if (EnemyInDirection(direction, team, x, y))
        {
            if(SetTilesOnCapture(newX, newY, team, direction))
            {
                board[newX][newY] = new Tile(team, true);
            }
        }
        //If empty tile, return false
        return false;
    }

    public void PlayerMove(int x, int y)
    {
        MakeMove(x, y, currentGo);
    }
    void MakeMove(int x, int y, Team team)
    {
        //Double check move is valid
        if (IsValidMove(x, y, team))
        {
            //Set claimed tile
            board[x][y] = new Tile(team, true);

            //Check every direction to see if tiles could be claimed
            List<Direction> enemyDirections = new List<Direction>();
            for (int i = 0; i < 10; i++)
            {
                if (EnemyInDirection((Direction)i, team, x, y))
                {
                    enemyDirections.Add((Direction)i);
                }
            }
            //Go though all directions enemy found, see if can claim tiles
            foreach (var direction in enemyDirections)
            {
                Debug.Log("SETTING TILES");
                SetTilesOnCapture(x, y, team, direction);
            }
            NewTurn();
        }
    }
}
