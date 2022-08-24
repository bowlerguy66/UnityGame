using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour {

	public int startX, startZ;
	public int spawnRadius;

	NetworkObject obj;
	bool spawnFlag;

	public override void OnStartClient() {
		base.OnStartClient();

		NetworkObject obj = base.LocalConnection.FirstObject;
		this.obj = obj;
		spawnFlag = true;

	}

	private void FixedUpdate() {

		if (spawnFlag) {
			float spawnDistance = Random.Range(0, spawnRadius * 10) / 10;
			int spawnAngleDeg = Random.Range(0, 360);
			float spawnAngle = spawnAngleDeg * (Mathf.PI / 180);
			float spawnX = startX + Mathf.Cos(spawnAngle) * spawnDistance;
			float spawnZ = startZ + Mathf.Sin(spawnAngle) * spawnDistance;
			float spawnY = TerrainManager.Instance.getBiomeManager().pollHeight(spawnX, spawnZ);
			RaycastHit hit;
			Vector3 origin = new Vector3(spawnX, spawnY + 3, spawnZ);
			Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity);
			if (spawnY != 0 && hit.collider != null) {
				Debug.Log($"Spawned player at {spawnX}, {spawnY + 3}, {spawnZ}");
				obj.transform.position = new Vector3(spawnX, spawnY + 3 , spawnZ);
				spawnFlag = false;
				this.obj = null;
			}
		}

	}

}
