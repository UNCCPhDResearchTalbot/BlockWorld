using UnityEngine;
using System.Collections;

public class NewCharScript : MonoBehaviour {
	
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
	Queue rotateQueue; // if already rotating, additional rotateInfo - stored in Vector2 or GameObject
	static float rspeed = 50;//30;
	
	// variables for the move
	public Vector3 moveTo;
	public GameObject moveToObj;
	bool moving;
	bool waitToRotate;
	Queue moveQueue;
	static float mspeed = 5;
	bool following;
	float timer = 0.0f;
	float timerMax = 2.0f;
	bool left = false;
	
	//variables for the pickup/putdown
	bool shrinking = false;
	bool growing = false;
	GameObject manipObj;
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
	//SpeechBubble speechfunc = null;
	bool speaking = false;
	string saywhat = "";
	
	CharacterController controller;
	bool forward = false;
	Vector3 ftarget;

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
		rotateQueue = new Queue();
		
		moveTo = nullVector;
		moveToObj = null;
		moving = false;
		waitToRotate = false;
		moveQueue = new Queue();
		following = false;
		
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
	void FixedUpdate () {
		if (forward) {
			Vector3 dir = thisChar.transform.position - ftarget;
			dir *= (-1/2)*mspeed;
			thisCharController.Move(dir*Time.deltaTime);
			Debug.Log ("Distance="+getDist (ftarget));
			if (getDist(ftarget) < 1) {
				forward = false;
				Debug.Log ("Done moving");
			}
		}
		if (armmat == null) {
			armmat = GlobalObjs.getMaterial(thisChar.name);
		}
		if (following) {
			// do nothing for 1 second	
			timer += Time.deltaTime;
			if (timer >= timerMax) {
				// ready to start
				timer = 0.0f;
				following = false;
			}
			
		} else {
			//if (workingNum == 5 || workingNum == 6 || workingNum == 3) {
			//	Debug.Log ("*********************"+thisChar.name+"'s move="+moving+" rotating="+rotating+" grow="+growing+" shrink="+shrinking+" pickup="+pickup+" wait="+waitToRotate);
			//}
			bool shortenmove = false;
			bool shortenrotate = false;
			//Debug.Log ("this.forward="+thisChar.transform.forward);
			//Debug.Log ("HAMLET.forwad="+GlobalObjs.Hamlet.transform.forward);
			if (shrinking) {
				// scale char down
				float samt = Mathf.Min (Time.deltaTime*sspeed, thisChar.transform.localScale.y - halfHeight); // so don't go past 10f shrinking
				
				if (!pickup) {
					manipObj.transform.parent = null;
				}
				
				thisChar.transform.localScale += new Vector3(0f, -1*samt, 0f);
				
				if (!pickup) { // if putting down, handle object while shrinking also
					//manipObj.transform.localScale = new Vector3(1f,1f,1f);
				//	manipObj.transform.localScale += new Vector3(0f, 1*samt, 0f);
					//Debug.Log ("Shrink size="+thisChar.renderer.bounds.size.y);
					manipObj.transform.position = new Vector3(manipObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, manipObj.transform.position.z);
					manipObj.transform.parent = thisChar.transform;
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
					growing = true; // start growing
					//Debug.Log ("Ready to grow");
				}
			}
			if (growing) {
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
					//workingNum = -1;
					//Debug.Log ("Reset working num in update if growing to "+workingNum+ " for "+thisChar.name);
				}
			}
			if (myProcess != null && myProcess.WaitForExit(1000)) {
				// tell everyone I'm done speaking
				Debug.Log ("Done Speaking at "+Time.time+" for "+thisChar.name);
				myProcess.Close ();
				myProcess = null;
				//speechfunc.showbubble = false;
				speaking=false;
				saywhat = "";
				GlobalObjs.removeOne(speakNum);
				//speakNum = -1;
			}
			if (rotating) { 
				// re-update direction in case target moved
				if (rotateToObj == null) {
					// no need to change
				} else {
					if (moving) {
						rotateTo = calculateObjPostn(rotateToObj);
						rotateDir = getDirection (rotateTo);
					} else {
						rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
						rotateDir = getDirection (rotateTo);
					}
				}
				float howmuch = rotateDir*Time.deltaTime*rspeed;
				float diff = getAngle(rotateTo);
				//Debug.Log ("Howmuch="+howmuch+", diff="+diff+" for "+thisChar.name+" rotating");
				if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) {
					howmuch = rotateDir*diff;
					//Debug.Log ("Smaller distance!"+diff+" for "+thisChar.name+"rotating");
					shortenrotate = true;
				}
				thisChar.transform.Rotate (Vector3.up * howmuch);	
				//Debug.Log ("Rotated " + howmuch + " for " + thisChar.name);
				if (Mathf.RoundToInt(getAngle(rotateTo)*10) == 0 || shortenrotate) { 
					//Debug.Log ("In finish rotating on update and rotate="+rotating+" moving="+moving+" for "+thisChar.name);
					// remove from global queue!
					shortenrotate = false;
					if (!moving) {
						rotating = false;
						rotateTo = nullVector;
						rotateToObj = null;
						rotateDir = 1;
						GlobalObjs.removeOne(workingNum);
						//workingNum = -1;
						//Debug.Log ("Reset working num in update rotating stopping section to "+workingNum +" for "+thisChar.name);
					
						if (rotateQueue.Count > 0) {
							rotating = true;
							rotateTo = nullVector;
							rotateToObj = null;
							rotateDir = 1;
							miniQueueObj pulled = (miniQueueObj)rotateQueue.Dequeue();
							workingNum = pulled.msgnum;
							//Debug.Log ("Changed working num in update rotating retrieve next in queue to "+workingNum+" for "+thisChar.name);
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
						} 
					}

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
					} else if (moveQueue.Count > 0) {
						//GlobalObjs.removeOne(workingNum);
						// in case a movement got queued after a look
						Debug.Log ("Another item in move queue after rotate for "+thisChar.name);
						moving = true;
						moveTo = nullVector;
						moveToObj = null;
						waitToRotate = false;
						miniQueueObj pulled = (miniQueueObj)moveQueue.Dequeue();
						following = pulled.following;
						workingNum = pulled.msgnum;
						Debug.Log ("*********************Dequeued "+workingNum +" for "+thisChar.name);
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
							moveTo = calculateObjPostn(moveToObj);//new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
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
					
					}
				}
			}
			if (moving && isVisible()) { //&& !rotating) { // make sure not rotating before do the move!
				// do movement based on Time.deltaTime*mspeed
				//Debug.Log ("Moving one step for "+thisChar.name);
				// check if object moved!
				if (moveToObj == null) {
					// no need to change
				} else {
					// check if object moved & if so, do a single rotate of full distance (since shouldn't be much) before of moving!
					Debug.Log ("Need to turn more first");
					rotateTo = calculateObjPostn(moveToObj);//new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
					int temprotateDir = getDirection (rotateTo);
					//float temprotateAmt = getAngle (rotateTo);
					if (!rotating) {
						transform.Rotate (Vector3.up * temprotateDir * getAngle (rotateTo));
					}
					moveTo = calculateObjPostn(moveToObj);//new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
				}
				float howfar = Time.deltaTime * mspeed;
			//	if (rotating) {
			//		howfar = 2.0f*howfar/3.0f;
			//	} // move slower when turning so don't do a big circle
				float diffdist = getDist(moveTo);
				//if(thisChar.name == "GraveDiggerTwo") {
					//Debug.Log ("howfar="+howfar+", Diff="+diffdist+" for "+thisChar.name+" moving !rotating");
				//}
				if (Mathf.Abs (diffdist) < Mathf.Abs(howfar)) {
					howfar = Mathf.Abs(diffdist); // shouldn't matter direction since always moving forward
					//Debug.Log ("Stopping early "+howfar+" for "+thisChar.name+" moving !rotating");
					shortenmove = true;
				}
	//			thisChar.transform.Translate(thisChar.transform.forward * howfar);
				thisChar.transform.position += -1*howfar*thisChar.transform.forward;
				//Debug.Log ("Cur Location="+thisChar.transform.position+" for "+thisChar.name+" moving");
				//Debug.Log ("Cur Destination="+moveTo+" for "+thisChar.name+" moving");
				// will need to check if movequeue is not empty after finish a rotate
				if (Mathf.RoundToInt (getDist(moveTo)*10) == 0 || shortenmove) { // check if going to bump into other char & if so, stop earlier
					// remove from global queue!
					shortenmove = false;
					moving = false;
					moveTo = nullVector;
					moveToObj = null;
					waitToRotate = false;
					GlobalObjs.removeOne(workingNum);
					//workingNum = -1;
					//Debug.Log ("Reset working num in update moving not rotating and stopping to "+workingNum+" for "+thisChar.name);
					if (rotateQueue.Count > 0) { // check for queued rotations before queued moves
						rotating = true;
						rotateTo = nullVector;
						rotateToObj = null;
						rotateDir = 1;
						miniQueueObj pulled = (miniQueueObj)rotateQueue.Dequeue();
						workingNum = pulled.msgnum;
						//Debug.Log ("Changed working num in update rotating retrieve next in queue to "+workingNum+" for "+thisChar.name);
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
					} else if (moveQueue.Count > 0) {
						Debug.Log ("Another item in queue for "+thisChar.name);
						moving = true;
						moveTo = nullVector;
						moveToObj = null;
						waitToRotate = false;
						miniQueueObj pulled = (miniQueueObj)moveQueue.Dequeue();
						following = pulled.following;
						workingNum = pulled.msgnum;
						Debug.Log ("*********************Dequeued "+workingNum+" for "+thisChar.name);
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
							moveTo = calculateObjPostn(moveToObj);//new Vector3(moveToObj.transform.position.x, 0, moveToObj.transform.position.z);
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
	
	}
	
	// functions for the character
	
	public void doRotate(float towherex, float towherey, GameObject towhatobj) {
		// add to global queue!!!
		// add to global queue
		GlobalObjs.printQueue("Start Rotate "+thisChar.name);
		//Debug.Log ("Rotate="+rotating+" Move="+moving+" for "+thisChar.name);
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(towherex, 0, towherey)):(towhatobj.transform.position), QueueObj.actiontype.rotate);
		GlobalObjs.globalQueue.Add(temp);
		
		if (rotating) {
			// wait & try again when done rotating	
			Debug.Log ("Already rotating for "+thisChar.name);
			if (towhatobj == null) {
				rotateQueue.Enqueue(new miniQueueObj(new Vector3(towherex, 0, towherey), null, temp.msgNum, false));
			} else {
				rotateQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z), towhatobj, temp.msgNum, false));
			}
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
		if (moving) {
			// wait & try again when done moving
			Debug.Log ("Already Walking for "+thisChar.name);
			if (towhatobj == null) {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, temp.msgNum, tofollow));
			} else {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z),towhatobj, temp.msgNum, tofollow));
			}
		} else if (rotating) {
			// wait until done rotating then try again
			Debug.Log ("Rotating, wait to walk for "+thisChar.name);
			if (towhatobj == null) {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, temp.msgNum, tofollow));
			} else {
				moveQueue.Enqueue (new miniQueueObj(new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z),towhatobj, temp.msgNum, tofollow));
			}
		} else {
			workingNum = temp.msgNum;//temp.msgNum;
			following = tofollow;
			//Debug.Log ("Starting walk with workingnum="+temp.msgNum+ " for "+thisChar.name);
			moving = true;
			Debug.Log ("No queue or rotation occurring for "+thisChar.name);
			if (towhatobj == null) {
				moveToObj = null;
				moveTo = new Vector3(x,0,y);
				if (getAngle(moveTo) == 0) {
					waitToRotate = false;
				} else {
					Debug.Log ("Need to rotate first for "+thisChar.name);
					waitToRotate = true; // need to check if need to rotate first
					rotating = true;
					rotateTo = moveTo; 
					rotateToObj = null;
					rotateDir = getDirection (rotateTo);
				}
				moving = true;
				
			} else {
				moveToObj = towhatobj;
				moveTo = calculateObjPostn(towhatobj);//new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z);
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
			Debug.Log ("Starting walk with rotate to " + x + ", "+ y +" with working #="+workingNum+" for "+thisChar.name);
		}
		//Debug.Log ("END Walk with Rotate="+rotating+" Move="+moving+" for "+thisChar.name+" num="+temp.msgNum);
		GlobalObjs.printQueue("End Walk "+thisChar.name);
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
	
	public void doForward(float amt) {
		// figure out transform.forward??
		//thisChar.transform.Translate(thisChar.transform.forward * amt, thisChar.);
		//controller = thisChar.GetComponent<CharacterController>();
		Vector3 moveDirection = GlobalObjs.Grave.transform.position;
		moveDirection.y = 0;
		forward = true;
		ftarget = moveDirection;
		//controller.Move(moveDirection * Time.deltaTime);
//		thisChar.transform.position += -1*amt*thisChar.transform.forward;
		//Debug.Log (thisChar.transform.forward*amt);
		//thisChar.transform.Translate (new Vector3(thisChar.transform.rotation.x, 0, thisChar.transform.rotation.z)* amt);
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
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.pickup);
		GlobalObjs.globalQueue.Add(temp);
		workingNum = temp.msgNum;
		//Debug.Log ("Changed working num in doPickup to "+workingNum+ " for "+thisChar.name);
		shrinking = true;
		manipObj = obj;
		pickup = true;
		curscalesize = obj.transform.localScale;
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
			//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
			shrinking = true;
			// figure out what object we are carrying
			if (thisChar.transform.childCount == 0) {
				// error no children
				Debug.Log ("No object to put down!");
			} else if (thisChar.transform.childCount == 1) {
				if (thisChar.transform.GetChild(0).gameObject.name == "ArmPrefab") {
					// no children
					Debug.Log ("Only child is an arm!");
				} else {
					manipObj = thisChar.transform.GetChild (0).gameObject;
				}
			} else {
				if (thisChar.transform.GetChild (0).gameObject.name == "ArmPrefab") {
					manipObj = thisChar.transform.GetChild (1).gameObject;
				} else {
					manipObj = thisChar.transform.GetChild (0).gameObject;
				}
			}
			
			//manipObj = thisChar.transform.GetChild(0).gameObject;
			pickup = false;
			Vector3 curpostn = manipObj.transform.position;
			Quaternion currot = manipObj.transform.rotation;
			//Debug.Log("manip local="+manipObj.transform.localPosition);
			//Debug.Log ("manip postn="+manipObj.transform.position);
			//Debug.Log ("parent postn="+thisChar.transform.position);
			//Debug.Log ("parent local="+thisChar.transform.localPosition);
			//manipObj.transform.position = thisChar.transform.right.normalized*.35f;//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
			//manipObj.transform.rotation = thisChar.transform.rotation;
//			thisChar.transform.DetachChildren();
			//Debug.Log("after manip local="+manipObj.transform.localPosition);
			//Debug.Log ("after manip postn="+manipObj.transform.position);
			//Debug.Log ("after parent postn="+thisChar.transform.position);
			//Debug.Log ("after parent local="+thisChar.transform.localPosition);
			manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;
			//thisChar.transform.right.normalized*.35f;//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
			//manipObj.transform.position.y = carrydropheight;
			manipObj.transform.rotation = currot;//thisChar.transform.rotation;
			curscalesize = manipObj.transform.localScale;
			
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
			//Debug.Log ("hd="+heightdiff+", w="+width+",bh="+bubbleheight+", bw="+bubblewidth+", sby="+startbubbley+", sbx="+startbubblex);
			GUI.DrawTexture(new Rect(startbubblex, startbubbley, bubblewidth, bubbleheight), bubbleTexture, ScaleMode.ScaleToFit, true, 0f);
			//GUI.Label (new Rect(startbubblex, ptheight.y, 30,30), "H");
			//GUI.Label (new Rect(startbubblex, ptheightabove.y, 30, 30), "A");
			//GUI.Label (new Rect(startbubblex, pt.y, 30,30), "P");
			//GUI.Label (new Rect(startbubblex, Screen.height-ptheight.y, 30,30), "S");
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
