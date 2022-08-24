using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : NetworkBehaviour {

	public GameObject groundItemPrefab;

    List<NetworkConnection> players;

    private void Start() {
        players = new List<NetworkConnection>();        
    }

    [Server]
    public List<NetworkConnection> getPlayers() {
        return players;
    }

    public override void OnSpawnServer(NetworkConnection connection) {
        base.OnSpawnServer(connection);
        players.Add(connection);
    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.L)) {
            Debug.Log("Curren't player positions: ");
            foreach (NetworkConnection connection in getPlayers()) {
                NetworkObject player = connection.FirstObject;                
                Transform transform = player.transform;
                Vector3 pos = transform.position;
                Debug.Log("  " + connection.ClientId + ": (" + pos.x + ", " + pos.y + ", " + pos.z + ")");

            }
        }

    }

	public void DropItem(Item item, Vector3 location) {
		GameObject groundObj = Instantiate(groundItemPrefab);
		groundObj.transform.position = location;
		ItemGround groundScript = groundObj.GetComponent<ItemGround>();
		groundScript.setItem(item);
		ServerManager.Spawn(groundObj);
	}

}
