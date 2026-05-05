using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MaxPlayers = 6;
    private NetworkManager m_Net;
    private bool m_WasRejected = false;

    private void Awake()
    {
        m_Net = GetComponent<NetworkManager>();
        // Asignar el callback ANTES de que arranque nada
        m_Net.ConnectionApprovalCallback = ApproveConnection;
        m_Net.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (m_Net != null)
            m_Net.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // En el cliente, si nos desconectan antes de haber entrado, es que la sala estaba llena
        if (!m_Net.IsServer && clientId == m_Net.LocalClientId)
        {
            m_WasRejected = true;
        }
    }

    private void ApproveConnection(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        // ConnectedClientsIds ya incluye al host, por eso comparamos con MaxPlayers
        bool roomFull = m_Net.ConnectedClientsIds.Count >= MaxPlayers;
        response.Approved = !roomFull;
        response.CreatePlayerObject = response.Approved;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 220, 150));

        if (!m_Net.IsClient && !m_Net.IsServer)
        {
            DrawConnectionButtons();
        }
        else
        {
            DrawStatusLabel();
            DrawColorButton();
        }

        GUILayout.EndArea();
    }

    private void DrawConnectionButtons()
    {
        if (m_WasRejected)
        {
            GUILayout.Label("Sala llena! No se pudo conectar.");
            if (GUILayout.Button("Volver"))
                m_WasRejected = false;
            return;
        }

        if (GUILayout.Button("Host")) m_Net.StartHost();
        if (GUILayout.Button("Client")) m_Net.StartClient();
        if (GUILayout.Button("Server")) m_Net.StartServer();
    }

    private void DrawStatusLabel()
    {
        string mode = m_Net.IsHost ? "Host" : m_Net.IsServer ? "Server" : "Client";
        GUILayout.Label($"Modo: {mode}");
    }

    private void DrawColorButton()
    {
        if (GUILayout.Button("Cambiar cor"))
        {
            var localPlayer = m_Net.SpawnManager.GetLocalPlayerObject();
            if (localPlayer != null)
                localPlayer.GetComponent<PlayerController>().RequestColorChange();
        }
    }
}