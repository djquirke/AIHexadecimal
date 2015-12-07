using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {

	Team team = Team.NONE, opposing_team = Team.NONE;
    private int width = 8, height = 8;
	private float timer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
	}

	public void Initialise(Team team)
	{
		this.team = team;
        if (team == Team.BLACK)
            opposing_team = Team.WHITE;
        else
            opposing_team = Team.BLACK;
	}

	public List<List<Tile>> AiMove(List<List<Tile>> board)
	{
        TreeNode root = new TreeNode();
		root.board = CopyBoard(board);
		root.team = team;
		TimeSpan t = DateTime.Now - new DateTime(2015, 12, 5);
		timer = (float)t.TotalSeconds;
		while((DateTime.Now - new DateTime(2015, 12, 5)).TotalSeconds < timer + 2)
		{
	        TreeNode selected = AISelection(root);
	        int score = Simulation (selected);
			Backup (selected, score);
		}
        float bestQ = -Mathf.Infinity;
        TreeNode bestNode = new TreeNode();
        foreach (TreeNode node in root.Children)
        {
            if (node.Q > bestQ)
            {
                bestQ = node.Q;
                bestNode = node;
            }
        }

		board = GameObject.Find("MoveLogic").GetComponent<MoveLogic>().MakeMove(bestNode.move.x, bestNode.move.y, team, root.board);
		return board;
	}

    TreeNode AISelection(TreeNode node)
    {
        TreeNode bestchild = new TreeNode();
        float bestUCT = -Mathf.Infinity;

		if (node.Children.Count == 0)
        {

            return Expand(node);

        }
        else
        {
			foreach (TreeNode child in node.Children)
            {
				if (child.N == 0)
                {
					return Expand (child);
                }

				float UCT = child.CalculateUCT();
                if (UCT > bestUCT)
                {
					bestUCT = UCT;
					bestchild = child;
                }

            }

        }
        return AISelection(bestchild);

    }

    TreeNode Expand(TreeNode node)
    {
		List<List<Tile>> temp_board = CopyBoard(node.board);

        List<Move> movesList = StoreValidMoves(node.team, temp_board);

		if(movesList.Count == 0)
			return node;

        foreach (Move move in movesList)
        {
			temp_board.Clear();
			temp_board = CopyBoard(node.board);
            TreeNode newnode = new TreeNode();
            newnode.Parent = node;
            if (newnode.Parent.team == Team.WHITE)
            {
                newnode.team = Team.BLACK;
            }
            else
            {
                newnode.team = Team.WHITE;
            }

            newnode.board = SimulateMove(move, node.team, temp_board);

            newnode.move = move;
            node.Children.Add(newnode);
        }

		int x = UnityEngine.Random.Range(0, node.Children.Count - 1);

        return node.Children[x];

    }

    int Simulation(TreeNode state)
    {
        bool terminal = false;
		int ret = new int();
		List<List<Tile>> temp_board = new List<List<Tile>>();

		while(!terminal)
        {
			temp_board.Clear();

			temp_board = CopyBoard(state.board);

			terminal = GameObject.Find("MoveLogic").GetComponent<MoveLogic>().AnyMovesAvail(state.team, temp_board);

			if(!terminal)
				break;

			List<Move> valid_moves = StoreValidMoves(state.team, temp_board);
			int r = UnityEngine.Random.Range(0, valid_moves.Count - 1);

			temp_board = SimulateMove(valid_moves[r], state.team, temp_board);
		}
		
        int[] scores = CalculateScores(temp_board);
		state.board = temp_board;

        if (scores[1] < scores[0])
        {
            //Black wins
			if (state.team == Team.WHITE) ret = -1;
			else ret = 1;
        }
        else if (scores[1] > scores[0])
        {
            //White wins
			if (state.team == Team.WHITE) ret = 1;
			else ret = -1;
        }
		else ret = 0;

		return ret;
	}

    bool Backup(TreeNode selected, int score)
    {
		bool running = true;

        while (selected != null && running)
        {
            selected.N++;

            if (selected.team.Equals(team))
            {
                selected.Q += score;
            }
            else if (selected.team.Equals(opposing_team))
            {
                selected.Q -= score;
            }

            running = Backup(selected.Parent, -score);
        }
		return false;
    }

    int[] CalculateScores(List<List<Tile>> board)
    {
        int[] counters = new int[2];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i][j].team == Team.BLACK)
				{
					//Debug.Log("is the error here?");
                    counters[0]++;
                }
                else if (board[i][j].team == Team.WHITE)
				{
					//Debug.Log("is the error here2?");
                    counters[1]++;
                }
            }
        }
        return counters;
    }

    List<Move> StoreValidMoves(Team team, List<List<Tile>> board)
    {
        List<Move> moves = new List<Move>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().IsValidMove(i, j, team, board))
                {
                    Move move = new Move(i, j);
                    moves.Add(move);
                }
            }
        }
        return moves;
    }

    List<List<Tile>> SimulateMove(Move pos, Team team, List<List<Tile>> board)
    {
		List<List<Tile>> newBoard = CopyBoard(board);
		//Debug.Log(pos.x.ToString() + " " + pos.y.ToString());
        if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().IsValidMove(pos.x, pos.y, team, newBoard))
        {
            List<Direction> enemyDirections = new List<Direction>();
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
                if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().EnemyInDirection((Direction)dir, team, pos.x, pos.y, newBoard))
                {
                    enemyDirections.Add((Direction)dir);
                }
            }

            foreach (Direction dir in enemyDirections)
            {
                if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().SetTilesOnCapture(pos.x, pos.y, team, dir, newBoard))
                {
                    //Debug.Log("legit move found");
                    newBoard[pos.x][pos.y] = new Tile(team, true);
                }
            }
        }

        return newBoard;
    }

	public List<List<Tile>> CopyBoard(List<List<Tile>> board)
	{
		List<List<Tile>> temp_board = new List<List<Tile>>();
		foreach(var row in board)
		{
			List<Tile> column = new List<Tile>();
			foreach(var tile in row)
			{
				column.Add(tile);
			}
			temp_board.Add(column);
		}
		return temp_board;
	}
}
