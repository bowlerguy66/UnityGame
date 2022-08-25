using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {

    Transform playerCamera;

    public float sprintSpeed = 12;
    public float walkSpeed = 7;

    public float jumpForce = 10; // 10^6

    public int defaultJumpCount = 2;
    public float jumpTolerance = 0.7f;

    float fDir = 0;
    float sDir = 0;
    bool moveIO;

    float angle;

    // The amount of jumps that the player has
    int jumps = 0;
    bool jumpFlag = false;

    public bool flying;

    private void Awake() {
    
        flying = false;

        // Calculate the correct center position for the cc so that the player doesn't float
        CharacterController controller = GetComponent<CharacterController>();
        controller.center = new Vector3(0, controller.center.y + controller.skinWidth, 0);

    }

    void Update() {

        fDir = 0;
        sDir = 0;

        if (playerCamera == null) return;

        if (!UIManager.Instance.inUIMode()) {
            if (Input.GetKey("w")) fDir += 1;
            if (Input.GetKey("s")) fDir += -1;
            if (Input.GetKey("d")) sDir += 1;
            if (Input.GetKey("a")) sDir += -1;
        }

        moveIO = fDir != 0 || sDir != 0;

        Quaternion angleQ = Quaternion.Euler(playerCamera.eulerAngles);
        float angleDeg = playerCamera.eulerAngles.y; // Angle is in degrees
        Vector2 forward = new Vector2(0, 1); // Represents pi/2 on the unit circle
        Vector2 modVec = new Vector2(sDir, fDir); // fDir is in the y coordinate because straight forward will equal pi/2;

        if (moveIO) {
            float angleRaw = angleDeg * (Mathf.PI / 180);
            float angleMod = angleBetween(forward, modVec);
            if (sDir != 0) angleMod *= sDir; // Change the horizontal direction

            angleRaw += angleMod;
            angle = angleRaw;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0 && !flying) {
            jumpFlag = true;
        }

        // Update flying or not
        if (Input.GetKeyDown(KeyCode.Z)) {
            flying = !flying;
        }

    }

    // Update is called once per frame
    // Used FixedUpdate for physics
    void FixedUpdate() {
        if (playerCamera == null) return;
        if (!flying) {
            updateGround();
        } else {
            updateFlying();
        }
    }

    void oldupdateGround() {

        //float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        //float velX = rb.velocity.x;
        //float velY = rb.velocity.y;
        //float velZ = rb.velocity.z;
        ////float velX = 0;
        ////float velY = 0;
        ////float velZ = 0;

        //// XZ movement
        //if (fDir != 0 || sDir != 0) {
        //    velX = Mathf.Sin(this.angle) * speed;
        //    velZ = Mathf.Cos(this.angle) * speed;
        //}

        //// Jumping
        //if (jumpFlag && jumps > 0) {
        //    velY = 0;
        //    rb.AddForce(0, jumpForce, 0);
        //    jumpFlag = false;
        //    jumps--;
        //}

        //rb.velocity = new Vector3(velX, velY, velZ);

    }

    void updateGround() {

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        //float offX = rb.velocity.x;
        //float offZ = rb.velocity.z;
        float offX = 0;
        float offZ = 0;

        // XZ movement
        if (fDir != 0 || sDir != 0) {
            offX = Mathf.Sin(this.angle) * speed;
            offZ = Mathf.Cos(this.angle) * speed;
        }

        CharacterController controller = GetComponent<CharacterController>();
        Vector3 offset = new Vector3(offX, Physics.gravity.y, offZ) * Time.deltaTime;
        controller.Move(offset);

        if (controller.isGrounded) jumps = defaultJumpCount;

        // Jumping
        if (jumpFlag && jumps > 0) {
            CharacterControllerForces ccf = GetComponent<CharacterControllerForces>();
            ccf.addForce(new Vector3(0, jumpForce, 0));
            jumpFlag = false;
            jumps--;
        }

    }

    void updateFlying() {

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        speed *= 2.5f;

        float velX = 0;
        float velY = 0;
        float velZ = 0;

        if (fDir != 0 || sDir != 0) {
            velX = Mathf.Sin(this.angle) * speed;
            velZ = Mathf.Cos(this.angle) * speed;
        }

        if (Input.GetKey(KeyCode.Space) && flying) {
            velY = speed;
        } else if (Input.GetKey(KeyCode.LeftControl) && flying) {
            velY = -speed;
        }

        CharacterController controller = GetComponent<CharacterController>();
        controller.Move(new Vector3(velX, velY, velZ));

    }

    void OnCollisionEnter(Collision col) {
        Debug.Log("yeah");
        int contactCt = col.contactCount;
        for (int i = 0; i < contactCt; i++) {
            ContactPoint p = col.GetContact(i);
            //Debug.Log("normY: " + p.normal.y);
            float normY = p.normal.y;
            if (normY >= jumpTolerance && normY <= (2 - jumpTolerance)) {
                jumps = defaultJumpCount;
                Debug.Log("Updatec docle");
                break;
            }
        }
    }

    // dot = cos(t)*lengthA*lengthB
    // t = acos(dot / (lengthA*lengthB))
    public float angleBetween(Vector2 a, Vector2 b) {
        return Mathf.Acos(dot(a, b) / (a.magnitude * b.magnitude));
    }

    public float dot(Vector2 a, Vector2 b) {
        return a.x * b.x + a.y * b.y;
    }

    public bool onGround() {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 1f);
    }

    public void setPlayerCamera(Transform cameraTransform) {
        this.playerCamera = cameraTransform;
    }

}
