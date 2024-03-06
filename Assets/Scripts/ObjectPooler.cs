using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform spawnPos, cardsParent;
    [SerializeField] private Transform[] P_Cards, D_Cards;
    [SerializeField] private Sprite[] cardImages;
    private Dictionary<string, GameObject> pool;
    private readonly int poolSize = 53;
    
    public void InitialiseDeck()
    {
        pool = new();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject card = Instantiate(prefab, spawnPos.position, Quaternion.identity, cardsParent);
            card.GetComponent<SpriteRenderer>().sprite = cardImages[i];
            pool.Add(cardImages[i].name, card);
        }
    }

    public void DealCard(string cardName, int index)
    {
        GameObject card = pool[cardName];
        Transform t = (index / 5 == 0) ? P_Cards[index % 5] : D_Cards[index % 5];
        card.transform.position = t.position;
        card.GetComponent<SpriteRenderer>().sortingOrder = index % 5 + 1;
        card.GetComponent<Animator>().SetBool("Turn", true);
    }

    public IEnumerator DiscardCard(string cardName)
    {
        GameObject card = pool[cardName];
        card.GetComponent<Animator>().SetBool("Turn", false);
        yield return new WaitForSeconds(0.3f);
        card.transform.position = spawnPos.position;
    }
}
