using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[GenerateSerializationForTypeAttribute(typeof(System.String))]
public class PlayerNameNetwork : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerName.Value = PlayerPrefs.GetString("PlayerName", "Player");
        }
    }


}
