using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour {

    private List<GameObject> objects = new List<GameObject>();

	private int rowsNum = 3;

	private int colsNum = 3;

	public GameObject squarePrefab;

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < rowsNum; i++) {
			for (int j = 0; j < colsNum; j++) {
                GameObject square = Instantiate(squarePrefab, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                if ((i + j) % 2 == 0) {
                    square.GetComponent<SquareItem>().Activate(true);
                }
                
				objects.Add(square);
			}
		}
	}
}
