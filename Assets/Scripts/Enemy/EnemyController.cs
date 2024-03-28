using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyScriptable enemyData;
    public float despawnDistance = 20f;
    public int scoreValue;

    [Header("Sight")]
    [SerializeField] float sightRange;
    [SerializeField] float eyeHeight;

    [Header("Attack")]
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 10;
    public float attackRange = 10f;
    public LayerMask attackLayer;

    public NavMeshAgent Agent{ get => agent; }
    public Transform Player{ get => player; }
    public Vector3 LastPlayerPos{ get => lastPlayerPos; set => lastPlayerPos = value; }

    NavMeshAgent agent;
    Transform player;
    Vector3 lastPlayerPos;
    Animator anim;
    PlayerUI ui;

    WaveSpawner waveSpawner;
    DropRateManager drop;

    #region current value
    float currentSpeed;
    int currentHealth;
    int currentDamage;
    string currentState;
    #endregion

    #region bool values
    public bool isDead;
    bool readyToAttack = true;
    bool isAttacking = false;
    bool isInSight;
    bool isInAttackRange;
    #endregion

    void Awake()
    {
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
        currentSpeed = enemyData.MoveSpeed;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().transform;
        ui = FindObjectOfType<PlayerUI>();

        waveSpawner = FindObjectOfType<WaveSpawner>();
        drop = GetComponent<DropRateManager>();
    }

    void Update()
    {
        if(player == null){ return; }

        isInSight = Physics.CheckSphere(transform.position, sightRange, attackLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, attackLayer);

        if(!isDead){
            if(Vector3.Distance(transform.position, player.transform.position) >= despawnDistance){
                ReturnEnemy();
            }

            if(isInSight && !isInAttackRange){ Chase(); }
            if(isInSight && isInAttackRange){ Attack(); }
        }
    }

    #region Movement
    public void Chase()
    {
        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        transform.LookAt(player.transform);

        agent.SetDestination(player.transform.position);
        anim.SetInteger("animation", 8);
    }
    #endregion

    #region Attack
    public void Attack()
    {
         if(!readyToAttack || isAttacking){ return; }

        readyToAttack = false;
        isAttacking = true;

        agent.isStopped = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        anim.SetInteger("animation", 5);
    }

    void AttackRaycast()
    {
        // RaycastHit hit = new RaycastHit();
        // Vector3 playerDistance = player.position - transform.position - (Vector3.up * eyeHeight);
        // Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), playerDistance.normalized);

        // if(Physics.Raycast(ray, out hit, attackRange, layer)){   
        //     if(hit.transform.TryGetComponent(out PlayerHealth ph)){ 
        //         if(!ph.isDead){
        //             ph.TakeDamage(attackDamage); 
        //         }
        //     }
        // } 
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        if(!player.isDead){
            player.TakeDamage(attackDamage);
        }
    }

    void ResetAttack()
    { 
        isAttacking = false; 
        readyToAttack = true;
    }
    #endregion

    #region Damage
    public void TakeDamage(int damage)
    {
        currentHealth -= Random.Range(damage - 2, damage);

        if(currentHealth <= 0){ Death(); }
    }
    #endregion

    #region Death
    void Death()
    {
        isDead = true;
        anim.SetInteger("animation", 7);

        ui.score.text = ui.AddScore(scoreValue).ToString();
        if(scoreValue > PlayerPrefs.GetInt("HighScore", 0)){
            PlayerPrefs.SetInt("HighScore", ui.scoreValue);
            ui.highScore.text = scoreValue.ToString();
        }

        waveSpawner.EnemyKilled();

        drop.EnemyDrops();

        Destroy(gameObject, 1f);
    }
    #endregion

    #region relocate enemy
    void ReturnEnemy()
    {
        WaveSpawner ws = FindObjectOfType<WaveSpawner>();
        transform.position = player.position + ws.spawnPoints[Random.Range(0, ws.spawnPoints.Count)].position;
    }
    #endregion
}
