using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Credit to https://gist.github.com/nobnak/c5a2588f3e5a8c0b798b
 * */
public class AutohideMouse : MonoBehaviour
{	

	public float hideAfterSeconds = 3f;
	public float thresholdInPixels = 3f;

	float _lastTime;
	Vector3 _lastMousePos;

	void Start()
	{
		_lastTime = Time.timeSinceLevelLoad;
		_lastMousePos = Input.mousePosition;
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		var dx = Input.mousePosition - _lastMousePos;
		var move = (dx.sqrMagnitude > (thresholdInPixels * thresholdInPixels));
		_lastMousePos = Input.mousePosition;

		if (move)
			_lastTime = Time.timeSinceLevelLoad;

		Cursor.visible = (Time.timeSinceLevelLoad - _lastTime) < hideAfterSeconds;
	}
}
