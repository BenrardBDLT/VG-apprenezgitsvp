using UnityEngine;
using System.Collections;
/// <summary>
/// ennemi le plus faible
/// possède 50 hp pour 5 de degats, peut attaquer avec un saut, un dash et un coup, parfois peut faire des combo de deux voir trois attaques
/// (aléatoire)
/// </summary>
public class Weak_Ennemy : Ennemy
{
    private enum AttackPattern { Normal, Jump, Dash, Combo2, Combo3 }
    private AttackPattern currentAttackPattern;

    private bool isAttacking = false;
    private Transform player; 

    public Weak_Ennemy() : base(50, 5, 3f) { }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (IsAlive)
        {
            if (!isAttacking && player != null)
            {
                isAttacking = true;

                // Regarde le joueur avant d'attaquer
                LookAtPlayer();

                // Choisir une attaque aléatoire
                currentAttackPattern = (AttackPattern)Random.Range(0, 5);

                switch (currentAttackPattern)
                {
                    case AttackPattern.Normal:
                        yield return StartCoroutine(NormalAttack());
                        break;
                    case AttackPattern.Jump:
                        yield return StartCoroutine(JumpAttack());
                        break;
                    case AttackPattern.Dash:
                        yield return StartCoroutine(DashAttack());
                        break;
                    case AttackPattern.Combo2:
                        yield return StartCoroutine(ComboAttack(2));
                        break;
                    case AttackPattern.Combo3:
                        yield return StartCoroutine(ComboAttack(3));
                        break;
                }

                yield return new WaitForSeconds(AttackDelay);
                isAttacking = false;
            }

            yield return null;
        }
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // On ignore l'axe Y pour éviter les rotations bizarres
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private IEnumerator NormalAttack()
    {
        Debug.Log("Attaque normale !");
        LookAtPlayer();
        Vector3 attackPosition = transform.position + transform.forward * 1.5f;

        yield return MoveForward(0.5f);

        ShowDamageArea(attackPosition, new Vector3(1f, 1f, 1f)); 
        yield return new WaitForSeconds(0.1f);
        DamagePlayerIfInZone(attackPosition, 1f);
    }

    private IEnumerator DashAttack()
    {
        Debug.Log("Dash Attack !");
        LookAtPlayer();
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * 5f;

        yield return MoveToPosition(endPosition, 0.3f);

        ShowDamageArea(transform.position, new Vector3(5f, 1f, 2f));
        yield return new WaitForSeconds(0.1f);
        DamagePlayerIfInZone(transform.position, 2f);
    }

    private IEnumerator JumpAttack()
    {
        Debug.Log("Jump Attack !");
        LookAtPlayer();
        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * 5f;

        yield return MoveToPosition(peakPosition, 0.5f);
        yield return new WaitForSeconds(0.2f);
        yield return MoveToPosition(startPosition, 0.1f);

        ShowDamageArea(startPosition, new Vector3(3f, 0.1f, 3f));
        yield return new WaitForSeconds(0f);
        DamagePlayerIfInZone(startPosition, 3f);
    }

    private IEnumerator ComboAttack(int numberOfAttacks)
    {
        Debug.Log($"Combo de {numberOfAttacks} attaques !");
        for (int i = 0; i < numberOfAttacks; i++)
        {
            int randomAttack = Random.Range(0, 3);
            switch (randomAttack)
            {
                case 0: yield return StartCoroutine(NormalAttack()); break;
                case 1: yield return StartCoroutine(JumpAttack()); break;
                case 2: yield return StartCoroutine(DashAttack()); break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveForward(float distance)
    {
        float moveDuration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * distance;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void ShowDamageArea(Vector3 position, Vector3 size)
    {
        GameObject damageArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
        damageArea.transform.position = position;
        damageArea.transform.localScale = size;
        damageArea.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
        Destroy(damageArea, 0.5f);
    }

    private void DamagePlayerIfInZone(Vector3 attackPosition, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, radius);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                Debug.Log("Le joueur a été touché !");
                col.GetComponent<Player>().TakeDamage(5);
            }
        }
    }
}


    //faire une fonction pour chaque attaque, sachant que combo2 est un combo de deux des attaques et combo 3 de trois des attaques
    //faire ensuite en sorte que l'ennemi choisisse aléatoirement l'attaque qu'il va utiliser
    //faire en sorte que lorsqu'il execute l'attaque, un triger se crée pour detecter si des degats avec le joueur vont etre ingligée
    //faire apparaitre la zone de degats en rouge si possible
    //sachant que attaque sautée crée un cercle au sol pourq lorsque l'ennemi retombe, dash crée un rectangle en face de lui sur tout la durée du dash, et attque normale est just un coup, soit un carré devant l'ennemi
    //animer les attaques (faire sauter l'objet ennemi lors de l'attaque saut, le faire aller dans une direction, un peu plus vite que dh'abitude lors du dash, et un tout petit coup vers l'avant pout l'attaque normale)
    //faire en sorte que les attaques aient un cooldown sauf pour si c'est un combo
    //si le joueur est dans le trigger au moment de l'impact de l'attaque, il prend des degats 



