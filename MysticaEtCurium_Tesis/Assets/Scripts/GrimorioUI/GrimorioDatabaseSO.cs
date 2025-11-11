using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GrimorioDatabase", menuName = "Grimorio/Database")]

public class GrimorioDatabaseSO : ScriptableObject
{
    public List<GrimorioEntrySO> entries = new List<GrimorioEntrySO>();

    public GrimorioEntrySO GetEntryById(string id)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i].id == id)
                return entries[i];
        }
        return null;
    }

    public int GetIndexOf(string id)
    {
        for (int i = 0; i < entries.Count; i++)
            if (entries[i] != null && entries[i].id == id)
                return i;
        return -1;
    }
}
