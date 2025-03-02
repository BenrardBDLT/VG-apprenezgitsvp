using UnityEngine;
using System.Collections;


public class Player : MonoBehaviour
{
    public int health { get; set; }
    public int MaxHealth { get; private set; }
    public int Damage { get; set; }
    public Card[] Cartes { get; private set; } // Array de 5 cartes max
    public string currentClass { get; private set; }

    public Vector3 SpawnPosition { get; private set; }

    //tout ce qui va etre dans la mécanique d'esquive 
    public float dodgeDuration = 0.5f; // Temps d’invulnérabilité et déplacement
    public float dodgeSpeed = 5f; // Vitesse du dash arrière
    public float dodgeCooldown = 1.5f; // Temps avant de pouvoir ré-esquiver
    private bool isInvulnerable = false;
    private bool canDodge = true;



    void Start(){
        SpawnPosition = transform.position;
        MaxHealth = 100;
    }
    public Player()
    {
        health = 100;
        Damage = 10;
        Cartes = new Card[5]; // Taille fixe de 5 cartes
        currentClass = "Basique";
    }

     private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        {
            StartCoroutine(Dodge());
        }
    }

    private IEnumerator Dodge()
    {
        canDodge = false;
        isInvulnerable = true;
        float elapsedTime = 0f;
        Vector3 dodgeDirection = -transform.forward; // Dash en arrière

        while (elapsedTime < dodgeDuration)
        {
            transform.position += dodgeDirection * dodgeSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isInvulnerable = false;

        // Attendre avant de pouvoir esquiver à nouveau
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }


    public void TakeDamage(int damage){
        
        if (isInvulnerable)
        {
            Debug.Log("Esquive réussie, aucun dégât pris !");
            return;
        }
        
        health -= damage;
        if(health < 0){
            Die();
        }
    }

    private void Die(){
        Debug.Log("le joueur est mort :(");
        health = MaxHealth;
        transform.position = SpawnPosition;

    }
    private void UpdateClass()
    {
        //definir le type en fonction du n nombre de carte de meme type (> 3 car 5 cartes.)
        
    }

    /*public void AddCard(Card newCard)
        {
            for (int i = 0; i < Cartes.Length; i++)
            {
                if (Cartes[i] == null) // Trouve une case vide
                {
                    Cartes[i] = newCard;
                    UpdateClass();
                    return;
                }
            }

            //Debug.Log("Le deck est plein ! Impossible d'ajouter une nouvelle carte.");
            //faire en sorte que la carte soit ajoutée dans l'inventaire.
            //plus tard faire en sorte de pouvoir changer de cartes, principe de deckbuilding.
        }*/
}
