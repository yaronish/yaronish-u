using UnityEngine;
using System.Collections;

public class SquareItem : MonoBehaviour {

	private bool isActive = false;

	public void Activate(bool value) 
	{
		isActive = value;
	}

	void Start ()
	{
		Invoke("showActiveColor", 1.5f);
	}

	void Update () 
	{
		if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.gameObject.GetComponent<SquareItem>().isActive) {
					hit.transform.gameObject.renderer.material.color = Color.green;
				} else {
					hit.transform.gameObject.renderer.material.color = Color.red;
				}
			}
		}
	}

	private void showActiveColor()
	{
		if (isActive) {
			gameObject.renderer.material.color = Color.green;
		}
		Invoke("showInactiveColor", 1.5f);
	}

	private void showInactiveColor()
	{
		gameObject.renderer.material.color = Color.white;
	}
}