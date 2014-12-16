using System;
using UnityEngine;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour 
{
	const String STATE_IN_PROGRESS = "InProgress";
	const String STATE_FAILED = "Failed";
	const String STATE_COMPLETED = "Completed";

    private List<GameObject> tiles = new List<GameObject>();
	private List<GameObject> activeTiles = new List<GameObject>();

	private int rowsNum = 3;

	private int colsNum = 3;

	public GameObject tilePrefab;

	private String state;

	void Start () 
	{
		initScene();
	}

	void Update () 
	{
		if (state == STATE_IN_PROGRESS && Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Tile")
			{
				GameObject clickedTile = hit.transform.gameObject;
				if (clickedTile.GetComponent<Tile>().IsActive) {
					clickedTile.renderer.material.color = Color.green;
					activeTiles.Remove(clickedTile);
					if (activeTiles.Count == 0) {
						state = STATE_COMPLETED;
					}
				} else {
					clickedTile.renderer.material.color = Color.red;
					state = STATE_FAILED;
				}
			}
		}

		if (state == STATE_COMPLETED) {
			nextLevel();
			state = null;
		}

		if (state == STATE_FAILED) {
			previousLevel();
			state = null;
		}
	}

	private Vector3 getPosition(int x, int y)
	{
		float posX, posY;
		posX = x - Mathf.Floor(rowsNum / 2) + ((rowsNum + 1) % 2) / 2f;
		posY = y - Mathf.Floor(colsNum / 2) + ((colsNum + 1) % 2) / 2f;
		return new Vector3(posX, posY, 0);
	}

	private void SetInProgress()
	{
		state = STATE_IN_PROGRESS;
	}

	private void nextLevel()
	{
		colsNum += 1;
		rowsNum += 1;
		initScene();
	}

	private void previousLevel()
	{
		if (colsNum > 3)
			colsNum -= 1;
		if (rowsNum > 3)
			rowsNum -= 1;
		initScene();
	}

	private void initScene()
	{	
		foreach(GameObject obj in tiles) {
			Destroy(obj);
		}
		activeTiles = new List<GameObject>();
		tiles = new List<GameObject>();

		for (int i = 0; i < rowsNum; i++) {
			for (int j = 0; j < colsNum; j++) {
				GameObject tile = Instantiate(tilePrefab, getPosition(i, j), Quaternion.identity) as GameObject;
				if ((i + j) % 2 == 0) {
					tile.GetComponent<Tile>().IsActive = true;
					activeTiles.Add(tile);
				}
				tiles.Add(tile);
			}
		}
		Invoke("SetInProgress", 2.1f);
	}
}
