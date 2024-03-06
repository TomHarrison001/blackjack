using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText, potText, roundText, dScoreText, pScoreText, betAmount, scoreText, highscoreText;
    [SerializeField] private GameObject betSlider, turnButtons, menu;
    private AudioManager audioManager;
    private GameController controller;
    private Slider slider;
    private ObjectPooler pool;
    private Collection deck, pHand, dHand, discard;
    private int coins, pot, round, bet;
    private readonly float delay = 1f;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        controller = FindObjectOfType<GameController>();
        slider = FindObjectOfType<Slider>();
        pool = FindObjectOfType<ObjectPooler>();
        pool.InitialiseDeck();
        ResetGame();
    }

    public void ResetGame()
    {
        round = 0;
        coins = 100;
        deck = new();
        pHand = new();
        dHand = new();
        discard = new();
        betSlider.SetActive(false);
        turnButtons.SetActive(false);
        menu.SetActive(false);
        deck.CreateDeck();
        RoundStart();
    }

    private void RoundStart()
    {
        round++;
        pot = 0;
        bet = (coins / 2);
        if (bet == 0) bet = 1;
        betAmount.text = "$" + bet.ToString();
        slider.value = 0.5f;
        coinsText.text = (coins < 0 ? "-$" : "$") + Mathf.Abs(coins).ToString();
        potText.text = "$" + pot.ToString();
        roundText.text = round.ToString() + " / 5";
        betSlider.SetActive(true);
    }

    public void SliderChange(Slider s)
    {
        bet = (int)(coins * s.value);
        if (bet == 0) bet = 1;
        betAmount.text = "$" + bet.ToString();
    }

    public void Bet()
    {
        coins -= bet;
        pot += bet;
        coinsText.text = (coins < 0 ? "-$" : "$") + Mathf.Abs(coins).ToString();
        potText.text = "$" + pot.ToString();
        betSlider.SetActive(false);
        StartCoroutine(Deal());
    }

    private IEnumerator Deal()
    {
        foreach (Collection hand in new List<Collection> { pHand, dHand })
        {
            hand.AddCard(deck.DrawCard());
            hand.AddCard(deck.DrawCard());
        }
        pool.DealCard(pHand.cards[0].name, 0);
        pool.DealCard(pHand.cards[1].name, 1);
        pool.DealCard(dHand.cards[0].name, 5);
        pool.DealCard("cardBack", 6);
        yield return new WaitForSeconds(delay);
        pScoreText.text = pHand.score.ToString();
        if (pHand.score == 21)
        {
            pScoreText.text = "Blackjack!";
            yield return new WaitForSeconds(delay);
            StartCoroutine(EndRound());
        }
        else
            turnButtons.SetActive(true);
    }

    public void Hit()
    {
        pHand.AddCard(deck.DrawCard());
        int cardCount = pHand.cards.Count - 1;
        pool.DealCard(pHand.cards[cardCount].name, cardCount);
        StartCoroutine (Score());
    }

    private IEnumerator Score()
    {
        yield return new WaitForSeconds(delay);
        pScoreText.text = pHand.score.ToString();
        if (pHand.score >= 21) StartCoroutine(EndRound());
        if (pHand.cards.Count == 5) StartCoroutine(EndRound());
    }

    public void Stand()
    {
        StartCoroutine(EndRound());
    }

    public void DoubleDown()
    {
        int bet = pot;
        coins -= bet;
        pot += bet;
        coinsText.text = (coins < 0 ? "-$" : "$") + Mathf.Abs(coins).ToString();
        potText.text = "$" + pot.ToString();
        pHand.AddCard(deck.DrawCard());
        int cardCount = pHand.cards.Count - 1;
        pool.DealCard(pHand.cards[cardCount].name, cardCount);
        pScoreText.text = pHand.score.ToString();
        StartCoroutine(EndRound());
    }

    private IEnumerator EndRound()
    {
        turnButtons.SetActive(false);
        StartCoroutine(pool.DiscardCard("cardBack"));
        yield return new WaitForSeconds(delay);
        pool.DealCard(dHand.cards[1].name, 6);
        yield return new WaitForSeconds(delay);
        dScoreText.text = dHand.score.ToString();
        yield return new WaitForSeconds(delay);
        int cardCount = dHand.cards.Count - 1;
        while (dHand.score < 17 && cardCount != 4)
        {
            dHand.AddCard(deck.DrawCard());
            cardCount++;
            pool.DealCard(dHand.cards[cardCount].name, cardCount + 5);
            dScoreText.text = dHand.score.ToString();
            yield return new WaitForSeconds(delay);
        }
        CalculateWinner();
        yield return new WaitForSeconds(delay);
        coins += pot;
        pot = 0;
        coinsText.text = (coins < 0 ? "-$" : "$") + Mathf.Abs(coins).ToString();
        potText.text = "$" + pot.ToString();
        dScoreText.text = "";
        pScoreText.text = "";
        StartCoroutine(DiscardCards());
    }

    private void CalculateWinner()
    {
        if (pHand.score > 21)
            pot = 0;
        else if (dHand.score > 21 || pHand.score > dHand.score)
            pot *= 2;
        else if (pHand.score < dHand.score)
            pot = 0;
        potText.text = "$" + pot.ToString();
    }

    private IEnumerator DiscardCards()
    {
        yield return new WaitForSeconds(delay);
        foreach (Collection hand in new List<Collection> { pHand, dHand })
        {
            while (hand.cards.Count != 0)
            {
                Card c = hand.DrawCard();
                discard.cards.Add(c);
                StartCoroutine(pool.DiscardCard(c.name));
            }
        }
        yield return new WaitForSeconds(delay);
        if (round != 5 && coins > 0) RoundStart();
        else StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(delay);
        scoreText.text = "Score: " + coinsText.text;
        if (coins > controller.Highscore)
            controller.Highscore = coins;
        highscoreText.text = "Highscore: $" + controller.Highscore.ToString();
        controller.GamesPlayed++;
        controller.RoundsPlayed += round;
        menu.SetActive(true);
    }

    public void PlayButtonAudio()
    {
        audioManager.Play("select");
        if (controller.Vibrate) Vibration.Vibrate(30);
    }

    public void LoadMainMenu()
    {
        controller.LoadLevel(0);
    }
}
