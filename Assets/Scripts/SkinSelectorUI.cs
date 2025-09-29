using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class SkinSelectorUI : MonoBehaviour
{
    [Header("Head Buttons")]
    public Button nextHeadButton;
    public Button previousHeadButton;

    [Header("eyes Buttons")]
    public Button nextEyesButton;
    public Button previousEyesButton;

    [Header("Torso Buttons")]
    public Button nextTorsoButton;
    public Button previousTorsoButton;

    [Header("Arms Buttons")]
    public Button nextArmsButton;
    public Button previousArmsButton;

    [Header("Legs Buttons")]
    public Button nextLegsButton;
    public Button previousLegsButton;

    private PlayerLobby localPlayerLobby;
    private Coroutine waitCoroutine;
    void Start()
    {
        SetAllButtonsInteractable(false);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        TryBindToLocalPlayer();
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        Unbind();
    }
    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton != null && clientId == NetworkManager.Singleton.LocalClientId)
        {
            TryBindToLocalPlayer();
        }
    }
    private void TryBindToLocalPlayer()
    {
        if (waitCoroutine != null) StopCoroutine(waitCoroutine);
        waitCoroutine = StartCoroutine(WaitForLocalPlayer());
    }
    private IEnumerator WaitForLocalPlayer()
    {
        while (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient || NetworkManager.Singleton.LocalClient == null)
        {
            yield return null;
        }

        while (NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            yield return null;
        }

        PlayerLobby player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerLobby>();
        if (player != null)
        {
            Bind(player);
        }

        waitCoroutine = null;
    }
    private void Bind(PlayerLobby player)
    {
        localPlayerLobby = player;

        nextHeadButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(0, true));
        previousHeadButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(0, false));

        nextTorsoButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(1, true));
        previousTorsoButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(1, false));

        nextArmsButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(2, true));
        previousArmsButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(2, false));

        nextLegsButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(3, true));
        previousLegsButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(3, false));

        nextEyesButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(4, true));
        previousEyesButton?.onClick.AddListener(() => localPlayerLobby.ChangePart(4, false));
       
    SetAllButtonsInteractable(true);
    }
    private void Unbind()
    {
        if (localPlayerLobby == null) return;

        nextHeadButton?.onClick.RemoveAllListeners();
        previousHeadButton?.onClick.RemoveAllListeners();
        nextTorsoButton?.onClick.RemoveAllListeners();
        previousTorsoButton?.onClick.RemoveAllListeners();
        nextArmsButton?.onClick.RemoveAllListeners();
        previousArmsButton?.onClick.RemoveAllListeners();
        nextLegsButton?.onClick.RemoveAllListeners();
        previousLegsButton?.onClick.RemoveAllListeners();
        nextEyesButton?.onClick.RemoveAllListeners();
        previousEyesButton?.onClick.RemoveAllListeners();

        SetAllButtonsInteractable(false);
        localPlayerLobby = null;
    }
    private void SetAllButtonsInteractable(bool interactable)
    {
        if (nextHeadButton) nextHeadButton.interactable = interactable;
        if (previousHeadButton) previousHeadButton.interactable = interactable;
        if (nextTorsoButton) nextTorsoButton.interactable = interactable;
        if (previousTorsoButton) previousTorsoButton.interactable = interactable;
        if (nextArmsButton) nextArmsButton.interactable = interactable;
        if (previousArmsButton) previousArmsButton.interactable = interactable;
        if (nextLegsButton) nextLegsButton.interactable = interactable;
        if (previousLegsButton) previousLegsButton.interactable = interactable;
        if (nextEyesButton) nextEyesButton.interactable = interactable;
        if (previousEyesButton) previousEyesButton.interactable = interactable;
    }
}