using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Core : MonoBehaviour {
	public int mapSizeX = 10;
	public int mapSizeY = 10;

	public GameObject tilePrefab;
	Tile[,] gameTiles;

	public Image BlueImage;
	public Text BluePoints;
	public Image RedImage;
	public Text RedPoints;
	public Button ApplyButton;

	public float clickDelta = 1f;
	public float clickTimeDelta = 0.1f;

	public bool redMove = true;
	public bool waitingForConfirmation = false;
	int actionX, actionY;
	void Start() {
		gameTiles = new Tile[mapSizeX, mapSizeY];
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				GameObject clone = Instantiate(tilePrefab, new Vector2(x * 1.1f + 1f, y * 1.1f + 1f), Quaternion.identity);
				gameTiles[x, y] = clone.GetComponent<Tile>();
			}
		}

		Camera.main.GetComponent<TouchCamera>().maximumRight = (mapSizeX - 1f) * 1.1f + 2f;
		Camera.main.GetComponent<TouchCamera>().maximumTop = (mapSizeY - 1f) * 1.1f + 2f;
		clickDelta = Mathf.Clamp(Screen.height * 0.01f, 1f, 7f);
		RandomFill();
		EvolutionTick();
		CalculateActions();
		UpdateUI();
	}

	void RandomFill() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				float value = Random.Range(0f, 1f);
				if(value < 0.6f) {
					continue;
				}
				else if(value < 0.8f) {
					gameTiles[x, y].state = TileType.RedTile;
				}
				else {
					gameTiles[x, y].state = TileType.BlueTile;
				}
			}
		}
	}

	Vector2 clickStart;
	float clickStartTime;
	bool isValidClick = false;
	void Update() {
		if(Input.GetKeyDown(KeyCode.Space)) {
			EvolutionTick();
		}

		if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !isValidClick) {
			isValidClick = true;
			clickStartTime = Time.time;
		}
		if(isValidClick) {
			if(Input.touchCount > 1) {
				isValidClick = false;
			}
			if(Input.touchCount == 1 && (Input.GetTouch(0).deltaPosition.magnitude > clickDelta ||
				 Time.time - clickStartTime > clickTimeDelta)) {
				isValidClick = false;
			}
		}
		if(Input.touchCount == 0) {
			if (isValidClick && !waitingForConfirmation) {
				Vector2 realPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				realPosition -= Vector2.one;
				realPosition /= 1.1f;
				int x = Mathf.RoundToInt(realPosition.x);
				int y = Mathf.RoundToInt(realPosition.y);
				if(x < 0 || y < 0 || x >= mapSizeX || y >= mapSizeY) {
					return;
				}
				waitingForConfirmation = true;
				actionX = x;
				actionY = y;
				DoAction(x, y);
				CalculateActions();
				UpdateUI();
			}
			isValidClick = false;
			clickStart = Vector2.zero;
			clickStartTime = 0f;
		}
	}

	public void ApplyButtonUI() {
		if (waitingForConfirmation) {
			redMove = !redMove;
			waitingForConfirmation = false;
			EvolutionTick();
			CalculateActions();
			UpdateUI();
		}
	}

	void DoAction(int x, int y) {
		Debug.Log(x + " " + y);
		if(gameTiles[x, y].state == TileType.None) {
			gameTiles[x, y].state = (redMove)? TileType.RedTile : TileType.BlueTile;
		}
		else {
			gameTiles[x, y].state = TileType.None;
		}
	}

	public void EvolutionTick() {
		ApplyChanges();
		CalculateActions();
	}

	void UpdateUI() {
		int blueCount = 0;
		int redCount = 0;

		ApplyButton.interactable = waitingForConfirmation;

		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (gameTiles[x, y].state == TileType.BlueTile) {
					blueCount++;
				} else if (gameTiles[x, y].state == TileType.RedTile) {
					redCount++;
				}
			}
		}

		RedPoints.text = redCount.ToString();
		BluePoints.text = blueCount.ToString();
		float blueTransparency = 1f, redTransparency = 1f;

		if (redMove) {
			redTransparency = 1f;
			blueTransparency = 0.5f;
		} else {
			blueTransparency = 1f;
			redTransparency = 0.5f;
		}

		BlueImage.color = new Color(Data.blueColor.r, Data.blueColor.g, Data.blueColor.b, blueTransparency);
		RedImage.color = new Color(Data.redColor.r, Data.redColor.g, Data.redColor.b, redTransparency);
	}
	void ApplyChanges() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				if (gameTiles[x, y].action == TileAction.Die) {
					gameTiles[x, y].state = TileType.None;
				} else if (gameTiles[x, y].action == TileAction.RessurectBlue) {
					gameTiles[x, y].state = TileType.BlueTile;
				} else if (gameTiles[x, y].action == TileAction.RessurectRed) {
					gameTiles[x, y].state = TileType.RedTile;
				}
				gameTiles[x, y].action = TileAction.None;
			}
		}
		UpdateUI();
	}

	void CalculateActions() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				List<Tile> neighbors = GetNeighbors(x, y);

				if (gameTiles[x, y].state == TileType.None) {
					if (neighbors.Count == 3) {
						TileType popular = GetMostPopularColor(neighbors);
						if (popular == TileType.RedTile) {
							gameTiles[x, y].action = TileAction.RessurectRed;
						} else if (popular == TileType.BlueTile) {
							gameTiles[x, y].action = TileAction.RessurectBlue;
						}
					} else {
						gameTiles[x, y].action = TileAction.None;
					}
				} else {
					if (neighbors.Count == 2 || neighbors.Count == 3) {
						gameTiles[x, y].action = TileAction.None;
					} else {
						gameTiles[x, y].action = TileAction.Die;
					}
				}
			}
		}
	}

	TileType GetMostPopularColor(List<Tile> neighbors) {
		int red = 0, blue = 0;
		for (int i = 0; i < neighbors.Count; i++) {
			if (neighbors[i].state == TileType.RedTile) {
				red++;
			} else if (neighbors[i].state == TileType.BlueTile) {
				blue++;
			}
		}
		if (red > blue) {
			return TileType.RedTile;
		} else if (red < blue) {
			return TileType.BlueTile;
		} else if (red == 0 && blue == 0) {
			return TileType.None;
		} else {
			return (Random.Range(0f, 1f) > 0.5f) ? TileType.RedTile : TileType.BlueTile;
		}
	}

	List<Tile> GetNeighbors(int i, int j) {
		List<Tile> result = new List<Tile>();

		for (int x = i - 1; x <= i + 1; x++) {
			for (int y = j - 1; y <= j + 1; y++) {
				if (x == i && y == j) {
					continue;
				}
				if (x < 0 || y < 0 || x >= mapSizeX || y >= mapSizeY) {
					continue;
				}
				if (gameTiles[x, y].state == TileType.None) {
					continue;
				}
				result.Add(gameTiles[x, y]);
			}
		}

		return result;
	}
}
