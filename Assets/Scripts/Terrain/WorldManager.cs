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

	// Awful solution because of problems with deserialization of Item
	// Current workaround is to pass all members of the item individually to the servers method,
	//   construction and deconstruction will work here I guess
	public void DropItem(Item item, Vector3 location, Vector3 startingVel) {
		DropItem(item.getID(), item.getCount(), item.isStackable(), location, startingVel);
	}

	[ServerRpc(RequireOwnership = false)]
	public void DropItem(ItemID id, int count, bool stackable, Vector3 location, Vector3 startingVel) {
		Item item = new Item(id, count, stackable);
		GameObject groundObj = Instantiate(groundItemPrefab);
		groundObj.transform.position = location;
		groundObj.GetComponent<Rigidbody>().velocity = startingVel;
		ItemGround groundScript = groundObj.GetComponent<ItemGround>();
		groundScript.setItem(item);
		ServerManager.Spawn(groundObj);
	}

}
