using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {
	
	// Use this for initialization
	private GameObject collectedTiles;
	private const float tileWidth= 1.17f;
	private GameObject tilePos;
	private int heightLevel = 0;
	
	private GameObject gameLayer;
	
	private GameObject tmpTile;
	
	private float startUpPosY;
	
	private float gameSpeed=1f ;
	private float outofbounceX;
//	private int blankCounter = 0;
	private int leftCounter = 0;
	private int rightCounter = 0;
	private int middleCounter = 0;
	private string lastTile = "right";

	private GameObject player;


//	public Transform player;
	
	
	void Start () 
	{
	//	_player=GetComponent<Player>();

		gameLayer = GameObject.Find("gameLayer");
		collectedTiles = GameObject.Find("tiles");
		for(int i = 0; i<31; i++)
		{
			GameObject tmpG1 = Instantiate(Resources.Load("ground_left", typeof(GameObject))) as GameObject;
			tmpG1.transform.parent = collectedTiles.transform.FindChild("gLeft").transform;
			tmpG1.transform.position = Vector2.zero;
			GameObject tmpG2 = Instantiate(Resources.Load("ground_middle", typeof(GameObject))) as GameObject;
			tmpG2.transform.parent = collectedTiles.transform.FindChild("gMiddle").transform;
			tmpG2.transform.position = Vector2.zero;
			GameObject tmpG3 = Instantiate(Resources.Load("ground_right", typeof(GameObject))) as GameObject;
			tmpG3.transform.parent = collectedTiles.transform.FindChild("gRight").transform;
			tmpG3.transform.position = Vector2.zero;
			GameObject tmpG4 = Instantiate(Resources.Load("ground_up", typeof(GameObject))) as GameObject;
			tmpG4.transform.parent = collectedTiles.transform.FindChild("gUp").transform;
			tmpG4.transform.position = Vector2.zero;
//			GameObject tmpG5 = Instantiate(Resources.Load("ground_down", typeof(GameObject))) as GameObject;
//			tmpG5.transform.parent = collectedTiles.transform.FindChild("gDown").transform;
//			tmpG5.transform.position = Vector2.zero;


		}
		collectedTiles.transform.position = new Vector2 (-60.0f, -20.0f);
		
		tilePos = GameObject.Find("startTilePosition");
		startUpPosY = tilePos.transform.position.y;
//		outofbounceX = tilePos.transform.position.x - 5.0f;
//		player = GameObject.Find("Player");
//		outofbounceX=player.transform.position.x-10.0f;
		
		
		fillScene ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		player = GameObject.Find("Player");
		outofbounceX=player.transform.position.x-20.0f;

		//gameLayer.transform.position = new Vector2 (gameLayer.transform.position.x - gameSpeed * Time.deltaTime, 0);

	
		foreach (Transform child in gameLayer.transform) 
		{
			
			if(child.position.x < outofbounceX)
			{
				
				switch(child.gameObject.name)
				{
					
				case "ground_left(Clone)":
					child.gameObject.transform.position = collectedTiles.transform.FindChild("gLeft").transform.position;
					child.gameObject.transform.parent= collectedTiles.transform.FindChild("gLeft").transform;
					break;
				case "ground_middle(Clone)":
					child.gameObject.transform.position = collectedTiles.transform.FindChild("gMiddle").transform.position;
					child.gameObject.transform.parent = collectedTiles.transform.FindChild("gMiddle").transform;
					break;
				case "ground_right(Clone)":
					child.gameObject.transform.position = collectedTiles.transform.FindChild("gRight").transform.position;
					child.gameObject.transform.parent= collectedTiles.transform.FindChild("gRight").transform;
					break;
				case "up(Clone)":
					child.gameObject.transform.position = collectedTiles.transform.FindChild("gUp").transform.position;
					child.gameObject.transform.parent= collectedTiles.transform.FindChild("gUp").transform;
					break;
//				case "down(Clone)":
//					child.gameObject.transform.position = collectedTiles.transform.FindChild("gDown").transform.position;
//					child.gameObject.transform.parent = collectedTiles.transform.FindChild("gDown").transform;
//					break;
//				case "Reward":
//					GameObject.Find("Reward").GetComponent<crateScript>().inPlay = false;
//					break;
					
				default:
					Destroy(child.gameObject);
					break;
				}
			}
		}
		if (gameLayer.transform.childCount < 35)
			spawnTile ();
	}
	
	private  void fillScene()
	{
		//Fill start
		for (int i = 0; i<10; i++)
		{
			setTile("middle");
		}
		setTile("right");
	}
	
	private void setTile(string type)
	{
		switch (type)
		{
		case "left":
			tmpTile = collectedTiles.transform.FindChild("gLeft").transform.GetChild(0).gameObject;
			break;
		case "middle":
			tmpTile = collectedTiles.transform.FindChild("gMiddle").transform.GetChild(0).gameObject;
			break;
		case "right":
			tmpTile = collectedTiles.transform.FindChild("gRight").transform.GetChild(0).gameObject;
			break;
		case "up":
			tmpTile = collectedTiles.transform.FindChild("gUp").transform.GetChild(0).gameObject;
			break;
//		case "down":
//			tmpTile = collectedTiles.transform.FindChild("gDown").transform.GetChild(0).gameObject;
//			break;
		}
		tmpTile.transform.parent = gameLayer.transform;
		tmpTile.transform.position = new Vector3(tilePos.transform.position.x+(tileWidth),startUpPosY+(heightLevel * tileWidth),0);
		tilePos = tmpTile;
		lastTile = type;
		
	}

	private void spawnTile()
	{
//		if (blankCounter > 0) {
//			
//			setTile("blank");
//			blankCounter--;
//			return;
//			
//		}
		if (rightCounter > 0) 
		{
					
			setTile("right");
			rightCounter--;
			return;
					
		}
		if (leftCounter > 0) 
		{
			
			setTile("left");
			leftCounter--;
			return;
			
		}

		if (middleCounter > 0) {
			
			setTile("middle");
			middleCounter--;
			return;
			
		}
		
		if (lastTile == "right") {
			
			changeHeight();
			//setTile("left");
			middleCounter = (int)Random.Range(4,8);
			
		}
//		else if(lastTile =="right"){
//			
//			blankCounter = (int)Random.Range(2,4);
//		}
		else if(lastTile == "middle"){
//			setTile("right");
			rightCounter = (int)Random.Range(5,9);
		}
		else if(lastTile == "up" /*||lastTile == "down" */){
			//			setTile("right");
			leftCounter = (int)Random.Range(5,9);
		}
		
	}
	
	private void changeHeight()
	{  
	

	int h=(int)Random.Range (0,7);
	if(h<=3)
     {
		int newHeightLevel = (int)Random.Range(0,4);

		if(newHeightLevel<heightLevel)
		{
			heightLevel--;
			setTile("left");
		}
		else if(newHeightLevel>heightLevel)
		{
		    setTile("up");
			heightLevel++;
		}
     }
	if(h==4)
	{
			heightLevel-=0;
			setTile("left");

    }
	if(h==5)
		{
			setTile("up");
			heightLevel++;
			setTile("up");
			heightLevel++;

			
		}
	if(h==6)
		{
			for(var i=0; i<=2;i++)
			{
			setTile("up");
			heightLevel++;
			}

		}
	}
	
	
}
