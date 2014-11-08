using UnityEngine;
using System.Collections;

public class CharFuncs : MonoBehaviour {
	
	public GUIStyle mystyle;
	public Texture myicon;
	public Texture bubbleTexture;
	
	// GameObject this function is tied to!!
	public GameObject thisChar;
	public CharacterController thisCharController;
	string voice;
	
	//variables for speech
	System.Diagnostics.Process myProcess;
	
	// variables for the rotate
	Vector3 rotateTo; // the destination point to turn to
	GameObject rotateToObj; // the destination object to turn to
	int rotateDir; // 1 to turn clockwise, -1 to turn counterclockwise
	bool rotating; // true if currently rotating
	//Queue rotateQueue; // if already rotating, additional rotateInfo - stored in Vector2 or GameObject
	static float rspeed = 50;//30;
	
	// variables for the move
	public Vector3 moveTo;
	public GameObject moveToObj;
	bool moving;
	//Queue moveQueue;
	static float mspeed = 5;
	bool following;
	public CharFuncs moveToObjFunc;
	public bool beingfollowed = false;
	float timer = 0.0f;
	float timerMax = 2.0f;
	bool left = false; // used for pointing on the left vs right side of character
	Queue genQueue;
	
	//variables for the pickup/putdown
	bool shrinking = false;
	bool growing = false;
	GameObject manipObj;
	GameObject extraObj;
	static float sspeed = 20f;
	bool pickup = true;
	static float carrydropheight = 0.5f;
	Vector3 curscalesize;
	
	//variables for pointing
	bool pointing = false;
	GameObject pointtarget;
	float pointertimer = 0.0f;
	float pointertimerMax = 2.0f;
	public GameObject prefabarm;
	Material armmat;
	GameObject arm;
	public int pointnum = -1;
	
	// static constants
	static Vector3 nullVector = new Vector3(0,0,0);
	float curHeight;
	float halfHeight;
	float holdwhere = 1.5f;
	
	public int workingNum = -1;
	public int speakNum = -1;
	bool speaking = false;
	string saywhat = "";
	float stimer = 0.0f;
	float stimerMin = 3.0f;
	

	// Use this for initialization of vars
	void Start () {
		mystyle.normal.textColor = Color.white;
		mystyle.alignment = TextAnchor.MiddleCenter;
		thisChar = gameObject;
		thisCharController = thisChar.GetComponent<CharacterController>();
		//speechfunc = (SpeechBubble)thisChar.GetComponent(typeof(SpeechBubble));
		speaking=false;
		armmat = GlobalObjs.getMaterial(thisChar.name);
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
		
		moveTo = nullVector;
		moveToObj = null;
		moving = false;
		following = false;
		
		genQueue = new Queue();
		
		growing = false;
		shrinking = false;
		manipObj = null;
		pickup = true;
		
		curHeight = thisChar.transform.localScale.y;
		halfHeight = curHeight/2f;
		
		pointing = false;
		pointtarget = null;
		pointertimer = 0.0f;
	
	}
	
	// Update is called once per frame - when action occurring, do something
	void Update () {
		if (armmat == null) {
			armmat = GlobalObjs.getMaterial(thisChar.name);
		}
		if (thisCharController == null) {
			thisCharController = thisChar.GetComponent<CharacterController>();
		}
		if (speaking) {
			stimer += Time.deltaTime;
		}
		if (myProcess != null && myProcess.WaitForExit(1000) && stimer > stimerMin) {
			// tell everyone I'm done speaking
			Debug.Log ("Done Speaking at "+Time.time+" for "+thisChar.name);
			myProcess.Close ();
			myProcess = null;
			//speechfunc.showbubble = false;
			speaking=false;
			saywhat = "";
			stimer = 0.0f;
			GlobalObjs.removeOne(speakNum);
			//speakNum = -1;
		}
		if (pointing) {
			pointertimer += Time.deltaTime;
			if (pointertimer >= pointertimerMax) {
				// done pointing
				pointertimer = 0.0f;
				pointing = false;
				// detach arm
				arm.transform.parent = null;
				// delete arm
				Destroy (arm);
				pointtarget = null;
				arm = null;
				GlobalObjs.removeOne(pointnum);
			} else {
				// update arm as needed to point to target if char or target moved
				Vector3 relativePoint = thisChar.transform.InverseTransformPoint(pointtarget.transform.position);
				if (relativePoint.x < 0.0f) {
					left = true;
				} else {
					left = false;
				}
				Vector3 correctedstart;
				if (!left) {
					correctedstart = thisChar.transform.position + thisChar.transform.right.normalized;
				} else {
					correctedstart = thisChar.transform.position - thisChar.transform.right.normalized;
				}
				Vector3 newstart = new Vector3(correctedstart.x, 3.5f, correctedstart.z);
				Vector3 offset = pointtarget.transform.position - newstart;
				//Vector3 scale = new Vector3(0.5f, 2.0f, 0.5f);
				Vector3 position = newstart + (offset / 2.0f);
				//GameObject myarm = Instantiate (prefabarm, position, Quaternion.identity) as GameObject;
				//arm.transform.localScale = new Vector3(.5f, 1f, .5f);
				arm.transform.position = newstart+ offset.normalized;
				arm.transform.up = offset;
				//myarm.renderer.material = armmat;
				// need to attach to character so can move with character
				//myarm.transform.parent = thisChar.transform;
				//myarm.transform.localScale = scale;
			}
		}
		
		// begin exclusive commands!
		bool shortenmove = false;
		bool shortenrotate = false;
		if (following) {
			// do nothing for 1 second	
			timer += Time.deltaTime;
			if (timer >= timerMax) {
				// ready to start
				timer = 0.0f;
				following = false;
			}
			
		} else if (shrinking) {
			// scale char down
			float samt = Mathf.Min (Time.deltaTime*sspeed, thisChar.transform.localScale.y - halfHeight); // so don't go past 10f shrinking
			
			if (!pickup) {
				manipObj.transform.parent = null;
			}
			if (extraObj != null) {
				extraObj.transform.parent = null;
			}
			
			thisChar.transform.localScale += new Vector3(0f, -1*samt, 0f);
			
			if (!pickup) { // if putting down, handle object while shrinking also - be sure to put down current object if picking up a new one
				//manipObj.transform.localScale = new Vector3(1f,1f,1f);
			//	manipObj.transform.localScale += new Vector3(0f, 1*samt, 0f);
				//Debug.Log ("Shrink size="+thisChar.renderer.bounds.size.y);
				manipObj.transform.position = new Vector3(manipObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, manipObj.transform.position.z);
				manipObj.transform.parent = thisChar.transform;
			}
			if (extraObj != null) {
				extraObj.transform.position = new Vector3(extraObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, extraObj.transform.position.z);
				extraObj.transform.parent = thisChar.transform;
			}
			
			if (thisChar.transform.localScale.y <= halfHeight) {
				Debug.Log ("Done shrinking "+thisChar.name);
				// move object, attach later -- need to figure out the right position better here
				Vector3 temp = thisChar.transform.position + thisChar.transform.right.normalized*-1*holdwhere;
				manipObj.transform.position = new Vector3 (temp.x, carrydropheight, temp.z);//thisChar.transform.position + thisChar.transform.right.normalized*holdwhere;
				//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
				//manipObj.transform.position.y = carrydropheight;
				manipObj.transform.rotation = thisChar.transform.rotation;
				shrinking = false; // done shrinking
				if (pickup) { // attach object
					manipObj.transform.parent = thisChar.transform;
					curscalesize = manipObj.transform.localScale;
					//manipObj.transform.localPosition=new Vector3(.5f, 0f, 0f); // put it to char's right side
					//manipObj.transform.localRotation=Quaternion.identity; // keep same rotation as char
					Debug.Log ("Attached object");
				} else { // detach object
					manipObj.transform.parent = null;
					Debug.Log ("Detached object");
					//manipObj.transform.parent = null;
				}
				if (extraObj != null) {
					extraObj.transform.position = new Vector3(temp.x, carrydropheight, temp.z);
					extraObj.transform.rotation = thisChar.transform.rotation;
					extraObj.transform.parent = null;
					extraObj = null;
				}
				growing = true; // start growing
				//Debug.Log ("Ready to grow");
			}
		} else if (growing) {
			float gamt = Mathf.Min (Time.deltaTime*sspeed, curHeight - thisChar.transform.localScale.y); // so don't grow past 20f
			if (pickup) {
				manipObj.transform.parent = null;
			}
			thisChar.transform.localScale += new Vector3(0f, 1*gamt, 0f);
			
			if (pickup) { // if picking up, handle object while growing also
			//	manipObj.transform.localScale += new Vector3(0f, -1*gamt, 0f);
				//manipObj.transform.localScale = new Vector3(1f,1f,1f);
				//Debug.Log ("Size="+thisChar.renderer.bounds.size.y);
				manipObj.transform.position = new Vector3(manipObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, manipObj.transform.position.z);
				manipObj.transform.parent = thisChar.transform;
			}
			if (thisChar.transform.localScale.y >= curHeight) {
				Debug.Log ("Done growing "+thisChar.name);
				// attach if picking up
				//if (pickup) {
				//	manipObj.transform.parent = thisChar.transform;
				//}
				growing = false; // done growing
				manipObj = null;
				GlobalObjs.removeOne(workingNum);
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				
				}
				//workingNum = -1;
				//Debug.Log ("Reset working num in update if growing to "+workingNum+ " for "+thisChar.name);
			}
		} else if (rotating) {
			// re-update direction in case target moved
			if (rotateToObj == null) {
				// no need to change
			} else {
				rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
				rotateDir = getDirection (rotateTo);
			}
			float howmuch = rotateDir*Time.deltaTime*rspeed;
			float diff = getAngle(rotateTo);
			//Debug.Log ("Howmuch="+howmuch+", diff="+diff+" for "+thisChar.name+" rotating");
			if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) {
				howmuch = rotateDir*diff;
				//Debug.Log ("Smaller distance!"+diff+" for "+thisChar.name+"rotating");
				shortenrotate = true;
			}
			/*if (workingNum == 190) {
				Debug.Log ("190 rotateobj="+rotateToObj.name+" for "+thisChar.name);//+" parent="+rotateToObj.transform.parent.name);
				Debug.Log ("Check value="+(rotateToObj != null && rotateToObj != thisChar && rotateToObj.transform.parent != thisChar));
				Debug.Log ("Check value name="+(rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)));//(rotateToObj != null && rotateToObj.name != thisChar.name && rotateToObj.transform.parent.name != thisChar.name));
				Debug.Log ("Check parent="+(rotateToObj.transform.parent != thisChar));
				//Debug.Log ("Check parent name="+(rotateToObj.transform.parent.name != thisChar.name));
				Debug.Log ("Shortenrotate="+shortenrotate);
			}*/
			if ((rotateToObj != null && rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)) || rotateToObj == null) { // if ask to look at self, don't do anything OR child object
				//Debug.Log ("Distance="+getDist (rotateTo));
				if (getDist(rotateTo) < 1.0f) {
					shortenrotate = true;
				} else {
					thisChar.transform.Rotate (Vector3.up * howmuch);	
				}
			} else {
				shortenrotate = true;
			}
			//Debug.Log ("Rotated " + howmuch + " for " + thisChar.name);
			if (Mathf.RoundToInt(getAngle(rotateTo)*10) == 0 || shortenrotate) { 
				//Debug.Log ("In finish rotating on update and rotate="+rotating+" moving="+moving+" for "+thisChar.name);
				// remove from global queue!
				shortenrotate = false;
				rotating = false;
				rotateTo = nullVector;
				//rotateToObj = null;
				rotateDir = 1;
				GlobalObjs.removeOne(workingNum);
				Debug.Log ("Done Rotating for " + thisChar.name);
				// check to see if need to do a move now!!
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				
				}
			}
		} else if (moving) {
			if (moveToObj == null) {
				// no need to change
			} else {
				moveTo = calculateObjPostn(moveToObj);
			}
			// turn towards target before moving if not facing target
			//Debug.Log ("Angle="+getAngle(moveTo));
			if (moveToObj != null && (moveToObj == thisChar || moveToObj.transform.parent == thisChar)) {
				// don't do the move - break out
				shortenmove = true;
			} else {
				if (Mathf.RoundToInt(getAngle (moveTo)*10) != 0) { 
					//Debug.Log ("Need to turn while walking");
					float howmuch = getDirection (moveTo)*Time.deltaTime*rspeed;
					float diff = getAngle(moveTo);
					//Debug.Log ("Howmuch="+howmuch+", diff="+diff+" for "+thisChar.name+" rotating");
					if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) { // so don't turn too much
						howmuch = getDirection(moveTo)*diff;
					}
					//Debug.Log ("Turning "+howmuch);
					thisChar.transform.Rotate (Vector3.up * howmuch);	
				}
				if (isVisible()) { // only start walking if can see target
					Vector3 dir = thisChar.transform.position - moveTo;
					dir = dir.normalized;
					dir = -1f*dir.normalized; // to make them move forward
					dir *= mspeed;
					// need to check if distance is more than how far I am from my target
					thisCharController.Move (dir*Time.deltaTime);
				}
			}
			//Debug.Log ("Distance="+getDist (moveTo));
			//Debug.Log ("Cur Postn="+thisChar.transform.position);
			//Debug.Log ("Move To Postn="+moveTo);
			//Debug.Log ("Dir="+dir.normalized);
			// only stop moving if not following someone and close to target OR if following and target is still moving
			//Debug.Log (thisChar.name+" following="+following+", getDist="+getDist (moveTo)+", beingfollowed="+(moveToObjFunc != null?moveToObjFunc.beingfollowed.ToString():"doesn't exist"));
			if (shortenmove || (getDist (moveTo) < 1 && !following) || (following && getDist (moveTo) < 1 && moveToObjFunc != null && moveToObjFunc.beingfollowed == false)) {
				moving = false;
				Debug.Log ("Done Moving");
				moveTo = nullVector;
				moveToObj = null;
				beingfollowed = false;
				GlobalObjs.removeOne(workingNum);
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				}
			}
		} else {
			// keep looking at our rotateToObj if not null -- may have to check how close I am first?
			if ((rotateToObj != null && rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)) ) {//rotateToObj != null && getDist (new Vector3(rotateToObj.transform.position.x, 0f, rotateToObj.transform.position.z)) >= 1.0f) {
				if (getDist (new Vector3(rotateToObj.transform.position.x, 0f, rotateToObj.transform.position.z)) >= 1.0f) {
					int myrotateDir = getDirection (new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z));
					float myhowmuch = getAngle(new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z));
					thisChar.transform.Rotate (Vector3.up*myhowmuch*myrotateDir);
				}
			}
		}
	
	}
	
	// functions for the character
	
	public void doRotate(float towherex, float towherey, GameObject towhatobj) {
		// add to global queue!!!
		// add to global queue
		GlobalObjs.printQueue("Start Rotate "+thisChar.name);
		//Debug.Log ("Rotate="+rotating+" Move="+moving+" for "+thisChar.name);
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(towherex, 0, towherey)):(towhatobj.transform.position), QueueObj.actiontype.rotate);
		GlobalObjs.globalQueue.Add(temp);
		
		if (rotating || moving || shrinking || growing) {
			// wait & try again when done rotating	
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(towherex, towherey, towhatobj, temp.msgNum, false, miniQueueObj.actiontype.rotate);
		} else {
			workingNum = temp.msgNum;
			//Debug.Log ("Changed working num in doRotate to "+workingNum+" for "+thisChar.name);
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
			Debug.Log ("Starting rotation to " + towhatobj + " for " + this.name);
		}
		//Debug.Log ("END Rotate with Rotate="+rotating+" Move="+moving+" for "+thisChar.name+" msg="+temp.msgNum);
		GlobalObjs.printQueue("End Rotate "+thisChar.name);
		
	}
	
	public void doWalk(float x, float y, GameObject towhatobj, bool tofollow) {
		// add to global queue
		GlobalObjs.printQueue("Start Walk "+thisChar.name);
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(x, 0, y)):(towhatobj.transform.position), QueueObj.actiontype.move);
		GlobalObjs.globalQueue.Add(temp);
		Debug.Log ("*********************Added "+temp.msgNum+" for "+thisChar.name);
		// do something
		Debug.Log("In doWalk for "+thisChar.name);
		if (moving || rotating || shrinking || growing) {
			// wait & try again when done moving
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(x, y, towhatobj, temp.msgNum, tofollow, miniQueueObj.actiontype.move);
		} else {
			rotateToObj = null;
			workingNum = temp.msgNum;
			moving = true;
			following = tofollow;
			if (towhatobj == null) {
				moveToObj = null;
				moveTo = new Vector3(x, 0, y);
				moveToObjFunc = null;
			} else {
				moveToObj = towhatobj;
				moveTo = calculateObjPostn(towhatobj);
				if (tofollow) {
					moveToObjFunc = (CharFuncs)moveToObj.GetComponent(typeof(CharFuncs));
					moveToObjFunc.beingfollowed = true;
				} else {
					moveToObjFunc = null;
				}
			}
			Debug.Log ("Starting walk to " + towhatobj + " for " + this.name);
		}

		GlobalObjs.printQueue("End Walk "+thisChar.name);
	}
	
	public void doStopAll() {
		rotating = false;
		rotateDir = 1;
		rotateTo = nullVector;
		rotateToObj = null;
		if (genQueue != null) {
			while (genQueue.Count > 0) {
				genQueue.Dequeue();
			}
		}
		moving = false;
		moveTo = nullVector;
		moveToObj = null;
		following = false;
		timer = 0.0f;
		left = false;
		shrinking = false;
		growing = false;
		manipObj = null;
		pickup = true;
		pointing = false;
		pointtarget = null;
		pointertimer = 0.0f;
		pointnum = -1;
		workingNum = -1;
		speakNum = -1;
		speaking = false;
		saywhat = "";
		GlobalObjs.globalQueue.RemoveRange(0, GlobalObjs.globalQueue.Count);
		Debug.Log ("Stopped Everything in method for "+thisChar.name);
	}
	
	public void doSpeak(string toSay) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, null, nullVector, QueueObj.actiontype.speak);
		GlobalObjs.globalQueue.Add(temp);
		speakNum = temp.msgNum;
		//speechfunc.showbubble = true;
		saywhat = toSay;
		//Debug.Log ("Said:"+toSay);	
		// clean up all ' and " to be /' and /"
		toSay = toSay.Replace("'", "");//"\\'");
		toSay = toSay.Replace ("\"", " ");
		toSay = toSay.ToLower ();
		//Debug.Log ("Cleaned said:"+toSay);
		speaking=true;
		myProcess = System.Diagnostics.Process.Start ("say", "-v "+voice + " \"" + toSay+"\"");
		
	}
	
	public void doForward(float amt) { // used only for testing in the UI
		rotateToObj = null;
		moveToObj = GlobalObjs.Grave;
		moveTo = calculateObjPostn(moveToObj);
		moving = true;

	}
	
	private int getDirection(Vector3 target) {
		int dir = 1;
		
		float crossprod = Vector3.Cross(thisChar.transform.forward, new Vector3(thisChar.transform.position.x - target.x, 0, thisChar.transform.position.z - target.z)).y;
		//Debug.Log ("CrossProd="+crossprod);
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
		//Debug.Log ("Dist="+result);
		return result;
	}
	
	public void doPickup(GameObject obj) {
		if (obj.name.ToLower() != "skull1" && obj.name.ToLower() != "skull2" && obj.name.ToLower() != "lantern" && obj.name.ToLower () != "shovel") {
			// not valid command - ignore
		} else {
			// add to global queue
			QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.pickup);
			GlobalObjs.globalQueue.Add(temp);
			if (moving || rotating || shrinking || growing) {
				// wait & try again when done moving
				Debug.Log ("Already doing something for "+thisChar.name);
				putInMiniQueue(-1, -1, obj, temp.msgNum, false, miniQueueObj.actiontype.pickup);
			} else {
				rotateToObj = null;
				workingNum = temp.msgNum;
				//Debug.Log ("Changed working num in doPickup to "+workingNum+ " for "+thisChar.name);
				shrinking = true;
				manipObj = obj;
				pickup = true;
				curscalesize = obj.transform.localScale;
				switch (thisChar.transform.childCount) {
					case 0:
						break;
					case 1:
						if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (0).gameObject;
						}
						break;
					case 2:
						if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (0).gameObject;
						} else if (thisChar.transform.GetChild (1).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (1).gameObject;
						}
						break;
					default:
						Debug.Log ("ERROR - too many children");
						break;
					
				}
			}
		}
	}
	
	public void doPutDown(GameObject obj) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.putdown);
		GlobalObjs.globalQueue.Add(temp);
		if (moving || rotating || shrinking || growing) {
			// wait & try again when done moving
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(-1, -1, obj, temp.msgNum, false, miniQueueObj.actiontype.putdown);
		} else {
			rotateToObj = null;
			if (thisChar.transform.GetChildCount () == 0) {
				// do nothing since not holding anything
				GlobalObjs.removeOne(temp.msgNum);
			} else {

				// figure out what object we are carrying
				if (thisChar.transform.childCount == 1) {
					if (thisChar.transform.GetChild(0).gameObject.name == "ArmPrefab") {
						// no children
						Debug.Log ("Only child is an arm!");
						GlobalObjs.removeOne(temp.msgNum);
					} else {
						if (thisChar.transform.GetChild (0).gameObject.name == obj.name) {
							workingNum = temp.msgNum;
							//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
							shrinking = true;
							manipObj = thisChar.transform.GetChild (0).gameObject;
						} else {
							// don't have the object right now
							GlobalObjs.removeOne(temp.msgNum);
						}
					}
				} else {
					// assume if there is two, one is definitely the object to putdown
					workingNum = temp.msgNum;
					//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
					shrinking = true;
					if (thisChar.transform.GetChild (0).gameObject.name == "ArmPrefab") {
						if (thisChar.transform.GetChild (1).gameObject.name == obj.name) {
							manipObj = thisChar.transform.GetChild (1).gameObject;
						} else {
							shrinking = false;
							GlobalObjs.removeOne(temp.msgNum); // don't have the right object
						}
					} else {
						if (thisChar.transform.GetChild (0).gameObject.name == obj.name) {
							manipObj = thisChar.transform.GetChild (0).gameObject;
						} else {
							shrinking = false;
							GlobalObjs.removeOne(temp.msgNum); // don't have right object
						}
					}
				}
				
				//manipObj = thisChar.transform.GetChild(0).gameObject;
				pickup = false;
				if (manipObj != null) {
					Vector3 curpostn = manipObj.transform.position;
					Quaternion currot = manipObj.transform.rotation;
					//manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;

					manipObj.transform.rotation = currot;//thisChar.transform.rotation;
					curscalesize = manipObj.transform.localScale;
				}
				
			}
		}
	}
	
	Vector3 calculateObjPostn(GameObject o) {
		Vector3 heading = o.transform.position - thisChar.transform.position;
		heading.y = 0;
		float distance = heading.magnitude;
		Vector3 direction = heading/distance;
		float minusamt = 2.8f; // if character object
		switch (o.name) {
		case "Lantern":
		case "Shovel":
		case "Skull1":
		case "Skull2":
			minusamt = 1.4f;
			break;
		case "Hamlet":
		case "Horatio":
		case "GraveDigger":
		case "GraveDiggerTwo":
			minusamt = 2.8f;
			break;
		default:
			minusamt = 0f;
			break;
		}
		//if (thisChar.name == "GraveDiggerTwo" && o.name == "GraveDigger") {
		//	Debug.Log ("Position G2="+thisChar.transform.position+" G1="+o.transform.position+" distance="+distance+" direction="+direction+" minusamt="+ minusamt+" postn="+(thisChar.transform.position + (direction *(distance - minusamt))));
		//}
		return thisChar.transform.position + (direction * (distance - minusamt));
	}
	
	void OnGUI() {
		// show text when speaking
		if(speaking) {
			GUI.Label (new Rect(150, 630, 1000, 30), new GUIContent(saywhat, myicon), mystyle);	
			//Debug.Log (saywhat);
			Vector3 ptheight = Camera.main.WorldToScreenPoint(thisChar.transform.position + new Vector3(0f, 40f, 0f));
			Vector3 ptheightabove = Camera.main.WorldToScreenPoint(thisChar.transform.position + new Vector3(0f, 41f, 0f));
			Vector3 pt = Camera.main.WorldToScreenPoint(thisChar.transform.position);
			//Debug.Log ("ph="+ptheight+", pha="+ptheightabove+", p="+pt);
			float heightdiff = ptheight.y - pt.y;
			float width = (2f/40f) * heightdiff;
			float bubbleheight = 1.15f * width;
			float bubblewidth = width;
			float startbubbley = pt.y +(bubbleheight/2);//ptheight.y;//Screen.height - ptheightabove.y -25f;
			float startbubblex = ptheight.x - (.4f*width);
			GUI.DrawTexture(new Rect(startbubblex, startbubbley, bubblewidth, bubbleheight), bubbleTexture, ScaleMode.ScaleToFit, true, 0f);

		}
	}
	
	public void doPoint(GameObject target) {
		QueueObj temp = new QueueObj(thisChar, target, target.transform.position, QueueObj.actiontype.point);
		GlobalObjs.globalQueue.Add(temp);
		pointnum = temp.msgNum;
		pointing = true;
		pointertimer = 0.0f;
		pointtarget = target;
		Vector3 relativePoint = thisChar.transform.InverseTransformPoint(pointtarget.transform.position);
		if (relativePoint.x < 0.0f) {
			left = true;
		} else {
			left = false;
		}
		Vector3 correctedstart;
		if (!left) {
			correctedstart = thisChar.transform.position + thisChar.transform.right.normalized;
		} else {
			correctedstart = thisChar.transform.position - thisChar.transform.right.normalized;
		}
		Vector3 newstart = new Vector3(correctedstart.x, 3.5f, correctedstart.z);
		Vector3 offset = target.transform.position - newstart;
		//Vector3 scale = new Vector3(0.5f, 2.0f, 0.5f);
		Vector3 position = newstart + (offset / 2.0f);
		GameObject myarm = Instantiate (prefabarm, position, Quaternion.identity) as GameObject;
		arm = myarm;
		myarm.transform.localScale = new Vector3(.5f, 1f, .5f);
		myarm.transform.position = newstart+ offset.normalized;
		myarm.transform.up = offset;
		Debug.Log ("Material name="+armmat.name);
		myarm.renderer.material = armmat;
		// need to attach to character so can move with character
		myarm.transform.parent = thisChar.transform;
		//myarm.transform.localScale = scale;
	}
	
	public void getNextInMiniQueue() {
		miniQueueObj pulled = (miniQueueObj)genQueue.Dequeue();
		workingNum = pulled.msgnum;
		Debug.Log ("*********************Dequeued "+workingNum+" for "+thisChar.name);
		switch (pulled.action) {
			case miniQueueObj.actiontype.move:
				moving = true;
				rotateToObj = null;
				following = pulled.following;
				if (pulled.getTargetType() == "Vector3") {
					moveTo = pulled.targetpt; 
					moveToObj = null;
					moveToObjFunc = null;
				} else {
					moveToObj = pulled.target;
					moveTo = calculateObjPostn(moveToObj);
					if (following) {
						moveToObjFunc = (CharFuncs)moveToObj.GetComponent(typeof(CharFuncs));
						moveToObjFunc.beingfollowed = true;
					} else {
						moveToObjFunc = null;
					}
				}
				break;
			case miniQueueObj.actiontype.rotate:
				rotating = true;
				if (pulled.getTargetType() == "Vector3") {
					rotateTo = pulled.targetpt; 
					rotateToObj = null;
					rotateDir = getDirection (rotateTo);
				} else {
					rotateToObj = pulled.target;
					rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
					rotateDir = getDirection(rotateTo);
				}		
				break;
			case miniQueueObj.actiontype.pickup:
				if (pulled.target.name.ToLower() != "skull1" && pulled.target.name.ToLower() != "skull2" && pulled.target.name.ToLower() != "lantern" && pulled.target.name.ToLower () != "shovel") {
					// not valid command - ignore
					GlobalObjs.removeOne(pulled.msgnum);
				} else {
					shrinking = true;
					rotateToObj = null;
					manipObj = pulled.target;
					pickup = true;
					curscalesize = pulled.target.transform.localScale;
					switch (thisChar.transform.childCount) {
						case 0:
							break;
						case 1:
							if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (0).gameObject;
							}
							break;
						case 2:
							if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (0).gameObject;
							} else if (thisChar.transform.GetChild (1).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (1).gameObject;
							}
							break;
						default:
							Debug.Log ("ERROR - too many children");
							break;
						
					}
				}
				break;
			case miniQueueObj.actiontype.putdown:
				rotateToObj = null;
				if (thisChar.transform.GetChildCount () == 0) {
					// do nothing since not holding anything
					GlobalObjs.removeOne(pulled.msgnum);
				} else {
	
					// figure out what object we are carrying
					if (thisChar.transform.childCount == 1) {
						if (thisChar.transform.GetChild(0).gameObject.name == "ArmPrefab") {
							// no children
							Debug.Log ("Only child is an arm!");
							GlobalObjs.removeOne(pulled.msgnum);
						} else {
							//workingNum = pulled.msgnum;
							//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
							if (thisChar.transform.GetChild (0).gameObject.name == pulled.target.name) {
								shrinking = true;
								manipObj = thisChar.transform.GetChild (0).gameObject;
							} else {
								GlobalObjs.removeOne(pulled.msgnum);
							}
						}
					} else {
						// assume if there is two, one is definitely the object to putdown
						//workingNum = pulled.msgnum;
						//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
						shrinking = true;
						if (thisChar.transform.GetChild (0).gameObject.name == "ArmPrefab") {
							if (thisChar.transform.GetChild (1).gameObject.name == pulled.target.name) {
								manipObj = thisChar.transform.GetChild (1).gameObject;
							} else {
								// invalid object
								shrinking = false;
								GlobalObjs.removeOne(pulled.msgnum);
							}
						} else {
							if (thisChar.transform.GetChild (0).gameObject.name == pulled.target.name) {
								manipObj = thisChar.transform.GetChild (0).gameObject;
							} else {
								shrinking = false;
								GlobalObjs.removeOne(pulled.msgnum);
							}
						}
					}
					
					//manipObj = thisChar.transform.GetChild(0).gameObject;
					pickup = false;
					if (manipObj != null) {
						Vector3 curpostn = manipObj.transform.position;
						Quaternion currot = manipObj.transform.rotation;
						//manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;

						manipObj.transform.rotation = currot;//thisChar.transform.rotation;
						curscalesize = manipObj.transform.localScale;
					}					
				}
				break;
			default:
				// do nothing
				break;
		}
		
	}
	
	public void putInMiniQueue(float x, float y, GameObject t, int n, bool f, miniQueueObj.actiontype a) {
		if (t == null) {
			genQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, n, f, a));
		} else {
			genQueue.Enqueue (new miniQueueObj(new Vector3(t.transform.position.x, 0, t.transform.position.z),t, n, f, a));
		}
	}
	
	public bool isVisible() {
		// checks if the current move target is in field of view of the character or not
		// update moveTo if Obj is not null
		if (moveToObj != null) {
			moveTo = calculateObjPostn(moveToObj);
		}
		Vector3 totarget = moveTo - thisChar.transform.position; 
		totarget.y = 0;
		Vector3 myforward = -1f*thisChar.transform.forward; // forward is actually backwards
		myforward.y = 0;
		float myprod = Vector3.Dot(myforward.normalized, totarget.normalized);
		if (myprod > Mathf.Cos (Mathf.Deg2Rad*45)) {
			//Debug.Log ("Myprod="+myprod+", Cos60="+Mathf.Cos (Mathf.Deg2Rad*60));
			return true;
		}
		//Debug.Log ("Myprod="+myprod+", Cos60="+Mathf.Cos (Mathf.Deg2Rad*60));
		return false;
		
	}
	
}

