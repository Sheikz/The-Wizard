using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Damageable), typeof(SpellCaster))]
public class PlayerController : MovingCharacter
{
	public GameObject dyingLight;

	private SpellCaster spellCaster;
	[HideInInspector]
	public Damageable damageable;
	private Vector3 wayPoint;           // Position to respawn after falling into a hole
	private Vector3 target;
	private List<int> spellCasted;
    private ItemLooter itemLooter;
    private bool autoMove = false;

	new void Awake()
	{
		base.Awake();
		spellCaster = GetComponent<SpellCaster>();
		damageable = GetComponent<Damageable>();
        itemLooter = GetComponentInChildren<ItemLooter>();
		spellCasted = new List<int>();
	}

	new protected void Start()
	{
        base.Start();

		setWayPoint(transform.position);
	}

	public void setWayPoint(Vector3 position)
	{
		wayPoint = position;
	}

	void FixedUpdate()
	{
        if (damageable.isDead)
            return;

		float inputX = InputManager.instance.getHorizontalInput();
		float inputY = InputManager.instance.getVerticalInput();
        if (inputX != 0 || inputY != 0)
            autoMove = false;

        if (autoMove)
        {
            direction = movement;
            updateAnimations();
            updateSortingOrder();
            return;
        }

		movement = new Vector2(inputX, inputY).normalized * movingSpeed;
        if (movement != Vector2.zero)
            direction = movement;

		updateAnimations();
        updateSortingOrder();
        if (!canMove || isFalling)
			return;
        if (buffReceiver.isStunned)
            return;

		rb.velocity = movement;
	}

    // Update is called once per frame
    void Update()
	{
        if (damageable.isDead)
            return;

        spellCasted.Clear();
        if (InputManager.instance.IsCommandDown(InputManager.Command.PrimarySpell))
        {
            if (InputManager.instance.isMouseHoveringItem && InputManager.instance.hoveringOverItem)
                itemLooter.pickup(InputManager.instance.hoveringOverItem);
        }
		if (InputManager.instance.IsCommandPressed(InputManager.Command.PrimarySpell))
		{
			if (!EventSystem.current.IsPointerOverGameObject()
                && !InputManager.instance.isMouseHoveringItem) // If clicking on UI or hovering over item
				spellCasted.Add(0);
		}

		if (InputManager.instance.IsCommandPressed(InputManager.Command.SecondarySpell))
			spellCasted.Add(1);

        if (InputManager.instance.IsCommandPressed(InputManager.Command.DefensiveSpell))
			spellCasted.Add(2);

        if (InputManager.instance.IsCommandPressed(InputManager.Command.Ultimate1))
			spellCasted.Add(3);

        if (InputManager.instance.IsCommandPressed(InputManager.Command.Ultimate2))
			spellCasted.Add(4);

        if (Input.GetButtonDown("Cancel"))
			UIManager.instance.switchMenu();

        if (InputManager.instance.IsCommandDown(InputManager.Command.SpellBook))
            UIManager.instance.spellWindowBySet.open();

        if (InputManager.instance.IsCommandDown(InputManager.Command.CharacterWindow))
            UIManager.instance.characterWindow.open();

        if (spellCasted.Count > 0)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;    // fix because camera see point at z = -5
        }

		foreach (int spell in spellCasted)
		{
            autoMove = false;

			if (spellCaster)
				spellCaster.castSpell(spell, target);
		}
    }

    internal IEnumerator moveToItemAndPickup(Item item, ItemLooter itemLooter)
    {
        autoMove = true;
        movement = (item.transform.position - itemLooter.transform.position).normalized * movingSpeed;
        rb.velocity = movement;
        while (((transform.position - item.transform.position).sqrMagnitude >= itemLooter.lootDistance * itemLooter.lootDistance))
        {
            if (!canAct || !canMove || isFalling || buffReceiver.isStunned)
                autoMove = false;
            if (!autoMove)
            {
                yield break;
            }
            movement = (item.transform.position - itemLooter.transform.position).normalized * movingSpeed;
            rb.velocity = movement;

            yield return null;
        }
        rb.velocity = Vector2.zero;
        movement = Vector2.zero;
        item.pullToSelf(itemLooter);
    }


    public override void receivesDamage()
	{
		if (UIManager.instance.screenMask.active)
			StartCoroutine(enableScreenMaskForOneFrame());
        SoundManager.instance.playSound("PlayerDamage");
	}

	private IEnumerator enableScreenMaskForOneFrame()
	{
		UIManager.instance.screenMask.gameObject.SetActive(true);
		yield return new WaitForSeconds(UIManager.instance.screenMask.hitMaskTime);
		UIManager.instance.screenMask.gameObject.SetActive(false);
	}

	public void hasFallen(float damageRatio)
	{
		transform.position = wayPoint;
		damageable.inflictDamageRatio(damageRatio);
		isFalling = false;
		transform.rotation = Quaternion.identity;
		transform.localScale = new Vector3(1, 1, 1);
	}

	public override void die()
	{
		Instantiate(dyingLight, transform.position + new Vector3(0, 0, -1), Quaternion.identity);
		GameObject centerText = UIManager.instance.centerText;
		ScreenMaskController screenMask = UIManager.instance.screenMask;
		UIManager.instance.showMenu(true);
        SoundManager.instance.playSound("PlayerDeath");

        if (centerText)
		{
			centerText.SetActive(true);
			centerText.GetComponent<Text>().color = Color.white;
			centerText.GetComponent<Text>().text = "Game Over";
		}

		if (screenMask)
		{
			screenMask.gameObject.SetActive(true);
			screenMask.GetComponent<Image>().CrossFadeAlpha(1.0f, 5, false);
		}
	}

	public void resetCooldowns()
	{
		spellCaster.resetCooldowns();
	}

	public void countAllLayers()
	{
		int[] layers = new int[32];
		for (int i =0; i < 32; i ++)
		{
			layers[i] = 0;
		}
		GameObject[] result = FindObjectsOfType<GameObject>() as GameObject[];
		foreach (GameObject obj in result)
		{
			layers[obj.layer]++;
		}
		for (int i = 0; i < 32; i++)
		{
			Debug.Log("Object of layer " + LayerMask.LayerToName(i) + " : " + layers[i]);
		}
	}
}

