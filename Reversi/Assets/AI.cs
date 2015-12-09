using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {

	Team team = Team.NONE, opposing_team = Team.NONE;
    private int width = 8, height = 8;
	private float timer = 0;

	public void Initialise(Team team)
	{
		this.team = team;
        if (team == Team.BLACK)
            opposing_team = Team.WHITE;
        else
            opposing_team = Team.BLACK;
	}


	//Monte Carlo Tree Search
	public List<List<Tile>> AiMove(List<List<Tile>> board)
	{
        TreeNode root = new TreeNode();
		root.board = CopyBoard(board);
		root.team = team;
		TimeSpan t = DateTime.Now - new DateTime(2015, 12, 5);
		timer = (float)t.TotalSeconds;
		//2 second computational budget - allows for player to see their move
		//and looks like the AI is actually thinking
		//whilst a more in depth and accurate tree can be created
		while((DateTime.Now - new DateTime(2015, 12, 5)).TotalSeconds < timer + 2)
		{
			//Select a node
	        TreeNode selected = AISelection(root);
			//Simulate random moves using the selected node
	        int score = Simulation (ref selected);
			//Traverse the tree to calculate scores for the simulated tree
			Backup (selected, score);
		}
        float bestQ = -Mathf.Infinity;
        TreeNode bestNode = new TreeNode();
		//Calculate the best node to move to based on the child nodes of the root
        foreach (TreeNode node in root.Children)
        {
            if (node.Q > bestQ)
            {
                bestQ = node.Q;
                bestNode = node;
            }
        }

		//Commit to making the best calculated move
		board = GameObject.Find("MoveLogic").GetComponent<MoveLogic>().MakeMove(bestNode.move.x, bestNode.move.y, team, root.board);
		return board;
	}

	//select a node to be expanded
    TreeNode AISelection(TreeNode node)
    {
        TreeNode bestchild = new TreeNode();
        float bestUCT = -Mathf.Infinity;

		//expand the node if it hasn't been expanded yet
		if (node.Children.Count == 0)
        {

            return Expand(node);

        }
        else
        {
			foreach (TreeNode child in node.Children)
            {
				//visit a child that hasn't been visited yet
				if (child.N == 0)
                {
					return Expand (child);
                }

				//if all children have been expanded, calculate the best child to visit
				//using the UCT evaluation function
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

	//Expand a node to find all its children and return a random child
    TreeNode Expand(TreeNode node)
    {
		List<List<Tile>> temp_board = CopyBoard(node.board);

        List<Move> movesList = StoreValidMoves(node.team, temp_board);

		if(movesList.Count == 0)
			return node;

		//simulate each of the available moves to fully expand a node.
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

	//simulate random moves until no moves are available
    int Simulation(ref TreeNode state)
    {
        bool terminal = false;
		int ret = new int();
		List<List<Tile>> temp_board = new List<List<Tile>>();
		Team temp_team = state.team;
		temp_board = CopyBoard(state.board);

		while(!terminal)
		{
			//quit if no more moves are available
			terminal = GameObject.Find("Board").GetComponent<BoardManager>().IsMoveAvailable(temp_team, temp_board);
			if(!terminal)
				break;

			List<Move> valid_moves = StoreValidMoves(temp_team, temp_board);
			int r = UnityEngine.Random.Range(0, valid_moves.Count - 1);

			//simulate a random move
			temp_board = SimulateMove(valid_moves[r], temp_team, temp_board);

			//switch teams go for next simulation
			if(temp_team == Team.BLACK)
				temp_team = Team.WHITE;
			else if(temp_team == Team.WHITE)
				temp_team = Team.BLACK;
		}
		
        int[] scores = CalculateScores(temp_board);
		state.board = temp_board;

		//the score for the backup function based on who wins in this state
        if (scores[1] < scores[0])
        {
            //Black wins
			if (state.team == team) ret = -1;
			else ret = 1;
        }
        else if (scores[1] > scores[0])
        {
            //White wins
			if (state.team == team) ret = 1;
			else ret = -1;
        }
		else ret = 0;

		return ret;
	}

	//Traverse through the tree incrementing the times it has been visited, N, by one
	//and the score for going to that node by the score passed in
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

	//iterate through the board to calculate the counters for each team
    int[] CalculateScores(List<List<Tile>> board)
    {
        int[] counters = new int[2];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i][j].team == Team.BLACK)
				{
                    counters[0]++;
                }
                else if (board[i][j].team == Team.WHITE)
				{
                    counters[1]++;
                }
            }
        }
        return counters;
    }

	//stores all the valid moves for the AI to iterate through
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

	//Same logic as the MakeMove function but this does not commit to actually making the move as it is only for the AI's tree
    List<List<Tile>> SimulateMove(Move pos, Team team, List<List<Tile>> board)
    {
		List<List<Tile>> newBoard = CopyBoard(board);
		//Double check that it is a valid move
        if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().IsValidMove(pos.x, pos.y, team, newBoard))
        {
            List<Direction> enemyDirections = new List<Direction>();
            foreach (var dir in Enum.GetValues(typeof(Direction)))
            {
				//find all the directions of the enemy tiles in relation to your move
                if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().EnemyInDirection((Direction)dir, team, pos.x, pos.y, newBoard))
                {
                    enemyDirections.Add((Direction)dir);
                }
            }

            foreach (Direction dir in enemyDirections)
            {
				//check to see how many are how many of the directions can then be captured and then capture them
                if (GameObject.Find("MoveLogic").GetComponent<MoveLogic>().SetTilesOnCapture(pos.x, pos.y, team, dir, newBoard))
                {
                    newBoard[pos.x][pos.y] = new Tile(team, true);
                }
            }
        }

        return newBoard;
    }

	//create a local copy of the board so that memory isn't overwritten
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
