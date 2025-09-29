using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 
public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;
    public Transform[] Puntos;
    public GameObject panelskin;

    private void Awake()
    {
        Instance = this;

        if (Puntos == null || Puntos.Length == 0)
        {
            Puntos = new Transform[5];
            for (int i = 0; i < 5; i++)
            {
                GameObject posObj = new GameObject("puntito" + (i + 1));
                posObj.transform.position = new Vector3(i * 2.5f, 0, 0);
                Puntos[i] = posObj.transform;
            }
        }
    }
    public void ActivePanel()
    {
        panelskin.SetActive(true);
    }
    public void DesactivePanel()
    {
        panelskin.SetActive(false);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }
    public override void OnNetworkDespawn()
    {

       NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        
    }
    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            StartCoroutine(AssignPositionAfterSpawn(clientId));
        }
    }
    private IEnumerator AssignPositionAfterSpawn(ulong clientId)
    {
        yield return new WaitForEndOfFrame();

        AssignLobbyPosition(clientId);
    }
    private void AssignLobbyPosition(ulong clientId)
    {
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
        int assignedIndex = GetFreePositionIndex(clientId);

        if (assignedIndex >= 0 && assignedIndex < Puntos.Length)
        {
            playerObject.transform.position = Puntos[assignedIndex].position;
        }
    }

    private int GetFreePositionIndex(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            return 0;
        }

        int totalPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
        if (totalPlayers <= Puntos.Length)
        {
            return totalPlayers - 1; 
        }

        return -1;
    }

    public void CheckIfAllReady()
    {
        int totalPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
        int readyPlayers = 0;

        for (int i = 0; i < totalPlayers; i++)
        {
            PlayerLobby player = NetworkManager.Singleton.ConnectedClientsList[i].PlayerObject.GetComponent<PlayerLobby>();
            if (player != null && player.isReady.Value)
            {
                readyPlayers++;
            }
        }

        if (IsServer && readyPlayers == totalPlayers)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }

    }
}
