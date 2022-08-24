using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerForces : MonoBehaviour {

    List<Vector3> forces;

    private void Awake() {
        forces = new List<Vector3>();    
    }

    private void FixedUpdate() {

        if (forces.Count <= 0) return;
        
        CharacterController controller = GetComponent<CharacterController>();

        List<Vector3> newForces = new List<Vector3>();
        Vector3 offset = new Vector3();
        foreach (Vector3 vec in forces) {
            offset += vec;
            //Vector3 agedVec = smartSubtract(vec, 0.1f);
            Vector3 agedVec = Vector3.Lerp(vec, Vector3.zero, 0.1f);
            if(agedVec.magnitude > 0.05f) newForces.Add(agedVec);
        }
        forces = newForces;

        controller.Move(offset);

    }

    public void addForce(Vector3 vec) {
        forces.Add(vec);
    }

    public Vector3 smartSubtract(Vector3 vec, float amt) {
        return new Vector3(smartSubtract(vec.x, amt), smartSubtract(vec.y, amt), smartSubtract(vec.z, amt));
    }

    private float smartSubtract(float val, float amt) {
        float newVal = Mathf.Abs(val) - amt;
        if (newVal < 0) return 0;
        return Mathf.Sign(val) * newVal;
    }

}
