using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Só existe no servidor. Rexistra qué cores están en uso e asigna cores únicas aos xogadores.
public class ColorRegistry : MonoBehaviour
{
    public static ColorRegistry Instance;

    // Cores en uso: clientId → índice de cor
    private readonly Dictionary<ulong, int> m_UsedColors = new();

    private void Awake()
    {
        Instance = this;
    }

    // Asigna un índice de cor libre ao clientId indicado.
    // Devolve -1 se non hai cores libres (non debería pasar con máx 6 xogadores e 6 cores).
    public int AssignColor(ulong clientId)
    {
        int index = GetFreeColorIndex();
        if (index >= 0)
            m_UsedColors[clientId] = index;
        return index;
    }

    // Cambia a cor dun xogador a outra cor libre (distinta da actual).
    public int ChangeColor(ulong clientId)
    {
        int current = m_UsedColors.ContainsKey(clientId) ? m_UsedColors[clientId] : -1;
        int index = GetFreeColorIndex(excludeIndex: current);
        if (index >= 0)
            m_UsedColors[clientId] = index;
        return index;
    }

    // Libera a cor dun xogador ao desconectarse.
    public void ReleaseColor(ulong clientId)
    {
        m_UsedColors.Remove(clientId);
    }

    private int GetFreeColorIndex(int excludeIndex = -1)
    {
        // Recoller índices ocupados
        var occupied = new HashSet<int>(m_UsedColors.Values);
        if (excludeIndex >= 0) occupied.Add(excludeIndex);

        // Buscar índices libres
        var free = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            if (!occupied.Contains(i))
                free.Add(i);
        }

        if (free.Count == 0) return -1;

        // Devolver un índice libre aleatorio
        return free[Random.Range(0, free.Count)];
    }
}