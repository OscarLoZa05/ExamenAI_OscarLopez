using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{
    private NavMeshAgent _navMesh;
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
    }

    public EnemyState _currentState;

    //Patrolling
    [SerializeField] private Transform[] _patrolPoints; 

    //Chasing
    private Transform _player;
    [SerializeField] private float _chasingRange = 7;

    //Attacking
    [SerializeField] private float _attackRange = 2;
    [SerializeField] private float _attackTimer;
    [SerializeField] private float _attackDelay = 5;
    
    void Awake()
    {
        _navMesh = GetComponent<NavMeshAgent>();

        _player = GameObject.FindWithTag("Player").transform;
    }
    
    void Start()
    {
        _currentState = EnemyState.Patrolling;
        _attackTimer = _attackDelay;
        RandomPoints();
    }

    
    void Update()
    {
        switch(_currentState)
        {
            case EnemyState.Patrolling:
                Patrolling();
            break;
            case  EnemyState.Chasing:
                Chasing();
            break;
            case EnemyState.Attacking:
                Attacking();
            break;
            default:
                Patrolling();
            break;
        }
    }

    void Patrolling()
    {
        if(OnRange(_chasingRange))
        {
            _currentState = EnemyState.Chasing;
        }
        if(_navMesh.remainingDistance < 0.5)
        {
            RandomPoints();
        }
    }

    void Chasing()
    {
        if(!OnRange(_chasingRange))
        {
            _currentState = EnemyState.Patrolling;
        }
        if(OnRange(_attackRange))
        {
            _currentState = EnemyState.Attacking;
        }
        _navMesh.SetDestination(_player.position);
    }

    void Attacking()
    {
        if(!OnRange(_attackRange))
        {
            _currentState = EnemyState.Chasing;
        }
        _attackTimer += Time.deltaTime;
        if(_attackTimer > _attackDelay)
        {
            _attackTimer = 0;
            Debug.Log("Attack!");
            _currentState = EnemyState.Chasing;
        }
    }

    public bool OnRange(float distance)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, _player.position);
        
        if(distanceToEnemy <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void RandomPoints()
    {
        _navMesh.SetDestination(_patrolPoints[Random.Range(0, _patrolPoints.Length)].position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _chasingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.black;
        foreach(Transform _point in _patrolPoints)
        {
            Gizmos.DrawWireSphere(_point.position, 1f);
        }
    }
}
