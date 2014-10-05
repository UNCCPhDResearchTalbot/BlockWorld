using UnityEngine;
using System.Collections;

public class CharFuncs : MonoBehaviour {
	
	// GameObject this function is tied to!!
	public GameObject thisChar;
	string voice;
	
	//variables for speech
	System.Diagnostics.Process myProcess;
	
	// variables for the rotate
	Vector3 rotateTo; // the destination point to turn to
	GameObject rotateToObj; // the destination object to turn to
	int rotateDir; // 1 to turn clockwise, -1 to turn counterclockwise
	bool rotating; // true if currently rotating
	Queue rotateQueue; // if already rotating, additional rotateInfo - stored in Vector2 or GameObject
	static float rspeed = 30;
	
	// variables for the move
	Vector3 moveTo;
	GameObject moveToObj;
	bool moving;
	bool waitToRotate;
	Queue moveQueue;
	static float mspeed = 5;
	
	//variables for the pickup/putdown
	bool shrinking = false;
	bool growing = false;
	GameObject manipObj;
	static float sspeed = 20f;
	bool pickup = true;
	static float carrydropheight = 0.5f;
	
	// static constants
	static Vector3 nullVector = new Vector3(0,0,0);
	float curHeight;
	float halfHeight;
	float holdwhere = 1.5f;
	
	int workingNum = -1;
	int speakNum = -1;

	// Use this for initialization of vars
	void Start () {
		thisChar = gameObject;
		switch(this.name) {
			case "Hamlet":
				voice = "Alex";
				break;
			case "GraveDigger":
				voice="Ralph";
				break;
			case "GraveDiggerTwo":
				voice="Bruce";
				break;
			case "Horatio":
				voice="Fred";
				break;
		}
		rotateTo = nullVector;
		rotateToObj = null;
		rotating = false;
		rotateDir = 1;
		rotateQueue = new Queue();
		
		moveTo = nullVector;
		moveToObj = null;
		moving = false;
		waitToRotate = false;
		moveQueue = new Queue();
		
		growing = false;
		shrinking = false;
		manipObj = null;
		pickup = true;
		
		curHeight = thisChar.transform.localScale.y;
		halfHeight = curHeight/2f;
	
	}
	
	// Update is called once per frame - when action occurring, do something
	void Update () {
		//Debug.Log ("this.forward="+thisChar.transform.forward);
		//Debug.Log ("HAMLET.forwad="+GlobalObjs.Hamlet.transform.forward);
		if (shrinking) {
			// scale char down
			float samt = Mathf.Min (Time.deltaTime*sspeed, thisChar.transform.localScale.y - halfHeight); // so don't go past 10f shrinking
			thisChar.transform.localScale += new Vector3(0f, -1*samt, 0f);
			//if (!pickup) { // if putting down, handle object while shrinking also
			//	manipObj.transform.localScale += new Vector3(0f, 1*samt, 0f);
			//}
			if (thisChar.transform.localScale.y <= halfHeight) {
				Debug.Log ("Done shrinking");
				// move object, attach later -- need to figure out the right position better here
				Vector3 temp = thisChar.transform.position + thisChar.transform.right.normalized*holdwhere;
				manipObj.transform.position = new Vector3 (temp.x, carrydropheight, temp.z);//thisChar.transform.position + thisChar.transform.right.normalized*holdwhere;
				//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
				//manipObj.transform.position.y = carrydropheight;
				manipObj.transform.rotation = thisChar.transform.rotation;
				shrinking = false; // done shrinking
				/*if (pickup) { // attach object
					manipObj.transform.parent = thisChar.transform;
					manipObj.transform.localPosition=new Vector3(.5f, 0f, 0f); // put it to char's right side
					manipObj.transform.localRotation=Quaternion.identity; // keep same rotation as char
					Debug.Log ("Attached object");
				} else { // detach object
					thisChar.transform.DetachChildren();
					Debug.Log ("Detached object");
					//manipObj.transform.parent = null;
				}*/
				growing = true; // start growing
				Debug.Log ("Ready to grow");
			}
		}
		if (growing) {
			float gamt = Mathf.Min (Time.deltaTime*sspeed, curHeight - thisChar.transform.localScale.y); // so don't grow past 20f
			thisChar.transform.localScale += new Vector3(0f, 1*gamt, 0f);
			//if (pickup) { // if picking up, handle object while growing also
			//	manipObj.transform.localScale += new Vector3(0f, -1*gamt, 0f);
			//}
			if (thisChar.transform.localScale.y >= curHeight) {
				Debug.Log ("Done growing");
				// attach if picking up
				if (pickup) {
					manipObj.transform.parent = thisChar.transform;
				}
				growing = false; // done growing
				manipObj = null;
				GlobalObjs.removeOne(workingNum);
				workingNum = -1;
			}
		}
		if (myProcess != null && myProcess.WaitForExit(1000)) {
			// tell everyone I'm done speaking
			Debug.Log ("Done Speaking at"+Time.time);
			myProcess.Close ();
			myProcess = null;
			GlobalObjs.removeOne(speakNum);
			speakNum = -1;
		}
		if (rotating) { 
			// re-update direction in case target moved
			if (rotateToObj == null) {
				// no need to change
			} else {
				rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
				rotateDir = getDirection (rotateTo);
			}
			float howmuch = rotateDir*Time.deltaTime*rspeed;
			float diff = getAngle(rotateTo);
			//Debug.Log ("Howmuch="+howmuch+", diff="+diff);
			if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) {
				howmuch = rotateDir*diff;
				//Debug.Log ("Smaller distance!"+diff);
			}
			thisChar.transform.Rotate (Vector3.up * howmuch);	
			Debug.Log ("Rotated " + howmuch + " for " + thisChar.name);
			if (Mathf.RoundToInt(getAngle(rotateTo)*10) == 0) { 
				// remove from global queue!
				GlobalObjs.removeOne(workingNum);
				workingNum = -1;
				if (rotateQueue.Count > 0) {
					rotating = true;
					rotateTo = nullVector;
					rotateToObj = null;
					rotateDir = 1;
					miniQueueObj pulled = (miniQueueObj)rotateQueue.Dequeue();
					workingNum = pulled.msgnum;
					if (pulled.getTargetType() == "Vector3") {
						rotateTo = pulled.targetpt; 
						rotateToObj = null;
						rotateDir = getDirection (rotateTo);
					} else {
						rotateToObj = pulled.target;
						rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
						rotateDir = getDirection(rotateTo);
					}
					
					Debug.Log ("Dequeued for " + thisChar.name + " value="+rotateTo);
				} else {
					rotating = false;
					rotateTo = nullVector;
					rotateToObj = null;
					rotateDir = 1;
					Debug.Log ("Done Rotating for " + thisChar.name);
					// check to see if need to do a move now!!
					if (moving) {
						// start moving!
						// first check if turned the right direction, else rotate before moving
						// might not need this
						waitToRotate = false;
					}
				}
			}
		}
		if (moving && !rotating) { // make sure not rotating before do the move!
			// do movement based on Time.deltaTime*mspeed
			Debug.Log ("Moving one step");
			// check if object moved!
			if (moveToObj == null) {
				// no need to change
			} else {
				// check if object moved & if so, do a single rotate of full distance (since shouldn't be much) before of moving!
				Debug.Log ("Need to turn more first");
				Vector3 temprotateTo = new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
				int temprotateDir = getDirection (rotateTo);
				transform.Rotate (Vector3.up * temprotateDir * getAngle (temprotateTo));
			}
			float howfar = Time.deltaTime * mspeed;
			float diffdist = getDist(moveTo);
			Debug.Log ("howfar="+howfar+", Diff="+diffdist);
			if (Mathf.Abs (diffdist) < Mathf.Abs(howfar)) {
				howfar = Mathf.Abs(diffdist); // shouldn't matter direction since always moving forward
				Debug.Log ("Stopping early "+howfar);
			}
//			thisChar.transform.Translate(thisChar.transform.forward * howfar);
			thisChar.transform.position += -1*howfar*thisChar.transform.forward;
			Debug.Log ("Cur Location="+thisChar.transform.position);
			Debug.Log ("Cur Destination="+moveTo);
			// will need to check if movequeue is not empty after finish a rotate
			if (Mathf.RoundToInt (getDist(moveTo)*10) == 0) { // check if going to bump into other char & if so, stop earlier
				// remove from global queue!
				GlobalObjs.removeOne(workingNum);
				workingNum = -1;
				if (moveQueue.Count > 0) {
					Debug.Log ("Another item in queue");
					moving = true;
					moveTo = nullVector;
					moveToObj = null;
					waitToRotate = false;
					miniQueueObj pulled = (miniQueueObj)moveQueue.Dequeue();
					workingNum = pulled.msgnum;
					if (pulled.getTargetType() == "Vector3") {
						moveTo = pulled.targetpt; 
						moveToObj = null;
						if (getAngle(moveTo) == 0) {
							waitToRotate = false;
						} else {
							waitToRotate = true; // need to check if need to rotate first
							rotating = true;
							rotateTo = moveTo; 
							rotateToObj = null;
							rotateDir = getDirection (rotateTo);
						}
					} else {
						moveToObj = pulled.target;
						moveTo = new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
						if (getAngle (moveTo) == 0) {
							waitToRotate = false;
						} else {
							waitToRotate = true; // need to check if need to rotate first
							rotating = true;
							rotateTo = moveTo;
							rotateToObj = moveToObj;
							rotateDir = getDirection (rotateTo);
						}
					}
					
					Debug.Log ("Dequeued move for " + thisChar.name + " value="+moveTo);
				} else {
					moving = false;
					moveTo = nullVector;
					moveToObj = null;
					waitToRotate = false;
					Debug.Log ("Done Moving for " + thisChar.name);
					
				}
			}
		}
	
	}
	
	// functions for the character
	
	public void doRotate(float towherex, float towherey, GameObject towhatobj) {
		// add to global queue!!!
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(towherex, 0, towherey)):(towhatobj.transform.position), QueueObj.actiontype.rotate);
		GlobalObjs.globalQueue.Add(temp);
		
		if (rotating) {
			// wait & try again when done rotating	
			Debug.Log ("Already rotating");
			if (towhatobj == null) {
				rotateQueue.Enqueue(new miniQueueObj(new Vector3(towherex, 0, towherey), null, temp.msgNum));
			} else {
				rotateQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z), towhatobj, temp.msgNum));
			}
		} else {
			workingNum = temp.msgNum;
			rotating = true;
			
			// set RotateDir as appropriate
			if (towhatobj == null) {
				rotateTo = new Vector3(towherex, 0, towherey); 
				rotateToObj = null;
				rotateDir = getDirection (rotateTo);
			} else {
				rotateTo = new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z);
				rotateToObj = towhatobj;
				rotateDir = getDirection (rotateTo);
			}
			//Debug.Log ("Starting rotation to " + towhatobj + " for " + this.name);
		}
	}
	
	public void doWalk(float x, float y, GameObject towhatobj) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(x, 0, y)):(towhatobj.transform.position), QueueObj.actiontype.move);
		GlobalObjs.globalQueue.Add(temp);
		
		// do something
		Debug.Log("In doWalk");
		if (moving) {
			// wait & try again when done moving
			Debug.Log ("Already Walking");
			if (towhatobj == null) {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, temp.msgNum));
			} else {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z),towhatobj, temp.msgNum));
			}
		} else if (rotating) {
			// wait until done rotating then try again
			Debug.Log ("Rotating, wait to walk");
			if (towhatobj == null) {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, temp.msgNum));
			} else {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z),towhatobj, temp.msgNum));
			}
		} else {
			workingNum = temp.msgNum;
			moving = true;
			Debug.Log ("No queue or rotation occurring");
			if (towhatobj == null) {
				moveToObj = null;
				moveTo = new Vector3(x,0,y);
				if (getAngle(moveTo) == 0) {
					waitToRotate = false;
				} else {
					Debug.Log ("Need to rotate first");
					waitToRotate = true; // need to check if need to rotate first
					rotating = true;
					rotateTo = moveTo; 
					rotateToObj = null;
					rotateDir = getDirection (rotateTo);
				}
				moving = true;
				
			} else {
				moveToObj = towhatobj;
				moveTo = new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z);
				if (getAngle (moveTo) == 0) {
					waitToRotate = false;
				} else {
					waitToRotate = true; // need to check if need to rotate first
					rotating = true;
					rotateTo = moveTo;
					rotateToObj = moveToObj;
					rotateDir = getDirection (rotateTo);
				}
				moving = true;
			}
			Debug.Log ("Starting walk with rotate to " + x + ", "+ y);
		}
	}
	
	public void doStopAll() {
		rotating = false;
		rotateDir = 1;
		rotateTo = nullVector;
		rotateToObj = null;
		while (rotateQueue.Count > 0) {
			rotateQueue.Dequeue();
		}
		moving = false;
		moveTo = nullVector;
		moveToObj = null;
		waitToRotate = false;
		while (moveQueue.Count > 0) {
			moveQueue.Dequeue ();
		}
		Debug.Log ("Stopped Everything in method");
	}
	
	public void doSpeak(string toSay) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, null, nullVector, QueueObj.actiontype.speak);
		GlobalObjs.globalQueue.Add(temp);
		speakNum = temp.msgNum;
		//Debug.Log ("Said:"+toSay);	
		// clean up all ' and " to be /' and /"
		toSay = toSay.Replace("'", "\\'");
		toSay = toSay.Replace ("\"", "\\\"");
		//Debug.Log ("Cleaned said:"+toSay);
		
		myProcess = System.Diagnostics.Process.Start ("say", "-v "+voice + " " + toSay);
		
	}
	
	public void doForward(float amt) {
		// figure out transform.forward??
		//thisChar.transform.Translate(thisChar.transform.forward * amt, thisChar.);
		thisChar.transform.position += -1*amt*thisChar.transform.forward;
		//Debug.Log (thisChar.transform.forward*amt);
		//thisChar.transform.Translate (new Vector3(thisChar.transform.rotation.x, 0, thisChar.transform.rotation.z)* amt);
	}
	
	private int getDirection(Vector3 target) {
		int dir = 1;
		
		float crossprod = Vector3.Cross(thisChar.transform.forward, new Vector3(thisChar.transform.position.x - target.x, 0, thisChar.transform.position.z - target.z)).y;
		Debug.Log ("CrossProd="+crossprod);
		if (crossprod > 0) {
			// turn clockwise
			dir = 1;
		} else {
			// turn counter clockwise
			dir = -1;
		}
		
		return dir;
	}
	
	private float getAngle(Vector3 target) {
		Vector3 targetvector = new Vector3(thisChar.transform.position.x - target.x, 0, thisChar.transform.position.z - target.z);
		float result = Vector3.Angle(thisChar.transform.forward, targetvector);
		return result;
	}
	
	private float getDist(Vector3 target) {
		float result = 	thisChar.transform.position.x - target.x;
		result = result*result;
		result = result + ((thisChar.transform.position.z - target.z)*(thisChar.transform.position.z - target.z));
		result = Mathf.Sqrt (result);
		Debug.Log ("Dist="+result);
		return result;
	}
	
	public void doPickup(GameObject obj) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.pickup);
		GlobalObjs.globalQueue.Add(temp);
		workingNum = temp.msgNum;
		shrinking = true;
		manipObj = obj;
		pickup = true;
	}
	
	public void doPutDown() {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, thisChar.transform.GetChild (0).gameObject, thisChar.transform.GetChild (0).gameObject.transform.position, QueueObj.actiontype.putdown);
		GlobalObjs.globalQueue.Add(temp);
		if (thisChar.transform.GetChildCount () == 0) {
			// do nothing since not holding anything
			GlobalObjs.removeOne(temp.msgNum);
		} else {
			workingNum = temp.msgNum;
			shrinking = true;
			manipObj = thisChar.transform.GetChild(0).gameObject;
			pickup = false;
			Vector3 curpostn = manipObj.transform.position;
			Quaternion currot = manipObj.transform.rotation;
			//Debug.Log("manip local="+manipObj.transform.localPosition);
			//Debug.Log ("manip postn="+manipObj.transform.position);
			//Debug.Log ("parent postn="+thisChar.transform.position);
			//Debug.Log ("parent local="+thisChar.transform.localPosition);
			//manipObj.transform.position = thisChar.transform.right.normalized*.35f;//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
			//manipObj.transform.rotation = thisChar.transform.rotation;
			thisChar.transform.DetachChildren();
			//Debug.Log("after manip local="+manipObj.transform.localPosition);
			//Debug.Log ("after manip postn="+manipObj.transform.position);
			//Debug.Log ("after parent postn="+thisChar.transform.position);
			//Debug.Log ("after parent local="+thisChar.transform.localPosition);
			manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;
			//thisChar.transform.right.normalized*.35f;//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
			//manipObj.transform.position.y = carrydropheight;
			manipObj.transform.rotation = currot;//thisChar.transform.rotation;
			
		}
	}
}
