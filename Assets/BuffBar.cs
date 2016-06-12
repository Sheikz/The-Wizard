using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffBar : MonoBehaviour 
{
    public BuffIcon buffIconPrefab;

    private List<BuffIcon> buffIcons;

    void Awake()
    {
        buffIcons = new List<BuffIcon>();
    }

	public void addBuff(Buff buff)
    {
        BuffIcon newBuff = Instantiate(buffIconPrefab);
        newBuff.buffName = buff.name;
        newBuff.setIcon(buff);
        newBuff.description = buff.description;
        newBuff.transform.SetParent(transform);
        buffIcons.Add(newBuff);
    }

    public void removeBuff(Buff buff)
    {
        for (int i = buffIcons.Count - 1; i >= 0; i--)
        {
            if (buffIcons[i].buffName == buff.name)
            {
                Destroy(buffIcons[i].gameObject);
                buffIcons.RemoveAt(i);
            }
        }
    }
}
