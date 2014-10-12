using UnityEngine;  
using System.Collections;  
  
[ExecuteInEditMode]  
public class SpeechBubble : MonoBehaviour  
{  
    //this game object's transform  
    private Transform goTransform;  
    //the game object's position on the screen, in pixels  
    private Vector3 goScreenPos;  
    //the game objects position on the screen  
    private Vector3 goViewportPos;  
  
    //the width of the speech bubble  
    public int bubbleWidth = 75;  
    //the height of the speech bubble  
    public int bubbleHeight = 38;  
  
    //an offset, to better position the bubble  
    public float offsetX = 0;  
    public float offsetY = 150;  //150
  
    //an offset to center the bubble  
    private int centerOffsetX;  
    private int centerOffsetY;  
  
    //a material to render the triangular part of the speech balloon  
    public Material mat;  
    //a guiSkin, to render the round part of the speech balloon  
    public GUISkin guiSkin;  
	
	public bool showbubble = false;
	Vector3 addheight;
	Vector3 topPos;
	Vector3 topScreenPos;
  
    //use this for early initialization  
    void Awake ()  
    {  
        //get this game object's transform  
        goTransform = this.GetComponent<Transform>();  
		addheight = new Vector3(0f,6f,0f);
    }  
  
    //use this for initialization  
    void Start()  
    {  
        //if the material hasn't been found  
        if (!mat)  
        {  
            Debug.LogError("Please assign a material on the Inspector.");  
            return;  
        }  
  
        //if the guiSkin hasn't been found  
        if (!guiSkin)  
        {  
            Debug.LogError("Please assign a GUI Skin on the Inspector.");  
            return;  
        }  
  
        //Calculate the X and Y offsets to center the speech balloon exactly on the center of the game object  
        centerOffsetX = bubbleWidth/2;  
        centerOffsetY = bubbleHeight/2;  
    }  
  
    //Called once per frame, after the update  
    void LateUpdate()  
    {  
        //find out the position on the screen of this game object  
        goScreenPos = Camera.main.WorldToScreenPoint(goTransform.position);   
		topScreenPos = Camera.main.WorldToScreenPoint(goTransform.position + addheight);
		topPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position + addheight);
		//Debug.Log ("Top point="+(this.gameObject.transform.position + addheight));
		//Debug.Log ("Toppos="+topPos+" screenpoint="+Camera.main.WorldToScreenPoint(this.gameObject.transform.position + addheight));
		//Debug.Log ("goScreenPos="+goScreenPos+" viewportpoint="+Camera.main.WorldToViewportPoint(goTransform.position));
  
        //Could have used the following line, instead of lines 70 and 71  
        goViewportPos = Camera.main.WorldToViewportPoint(goTransform.position);  
        //goViewportPos.x = goScreenPos.x/(float)Screen.width;  
        //goViewportPos.y = goScreenPos.y/(float)Screen.height;  
    }  
  
    //Draw GUIs  
    void OnGUI()  
    {  
		/*if (showbubble) {
	        //Begin the GUI group centering the speech bubble at the same position of this game object. After that, apply the offset  
	        GUI.BeginGroup(new Rect(topScreenPos.x-centerOffsetX-offsetX,Screen.height-topScreenPos.y-centerOffsetY-offsetY+125,bubbleWidth,bubbleHeight));  
	  //Debug.Log ("Bubble="+(Screen.height-topScreenPos.y-centerOffsetY-offsetY+50));
	            //Render the round part of the bubble  
	            GUI.Label(new Rect(0,0,20,10),"",guiSkin.customStyles[0]);  
	  
	            //Render the text  
	            GUI.Label(new Rect(10,25,19,5)," ",guiSkin.label);  
	  
	            
	  
	        GUI.EndGroup();  
		}*/
    }  
  
    //Called after camera has finished rendering the scene  
    void OnRenderObject()  
    {  
		/*if (showbubble) {
	        //push current matrix into the matrix stack  
	        GL.PushMatrix();  
	        //set material pass  
	        mat.SetPass(0);  
	        //load orthogonal projection matrix  
	        GL.LoadOrtho();  
		//GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
	        //a triangle primitive is going to be rendered  
	        GL.Begin(GL.TRIANGLES);  
	  
	            //set the color  
	            GL.Color(Color.white);  
	  
	            //Define the triangle vetices  
	            GL.Vertex3(topPos.x, topPos.y+.01f, 0.1f);  
		//Debug.Log ("TopPos="+topPos);
		//Debug.Log ("First Y="+(topPos.y));
	            GL.Vertex3(topPos.x - (bubbleWidth/3)/(float)Screen.width, topPos.y+offsetY/Screen.height - .20f, 0.1f);  
		//Debug.Log ("Second Y="+(goViewportPos.y+offsetY/Screen.height));
	            GL.Vertex3(topPos.x - (bubbleWidth/8)/(float)Screen.width, topPos.y+offsetY/Screen.height - .20f, 0.1f);  
	  
	        GL.End();  
	        //pop the orthogonal matrix from the stack  
	        GL.PopMatrix(); 
		}*/
    }  
}  
