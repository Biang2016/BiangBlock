using UnityEngine;

public class UISetActiveFalseAfterSeconds : MonoBehaviour
{

	float ticker = 0;
	public float ShowSeconds = 2f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gameObject.activeSelf) {
			ticker += Time.deltaTime;
			if (ticker > ShowSeconds) {
				gameObject.SetActive (false);
				ticker = 0;
			}
		} 
	}

}

