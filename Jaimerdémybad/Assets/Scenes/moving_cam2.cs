using UnityEngine;
using System.Collections;


public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Le joueur que la caméra suit
    public float distance = 5f; // Distance entre la caméra et le joueur
    public float minDistance = 3f; // Distance minimale entre la caméra et l'ennemi
    public float sensitivityX = 10f; // Sensibilité pour la rotation horizontale
    public float sensitivityY = 10f; // Sensibilité pour la rotation verticale
    public float minY = -80f; // Limite minimale pour l'angle vertical
    public float maxY = 80f;  // Limite maximale pour l'angle vertical

    public float lockOnSmoothSpeed = 5f; // Vitesse de lissage pour le verrouillage
    // La variable lockedEnemy est maintenant privée
    private Transform lockedEnemy = null;

    // Propriété publique pour accéder à lockedEnemy
    public Transform LockedEnemy
    {
        get { return lockedEnemy; }
        set { lockedEnemy = value; }
    }
 // Ennemi verrouillé
    public bool isLockingOn = false; // Mode lock-on ou non

    private float rotationX = 0f; // Rotation accumulée sur l'axe horizontal
    private float rotationY = 0f; // Rotation accumulée sur l'axe vertical


    

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Vérifie si la molette de la souris est cliquée pour activer/désactiver l'aide à la visée
        if (Input.GetMouseButtonDown(2)) // Molette de la souris
        {
            LockOnCamera();
        }

        // Si la caméra est en lock-on, on fixe la rotation du cube sur l'ennemi
        if (isLockingOn && lockedEnemy != null)
        {
            Ennemy ennemyScript = lockedEnemy.GetComponent<Ennemy>();

            // Vérifier que l'ennemi est toujours vivant
            if (ennemyScript != null && !ennemyScript.IsAlive)
            {
                Debug.Log("ennemi mort");
                // L'ennemi est mort, on désactive le lock-on et repasse à la caméra libre
                isLockingOn = false;
                lockedEnemy = null; // Reset lockedEnemy pour éviter l'accès à un objet détruit
                HandleFreeCameraMovement();
            }
            else
            {
                // La caméra suit l'ennemi, mais reste derrière le joueur
                Vector3 direction = lockedEnemy.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;

                // Position derrière le joueur avec ajustement de la hauteur
                Vector3 behindPlayer = target.position - (target.forward * distance);
                behindPlayer.y += 2f; // Surélever un peu la caméra

                // Lissage pour obtenir une transition douce entre les positions
                transform.position = Vector3.Lerp(transform.position, behindPlayer, Time.deltaTime * lockOnSmoothSpeed);
            }
        }
        else
        {
            // Si on n'est pas en lock-on, on gère le mouvement libre de la caméra
            HandleFreeCameraMovement();
        }
    }


    void LockOnCamera()
        {
            // Si aucun ennemi n'est verrouillé, chercher l'ennemi le plus proche
            if (!isLockingOn)
            {
                // Trouver les ennemis proches du joueur
                Collider[] enemiesInRange = Physics.OverlapSphere(target.position, distance);
                Transform closestEnemy = null;
                float closestDistance = Mathf.Infinity;

                foreach (var collider in enemiesInRange)
                {
                    if (collider.CompareTag("Enemy")) // S'assurer que l'objet est un ennemi
                    {
                        float dist = Vector3.Distance(target.position, collider.transform.position);
                        if (dist < closestDistance)
                        {
                            closestEnemy = collider.transform;
                            closestDistance = dist;
                        }
                    }
                }

                if (closestEnemy != null)
                {
                    lockedEnemy = closestEnemy;
                    isLockingOn = true;
                    // Activer la transition lisse pour le verrouillage
                    StartCoroutine(SmoothLockOn());
                }
            }
            else
            {
                // Si on est déjà en lock-on, revenir en mode caméra libre
                lockedEnemy = null;
                isLockingOn = false;
            }
        }

        IEnumerator SmoothLockOn()
        {
            while (lockedEnemy != null)
            {
                // Vérifie si l'ennemi est trop loin ou est mort
                float distanceToEnemy = Vector3.Distance(target.position, lockedEnemy.position);
                Ennemy ennemyScript = lockedEnemy.GetComponent<Ennemy>();
                if (ennemyScript == null || ennemyScript.IsAlive == false || distanceToEnemy > minDistance)
                {
                    // Si l'ennemi est trop loin ou mort, on repasse en mode libre
                    lockedEnemy = null; // Enlève l'ennemi verrouillé
                    isLockingOn = false; // Désactive le mode lock-on
                    yield break; // On sort de la coroutine et repasse à la gestion de la caméra libre
                }

                // Effectuer un lissage de la position et de la rotation pour le verrouillage
                Vector3 targetPosition = lockedEnemy.position;
                Vector3 direction = targetPosition - transform.position;

                // Lissage de la position de la caméra
                transform.position = Vector3.Lerp(transform.position, targetPosition - direction.normalized * distance, Time.deltaTime * lockOnSmoothSpeed);

                // Lissage de la rotation pour regarder l'ennemi
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lockOnSmoothSpeed);

                yield return null;
            }
        }

        void HandleFreeCameraMovement()
        {
            if (isLockingOn) return;

            float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX += mouseX;
            rotationY -= mouseY;

            rotationY = Mathf.Clamp(rotationY, minY, maxY);

            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0f);
            Vector3 offset = rotation * new Vector3(0, 0, -distance);

            transform.position = target.position + offset;
            transform.LookAt(target);
        }



}
