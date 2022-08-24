using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Transform player;

    public float fieldOfView = 110f;

    public float hSpeed = 300f;
    public float vSpeed = 300f;

    float yaw = 0f;
    float pitch = 0f;

    public float cameraDistance = 7.5f;
    public bool thirdPerson = false;

    public float fpyOffset = 2;

    private void Awake() {
        FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifier_OnFirstObjectSpawned;
        UIManager.UIModeChange += UIManager_UIModeChange;
    }

    private void OnDestroy() {
        FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifier_OnFirstObjectSpawned;
		UIManager.UIModeChange -= UIManager_UIModeChange;
    }

    private void FirstObjectNotifier_OnFirstObjectSpawned(GameObject obj) {

        this.player = obj.transform;

        Camera cam = GetComponent<Camera>();
        cam.fieldOfView = fieldOfView;

        obj.GetComponent<PlayerMovement>().setPlayerCamera(transform);
        obj.GetComponentInChildren<Canvas>().GetComponent<LookAtCamera>().SetCamera(cam);

    }

    private void UIManager_UIModeChange(bool uiMode) {
        if (uiMode) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update() {

        //Debug.Log($"player pos: {player.position.x}, {player.position.y}, {player.position.z}");

        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.F5)) {
            setThirdPerson(!thirdPerson);
        }

        if (!UIManager.Instance.inUIMode()) {
            yaw += hSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            float oldPitch = pitch; // Store the pitch in case we go out of bounds (> 90 || < -90)
            pitch += vSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime * -1; // Invert vertical
            if (pitch > 90 || pitch < -90) pitch = oldPitch;
        }


        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        if (!thirdPerson)
        {
            updateFirstPerson();
        }
        else
        {
            updateThirdPerson();
        }

    }

    void updateFirstPerson() {
        transform.position = player.position + new Vector3(0, fpyOffset, 0);
    }

    void updateThirdPerson() {

        float horAngle = transform.eulerAngles.y * (Mathf.PI / 180);
        float verAngle = transform.eulerAngles.x * (Mathf.PI / 180);

        float horDist = cameraDistance * Mathf.Cos(verAngle);

        float xOff = player.position.x - horDist * Mathf.Sin(horAngle);
        float zOff = player.position.z - horDist * Mathf.Cos(horAngle);
        float yOff = player.position.y + cameraDistance * Mathf.Sin(verAngle);
        transform.position = new Vector3(xOff, yOff, zOff);

    }

    public void setThirdPerson(bool thirdPerson) {
        this.thirdPerson = thirdPerson;
        Camera cam = GetComponent<Camera>();
        if (thirdPerson) {
            cam.fieldOfView = fieldOfView;
            transform.Translate(new Vector3(-cameraDistance, 0f, 0f));
        } else {
            cam.fieldOfView = fieldOfView + 20;
        }
    }

}
