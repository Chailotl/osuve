using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private bool freeLook;

	private Transform _trans;
	private float _yaw;
	private float _pitch;

	void Start()
	{
		// Get transform reference
		_trans = gameObject.GetComponent<Transform>();

		// Get current yaw and pitch
		_pitch = _trans.rotation.eulerAngles.x;
		_yaw = _trans.rotation.eulerAngles.y;
	}

	void Update()
	{
		/// Movement

		// Get copy of position
		Vector3 pos = _trans.position;

		// Get copy of forward vector
		Vector3 fv = _trans.forward;

		// Remove y component of vector and normalize
		fv.y = 0;
		fv = fv.normalized;

		// Process input to move camera
		if (Input.GetKey("w")) { pos += fv * speed * Time.deltaTime; }
		if (Input.GetKey("s")) { pos -= fv * speed * Time.deltaTime; }
		if (Input.GetKey("a")) { pos -= _trans.right * speed * Time.deltaTime; }
		if (Input.GetKey("d")) { pos += _trans.right * speed * Time.deltaTime; }
		if (freeLook && Input.GetKey("space")) { pos.y += speed * Time.deltaTime; }
		if (freeLook && Input.GetKey("left shift")) { pos.y -= speed * Time.deltaTime; }

		// Set position
		_trans.position = pos;


		/// Mouse Look

		// Lock cursor
		if (freeLook && Input.GetMouseButtonDown(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Move cursor only if locked
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			// Process mouse delta
			_yaw += Input.GetAxis("Mouse X");
			_pitch += -Input.GetAxis("Mouse Y");

			// Clamp pitch to +-90 degrees
			_pitch = Mathf.Clamp(_pitch, -89.9f, 89.9f);

			// Set children's transform's rotation
			_trans.rotation = Quaternion.Euler(_pitch, _yaw, 0);
		}
	}
}
