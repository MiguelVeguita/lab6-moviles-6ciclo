using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]

public class PlayerLobby : NetworkBehaviour
{
    public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    public NetworkVariable<int> headIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> eyesIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> torsoIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> armsIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> legsIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private PlayerSkin playerSkin;
    private int totalSkins;
    public override void OnNetworkSpawn()
    {
        playerSkin = GetComponent<PlayerSkin>();

        headIndex.OnValueChanged += OnSkinChanged;
        eyesIndex.OnValueChanged += OnSkinChanged;
        torsoIndex.OnValueChanged += OnSkinChanged;
        armsIndex.OnValueChanged += OnSkinChanged;
        legsIndex.OnValueChanged += OnSkinChanged;
        UpdatePlayerSkin();
    }
    public override void OnNetworkDespawn()
    {
        headIndex.OnValueChanged -= OnSkinChanged;
        eyesIndex.OnValueChanged -= OnSkinChanged;
        torsoIndex.OnValueChanged -= OnSkinChanged;
        armsIndex.OnValueChanged -= OnSkinChanged;
        legsIndex.OnValueChanged -= OnSkinChanged;
    }
    private void OnSkinChanged(int previousValue, int newValue)
    {
        UpdatePlayerSkin();
    }
    private void UpdatePlayerSkin()
    {
        if (playerSkin != null)
        {
            playerSkin.UpdateSkin(headIndex.Value, eyesIndex.Value, torsoIndex.Value, armsIndex.Value, legsIndex.Value);
        }
    }
    public void ToggleReady()
    {
        if (IsOwner)
        {
            SetReadyServerRpc(!isReady.Value);
        }
    }

    public void ChangePart(int partType, bool next)
    {
        if (!IsOwner) return;

        int direction = next ? 1 : -1;
        ChangePartServerRpc(partType, direction);
    }

    [ServerRpc]
    private void ChangePartServerRpc(int partType, int direction, ServerRpcParams rpcParams = default)
    {
        switch (partType)
        {
            case 0:
                headIndex.Value = GetNewIndex(headIndex.Value, direction, playerSkin.heads.Length);
                break;
            case 1:
                torsoIndex.Value = GetNewIndex(torsoIndex.Value, direction, playerSkin.torsos.Length);
                break;
            case 2:
                armsIndex.Value = GetNewIndex(armsIndex.Value, direction, playerSkin.arm.Length);
                break;
            case 3:
                legsIndex.Value = GetNewIndex(legsIndex.Value, direction, playerSkin.legs.Length);
                break;
            case 4:
                eyesIndex.Value = GetNewIndex(eyesIndex.Value, direction, playerSkin.eyes.Length);
                break;
        }
    }

    [ServerRpc]
    private void SetReadyServerRpc(bool ready, ServerRpcParams rpcParams = default)
    {
        isReady.Value = ready;
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.CheckIfAllReady();
        }
    }

    private int GetNewIndex(int currentIndex, int direction, int maxIndex)
    {
        if (maxIndex == 0) return 0;
        int newIndex = currentIndex + direction;
        if (newIndex < 0) newIndex = maxIndex - 1;
        return newIndex % maxIndex;
    }
}
