using UnityEngine;
using System.Collections;

public class miniQueueObj : MonoBehaviour {
	
	public GameObject target;
	public Vector3 targetpt;
	public int msgnum;
	public bool following;
	public actiontype action;
	public enum actiontype {
		move,
		rotate,
		pickup,
		putdown,
		other
	};
	
	public miniQueueObj(Vector3 p, GameObject t, int n, bool f, actiontype a) {
		target = t;
		targetpt = p;
		msgnum = n;
		following = f;
		action = a;
	}
	
	public miniQueueObj(Vector3 p, GameObject t, int n, bool f) {
		target = t;
		targetpt = p;
		msgnum = n;
		following = f;

	}
	
	public string getTargetType() {
		if (target == null) {
			return "Vector3";
		} else {
			return "GameObject";
		}
	}
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
