using System;
using System.Collections.Generic;

public class Collection
{
    public List<Card> cards = new();
    public int score;

    public void CreateDeck()
    {
        foreach (string rank in new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" })
        {
            foreach (Suits suit in Enum.GetValues(typeof(Suits)))
            {
                cards.Add(new Card(rank, suit));
            }
        }
        Shuffle();
    }

    public void AddCard(Card c)
    {
        cards.Add(c);
        ScoreHand();
    }

    private void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int index = UnityEngine.Random.Range(0, cards.Count - 1);
            cards[i] = cards[index];
            cards[index] = temp;
        }
    }
    
    public Card DrawCard()
    {
        Card output = null;
        if (cards.Count != 0)
        {
            output = cards[0];
            cards.RemoveAt(0);
        }
        return output;
    }

    private void ScoreHand()
    {
        score = 0;
        int aces = 0;
        foreach (Card c in cards)
        {
            try
            {
                score += int.Parse(c.rank);
            }
            catch
            {
                if (c.rank == "Ace") aces++;
                else score += 10;
            }
        }
        while (aces > 0)
        {
            if (score + 10 + aces <= 21)
                score += 11;
            else
                score++;
            aces--;
        }
    }
}
