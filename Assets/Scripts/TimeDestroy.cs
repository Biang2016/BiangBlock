using UnityEngine;

public class TimeDestroy : MonoBehaviour
{
	public float timeToDestroy = 1f;
	float timeToDestroyTicker = 0f;
	// Use this for initialization
	void Start ()
	{
		timeToDestroyTicker = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		timeToDestroyTicker += Time.deltaTime;
		if (timeToDestroyTicker >= timeToDestroy) {
			Destroy (gameObject);
		}
	}
}

