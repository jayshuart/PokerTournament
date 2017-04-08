using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
HAND STRENGTH CHANCES
Royal flush <0.001%
Straight flush (not including royal flush) <0.002%
Four of a kind 0.02%
Full house 0.14%
Flush (excluding royal flush and straight flush) 0.20%
Straight (excluding royal flush and straight flush) 0.39%
Three of a kind 2.11%
Two pair 4.75%
One pair 42.30%
No pair / High card 50.10%
*/

namespace PokerTournament
{
    /// <summary>
    /// Team 10 Poker Player
    /// Kevin Idzik, Aiden Melendez, Joel Shuart
    /// </summary>
    class Player10 : Player
    {
        #region fields
        //hand weights
        private float estimatedHandStrength; //estimation of their hand strength
        private int handStrength; //own hand strength - Evaluate.RateAHand(hand, out highCard);
        private float bluffWeight; //how willing are we to just bluff?

        //"memory"
        private int bettingCycleCount; //times we went back and forth betting/raising
        private float bluffLikelihood; //based on how they played and actual hand strength

        #endregion
        /// <summary>
        /// Player10 Constructor
        /// </summary>
        /// <param name="idNum">Player ID number</param>
        /// <param name="nm">Player name</param>
        /// <param name="mny">Amount of money the player has</param>
        public Player10(int idNum, string nm, int mny) : base(idNum, nm, mny)
        {
            //initialize fields
            estimatedHandStrength = 1;
            handStrength = 0;
            bluffWeight = 0.0f;

            bettingCycleCount = 0;
            bluffLikelihood = 0.0f;
        }

        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {

            return new PlayerAction(Name, "Bet1", "bet", 1); //just so it doesnt bitch about not having a return
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            //throw new NotImplementedException();

            //review actions of other player
            EvaluateActions(actions);

            //EVAL HAND
            //save hand strength
            Card highCard = null;
            handStrength = Evaluate.RateAHand(hand, out highCard); //(1 weak - 10 strong af)

                //fold if really weak

                //bluff for 2+ at this point in the round

            return new PlayerAction(Name, "Bet1", "bet", 1);

        }

        public override PlayerAction Draw(Card[] hand)
        {
            // BRANCHING BEHAVIOR TREE (?) //

            // Get hand Eval
            Card highCard = null;
            handStrength = Evaluate.RateAHand(hand, out highCard);

            // number of tossed cards (and number to be drawn)
            // pass into PlayerAction return at the end
            int removed = 0;
            
            // Do stuff according to handStrength
            switch (handStrength)
            {
                case 1: // weakest hand: HIGH CARD
                    if (highCard.Value >= 10)       // Check the highCard's value, if highCard is a 10-J-Q-K-A
                    {
                        
                        for (int i = 0; i < hand.Length; i++)   // remove everything but the high card
                        {
                            if (hand[i] == highCard)
                                continue;   // ignore if the current card is the high card

                            hand[i] = null; // remove
                            removed++;
                        }

                        //thisAction = new PlayerAction(Name, "Draw", "draw", removed); ////////////////DO THIS AT THE END OF SWITCH?????

                        Console.WriteLine("Player 10 threw away and will draw" + removed + " cards.");
                    }
                    else // if high card is not 10-J-Q-K-A then all these cards mean literally nothing, toss all
                    {
                        for (int i = 0; i < hand.Length; i++)
                            hand[i] = null;

                        //thisAction = new PlayerAction(Name, "Draw", "draw", 5);///////////////////////////

                        Console.WriteLine("Player 10 throws away its entire hand.");
                    }
                    break;

                case 2: // 1-PAIR
                    int pairValue = 0;  // have to get the value of the 1pair, must be initialized to something

                    for (int i = 2; i < 15; i++) // check all values
                    {
                        if (Evaluate.ValueCount(i, hand) == 2)  // count occurences of value (once 2 are found, break from for loop)
                        {
                            pairValue = i;
                            break;
                        }
                    }

                    // optimize chances of getting a higher hand
                    // if the high card is not one of the pair AND it is 10-J-Q-K-A
                    if (highCard.Value != pairValue && highCard.Value >= 10)
                    {
                        for (int i = 0; i < hand.Length; i++)
                        {
                            if (hand[i].Value == pairValue || hand[i].Value == highCard.Value)
                                continue;   // do not toss if the current card is one of the pair OR if it is the HIGH CARD (that is different from the pair in this case)

                            hand[i] = null;
                        }

                        removed = 2;
                    }
                    else // otherwise toss everything that isn't the pair
                    {
                        for (int i = 0; i < hand.Length; i++)
                        {
                            if (hand[i].Value == pairValue)
                                continue;
                            hand[i] = null;
                        }

                        removed = 3;
                    }

                    break;

                case 3: // 2-PAIR
                    // Get 2 pairs value
                    int pair1Value = 0;
                    int pair2Value = 0;

                    // Count occurances of values and put as pair1's value
                    for (int i = 2; i < 15; i++)
                    {
                        if (Evaluate.ValueCount(i, hand) == 2)
                        {
                            pair1Value = i;
                            break;
                        }
                    }

                    // Count occurences of values and put as pair2's value
                    for (int i = 2; i < 15; i++)
                    {
                        if (Evaluate.ValueCount(i, hand) == 2)
                        {
                            if (i == pair1Value) continue;  // make sure to ignore pair 1
                            pair2Value = i;
                            break;
                        }
                    }

                    // Check if either pair's value is the high card
                    if (pair1Value == highCard.Value || pair2Value == highCard.Value)
                    {
                        for (int i = 0; i < hand.Length; i++)   // toss the 1 remaining card
                        {
                            if (hand[i].Value == pair1Value || hand[i].Value == pair2Value) continue;
                            hand[i] = null;
                        }

                        removed = 1;
                    }
                    else
                    {
                        // Any other factors to decide what to do????

                        // Otherwise return a stand pat action
                        return new PlayerAction(Name, "Draw", "stand pat", 0);
                    }
                    break;

                case 4: // 3-OF-A-KIND
                    // Get the triple's value
                    int tripleValue = 0;
                    for (int i = 2; i < 15; i++)
                    {
                        if (Evaluate.ValueCount(i, hand) == 3)
                        {
                            pairValue = i;
                            break;
                        }
                    }

                    // optimize chances of getting a higher hand
                    // if the high card is not one of the triple AND it is 10-J-Q-K-A
                    if (highCard.Value != tripleValue && highCard.Value >= 10)
                    {
                        for (int i = 0; i < hand.Length; i++)
                        {
                            if (hand[i].Value == tripleValue || hand[i].Value == highCard.Value)
                                continue;

                            hand[i] = null;
                        }

                        removed = 1;
                    }
                    else
                    {
                        // otherwise, toss the cards that aren't the triple and not 10-J-Q-K-A
                        for (int i = 0; i < hand.Length; i++)
                        {
                            if (hand[i].Value == tripleValue) continue;

                            hand[i] = null;
                        }

                        removed = 2;
                    }
                    break;

                // case 5: // STRAIGHT
                    // probably not worth it to toss anything draw again, weigh this?
                // case 6: // FLUSH
                    // same as STRIAGHT

                //case 7: // FULL HOUSE
                    // which pair has the high card? (the triple or double?)
                        //???????????
                
                // CASE 8: If 4 of a kind
                    // if the diffent card is the high AND (10+)
                        // weight whether or not to risk discarding the quadruple?
                    // otherwise stand pat


                case 9: // STRAIGHT FLUSH
                case 10: // ROYAL FLUSH
                    // just stand pat like a winner
                    return new PlayerAction(Name, "Draw", "draw", removed);
            }

            // otherwise, do approriate action
            return new PlayerAction(Name, "Draw", "draw", removed);
        }

        #region Helper Methods
        /// <summary>
        /// Gets list fo player actions and attempts to figure out what they mean, assigning fuzzy logic weights to the other players actions
        /// </summary>
        /// <param name="actions">List of player actions</param>
        private void EvaluateActions(List<PlayerAction> actions)
        {
            //review other players actions
            foreach (PlayerAction act in actions)
            {
                //only check other players acts
                if (act.Name != this.Name)
                {
                    //divide phases by bet or draw
                    if (act.ActionPhase == "draw") //draw
                    {
                        //check action name (stand pat, or draw)
                        if (act.ActionName == "stand pat")  //not taking any cards
                        {
                            //prolly has solid hand if they dont want to change out cards
                            estimatedHandStrength += 3; //atleast rank 5 uses all 5 cards
                        }
                        else //draw
                        {
                            //switch based on how many cards are to be drawn
                            switch (act.Amount)
                            {
                                case 1:  //1, 2, 3 cards measn they prolly have somthing- average
                                    estimatedHandStrength += 2;
                                    break;
                                case 2:
                                    estimatedHandStrength += 1;
                                    break;
                                case 3: //most likely atleast have a pair
                                    estimatedHandStrength = 2;
                                    break;
                                case 4: //4+ cards mean they dont have anything
                                case 5:
                                    estimatedHandStrength = 1; // set to 1 bc they dont have anythin
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                    else //bet 1 or 2, should be same checks
                    { 
                       //switch for action
                       switch(act.ActionName)
                        {
                            case "check":
                                break;
                            case "bet": //bet and raise should have same logic
                            case "raise":
                                //how much was bet?
                                if (act.Amount >= 100)
                                {
                                    estimatedHandStrength += 2 / bettingCycleCount; //scale incase of back and forth betting
                                }
                                else if (act.Amount >= 50)
                                {
                                    estimatedHandStrength += 1 / bettingCycleCount; //scale incase of back and forth betting
                                }
                                else if (act.Amount >= 25)
                                {
                                    estimatedHandStrength += .5f;
                                }
                                else if (act.Amount >= 15)
                                {
                                    estimatedHandStrength += .3f;
                                }
                                else if (act.Amount >= 10)
                                {
                                    estimatedHandStrength += .2f;
                                }
                                else if (act.Amount >= 5)
                                {
                                    estimatedHandStrength += .1f;
                                }
                                else if (act.Amount >= 2)
                                {
                                    estimatedHandStrength += .04f;
                                }
                                else if (act.Amount >= 1)
                                {
                                    estimatedHandStrength += .03f;
                                }

                                //up betting cycle count so we know if we are going back and forth
                                bettingCycleCount++;
                                break;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// used to chose what action todo in response to the last act of the other player and our weights
        /// </summary>
        /// <param name="lastAct">last PlayerAction done by the other player</param>
        /// <returns></returns>
        private PlayerAction ResponseAction(PlayerAction lastAct)
        {
            PlayerAction response = null;

            //switch for action
            switch (lastAct.ActionName)
            {
                case "call":
                    //call or fold
                    break;
                case "fold":
                    //they folded, we shouldnt do anything
                    break;
                case "check":
                    //check, bet, or fold
                    break;
                case "bet": //bet and raise should have same logic
                case "raise":
                    //raise, call, or fold
                    break;
            }


            //we know what todo! - return our repsonse
            return response;
        }
        #endregion
    }
}
