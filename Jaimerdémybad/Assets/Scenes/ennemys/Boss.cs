using UnityEngine;

public class Boss : Ennemy
{

    public Boss() : base(2500, 50, 2f){}
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
