using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Data")]
public class EnemyScriptable : ScriptableObject
{
    [SerializeField] float moveSpeed;
    [SerializeField] int maxHealth;
    [SerializeField] int damage;

    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
    public int Damage { get => damage; private set => damage = value; }
}
