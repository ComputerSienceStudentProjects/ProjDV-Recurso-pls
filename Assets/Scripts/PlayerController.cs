using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/**
 * <summary>
 *  PlayerController Class
 * </summary>
 * <version>
 *  21/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public class PlayerController : MonoBehaviour
{
    [Header("Player Controller external references")]
    [SerializeField] private GameObject movementCircleObject;
    [SerializeField] private GameObject castPoint;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private CameraController cameraController;
    
    [Header("Game events")]
    [SerializeField] private GameEvent updateHpBarsEvent;
    
    [Header("Character Settings")]
    [SerializeField] private int baseDamage;
    [SerializeField] private int initialHealth;
    [SerializeField] private float health;
    [SerializeField] private float maxAttackRange;
    [SerializeField] private float maxMovementRange;
    [SerializeField] private bool bSelected;
    
#region Private Properties
    private Material _selectedMaterial;
    private Material _walkingMaterial;
    private AIControllable _aiTarget;
    private Animator _animator;
    private LineRenderer _pathLineRenderer;
    private NavMeshAgent _agent;

    private bool _hasMovedAlready;
    private bool _hasAttackedAlready;
    private bool _bShouldCheckIfReached;
#endregion

#region Public Methods
    #region Public Async Tasks
        public async void PerformMove(Vector3 point)
        {
            if (_hasMovedAlready) return;
            if (Vector3.Distance(transform.position, point) > maxMovementRange) return;
            OnDeselected();
            var navMeshPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, navMeshPath);
            if (navMeshPath.status == NavMeshPathStatus.PathInvalid) return;
            await RotateTowards(navMeshPath.corners[navMeshPath.corners.Length - 1]);
            _animator.SetBool("isMoving", true);
            _agent.SetPath(navMeshPath);
            DrawPath(navMeshPath);
            _bShouldCheckIfReached = true;
            _selectedMaterial.SetFloat("_OutlineSize", 0f);
            _walkingMaterial.SetFloat("_OutlineSize", 1.01f);
            _hasMovedAlready = true;
        }
        
        public async void PlayAttackAnim(Vector3 aiTargetPos, AIControllable aiTargetController, bool sucess)
        {
            await RotateTowards(aiTargetPos);
            _animator.SetTrigger("Attack");
            _aiTarget = aiTargetController ? aiTargetController : null;
        }
    #endregion
    public Vector3 GetCastPoint()
    {
        return castPoint.transform.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
        updateHpBarsEvent.Raise();
    }
    
    public void PerformAttack()
    {
        _aiTarget.TakeDamage(GetBaseDamage());
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
        if (health > initialHealth)
        {
            health = initialHealth;
        }
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
    
    public void OnSelected()
    {
        movementCircleObject.SetActive(true);
        bSelected = true;
        _selectedMaterial.SetFloat("_OutlineSize", 1.01f);
    }

    public void OnDeselected()
    {
        movementCircleObject.SetActive(false);
        bSelected = false;
        _selectedMaterial.SetFloat("_OutlineSize", 0f);
    }

    

    public void ResetMovementFlag()
    {
        _hasMovedAlready = false;
    }
#endregion

#region Private Methods
    #region Unity Default
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            cameraController = Camera.main.gameObject.GetComponent<CameraController>();
            
            _pathLineRenderer = GetComponent<LineRenderer>();
            health = initialHealth;
            
            _selectedMaterial = meshRenderer.materials[1];
            _walkingMaterial = meshRenderer.materials[2];
        }
        
        private void Update()
        {
            movementCircleObject.transform.localScale = new Vector3(maxMovementRange * 2f, maxMovementRange * 2f, 1f);
            if (!_bShouldCheckIfReached) return;
            if (!ReachedDestinationOrGaveUp()) return;
            cameraController.UnlockOnGameObject();
            _pathLineRenderer.positionCount = 0;
            _animator.SetBool("isMoving", false);
            _walkingMaterial.SetFloat("_OutlineSize", 0f);
            if (bSelected) _selectedMaterial.SetFloat("_OutlineSize", 1.01f);
            _bShouldCheckIfReached = false;
        }
    #endregion

    #region Private Async Tasks
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
    #endregion
    
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
    
    private void DrawPath(NavMeshPath path)
    {
        var positions = path.corners;
        _pathLineRenderer.positionCount = positions.Length;
        for (int i = 0; i < positions.Length; i++)
        {
            _pathLineRenderer.SetPosition(i, positions[i] + new Vector3(0, 0.5f, 0));
        }
    }
#endregion
}
