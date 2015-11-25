using UnityEngine;
using System.Collections;

struct TreeNode{
	float Q = 0.0f;
	int n = 0;
	TreeNode parent;
	
	void IncrementN() { n++; }
	void IncrementQ() { Q++; }
	void DecrementQ() { Q++; }
	TreeNode Parent() { return parent; }
}

public class BackPropagation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	IList<TreeNode> bestSim;


	void Backup(IList<TreeNode> curSim, TreeNode selected)
	{
		while (selected != null)
		{
			//selected
		}

		if (curSim > bestSim)
		{
			bestSim = curSim;
		}
	}
}
