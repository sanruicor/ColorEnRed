using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private NetworkManager m_Net;

    private void Awake()
    {
        m_Net = GetComponent<NetworkManager>();
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
        if (GUILayout.Button("Host"))   m_Net.StartHost();
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