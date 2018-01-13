using UnityEngine;
using System;
public class Tile {
	public TileType type;
	public GameObject associatedGameObject;
	public int neighborTiles;
	public TileType neighborMajority;
}

public enum TileType
{
	RedTile = 0,
	BlueTile = 1,
	None = -1
}