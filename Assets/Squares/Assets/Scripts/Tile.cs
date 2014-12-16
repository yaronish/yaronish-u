using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	private bool isActive = false;

	public bool IsActive{
		get {
			return isActive;
		}
		set {
			isActive = value;
		}
	}

	void Start ()
	{
		if (IsActive) {
			Invoke("showActiveColor", 1f);
			Invoke("showInactiveColor", 2f);
		}
	}

	public void showActiveColor()
	{
		this.transform.Rotate(new Vector3(360, 0, 0));
		this.renderer.material.color = Color.green;
	}

	public void showInactiveColor()
	{
		this.transform.Rotate(new Vector3(360, 0, 0));
		this.renderer.material.color = Color.white;
	}
}