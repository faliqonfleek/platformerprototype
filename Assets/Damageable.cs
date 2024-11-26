using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit; 
    
    Animator animator;

    [SerializeField]
    private int _maxHealth;

    [SerializeField]
    public int MaxHealth
    {
        get 
        {
            return _maxHealth;
        }
        set 
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    [SerializeField]
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool isAlive = true;

    [SerializeField]
    private bool isInvincible = false;
    
    private float timeSinceHit = 0;
    [SerializeField]
    private float invincibilityTimer = 0.25f;

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);
        }
    }

    // The velocity should not be changed while this is true but needs to be respected by other physics components like
    // the player controlled
    public bool LockVelocity
    {
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            // Notify other subscribed components that the damageable was hit to handle the knockback and such
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            
            return true;
        }
        
        // Unable to hit
        return false;
    }
}
