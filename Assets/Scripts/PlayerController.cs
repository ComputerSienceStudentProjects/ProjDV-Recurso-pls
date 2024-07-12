using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject movementCircleObject;
    [SerializeField] private int baseDamage;
    [SerializeField] private float maxAttackRange;
    [SerializeField] private float maxMovementRange;
    [SerializeField] private int initialHealth;
    [SerializeField] private bool bSelected = false;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material walkingMaterial;
    [SerializeField] private GameObject castPoint;
    [SerializeField] private CameraController cameraController;
    
    private bool _hasMovedAlready = false;
    private bool _hasAttackedAlready = false;
    private Animator _animator;
    private LineRenderer _pathLineRenderer;
    private bool bShouldCheckIfReached = false;
    [SerializeField] private float health;
    private NavMeshAgent _agent;
    
    
    [SerializeField] private GameEvent updateHPBarsEvent;
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        cameraController = Camera.main.gameObject.GetComponent<CameraController>();
        
       
        
        _pathLineRenderer = GetComponent<LineRenderer>();
        health = initialHealth;
        
        
        selectedMaterial = meshRenderer.materials[1];
        walkingMaterial = meshRenderer.materials[2];
    }
    
    private bool ReachedDestinationOrGaveUp()
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    
    private void Update()
    {
        movementCircleObject.transform.localScale = new Vector3(maxMovementRange * 2f, maxMovementRange * 2f, 1f);
        if (!bShouldCheckIfReached) return;
        if (!ReachedDestinationOrGaveUp()) return;
        cameraController.UnlockOnGameObject();
        _pathLineRenderer.positionCount = 0;
        _animator.SetBool("isMoving",false);
        walkingMaterial.SetFloat("_OutlineSize", 0f);
        if (bSelected) selectedMaterial.SetFloat("_OutlineSize", 1.01f);
        bShouldCheckIfReached = false;
    }

    public void OnSelected()
    {
        movementCircleObject.SetActive(true);
        bSelected = true;
        selectedMaterial.SetFloat("_OutlineSize", 1.01f);
    }

    public void OnDeselected()
    {
        movementCircleObject.SetActive(false);
        bSelected = false;
        selectedMaterial.SetFloat("_OutlineSize", 0f);
    }
    
    public async void PerformMove(Vector3 point)
    {
        if (_hasMovedAlready) return;
        if (Vector3.Distance(transform.position, point) > maxMovementRange) return;
        OnDeselected();
        var navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas,navMeshPath);
        if (navMeshPath.status == NavMeshPathStatus.PathInvalid) return;
        await RotateTowards(navMeshPath.corners[navMeshPath.corners.Length - 1]);
        _animator.SetBool("isMoving",true);
        _agent.SetPath(navMeshPath);
        DrawPath(navMeshPath);
        bShouldCheckIfReached = true;
        selectedMaterial.SetFloat("_OutlineSize", 0f);
        walkingMaterial.SetFloat("_OutlineSize", 1.01f);
        _hasMovedAlready = true;
    }

    public void ResetMovementFlag()
    {
        _hasMovedAlready = false;
    }

    private async Task RotateTowards(Vector3 corner)
    {
        Vector3 direction = (corner - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            await Task.Yield();
        }

        transform.rotation = lookRotation;
    }

    private void DrawPath(NavMeshPath path)
    {
        var positions = path.corners;
        _pathLineRenderer.positionCount = positions.Length;
        for (int i = 0; i < positions.Length; i++)
        {
            _pathLineRenderer.SetPosition(i, positions[i] + new Vector3(0,0.5f,0));
        }
    }

    public Vector3 GetCastPoint()
    {
        return castPoint.transform.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
        updateHPBarsEvent.Raise();
    }

    public async void PlayAttackAnim(Vector3 aiTargetPos,AIController aiTargetController,bool sucess)
    {
        await RotateTowards(aiTargetPos);
        _animator.SetTrigger("Attack");
        await isDoneAttaking();
        if (!sucess) return;
        aiTargetController.TakeDamage(GetBaseDamage());
    }


    private async Task isDoneAttaking()
    {
        while (true)
        {
            if (_animator.GetBool("Attack"))
            { 
                await Task.Yield();
            }
            break;
        }
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetBaseDamage()
    {
        return baseDamage;
    }
    
    public float GetMaxRange()
    {
        return maxAttackRange;
    }

    public void AddHp(float healingValue)
    {
        health += healingValue;
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
        _hasMovedAlready = hasMoved;
    }

    public void SetAttacked(bool hasAttacked)
    {
        _hasAttackedAlready = hasAttacked;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public void SetBaseDMG(int baseDamage)
    {
        this.baseDamage = baseDamage;
    }

    public float GetHealthPercentage()
    {
        return health / initialHealth;
    }
}
