using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {
	public int mapSizeX = 10;
	public int mapSizeY = 10;

	Tile[,] gameTiles;

	void Start() {
		gameTiles = new Tile[mapSizeX, mapSizeY];
	}

	void EvolutionTick() {
		EvolutionPrecalculations();
		EvolutionActions();
	}

	void EvolutionActions() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				ActionType action = WhatShouldTileDo(x, y);
				if(action == ActionType.Ressurect) {
					gameTiles[x, y].type = gameTiles[x, y].neighborMajority;
				}
				else if(action == ActionType.Die) {
					gameTiles[x, y].type = TileType.None;
				}
				gameTiles[x, y].neighborMajority = TileType.None;
				gameTiles[x, y].neighborTiles = 0;
			}
		}
	}

	ActionType WhatShouldTileDo(int x, int y) {
		if(gameTiles[x, y].type == TileType.None) {
			if(gameTiles[x, y].neighborTiles >= 3) {
				return ActionType.Ressurect;
			}
			else {
				return ActionType.None;
			}
		}
		else {
			if(gameTiles[x, y].neighborTiles == 2 || gameTiles[x, y].neighborTiles == 3) {
				return ActionType.None;
			}
			else {
				return ActionType.Die;
			}
		}
	}

	enum ActionType {
		Ressurect,
		Die,
		None
	}

	void EvolutionPrecalculations() {
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				List<Tile> neighborTiles = GetNeighrboringTiles(x, y);
				gameTiles[x, y].neighborTiles = neighborTiles.Count;

				int redCnt = 0, blueCnt = 0;

				for (int i = 0; i < neighborTiles.Count; i++) {
					if (neighborTiles[i].type == TileType.RedTile) {
						redCnt++;
					} else if (neighborTiles[i].type == TileType.BlueTile) {
						blueCnt++;
					}
				}

				gameTiles[x, y].neighborMajority = (redCnt > blueCnt) ? TileType.RedTile : TileType.BlueTile;
			}
		}
	}

	List<Tile> GetNeighrboringTiles(int x, int y) {
		List<Tile> tiles = new List<Tile>();

		for (int i = x - 1; i <= x + 1; i++) {
			for (int j = y - 1; j <= y + 1; j++) {
				if (i == j || !CheckIsInBounds(i, j)) {
					continue;
				}
				if (gameTiles[i, j].type != TileType.None) {
					tiles.Add(gameTiles[i, j]);
				}
			}
		}
		return tiles;
	}

	bool CheckIsInBounds(int x, int y) {
		if (x < 0 || y < 0) {
			return false;
		}
		if (x >= mapSizeX || y >= mapSizeY) {
			return false;
		}
		return true;
	}
}
