using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PieceHandler : MonoBehaviour {

	public TextAsset asset;
	private List<Piece> pieces = new List<Piece>();
	
	public List<Piece> pieceInfo {
		get { return pieces; }
	}

	void OnEnable () {
		string pieceString = asset.ToString();
		byte[] pieceBytes = Encoding.ASCII.GetBytes(pieceString);

		//if (Encoding.ASCII.GetBytes(pieceString[1].ToString())[0] == (byte)(13)) {
		//	print("It|s working");
		//}
		int colorCharCounter = -1;
		int numBlocks = 0;
		List<Vector2> blockCoordinates = new List<Vector2>();
		Vector2 currentPos = Vector2.zero;
		Vector2 pivotPos = Vector2.zero;
		Color32 color = new Color32(0,0,0,255);
		for (int i = 0; i < pieceBytes.Length; i++) {
			switch (pieceBytes[i]) {
				case ((byte)10): {
					break;
				}
				
				case ((byte)(13)): {
					currentPos += Vector2.down;
					currentPos.x = 0;
					//print("/n");
					break;
				}
					

				case ((byte)(35)): {
					numBlocks += 1;
					blockCoordinates.Add(currentPos);
					currentPos += Vector2.right;
					//print("#");
					break;

				}
				
				case ((byte)(46)): {
					print(".");
					Vector2[] coordinates = blockCoordinates.ToArray();
					pieces.Add(new Piece(numBlocks, coordinates, pivotPos, color));
					//Reset all variables
					colorCharCounter = -1;
					numBlocks = 0;
					blockCoordinates = new List<Vector2>();
					currentPos = Vector2.zero;
					pivotPos = Vector2.zero;
					color = new Color32(0, 0, 0, 255);
					break;
				}
				case ((byte)(95)): {
					//print("_");
					currentPos += Vector2.right;
					break;
				}
				case ((byte)(111)): {
					//print("o");
					pivotPos = currentPos;
					currentPos.x += 1;
					break;
				}
				default: {
					int num = 0;
					int hash = (int)(pieceBytes[i]);
					if (hash >= 97 && hash <= 102) {
						num = hash - 87;
					}
					else if (hash >= 48 && hash <= 57) {
						num = hash - 48;
					}
					else {

						Debug.LogError("Unexpected character in piece txt");
						Debug.LogError(pieceBytes[i].ToString());
						break;
					}
					//print(num);
					//print("color defining char found, assingning color parameter num " + colorCharCounter);
					colorCharCounter += 1;
					if (colorCharCounter == 6) {
						colorCharCounter = 0;
					}
					switch (colorCharCounter) {

						case 0:
						color.r = (byte)(num * 16);
						continue;
						case 1:
						color.r += (byte)(num);
						continue;
						case 2:
						color.g = (byte)(num * 16);
						//print(color.g);
						continue;
						case 3:
						color.g += (byte)(num);
						//Debug.Log(num);
						continue;
						case 4:
						color.b = (byte)(num * 16);
						continue;
						case 5:
						color.b += (byte)(num);
						continue;

					}

					break;
				}
			}

			//Find the start of piece code
			//if (pieceBytes[i] == (byte)(46)) {
			//	//Find the end
			//	for (int j = i; j < pieceBytes.Length; j++) {

			//	}

			//	int j = i;
			//	while (true) {
			//		j += 1;
			//		switch (pieceBytes[j]) {
			//			case ((byte)(13)):
			//			print("/n");
			//			return;
			//			case ((byte)(35)):
			//			print("#");
			//			return;
			//			case ((byte)(46)):
			//			print(".");
			//			return;
			//			case ((byte)(95)):
			//			print("_");
			//			return;
			//			case ((byte)(111)):
			//			print("o");
			//			return;
			//		}
			//	}
			//}
		}
		print(pieces.Count);
		print(pieces[0].color.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
public class Piece {
	public Piece(int _numBlocks, Vector2[] _blockCoordinates, Vector2 _pivotCoordinates, Color32 _color) {
		numBlocks = _numBlocks;
		blockCoordinates = _blockCoordinates;
		pivotCoordinates = _pivotCoordinates;
		color = _color;
	}
	public Piece() {

	}
	public int numBlocks;
	public Vector2[] blockCoordinates;
	public Vector2 pivotCoordinates;
	public Color32 color;
}
