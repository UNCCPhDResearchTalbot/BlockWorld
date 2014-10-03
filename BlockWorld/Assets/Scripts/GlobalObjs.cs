using UnityEngine;
using System.Collections;

public class GlobalObjs : MonoBehaviour
{
	
	public static GameObject Hamlet = null;
	public static CharFuncs HamletFunc = null;
	public static GameObject Horatio = null;
	public static CharFuncs HoratioFunc = null;
	public static GameObject GraveDigger = null;
	public static CharFuncs GraveDiggerFunc = null;
	public static GameObject GraveDiggerTwo = null;
	public static CharFuncs GraveDiggerTwoFunc = null;
	GameObject[] templist = null;
	public static Queue globalQueue = new Queue();
	public static GameObject Skull1 = null;
	public static GameObject Skull2 = null;
	public static GameObject Shovel = null;
	public static GameObject Lantern = null;
	public static GameObject Box = null;
	
	// Use this for initialization
	void Start () {
		if (Hamlet == null) {
			templist = GameObject.FindGameObjectsWithTag("Hamlet");
			Hamlet = templist[0];
			HamletFunc = (CharFuncs) Hamlet.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Horatio == null) {
			templist = GameObject.FindGameObjectsWithTag("Horatio");
			Horatio = templist[0];
			HoratioFunc = (CharFuncs) Horatio.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDigger == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDigger");
			GraveDigger = templist[0];
			GraveDiggerFunc = (CharFuncs) GraveDigger.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDiggerTwo == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDiggerTwo");
			GraveDiggerTwo = templist[0];
			GraveDiggerTwoFunc = (CharFuncs) GraveDiggerTwo.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			Box = templist[0];
			templist = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Hamlet == null) {
			templist = GameObject.FindGameObjectsWithTag("Hamlet");
			Hamlet = templist[0];
			HamletFunc = (CharFuncs) Hamlet.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Horatio == null) {
			templist = GameObject.FindGameObjectsWithTag("Horatio");
			Horatio = templist[0];
			HoratioFunc = (CharFuncs) Horatio.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDigger == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDigger");
			GraveDigger = templist[0];
			GraveDiggerFunc = (CharFuncs) GraveDigger.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (GraveDiggerTwo == null) {
			templist = GameObject.FindGameObjectsWithTag("GraveDiggerTwo");
			GraveDiggerTwo = templist[0];
			GraveDiggerTwoFunc = (CharFuncs) GraveDiggerTwo.GetComponent (typeof(CharFuncs));
			templist = null;
		}
		if (Box == null) {
			templist = GameObject.FindGameObjectsWithTag("Box");
			Box = templist[0];
			templist = null;
		}
	
	}
	
	public GameObject getObject(string name) {
		switch (name) {
		case "Hamlet":
			return Hamlet;
			break;
		case "Horatio":
			return Horatio;
			break;
		case "GraveDigger":
			return GraveDigger;
			break;
		case "GraveDiggerTwo":
			return GraveDiggerTwo;
			break;
		case "Skull1":
			return Skull1;
			break;
		case "Skull2":
			return Skull2;
			break;
		case "Shovel":
			return Shovel;
			break;
		case "Lantern":
			return Lantern;
			break;
		default:
			return null;
			break;
		}
	}
	
}

