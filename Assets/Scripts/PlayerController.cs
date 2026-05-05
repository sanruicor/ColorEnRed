using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // As 6 cores predefinidas
    private static readonly Color[] PredefinedColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.orange,
        Color.magenta
    };

    // NetworkVariable sincroniza a cor en todos os clientes automaticamente
    private NetworkVariable<int> m_ColorIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private Renderer m_Renderer;

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        // Subscribirse aos cambios de cor para actualizar o material
        m_ColorIndex.OnValueChanged += OnColorChanged;

        if (IsServer)
        {
            // O servidor asigna posición e cor aleatorias ao spawnear
            transform.position = GetRandomPosition();
            m_ColorIndex.Value = Random.Range(0, PredefinedColors.Length);
        }

        // Aplicar a cor actual (importante para clientes que se conectan tarde)
        ApplyColor(m_ColorIndex.Value);
    }

    public override void OnNetworkDespawn()
    {
        m_ColorIndex.OnValueChanged -= OnColorChanged;
    }

    // Chamado dende GameManager cando o xogador local preme o botón
    public void RequestColorChange()
    {
        ChangeColorRpc();
    }

    [Rpc(SendTo.Server)]
    private void ChangeColorRpc()
    {
        m_ColorIndex.Value = Random.Range(0, PredefinedColors.Length);
    }

    private void OnColorChanged(int oldIndex, int newIndex)
    {
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