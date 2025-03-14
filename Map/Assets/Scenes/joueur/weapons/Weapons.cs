using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Collider weaponCollider; // Collider de l'arme
    public int damage = 10;         // Dégâts infligés

    void Start()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // Désactive la hitbox au début
        }
    }

    void Update()
    {
        // Active la hitbox quand l'attaque est effectuée
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            Attack();
        }
    }

    void Attack()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true; // Active la hitbox pendant l'attaque
            Debug.Log("Attaque !");

            // Optionnel : réinitialise la hitbox après un court délai
            Invoke("DisableWeaponCollider", 0.1f); // 0.1 seconde pour la durée de l'attaque
        }
    }

    void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // Désactive la hitbox après l'attaque
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Ennemy enemy = other.GetComponent<Ennemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Applique les dégâts à l'ennemi
                Debug.Log("L'ennemi prend des dégâts !");
            }
        }
    }
}
