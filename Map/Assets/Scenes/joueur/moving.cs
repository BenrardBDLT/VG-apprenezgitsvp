using UnityEngine;
using System.Collections;

public class Moving : MonoBehaviour
{
    public CameraOrbit cameraOrbit; // Référence au script CameraOrbit

    private float moveInput;       // Valeur d'entrée de mouvement
    private bool isGrounded;       // Vérifier si le joueur est au sol
    
    public float moveSpeed = 5f;   // Vitesse de déplacement
    public float jumpForce = 7f;   // Force du saut
    private Rigidbody rb;          // Référence au Rigidbody du joueur

    public LayerMask enemyLayer;   // Calque pour détecter les ennemis
    public Transform cameraTransform; // Référence à la caméra pour orienter le mouvement

    public float attackRadius = 1.5f; // Rayon de la zone d'attaque

    private float attackCooldown = 1f;  // Temps de cooldown entre les attaques (en secondes)
    private bool canAttack = true;     // Vérifie si l'attaque est disponible

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Récupère le Rigidbody au début
        cameraOrbit = Camera.main.GetComponent<CameraOrbit>(); // Récupère le script CameraOrbit attaché à la caméra
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCollisions();
        //HandleSprint();

        if (Input.GetMouseButtonDown(0) && canAttack) // Clic gauche et vérification du cooldown
        {
            StartCoroutine(AttackCooldown()); // Commence le cooldown
            HandleAttack();
        }
    }

    private void HandleAttack()
    {
        // Vérifie si des ennemis sont dans la zone d'attaque
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Appliquer les dégâts à l'ennemi
            Ennemy ennemyScript = enemy.GetComponent<Ennemy>();
            if (ennemyScript != null)
            {
                // Dégâts à changer en fonction des besoins
                ennemyScript.TakeDamage(10); // Applique des dégâts
                Debug.Log("L'ennemi prend des dégâts !");
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;  // Désactive les attaques pendant le cooldown
        yield return new WaitForSeconds(attackCooldown);  // Attend la durée du cooldown
        canAttack = true;   // Permet à nouveau l'attaque
    }

    // Gérer le mouvement horizontal et la rotation en fonction de la caméra
    private void HandleMovement()
    {
        if (cameraOrbit == null)
        {
            Debug.LogError("cameraOrbit n'est pas assigné !");  // Ajoute un message d'erreur si cameraOrbit est null
            return;  // Si cameraOrbit est null, on arrête la fonction
        }

        // Récupérer les entrées utilisateur
        float xDir = Input.GetAxis("Horizontal"); // Mouvement latéral
        float zDir = Input.GetAxis("Vertical");   // Mouvement avant/arrière

        // Ne se déplacer que si une touche est pressée
        if (zDir != 0 || xDir != 0)
        {
            // Calculer la direction de la caméra sur le plan horizontal
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f; // Ignorer l'inclinaison verticale
            cameraForward.Normalize();

            // Calculer la direction du mouvement
            Vector3 movementDirection = cameraForward * zDir + cameraTransform.right * xDir;
            movementDirection.Normalize();

            // Si le mode lock-on est activé, faire pivoter le joueur vers l'ennemi
            if (cameraOrbit.isLockingOn && cameraOrbit.LockedEnemy != null)
            {
                Vector3 directionToEnemy = cameraOrbit.LockedEnemy.position - transform.position;
                directionToEnemy.y = 0f; // Ignore la composante verticale
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
            else
            {
                // Sinon, le joueur regarde la direction du mouvement
                if (movementDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }

            // Avancer dans la direction calculée
            rb.linearVelocity = new Vector3(movementDirection.x * moveSpeed, rb.linearVelocity.y, movementDirection.z * moveSpeed);
        }
    }
    private void HandleSprint(){
        
    }

    // Gérer le saut
    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump")) // Si au sol et qu'on appuie sur Jump
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Applique la force du saut
        }
    }

    // Vérifier les collisions avec les ennemis
    private void HandleCollisions()
    {
        // Collision avec les ennemis (ici on suppose que les ennemis sont dans un calque dédié)
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 0.5f, enemyLayer);
        foreach (var enemy in hitEnemies)
        {
            // Ici tu peux ajouter le code de gestion des collisions avec les ennemis
            // Ex: réduire la vie du joueur ou faire un dégât
            //Debug.Log("Collision avec un ennemi!");
        }
    }

    // Vérifier si le joueur est au sol
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Si le joueur touche le sol
        {
            isGrounded = true;
        }
    }

    // Réinitialiser la vérification du sol quand le joueur quitte le sol
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Si le joueur quitte le sol
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Dessine la zone d'attaque dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
