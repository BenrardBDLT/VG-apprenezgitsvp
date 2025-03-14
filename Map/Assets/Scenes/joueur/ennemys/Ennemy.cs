using UnityEngine;
using System.Collections;

public class Ennemy : MonoBehaviour
{
    public int Health { get; private set; }
    public int Damage { get; set; }
    public bool IsAlive = true;

    public float AttackDelay {get; set;} // Délai entre les attaques (en secondes)
    private bool canAttack = true; // Pour empêcher plusieurs attaques simultanées

    


    public Ennemy(int baseHealth, int baseDamage, float attackDelay)
    {
        Health = baseHealth;
        Damage = baseDamage;
        AttackDelay = attackDelay;
    }
    private void Start()
    {
    
    }


    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(DamageFeedback()); // Effet visuel facultatif
        if (Health <= 0)
        {
            Die();
            IsAlive = false;
        }
    
        }

    private IEnumerator DamageFeedback()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = originalColor;
        }
    }


    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(AttackDelay); // Attente du délai
        canAttack = true; // L'ennemi peut attaquer à nouveau
    }

    public void Attack(Player target)
    {
        if (canAttack && target != null)
        {
            target.TakeDamage(Damage); // Applique les dégâts au joueur
            StartCoroutine(AttackCooldown()); // Lance le cooldown entre les attaques
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " est mort !");
        gameObject.SetActive(false);
 
    }
}
