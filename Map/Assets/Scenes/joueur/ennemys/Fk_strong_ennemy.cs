using UnityEngine;

public class Fk_strong_ennemy : Ennemy
{

    public Fk_strong_ennemy() : base(250, 15, 1f){}
    // Initialisation des valeurs spécifiques pour le boss
   
    void Start()
    {
        // Appelle la méthode d'initialisation dans Start
    
    }
    public void AttackPlayer(Player target)
    {
        Attack(target); // Appelle la méthode Attack de la classe de base Ennemy
    }
}
