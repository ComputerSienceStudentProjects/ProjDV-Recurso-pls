using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


/**
 * <summary>
 *  
 * </summary>
 * <version>
 *  <date here>/07/2024
 * </version>
 * <author>
 *  Diogo Capela (<Email here>)
 * </author>
 */
public class AIControllable : MonoBehaviour
{
    [SerializeField] private int initialHealth;
    [SerializeField] private int baseDMG;
    [SerializeField] private float health;
    [SerializeField] FloatingHealth floatingHealthBar;
    private bool _hasMovedAlready = false;
    private bool _hasAttackedAlready = false;
    private float leastDistance = 100f;
    private GameObject currentTarget;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private float movementRange = 15.0f;
    private Vector3 destination;
    private NavMeshAgent _agent;
    private Animator _animator;
    private LineRenderer _pathLineRenderer;

    [SerializeField] GameEvent AddToLog;

    private string logText;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _pathLineRenderer = GetComponent<LineRenderer>();
    }

    public void ResetHealth()
    {
        health = initialHealth;
    }

    public void TakeDamage(float damage)
    {
        floatingHealthBar.UpdateHealthBar(health, initialHealth);
        health -= damage;
        _animator.SetTrigger("take_damage");
        if (health <= 0)
            _animator.SetTrigger("death");
    }

    public void Die()
    {
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
        return baseDMG;
    }

    public void setBaseDmg(int baseDamage)
    {
        baseDamage = baseDMG;
    }

    public int FindClosestTarget(List<GameObject> playerParty)
    {
        int closestIndex = -1;
        for (int i = 0; i < playerParty.Count; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, playerParty[i].transform.position);
            if (distance < leastDistance) { leastDistance = distance; currentTarget = playerParty[i]; closestIndex = i; }
        }

        if (closestIndex == -1)
        {
            Debug.Log(string.Format("Ai {0} did not find a close enemy", gameObject.name));
        }

        Debug.Log(string.Format("Ai {0} chose closest target as {1} with instanceID {2}", gameObject.name, playerParty[closestIndex].name, playerParty[closestIndex].GetInstanceID()));

        return closestIndex;
    }

    private async Task ThreatTarget()
    {
        //Check distance to target
        //If in attack distance rotate, calculate odds and play animation of attack
        //If not in attack distance move towards player to the maximum movementRange or to the maximum attackRange



        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        Debug.Log(distance + "," + direction);
        if (distance <= attackRange)
        {
            Debug.Log("We can attack");
            await RotateTowards(direction);
            //do attack here then return
            return;
        }
        else if (distance > attackRange && distance <= movementRange + attackRange)
        //doing this addition should let the ai attack if it reached the end of its movementRange but cant still reach the player with its attackRange*
        //*i think, needs testing.
        {
            Debug.Log("We're too far to attack but we can move towards the target and attack");
            logText =
                string.Format("Ai {0} moved to attack {1}", gameObject.name, currentTarget.gameObject.name);
            AddToLog.SetArgument("text", typeof(string), logText);
            AddToLog.Raise();
            await RotateTowards(direction);
            destination = transform.position + direction * (distance - attackRange);
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            DrawPath(path);
        }
        else
        {
            Debug.Log("The target is too far away so we're moving towards it, however we will not reach attacking distance");
            logText =
                string.Format("Ai {0} moves closer to {1}", gameObject.name, currentTarget.gameObject.name);
            AddToLog.SetArgument("text", typeof(string), logText);
            AddToLog.Raise();
            await RotateTowards(direction);
            destination = transform.position + direction * movementRange;
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            DrawPath(path);
        }
        _animator.SetBool("is_walking", true);
        await WaitUntilReachedTarget(destination, direction);
    }

    private async Task RotateTowards(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            await Task.Yield();
        }

        transform.rotation = lookRotation;
    }

    private async Task WaitUntilReachedTarget(Vector3 destination, Vector3 direction)
    {
        _agent.destination = destination;
        while (!ReachedDestinationOrGaveUp())
            await Task.Yield();
    }

    private bool ReachedDestinationOrGaveUp()
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    _animator.SetBool("is_walking", false);
                    _pathLineRenderer.positionCount = 0;
                    return true;
                }
            }
        }
        return false;
    }
    public async Task PerformTurn()
    {
        if (currentTarget == null)
        {
            FindClosestTarget(new List<GameObject>(GameObject.FindGameObjectsWithTag("Player")));
        }
        if (currentTarget == null) return;

        if (health >= 50)
        {
            await ThreatTarget();
            await Attack();
        }
        else
        {
            await Flee(); Heal();
        }

        await FinishTurn();
    }

    private bool isHealingOrAttacking = false;
    [SerializeField] private float healPower = 5.0f;

    private async Task FinishTurn()
    {
        while (isHealingOrAttacking)
        {
            await Task.Yield();
        }
    }

    private async Task Attack()
    {
        //  We're in range of attack within a decent health amount so trading attacks is a decent idea, below 50hp the hit chance should be very low so it makes sense to retreat and heal up,
        //the ai will not be able to run away further then the player can move unless the player also chose to desengage the ai so the heal can be canceled though that might not be the smartest choice.
        //  Will need balancing most likely if I can implement the heal after a turn of not being attacked mechanic, for now it will be somewhat janky.
        if (Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            await RotateTowards(direction);
            _animator.SetTrigger("attack");
            isHealingOrAttacking = true;

        }
    }

    private async Task Flee()
    {
        //  We're low so we're gonna retreat and heal, the heal is low so if the player chases he can kill even if the ai is healing, also probably should have a cooldown
        //  Healing 15 health points every 3 turns should be fine since the player attacks for 10 damage so well still be 15 points in the red while slowing down the player
        //  Ideally we would have a flag to only be able to heal if we havent been attacked in the previous turn and we should heal for longer,
        //to make the player have to target the fleeing ai to prevent it from coming back stronger after.
        destination = FindFurthestValidFleePoint(36);
        Vector3 direction = (transform.position - destination).normalized;
        _animator.SetBool("is_walking", true);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        DrawPath(path);
        await WaitUntilReachedTarget(destination, direction);


    }
    Vector3 FindFurthestValidFleePoint(int samplePoints)
    {
        Vector3 bestFleePoint = Vector3.zero;
        float maxDistance = 0.0f;

        for (int i = 0; i < samplePoints; i++)
        {
            // Calculate the direction for the current sample point
            float angle = i * Mathf.PI * 2 / samplePoints;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            // Calculate the potential flee point
            Vector3 potentialFleePoint = currentTarget.transform.position + direction * movementRange;

            // Check if the potential flee point is on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialFleePoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                float distanceToPlayer = Vector3.Distance(hit.position, currentTarget.transform.position);
                if (distanceToPlayer > maxDistance)
                {
                    maxDistance = distanceToPlayer;
                    bestFleePoint = hit.position;
                }
            }
        }

        return bestFleePoint;
    }

    public void DoDamage()
    {
        //Hit chance will be linear with hp so 100hp means 100% hit chance, 50hp means 50% hit chance, below 50hp we heal because its less then a 50/50 chance to hit
        float hitChance = UnityEngine.Random.Range(1.0f, 100.0f);
        bool willHit = hitChance >= initialHealth - health;
        if (willHit)
        {
            currentTarget.GetComponent<PlayerController>().TakeDamage(getBaseDMG());
            Debug.Log("Did hit!");
            Debug.Log("Hit chance: " + hitChance + ", hit chance needed:" + (initialHealth - health));
            logText = gameObject.name + " has attacked " + currentTarget.name + " for " + getBaseDMG() + "HP), with " + hitChance + "% chance of hitting.";
            AddToLog.SetArgument("text", typeof(string), logText);
            AddToLog.Raise();
        }
        else
        {
            Debug.Log("Did not hit!");
            Debug.Log("Hit chance: " + hitChance + ", hit chance needed:" + (initialHealth - health));
            logText = gameObject.name + " has failed to attack " + currentTarget.name + " for " + getBaseDMG() + "HP), with " + hitChance + "% chance of hitting.";
            AddToLog.SetArgument("text", typeof(string), logText);
            AddToLog.Raise();
        }
        isHealingOrAttacking = false;
    }

    private void Heal()
    {
        _animator.SetTrigger("heal");
        isHealingOrAttacking = true;
        logText = gameObject.name + " has fled and healed for (" + healPower + " HP).";
        AddToLog.SetArgument("text", typeof(string), logText);
        AddToLog.Raise();
    }

    public void DoHeal()
    {
        health += healPower;
        isHealingOrAttacking = false;
    }

    private void DrawPath(NavMeshPath path)
    {
        var positions = path.corners;
        _pathLineRenderer.positionCount = positions.Length;
        for (int i = 0; i < positions.Length; i++)
        {
            _pathLineRenderer.SetPosition(i, positions[i] + new Vector3(0, 0.5f, 0));
        }

    }
}
