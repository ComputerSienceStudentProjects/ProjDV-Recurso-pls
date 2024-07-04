using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private int initialHealth;
    private int baseDMG;
    private float health;
    private bool _hasMovedAlready = false;
    private bool _hasAttackedAlready = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool HasAttacked()
    {
        return _hasAttackedAlready;
    }

    public bool HasMoved()
    {
        return _hasMovedAlready;
    }

    public void SetMoved(bool hasMoved)
    {
        this._hasMovedAlready = hasMoved;
    }

    public void SetAttacked(bool hasAttacked)
    {
        this._hasAttackedAlready = hasAttacked;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public int getBaseDMG()
    {
        throw new System.NotImplementedException();
    }

    public void setBaseDmg(int baseDamage)
    {
        baseDamage = baseDMG;
    }
}
