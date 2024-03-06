public class Card
{
    public string rank;
    public Suits suit;
    public string name;

    public Card(string rank, Suits suit)
    {
        this.rank = rank;
        this.suit = suit;
        name = rank + " of " + suit;
    }
}
