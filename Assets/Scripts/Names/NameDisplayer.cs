using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameDisplayer : NetworkBehaviour {

    [SerializeField]
    private TextMeshPro _text;

    public override void OnStartClient() {
        base.OnStartClient();
        SetName();
        PlayerNameTracker.OnNameChange += PlayerNameTracker_OnNameChange;
    }

    public override void OnStopClient() {
        base.OnStopClient();
        PlayerNameTracker.OnNameChange -= PlayerNameTracker_OnNameChange;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner) {
        base.OnOwnershipClient(prevOwner);
        SetName();
    }

    private void PlayerNameTracker_OnNameChange(NetworkConnection arg1, string arg2) {
        if (arg1 != base.Owner) return;
        SetName();
    }

    private void SetName() {
        string result = null;
        //Owner does not exist
        if (base.Owner.IsValid) {
            result = PlayerNameTracker.GetPlayerName(base.Owner);
        }

        if (string.IsNullOrEmpty(result)) {
            result = "Unset";
        }

        _text.text = result;

    }

}
