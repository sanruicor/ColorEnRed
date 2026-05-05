using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static readonly Color[] PredefinedColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.orange,
        Color.magenta
    };

    private NetworkVariable<int> m_ColorIndex = new NetworkVariable<int>( -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Renderer m_Renderer;

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        m_ColorIndex.OnValueChanged += OnColorChanged;

        if (IsServer)
        {
            transform.position = GetRandomPosition();
            // Pedir cor única ao rexistro
            int assignedColor = ColorRegistry.Instance.AssignColor(OwnerClientId);
            if (assignedColor >= 0)
                m_ColorIndex.Value = assignedColor;
        }

        if (m_ColorIndex.Value >= 0)
            ApplyColor(m_ColorIndex.Value);
    }

    public override void OnNetworkDespawn()
    {
        m_ColorIndex.OnValueChanged -= OnColorChanged;

        // Liberar a cor ao desconectarse (só no servidor)
        if (IsServer)
            ColorRegistry.Instance.ReleaseColor(OwnerClientId);
    }

    public void RequestColorChange()
    {
        ChangeColorRpc();
    }

    [Rpc(SendTo.Server)]
    private void ChangeColorRpc()
    {
        // Pedir unha cor libre diferente á actual
        int newColor = ColorRegistry.Instance.ChangeColor(OwnerClientId);
        if (newColor >= 0)
            m_ColorIndex.Value = newColor;
    }

    private void OnColorChanged(int oldIndex, int newIndex)
    {
        if (newIndex >= 0)
            ApplyColor(newIndex);
    }

    private void ApplyColor(int index)
    {
        if (m_Renderer != null)
            m_Renderer.material.color = PredefinedColors[index];
    }

    private static Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f));
    }
}