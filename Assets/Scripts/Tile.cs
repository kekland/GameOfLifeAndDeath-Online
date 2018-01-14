using UnityEngine;
using System;
public class Tile : MonoBehaviour{
	public TileType state;
	public TileAction action;
	public SpriteRenderer associatedGameObjectRenderer;
	public SpriteRenderer associatedGameObjectIndicatorRenderer;

	public void UpdateTileOuter(Color outerColor) {
		associatedGameObjectRenderer.color = outerColor;
	}
	public void UpdateTileInner(Color innerColor) {
		associatedGameObjectIndicatorRenderer.color = innerColor;
	}
	void Awake() {
		state = TileType.None;
		action = TileAction.None;
	}
}
