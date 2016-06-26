using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffBar : MonoBehaviour 
{
    public BuffIcon buffIconPrefab;

    public List<BuffIcon> buffIcons;
    private BuffsReceiver buffReceiver; // The buff receiver this buff bar show buffs for

    void Awake()
    {
        buffIcons = new List<BuffIcon>();
    }

    void Start()
    {
        buffReceiver = GameManager.instance.hero.GetComponent<BuffsReceiver>();
    }

    void FixedUpdate()
    {
        List<Buff> buffList = buffReceiver.getBuffList();
        int i;
        for (i=0; i < buffList.Count; i++)
        {
            if (buffIcons.Count - 1 >= i)
            {
                buffIcons[i].refresh(buffList[i]);
            }
            else
            {
                addBuff(buffList[i]);
            }
        }
        for (; i < buffIcons.Count; i++) // Disabling the icons which no longer have a buff to show
        {
            buffIcons[i].enable(false);
        }
    }

	public void addBuff(Buff buff)
    {
        BuffIcon newBuff = Instantiate(buffIconPrefab);
        newBuff.buff = buff;
        newBuff.setIcon(buff);
        newBuff.transform.SetParent(transform);
        buffIcons.Add(newBuff);
    }
}
