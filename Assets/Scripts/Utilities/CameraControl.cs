using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTANTS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float ZOOM_MIN = 3f;
	public float ZOOM_MAX = 10f;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isMouseDown = false;
	private Vector3 _mousePosition;
	private Camera _camera;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called when the Game Object is started
	/// </summary>
	private void Start() {
		_camera = Camera.main;
	}

	/// <summary>
	/// Called every frame
	/// </summary>
	private void Update () {
		// Zoom the camera if you can
		if (_isZoomInRange()) {
			_camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
		}

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// Move the camera when keys are pressed
		if (horizontal != 0 || vertical != 0) {
			transform.Translate(horizontal * 0.1f, vertical * 0.1f, 0);
		}

		// Check if the mouse is down and start dragging
		if (Input.GetMouseButtonDown(1)) {
			_isMouseDown = true;
			_mousePosition = Input.mousePosition;
		} else if (Input.GetMouseButtonUp(1)) {
			_isMouseDown = false;
			MapManager.Instance.updateMap();
		} else if (_isMouseDown) {
			// Get the difference in mouse location
			Vector3 move = Input.mousePosition - _mousePosition;

			// Set the new mouse location
			_mousePosition = Input.mousePosition;
			transform.Translate(move.x * -0.01f, move.y * -0.01f, 0);
		}
	}

	/// <summary>
	/// Called to see if the camera is allowed to be zoomed
	/// </summary>
	/// <returns><c>true</c>, if new zoom is within range, <c>false</c> otherwise</returns>
	private bool _isZoomInRange() {
		bool returnValue = false;
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		float newSize = _camera.orthographicSize - scroll;

		// Check if the new field of view is within the constraints
		if (scroll != 0 && newSize <= ZOOM_MAX && newSize >= ZOOM_MIN) {
			returnValue = true;
		}

		return returnValue;
	}
}
