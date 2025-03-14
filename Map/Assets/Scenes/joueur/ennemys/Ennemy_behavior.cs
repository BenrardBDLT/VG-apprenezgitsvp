using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float moveSpeed = 2f;         // Vitesse de déplacement de l'ennemi
    public Transform player;             // Référence au joueur (à assigner dans l'inspecteur)
    private Rigidbody rb;                // Référence au Rigidbody de l'ennemi

    private bool isAttacking = false;    // Cooldown de l'attaque


    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Récupère le Rigidbody de l'ennemi
        if (player == null) 
        {
            Debug.LogError("Player transform not assigned in EnemyBehavior.");
        }
    }

    void Update()
    {
        // Calculer la distance entre l'ennemi et le joueur
        float distance = Distance_To_Player();

        // Si l'ennemi est trop loin, il suit le joueur
        if (distance > 10)
        {
            StopMoving();
        }
        else if (distance > 2 )
        {
            FollowPlayer();
        }
        // Si l'ennemi est à une distance inférieure ou égale à 2, il attaque
        else if (distance <= 2 && !isAttacking)
        {
            StopMoving();
            AttackPlayer();
        }
    }

    // Suivre le joueur lentement
    private void FollowPlayer()
    {
        if (player == null) return;

        // Calcule la direction vers le joueur
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Déplace l'ennemi vers le joueur
        rb.linearVelocity = new Vector3(directionToPlayer.x * moveSpeed, rb.linearVelocity.y, directionToPlayer.z * moveSpeed);
    }

    // Arrêter l'ennemi
    private void StopMoving()
    {
        // Mettre la vitesse de l'ennemi à zéro
        rb.linearVelocity = Vector3.zero;
    }

    // Retourne la distance entre l'ennemi et le joueur
    public float Distance_To_Player()
    {
        if (player != null)
        {
            return Vector3.Distance(transform.position, player.position);
        }
        else
        {
            Debug.LogWarning("Player not assigned. Returning -1.");
            return -1f;
        }
    }

    // L'ennemi attaque le joueur
    private void AttackPlayer()
    {
        Ennemy ennemyScript = GetComponent<Ennemy>();
        if (ennemyScript != null && player != null)
        {
            // Assure-toi que le joueur possède bien un composant Player
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                ennemyScript.Attack(playerScript); // Appelle la méthode Attack de l'ennemi
            }
            else
            {
                Debug.LogError("Player script not found on player GameObject.");
            }
        }
        
        else if (player == null && ennemyScript != null) 
        {
            Debug.LogError("c'est le joeuur le pblm");
        }
        else if (ennemyScript == null && player != null) 
        {
            Debug.LogError("c'est l'ennemi le pblm");
        }

        else
        {
            Debug.LogError("les deux.");
        }
    }
}
