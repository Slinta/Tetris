using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tetris : MonoBehaviour {

	 
	public GameObject Block;
	public GameObject Border;
	public static int width = 10;
	public static int height = 22;
	bool[,] field;
	GameObject[,] referenceObjects;
	Transform[] structure = null; 
	Transform[] NextStructure = null; 
	float timerD = 0.2f;
	float downArrowRepeatRate = 0.05f; 
	float GameTicker;
	public float TickTime;
	public Text scoreTxt;
	public Text cycleTime;
	int score;
	int seqenceDest;
	bool holdDown = false;
	public Transform nullTransform;
	public bool LeftHeld;
	public bool RightHeld;
	float timerS;
	float InGameTime;
	bool ResetHeldButtons = false;
	bool AmDead = false;
	public bool pause = false;
	public TextAsset piecesTxt;
	public PieceHandler pieceCollection;

	private void Awake() {
		UiBehaviour.OnPause += UiBehaviour_OnPause;
	}

	private void OnDestroy() {
		UiBehaviour.OnPause -= UiBehaviour_OnPause;
	}

	private void UiBehaviour_OnPause() {
		pause = !pause;
	}

	void Start () {
		InGameTime = Time.realtimeSinceStartup;
		
		Camera cam = gameObject.GetComponent<Camera> ();
		
		Vector2 middleFillPos = Vector2.zero;
		middleFillPos = new Vector2(width * 0.5f - 0.5f, height * 0.5f + 0.5f);
		GameObject middleFill = (GameObject)Instantiate(Border, middleFillPos, Quaternion.identity);
		middleFill.transform.localScale = new Vector3(width, height, 1);
		//Debug.Log (cam.aspect);
		cam.orthographicSize = Mathf.Max(height, width / cam.aspect) * 0.5f;

		//Camera top left point now always will be 0,height
		//print(cam.aspect);

		if (height > width / cam.aspect) {
			//WorldSpace top point will be - 0.5f  height + 0.5f
			//From that we can get the position by subtracting orthographic size
			Vector3 topLeftPoint = new Vector3(-0.5f, height + 0.5f, -10);
			Vector3 posToMatchTopLeft = topLeftPoint - new Vector3(-(cam.orthographicSize * cam.aspect), cam.orthographicSize, 0);
			cam.gameObject.transform.position = posToMatchTopLeft;
		}
		else {
			Vector3 bottomLeftPoint = new Vector3(-0.5f, 0.5f, -10);
			Vector3 posToMatchTopLeft = bottomLeftPoint + new Vector3((cam.orthographicSize * cam.aspect), cam.orthographicSize, 0);
			cam.gameObject.transform.position = posToMatchTopLeft;
		}

		//if (width % 2 == 0) {

		//	gameObject.transform.position = new Vector3 (width / 2 + 0.5f, height / 2f + 0f, -10);

		//} else {
		//	gameObject.transform.position = new Vector3 (width / 2, height / 2f + 2, -10);
		//}

		int decider = Random.Range(0, pieceCollection.pieceInfo.Count);
		StructureCreator(decider);
		field = new bool[width, height + 100];
		referenceObjects = new GameObject[width, height];
		
		//Debug.Log (Border.gameObject.name);

	}

	void Update() {
		if (pause) {
			return;
		}

		if (Input.GetKey (KeyCode.LeftArrow) || LeftHeld == true) {
			if (timerS == 0) {
				Left ();
			}
			timerS += Time.deltaTime; 
			if (timerS > 0.5f) {
				Left ();
				timerS = 0.4f;
			}
		}

		if (Input.GetKey (KeyCode.RightArrow) || RightHeld == true) {
			if (timerS == 0) {
				Right ();
			}
			timerS += Time.deltaTime; 
			if (timerS > 0.5f) {
				Right ();
				timerS = 0.4f;
			}
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {
			timerS = 0;
		}
		if (Input.GetKey (KeyCode.DownArrow) || holdDown == true) {
			if (structure != null) {
				timerD += Time.deltaTime; 
				if (timerD > downArrowRepeatRate) {
					MainProcess ();
					timerD = 0f;
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Up ();
		}
		GameTicker += Time.deltaTime;
		if (GameTicker > TickTime) {
			MainProcess ();
			GameTicker = 0;
		}

	}

	void MainProcess () {
		int numNone = 0;

		if (structure == null) {
			score += seqenceDest * seqenceDest; 
			scoreTxt.text = score.ToString();
			seqenceDest = 0;
			structure = NextStructure;
			Transform mover = nullTransform;
			mover.position = new Vector3 (width, height, 0);
			Vector2 lowestPointCoordinates = Vector2.zero;
			foreach (Transform IsIT in structure) {
				if (IsIT.name == "LowestPoint") {
					lowestPointCoordinates = (Vector2)IsIT.position;
				}
				IsIT.parent = mover;
			}
			
			mover.position = new Vector3 ((int)(width / 2), 2 + height + (height - lowestPointCoordinates.y), 0);
			//Debug.Log (mover.position);
			foreach (Transform child in structure) {
				child.parent = null;

				child.position = new Vector3 (Mathf.RoundToInt (child.position.x), Mathf.RoundToInt (child.position.y), 0);
			}
			mover.position = new Vector3 (width - 3, height + 1, 0);
			int decider = Random.Range (0, pieceCollection.pieceInfo.Count);
			StructureCreator (decider);
		}

		foreach(Transform movingBlock in structure){ 
			if ((int)movingBlock.position.y == 1 || ((int)movingBlock.position.y < height && field [(int)movingBlock.position.x, (int)movingBlock.position.y - 1] == true)) {
				//Debug.Log ("colided");
				foreach (Transform structureComponent in structure) {
					if ((int)structureComponent.position.y >= height) {
						Debug.Log ("ENDgAME");
						if (AmDead == false){
							RecordScore ();
						}

						SceneManager.LoadScene ("Menu");

					} else {
						field [(int)structureComponent.position.x, (int)structureComponent.position.y] = true;
						referenceObjects [(int)structureComponent.position.x, (int)structureComponent.position.y] = structureComponent.gameObject;
							
					}
				}
				List<int> yToUse = new List<int>();
				foreach (Transform structureComponent in structure) {
					if (yToUse.Contains((int)structureComponent.position.y) == false){
						
						yToUse.Add ((int)structureComponent.position.y);
					}
				}
				yToUse.Sort ();
				for (int FUCKOFF = yToUse.Count - 1; FUCKOFF >= 0; FUCKOFF--){
					ClearBlocks (yToUse [FUCKOFF]);
					//Debug.Log (yToUse [FUCKOFF]);
				}
				//Debug.Log (usedY.Count);
				if (ResetHeldButtons == true) {
					DownReset();
					ResetHeldButtons = false;
				}
				structure = null;
				break;
			} 
			else {
				numNone += 1; 
			}
		}
		if (structure != null) {
			if (numNone == structure.Length) {
				foreach (Transform movingBlock in structure) {
					movingBlock.position = new Vector3(movingBlock.position.x, movingBlock.position.y - 1, 0);
				}
			}
		}
	}
	void ClearBlocks(int yPos) {
		if (yPos > height - 1) {
			return;
		}
		int counter = 0;
		for (int x = 0; x < width; x++) {
			if (field [x, yPos] == true) {
				counter++;
			}
		}
		if (counter == (width)) {
			//Debug.Log ("Should Delete");
			seqenceDest += 1; 
			for(int x = 0; x < width; x++)	{
				field [x, yPos] = false;
				GameObject.Destroy( referenceObjects[x,yPos] ); 
				referenceObjects[x,yPos] = null;
			}

			for (int y = yPos; y < (height - 1); y++) {
				//Debug.Log ("deleting " + y + " row");
					
				//Debug.Log ("repairing " + y + " row");
				for (int x = 0; x < width; x++) {
					field [x, y] = field [x, y + 1];
					field [x, y + 1] = false;
					if (referenceObjects [x, y + 1] != null) {
						//Debug.Log ("the object at [" + x + " , " + y + " ] exists");
						GameObject manipd = referenceObjects [x, y + 1];
						referenceObjects [x, y + 1] = null;
						manipd.transform.position = new Vector3(manipd.transform.position.x, manipd.transform.position.y - 1, 0);
						referenceObjects [x, y] = manipd;
					}
				}
			}
		}

	}

	void StructureCreator(int decider) {
		Piece chosenPiece = pieceCollection.pieceInfo[decider];
		//Create all blocks + one mandatory pivot
		GameObject[] blocks = new GameObject[chosenPiece.numBlocks + 1];
		for (int i = 0; i < chosenPiece.numBlocks; i++) {
			blocks[i] = Instantiate(Block, new Vector3(width + chosenPiece.blockCoordinates[i].x, height + chosenPiece.blockCoordinates[i].y, 0), Quaternion.identity);
			if (i == chosenPiece.numBlocks - 1) {
				blocks[i].name = "LowestPoint";
			}
		}
		blocks[chosenPiece.numBlocks] = Instantiate(Block, new Vector3(width + chosenPiece.pivotCoordinates.x, height + chosenPiece.pivotCoordinates.y, 0), Quaternion.identity);
		blocks[chosenPiece.numBlocks].name = "pivot";
		foreach (GameObject block in blocks) {
			block.GetComponent<SpriteRenderer>().color = chosenPiece.color;
		}
		NextStructure = new Transform[chosenPiece.numBlocks + 1];
		for (int i = 0; i < chosenPiece.numBlocks + 1; i++) {
			NextStructure[i] = blocks[i].transform;
		}



		//if (decider == 0) {
		//	// I block
		//	NextStructure = new Transform[4];
		//	GameObject block1 = NewBlock(0, 0);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2), height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2), height - 4 + 2, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2), height - 4 + 3, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;
		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 255, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 255, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 255, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 255, 255);
		//}
		//else if (decider == 1) {
		//	// T block
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) - 1, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;
		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 255, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 255, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 255, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 255, 255);
		//}
		//else if (decider == 2) {
		//	// S block
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) - 1, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 1, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;

		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 0, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 0, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 0, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (0, 255, 0, 255);

		//}
		//else if (decider == 3) {
		//	//Z block
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) - 1, height - 4 + 1, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;


		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 0, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 0, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 0, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (255, 0, 0, 255);
		//}
		//else if (decider == 4) {
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 2, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 0, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;
		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (255, 127, 0, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (255, 127, 0, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (255, 127, 0, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (255, 127, 0, 255);
		//}
		//else if (decider == 5) {
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 2, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) - 1, height - 4 + 0, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;
		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (0, 0, 255, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (0, 0, 255, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (0, 0, 255, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (0, 0, 255, 255);
		//}
		//else if (decider == 6) {
		//	NextStructure = new Transform[4];
		//	GameObject block1 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block2 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 0, 0), Quaternion.identity);
		//	GameObject block3 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 0, height - 4 + 1, 0), Quaternion.identity);
		//	GameObject block4 = (GameObject)Instantiate (Block, new Vector3 ((int)(width + 2) + 1, height - 4 + 1, 0), Quaternion.identity);
		//	block3.name = "pivot";
		//	NextStructure [0] = block1.transform;
		//	NextStructure [3] = block2.transform;
		//	NextStructure [2] = block3.transform;
		//	NextStructure [1] = block4.transform;

		//	block1.GetComponent<SpriteRenderer> ().color = new Color32 (255, 255, 0, 255);
		//	block2.GetComponent<SpriteRenderer> ().color = new Color32 (255, 255, 0, 255);
		//	block3.GetComponent<SpriteRenderer> ().color = new Color32 (255, 255, 0, 255);
		//	block4.GetComponent<SpriteRenderer> ().color = new Color32 (255, 255, 0, 255);
		//}

		TickTime -= TickTime / 200;
		cycleTime.text = ((Mathf.Round (TickTime * 100)) / 100).ToString();
	}
	public void Up(){
		if (structure != null && pause == false) {
			Transform pivot = null;
			foreach (Transform IsIT in structure) {
				if (IsIT.name == "pivot") {
					pivot = IsIT;
					foreach (Transform child in structure) {
						if (child.name != "pivot") {
							child.SetParent (pivot);
						}
					}
				}
			}
			pivot.Rotate (0, 0, 90);
			bool nope = false;
			foreach (Transform child in structure) {
				int ClampedX = Mathf.RoundToInt (child.position.x);
				int ClampedY = Mathf.RoundToInt (child.position.y);
				if (ClampedX >= width || ClampedX < 0 || ClampedY < 0) {
					nope = true;
					break;
				}
				if (field[ClampedX,ClampedY] == true) {
					nope = true;
					break;
				}
			}
			if (nope == true) {
				pivot.Rotate (0, 0, -90);
				foreach (Transform child in structure) {
					child.parent = null;

					child.position = new Vector3 (Mathf.RoundToInt (child.position.x), Mathf.RoundToInt (child.position.y), 0);
				}

			} 
			else {
				foreach (Transform child in structure) {
					child.parent = null;

					child.position = new Vector3 (Mathf.RoundToInt (child.position.x), Mathf.RoundToInt (child.position.y), 0);
				}
			}
		}
	}
	public void DownSet(){
		holdDown = true;
	}
	public void DownReset(){
		holdDown = false;
		Debug.Log("Resetting Down");
	}
	public void LeftSet(){
		LeftHeld = true;
	}
	public void LeftReset(){
		LeftHeld = false;
		timerS = 0;
	}
	public void RightSet(){
		RightHeld = true;
	}
	public void RightReset(){
		RightHeld = false;
		timerS = 0;
	}
	public void DownSpecial(float t) {
		Debug.Log("setting Down, reset in " + t);
		holdDown = true;
		Invoke("DownReset", t);
		ResetHeldButtons = true;
	}

	public void Left(){
		if (structure != null) {
			bool counter = false;
			foreach (Transform movingBlock in structure) {
				if (movingBlock.position.y < height - 1) {
					if (movingBlock.position.x <= 0 || field [(int)movingBlock.position.x - 1, (int)movingBlock.position.y] == true) {
						counter = true;
					}
				} 
				else {
					if (movingBlock.position.x <= 0) {
						counter = true;
					}
				}
			}
			if (counter == false) {
				foreach (Transform movingBlock in structure) {
					movingBlock.position = new Vector3 (movingBlock.position.x - 1, movingBlock.position.y, movingBlock.position.z);
				}
			}
		}
	}
	public void Right(){
		if (structure != null) {
			bool counter = false;
			foreach (Transform movingBlock in structure) {
				if (movingBlock.position.y < height) {
					if (movingBlock.position.x >= width - 1 || field [(int)movingBlock.position.x + 1, (int)movingBlock.position.y] == true) {
						counter = true;
					}
				}
				else {
					if ( movingBlock.position.x >= width - 1 ) {
						counter = true;
					}
				}
			}
			if (counter == false) {
				foreach (Transform movingBlock in structure) {
					movingBlock.position = new Vector3 (movingBlock.position.x + 1, movingBlock.position.y, movingBlock.position.z);
				}
			}
		}
	}
	void RecordScore() {

		AmDead = true;

		int[] scores = new int[10]; 
		string[] resultsStr = new string[10];
		for (int i = 0; i < scores.Length; i++) {
			scores[i] = PlayerPrefs.GetInt ("Result" + i);
			resultsStr [i] = PlayerPrefs.GetString ("Listing" + i);
		}
		bool isOrdered = false;
		int currentComparison = 9;
		while (isOrdered == false) {
			if (currentComparison < 0) {
				isOrdered = true;
				continue;
			}
			if (score >= scores [currentComparison]) {
				Debug.Log (score + " > " + scores [currentComparison]);
				int temp = scores [currentComparison];
				string tempStr = resultsStr [currentComparison];
				scores [currentComparison] = score;
				resultsStr [currentComparison] = (" points were scored on an " + Tetris.width + "x" + Tetris.height + " field. It took " + ((Mathf.Round ((Time.realtimeSinceStartup - InGameTime) * 100)) / 100) + " seconds. Date: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
				if (currentComparison + 1 < scores.Length) {
					Debug.Log ((currentComparison + 1) + "currentComparison + 1 < scores.Length" + scores.Length);
					scores [currentComparison + 1] = temp;
					resultsStr [currentComparison + 1] = tempStr;
				} 

			} 
			else 
			{
				isOrdered = true;
			}
			currentComparison -= 1;
		}
		int savingProgress = 0;
		foreach (int savedScore in scores) {
			PlayerPrefs.SetInt ("Result" + savingProgress, savedScore);




			savingProgress = savingProgress + 1;

			Debug.Log (savedScore.ToString());
		}
		savingProgress = 0;
		foreach (string txt in resultsStr) {
			PlayerPrefs.SetString ("Listing" + savingProgress, txt);

			savingProgress = savingProgress + 1;
		}

	}
}
