﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class MovingCharacter : MonoBehaviour
{
    public float baseSpeed;
    public float speed;

    protected Vector2 movement;     // Direction in which the character moves

    protected Vector2 direction;    // Direction in which he is facing
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    private bool isSlowed = false;
    protected bool canMove = true;      // Can he move?
    [HideInInspector]
    public bool canAct = true;       // Can he do anything at all?
    [HideInInspector]
    public bool isRooted = false;
    [HideInInspector]
    public bool isStunned = false;

    protected bool isFalling = false;
    [HideInInspector]
    public bool isFlying = false;
    private float spinningSpeed = 10f;
    private float fallingDuration = 3f;

    // Use this for initialization
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void updateAnimations()
    {
        if (isStunned)
            return;

        if (anim)
        {
            if (movement != Vector2.zero)
            {
                anim.SetBool("Moving", true);
            }
            else
                anim.SetBool("Moving", false);

            anim.SetFloat("DirectionX", direction.x);
            anim.SetFloat("DirectionY", direction.y);
        }
    }

    public void applySlow(float slowPercent, float duration)
    {
        if (isSlowed)
            return;
        if (slowPercent == 1)
            StartCoroutine(stunForSeconds(duration));
        else
            StartCoroutine(slowForSeconds(slowPercent, duration));
    }

    private IEnumerator slowForSeconds(float slowPercent, float duration)
    {
        float originalSpeed = speed;
        speed *= slowPercent;
        isSlowed = true;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
        isSlowed = false;
    }

    /// <summary>
    /// Is falling into a hole
    /// </summary>
    public void startFalling(float damageRatio)
    {
        if (isFlying)
            return;
        if (!isFalling)
        {
            StartCoroutine(fallAnimation(spinningSpeed, fallingDuration, damageRatio));
            isStunned = true;
        }
        isFalling = true;
    }

    /// <summary>
    /// Can the character move?
    /// </summary>
    /// <param name="value"></param>
    public void enableMovement(bool value)
    {
        canMove = value;
        if (!value)
        {
            rb.velocity = Vector3.zero;
            //movement = Vector2.zero;
        }
    }

    /// <summary>
    /// Can the character do anything at all?
    /// </summary>
    /// <param name="value"></param>
    public void enableAction(bool value)
    {
        canAct = value;
        enableMovement(value);
    }

    private IEnumerator fallAnimation(float spinningSpeed, float duration, float damageRatio = 0f)
    {
        float startingTime = Time.time;
        circleCollider.enabled = false;
        while (Time.time - startingTime < duration)
        {
            transform.Rotate(0, 0, spinningSpeed);
            float scale = Mathf.Lerp(1f, 0f, (Time.time - startingTime)/duration);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForFixedUpdate();
        }
        circleCollider.enabled = true;
        PlayerController pc = GetComponent<PlayerController>();
        Damageable dmg = GetComponent<Damageable>();
        if (pc)
            pc.hasFallen(damageRatio);
        else if (dmg)
            dmg.die();
        else
            Destroy(gameObject);
    }

    public void stopMovementFor(float duration)
    {
        StartCoroutine(stopMovementRoutine(duration));
    }

    private IEnumerator stopMovementRoutine(float duration)
    {
        enableMovement(false);
        yield return new WaitForSeconds(duration);
        enableMovement(true);
    }

    public void killAfterSeconds(float duration)
    {
        StartCoroutine(coroutineKillAfterSeconds(duration));
    }

    private IEnumerator coroutineKillAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        die();
        Destroy(gameObject);
    }

    public void stunFor(float stunDuration)
    {
        if (isStunned)
            return;

        StartCoroutine(stunForSeconds(stunDuration));
    }

    IEnumerator stunForSeconds(float duration)
    {
        isStunned = true;
        float savedAnimSpeed = anim.speed;
        anim.speed = 0;
        yield return new WaitForSeconds(duration);
        anim.speed = savedAnimSpeed;
        isStunned = false;
    }

    public abstract void die();

    public abstract void receivesDamage();
}
