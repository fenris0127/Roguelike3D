using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInput.OnFootActions input;

    CharacterController controller;
    Animator animator;
    PlayerUI ui;

    [SerializeField] ParticleSystem ps;
    [SerializeField] GameObject defaultWeapon;
    [SerializeField] Transform weaponHolder;
    [SerializeField] float powerTimer;

    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;
    [SerializeField] float normalSpeed = 8f;
    [SerializeField] float dashSpeed = 16f;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;

    [Header("Attack")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 10;
    public static int weaponDamage;
    public LayerMask attackLayer;

    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Transform projectileTransform;
    public float timeBtwShots;
    float projectileTime;

    [Header("Effect/Sound")]
    public GameObject hitEffect;
    [Space]
    AudioSource audioSource;
    public AudioClip swordSwing;
    public AudioClip swordHitSound;
    public AudioClip hammerSwing;
    public AudioClip hammerHitSound;

    [Header("Animation")]
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";

    Vector3 playerVelocity;
    PlayerHealth playerHealth;
    float xRotation = 0f;
    int attackCount;
    string currentAnimationState;

    #region bool value
    bool isGrounded;
    bool isDashing;
    bool readyToAttack = true;
    bool isAttacking = false;
    #endregion

    void Awake()
    { 
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        ui = GetComponent<PlayerUI>();
        playerHealth = GetComponent<PlayerHealth>();

        playerInput = new PlayerInput();
        input = playerInput.OnFoot;
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ps.Stop();
    }

    void Start()
    {
        GameObject weapon = Instantiate(defaultWeapon, weaponHolder);
        weapon.transform.position = weaponHolder.position;

        weaponDamage = weaponHolder.GetComponentInChildren<WeaponPickUp>().weaponDamage;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if(input.Attack.IsPressed()){ Attack(); }

        Shoot();

        SetAnimations();
    }

    void FixedUpdate() 
    { 
        MoveInput(input.Movement.ReadValue<Vector2>()); 
    }

    void LateUpdate() 
    { 
        LookInput(input.Look.ReadValue<Vector2>()); 
    }

    void OnEnable(){ input.Enable(); }
    void OnDisable(){ input.Disable(); }

    #region Input
    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Dash.started += ctx => Dash();
        input.Attack.started += ctx => Attack();
    }

    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;

        if(isGrounded && playerVelocity.y < 0){ playerVelocity.y = -2f; }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void LookInput(Vector3 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * sensitivity));
    }
    #endregion

    #region Action
    void Jump()
    {
        if (isGrounded){ 
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity); 
        }
    }

    public void Dash()
    {
        isDashing = !isDashing;

        if(isDashing){
            speed = dashSpeed;
            ps.Play();
        }else{
            speed = normalSpeed;
            ps.Stop();
        }
    }
    #endregion

    #region Animation
    public void ChangeAnimationState(string newState) 
    {
        if (currentAnimationState == newState){ return; }

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnimations()
    {
        if(!isAttacking){
            if(playerVelocity.x == 0 &&playerVelocity.z == 0){ 
                ChangeAnimationState(IDLE);
            }else{ 
                ChangeAnimationState(WALK); 
            }
        }
    }
    #endregion

    #region Attack
    public void Attack()
    {
        if(!readyToAttack || isAttacking){ return; }

        readyToAttack = false;
        isAttacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        if(weaponHolder.GetChild(0).name.Contains("Hammer") || weaponHolder.GetChild(0).name.Contains("Maces")){
            audioSource.PlayOneShot(hammerSwing);
        }else{
            audioSource.PlayOneShot(swordSwing);
        }

        if(attackCount == 0){
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }else{
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {
        RaycastHit hit = new RaycastHit();
        bool isHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, attackDistance, attackLayer);

        if(isHit){   
            HitTarget(hit.point, hit.collider.gameObject);

            if(hit.transform.TryGetComponent(out EnemyController ec)){ 
                if(!ec.isDead){
                    ec.TakeDamage(attackDamage + weaponDamage); 
                }
            }
        } 
    }

    void HitTarget(Vector3 pos, GameObject obj)
    {
        audioSource.pitch = 1;
        if(weaponHolder.GetChild(0).name.Contains("Hammer") || weaponHolder.GetChild(0).name.Contains("Maces")){
            audioSource.PlayOneShot(hammerHitSound);
        }else{
            audioSource.PlayOneShot(swordHitSound);
        }

        if(!obj.GetComponent<EnemyController>().isDead){
            GameObject effect = Instantiate(hitEffect, pos, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

    void Shoot()
    {
        if(projectileTime > 0f){
            projectileTime -= Time.deltaTime;
        }else{
            if(input.Shoot.IsPressed()){
                Projectile projectile = Instantiate(projectilePrefab, projectileTransform.position, projectileTransform.rotation);
                // projectile.transform.SetParent(projectileTransform);
                projectileTime = timeBtwShots;
            }
        }
    }
    #endregion

    #region stat up
    public IEnumerator PowerUp(int amount)
    {
        attackDamage += amount;
        yield return new WaitForSeconds(powerTimer);
        attackDamage -= amount;
    }

    public IEnumerator SpeedUp(int amount)
    {
        if(!isDashing){
            speed += amount;
            yield return new WaitForSeconds(powerTimer);
            speed -= amount;
        }
    }

    public void HealthUp(int healthUpPoint)
    {
        playerHealth.maxHealth += healthUpPoint;
        playerHealth.RestoreHealth(healthUpPoint / 2);
    }
    #endregion

    // private void OnTriggerEnter(Collider other) {
    //     if(other.tag == "Potion"){
    //         if(other.gameObject.TryGetComponent(out HealthPotion health) &&
    //             playerHealth.CurrentHealth < playerHealth.maxHealth){
    //             health.Collect();
    //         }
    //     }else if(other.tag == "PowerUp"){
    //         if(other.gameObject.TryGetComponent(out PowerUp powerUp)){
    //             powerUp.Collect();
    //         }
    //     }else if(other.tag == "SpeedBoost"){
    //         if(other.gameObject.TryGetComponent(out SpeedUp speedUp)){
    //             speedUp.Collect();
    //         }
    //     }else if(other.tag == "HealthUp"){
    //         if(other.gameObject.TryGetComponent(out HealthUp healthUp)){
    //             healthUp.Collect();
    //         }
    //     }
    // }
}