using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibleUnit : VisibleObject
{
    public bool alwaysVisible = false;
    private bool isVisible = false;
	private bool hasBeenSeenThisFrame = false;
    private Damageable dmg;

    void Awake()
    {
        dmg = GetComponent<Damageable>();
    }

	// Update is called once per frame
	void FixedUpdate()
	{
        if (alwaysVisible)
            return;

        if (dmg && dmg.isDead)
            return;

		if (!isVisible && hasBeenSeenThisFrame)
		{
			setEnabled(true);
			isVisible = true;
		}
		else if (isVisible && !hasBeenSeenThisFrame)
		{
			setEnabled(false);
			isVisible = false;
		}

		hasBeenSeenThisFrame = false;
	}

	public override void setVisible()
	{
		hasBeenSeenThisFrame = true;
	}
}
