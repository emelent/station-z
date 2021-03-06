using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public Camera mainCam;
	float shakeAmount = 0;
	Vector3 initPos;
	void Awake(){
		if(mainCam == null){
			mainCam = Camera.main;
		}
		initPos = mainCam.transform.position;
	}

	public void Shake(float amount, float length){
		shakeAmount = amount;
		InvokeRepeating("DoShake", 0, 0.01f);
		Invoke("StopShake", length);
	}

	void DoShake(){
		if(shakeAmount >0){
			Vector3 camPos = mainCam.transform.position;
			float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
			float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
			
			camPos.x += offsetX;
			camPos.y += offsetY;

			mainCam.transform.position = camPos;
		}
	}

	void StopShake(){
		CancelInvoke("DoShake");
		
		mainCam.transform.position = initPos;
	}
}
