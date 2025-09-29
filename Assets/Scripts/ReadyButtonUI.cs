using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class ReadyButtonUI : MonoBehaviour
{
    public Button readyButton;           
    public TMP_Text readyButtonText;        
    private PlayerLobby localPlayerLobby;
    private Coroutine waitCoroutine;
    private void Start()
    {
        if (readyButton == null)return;
     
        readyButton.onClick.RemoveAllListeners();
        readyButton.interactable = false;
        UpdateButtonText(false);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        TryBindToLocalPlayer();
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        Unbind();
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton != null && clientId == NetworkManager.Singleton.LocalClientId)
        {
            TryBindToLocalPlayer();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (localPlayerLobby != null && localPlayerLobby.NetworkObject != null &&
            localPlayerLobby.NetworkObject.OwnerClientId == clientId)
        {
            Unbind();
        }
    }

    private void TryBindToLocalPlayer()
    {
        if (NetworkManager.Singleton == null) { return; }

        ulong localId = NetworkManager.Singleton.LocalClientId;
        Unity.Netcode.NetworkClient localClient;
        bool exists = NetworkManager.Singleton.ConnectedClients.TryGetValue(localId, out localClient);

        if (!exists || localClient == null || localClient.PlayerObject == null)
        {
            if (waitCoroutine == null)
            {
                waitCoroutine = StartCoroutine(WaitForLocalPlayer());
            }
            return;
        }

        PlayerLobby player = localClient.PlayerObject.GetComponent<PlayerLobby>();
        Bind(player);
    }

    private IEnumerator WaitForLocalPlayer()
    {
        while (true)
        {
            if (NetworkManager.Singleton == null) { yield break; }

            ulong localId = NetworkManager.Singleton.LocalClientId;
            Unity.Netcode.NetworkClient localClient;
            bool exists = NetworkManager.Singleton.ConnectedClients.TryGetValue(localId, out localClient);

            if (exists && localClient != null && localClient.PlayerObject != null)
            {
                PlayerLobby player = localClient.PlayerObject.GetComponent<PlayerLobby>();
                Bind(player);
                waitCoroutine = null;
                yield break;
            }

            yield return null;
        }
    }

    private void Bind(PlayerLobby player)
    {
        if (player == null) { return; }

        localPlayerLobby = player;

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(localPlayerLobby.ToggleReady);
        readyButton.interactable = true;

        localPlayerLobby.isReady.OnValueChanged += OnReadyValueChanged;

        UpdateButtonText(localPlayerLobby.isReady.Value);
    }

    private void Unbind()
    {
        if (localPlayerLobby == null) { return; }

        localPlayerLobby.isReady.OnValueChanged -= OnReadyValueChanged;
        readyButton.onClick.RemoveAllListeners();
        readyButton.interactable = false;

        localPlayerLobby = null;
        UpdateButtonText(false);
    }

    private void OnReadyValueChanged(bool previousValue, bool newValue)
    {
        UpdateButtonText(newValue);
    }
    private void UpdateButtonText(bool isReady)
    {
        if (readyButtonText == null) { return; }
        if (isReady)
        {
            readyButtonText.text = "Nolisto";
        }
        else
        {
            readyButtonText.text = "Listo";
        }
    }
}
