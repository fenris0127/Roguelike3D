using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public int projectileDamage;
    public float projectileSpeed = 1f;

    [SerializeField] GameObject[] destroyOnHit;
    [SerializeField] UnityEvent onHit;
    [SerializeField] GameObject hitEffect = null;

    PlayerInput playerInput;
    PlayerInput.OnFootActions input;

    // [SerializeField] EnemyController target;
    Vector3 targetPoint;

    float lifeAfterImpact = 2f;

    bool isHoming = false;

    void Awake()
    {
        playerInput = new PlayerInput();
        input = playerInput.OnFoot;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) 
    {
        // target = other.gameObject.GetComponent<EnemyController>();
        
        Debug.Log("Hit1");

        if(other.tag == "Enemy" || other.tag == "Ground"){
            // target.TakeDamage(projectileDamage);
            Debug.Log("Hit2");
            Destroy(gameObject);
        }
        
        projectileSpeed = 0;

        onHit.Invoke();

        if(hitEffect != null){ Instantiate(hitEffect); }

        foreach(GameObject toDestroy in destroyOnHit){
            Destroy(toDestroy);
        }
        
        Destroy(gameObject, lifeAfterImpact);
        
    }
}
