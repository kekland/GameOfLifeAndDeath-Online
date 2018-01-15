// Just add this script to your camera. It doesn't need any configuration.

using UnityEngine;

public class TouchCamera : MonoBehaviour {
	public Camera camera;
	Vector2?[] oldTouchPositions = {
		null,
		null
	};
	Vector2 oldTouchVector;
	float oldTouchDistance;

	public bool AllowRotation = false;

	public float maximumTop, maximumRight;
	public float aspectRatio;
	public float aspectRatioInverse;
	void Start() {
		aspectRatio = (float)Screen.width / (float)Screen.height;
		aspectRatioInverse = 1f / aspectRatio;
	}
	void Update() {
		if (Input.touchCount == 0) {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
		}
		else if (Input.touchCount == 1) {
			if (oldTouchPositions[0] == null || oldTouchPositions[1] != null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
				oldTouchPositions[1] = null;
			}
			else {
				Vector2 newTouchPosition = Input.GetTouch(0).position;

				transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * camera.orthographicSize / camera.pixelHeight * 2f));

				oldTouchPositions[0] = newTouchPosition;
			}
		}
		else {
			if (oldTouchPositions[1] == null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
				oldTouchPositions[1] = Input.GetTouch(1).position;
				oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
				oldTouchDistance = oldTouchVector.magnitude;
			}
			else {
				Vector2 screen = new Vector2(camera.pixelWidth, camera.pixelHeight);

				Vector2[] newTouchPositions = {
					Input.GetTouch(0).position,
					Input.GetTouch(1).position
				};
				Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
				float newTouchDistance = newTouchVector.magnitude;

				transform.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * camera.orthographicSize / screen.y));
				if (AllowRotation) {
					transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
				}
				camera.orthographicSize *= oldTouchDistance / newTouchDistance;
				transform.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * camera.orthographicSize / screen.y);

				oldTouchPositions[0] = newTouchPositions[0];
				oldTouchPositions[1] = newTouchPositions[1];
				oldTouchVector = newTouchVector;
				oldTouchDistance = newTouchDistance;
			}
		}
		bool isWide = Screen.width > Screen.height;
		aspectRatio = (float)Screen.width / (float)Screen.height;
		aspectRatioInverse = 1f / aspectRatio;
		if(!isWide && camera.orthographicSize * aspectRatio > maximumRight / 2f) {
			camera.orthographicSize = (maximumRight / 2f) * aspectRatioInverse;
		}
		if(isWide && camera.orthographicSize > maximumTop / 2f) {
			camera.orthographicSize = maximumTop / 2f;
		}

		float orthoSizeY = camera.orthographicSize;
		float orthoSizeX = camera.orthographicSize * aspectRatio;

		float cameraLeftBound = camera.transform.position.x - orthoSizeX;
		float cameraRightBound = camera.transform.position.x + orthoSizeX;
		float cameraBottomBound = camera.transform.position.y - orthoSizeY;
		float cameraTopBound = camera.transform.position.y + orthoSizeY;

		if(cameraLeftBound < 0f && cameraRightBound >= maximumRight) {
			camera.transform.position = new Vector2(maximumRight / 2f, camera.transform.position.y);
		}
		else if(cameraLeftBound < 0f) {
			camera.transform.position = new Vector2(orthoSizeX, camera.transform.position.y);
		}
		else if (cameraRightBound >= maximumRight) {
			camera.transform.position = new Vector2(maximumRight - orthoSizeX, camera.transform.position.y);
		}

		if (cameraBottomBound < 0f && cameraTopBound >= maximumTop) {
			camera.transform.position = new Vector2(camera.transform.position.x, maximumTop / 2f);
		} else if (cameraBottomBound < 0f) {
			camera.transform.position = new Vector2(camera.transform.position.x, orthoSizeY);
		} else if (cameraTopBound >= maximumTop) {
			camera.transform.position = new Vector2(camera.transform.position.x, maximumTop - orthoSizeY);
		}

		camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, -10f);
	}
}
