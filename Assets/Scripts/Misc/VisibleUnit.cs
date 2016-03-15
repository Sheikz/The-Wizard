using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibleUnit : VisibleObject
{
    private bool isVisible = false;
	private bool hasBeenSeenThisFrame = false;

	// Update is called once per frame
	void FixedUpdate()
	{
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
