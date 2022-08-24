using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistenceBroadcaster : MonoBehaviour {
    void Update() {
		Debug.Log($"I am {gameObject.name} and I exist");
    }
}
