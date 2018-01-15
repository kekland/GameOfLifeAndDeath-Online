using UnityEngine;
using System;
public class Tile : MonoBehaviour{
	public TileType state;
	public TileAction action;
	public GameObject innerRedObject;
	public GameObject innerBlueObject;
	public GameObject innerBlackObject;
	public GameObject outerRedObject;
	public GameObject outerBlueObject;

	public GameObject sacrificeObject;
	public GameObject sacrificeFirstHalf;
	public GameObject sacrificeSecondHalf;

	public float innerRed;
	public float innerBlue;
	public float innerBlack;
	public float outerRed;
	public float outerBlue;

	public float defaultInnerValue;
	public float animationSpeed;
	void UpdateScale(GameObject gameobject, float value) {
		gameobject.transform.localScale = Vector3.Lerp(gameobject.transform.localScale, new Vector3(value, value, value), Time.deltaTime * animationSpeed);
	}
	void Update() {
		switch(state) {
			case TileType.BlueTile: outerBlue = 1f; outerRed = 0f;  break;
			case TileType.RedTile: outerBlue = 0f; outerRed = 1f; break;
			case TileType.None: outerBlue = 0f; outerRed = 0f; break;
		}
		switch(action) {
			case TileAction.RessurectBlue: innerRed = 0f; innerBlue = defaultInnerValue; innerBlack = 0f; break;
			case TileAction.RessurectRed: innerRed = defaultInnerValue; innerBlue = 0f; innerBlack = 0f; break;
			case TileAction.Die: innerRed = 0f; innerBlue = 0f; innerBlack = defaultInnerValue; break;
			case TileAction.None: innerRed = 0f; innerBlue = 0f; innerBlack = 0f; break;
		}
		UpdateScale(innerRedObject, innerRed);
		UpdateScale(innerBlueObject, innerBlue);
		UpdateScale(innerBlackObject, innerBlack);
		UpdateScale(outerRedObject, outerRed);
		UpdateScale(outerBlueObject, outerBlue);
	}
	void Awake() {
		state = TileType.None;
		action = TileAction.None;
	}
}
