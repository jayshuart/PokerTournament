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
        #region Fields
        //hand weights
        private float theirHand; //estimation of their hand strength
        private int handStrength; //own hand strength - Evaluate.RateAHand(hand, out highCard);
        private float bluffWeight; //how willing are we to just bluff?

        //"memory"
        private int bettingCycleCount; //times we went back and forth betting/raising
        private float bluffLikelihood; //based on how they played and estim hand strength
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
            theirHand = 1;
            handStrength = 0;
            bluffWeight = 0.0f;

            bettingCycleCount = 1;
            bluffLikelihood = 0.0f;
        }

        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            PlayerAction otherPlayerAction = null;

            //Loop to get the other player's action
            foreach (PlayerAction action in actions)
            {
                if (action.Name != this.Name)
                    otherPlayerAction = action;
            }

            //Evaluate hand strength and get the high card
            Card highCard = null;
            handStrength = Evaluate.RateAHand(hand, out highCard);

            EvaluateActions(actions); //Check the actions of the other player

            Console.WriteLine("Dealer: " + Dealer);
            Console.WriteLine("Money: " + Money);
            Console.WriteLine("HighCard: " + highCard);
            Console.WriteLine("otherPlayerAction: " + otherPlayerAction);
            Console.WriteLine("theirHand: " + theirHand);
            Console.WriteLine("handStrength: " + handStrength);
            Console.WriteLine("bluffWeight: " + bluffWeight);
            Console.WriteLine("bettingCycleCount: " + bettingCycleCount);
            Console.WriteLine("bluffLikelihood: " + bluffLikelihood);

            if (actions.Count > 0 && actions[actions.Count].ActionPhase == "fold") //If the last action was fold
                Reset(); //Reset all values
            else //Respond to actions other than fold
                return ResponseAction(otherPlayerAction, highCard);

            return null;
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            //throw new NotImplementedException();
            //action to be done
            PlayerAction act = null;

            //EVAL HAND
            Card highCard = null;
            handStrength = Evaluate.RateAHand(hand, out highCard); //(1 weak - 10 strong af)

            //review actions of other player
            EvaluateActions(actions);

            //respond ot their action
            /*if(actions[actions.Count].ActionPhase == "Draw")
            {
                //last thing done was drawing - we get the first remove
                act = InitalBetting();
            }
            else */
            if (actions[actions.Count].ActionPhase == "fold")
            {
                //round is over reset all values
                Reset();
            }
            else //they did something we should respond to that
            {
                act = ResponseAction(actions[actions.Count], highCard);
            }

            return act;
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
        /// Gets list of player actions and attempts to figure out what they mean, assigning fuzzy logic weights to the other players actions
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
                            theirHand += 3; //atleast rank 5 uses all 5 cards
                        }
                        else //draw
                        {
                            //switch based on how many cards are to be drawn
                            switch (act.Amount)
                            {
                                case 1:  //1, 2, 3 cards means they prolly have something average
                                    theirHand += 2;
                                    break;
                                case 2:
                                    theirHand += 1;
                                    break;
                                case 3: //most likely atleast have a pair
                                    theirHand = 2;
                                    break;
                                case 4: //4+ cards mean they dont have anything
                                case 5:
                                    theirHand = 1; // set to 1 bc they dont have anything
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
                                //they dont wanna open, might indicate not great cards. not a real strong tell though
                                theirHand -= 0.3f;

                                //reset betting cycle
                                bettingCycleCount = 1;
                                break;
                            case "call":
                                //player is calling we can reset bet cycle
                                bettingCycleCount = 1;
                                break;
                            case "bet": //bet and raise should have same logic
                            case "raise":
                                //how much was bet?
                                if(act.Amount <= 5)
                                {
                                    theirHand += .1f;
                                }
                                else if (act.Amount <= 15)
                                {
                                    theirHand += .5f;
                                }
                                else if (act.Amount <= 35)
                                {
                                    theirHand += .7f;
                                }
                                else if (act.Amount <= 50)
                                {
                                    theirHand += 1.0f / bettingCycleCount;
                                }
                                else if (act.Amount <= 100)
                                {
                                    theirHand += 2.0f / bettingCycleCount; //dont make it go up tons if they but like this a buncha times
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
        /// Used to chose what action to do in response to the last act of the other player and our weights
        /// </summary>
        /// <param name="lastAct">Last PlayerAction done by the other player</param>
        /// <returns></returns>
        private PlayerAction ResponseAction(PlayerAction lastAct, Card highCard)
        {
            //how much wiggle room are we giving our estimatedHand weights?
            float wiggleRoom = -1; //negative for downward wiggle

            //round the estimated hand stregnth, also accounts for wiggle room
            int roundedEstimate = (int)Math.Round(theirHand + wiggleRoom, MidpointRounding.AwayFromZero);

            //PlayerAction to be returned and done by our AI
            PlayerAction response = null;

            //First round betting this will be null
            if (lastAct != null && lastAct.ActionPhase != "Draw")
                //switch for action
                switch (lastAct.ActionName)
                {
                    case "call": //call or fold
                        //compare estimHand and our own hands strength
                        if(roundedEstimate > handStrength)
                        {
                            //estim is more we should fold
                            response = new PlayerAction(Name, lastAct.ActionPhase, "fold", 0); //fold in the same phase with 0 dollars bc folding
                        }
                        else
                        {
                            //we trust our hand- call
                            response = new PlayerAction(Name, lastAct.ActionPhase, "call", 0);
                        }
                        break;
                    case "fold":
                        //they folded, we shouldnt do anything
                        break;
                    case "check": //check, bet, or fold
                        //how weak is our hand?
                        if(handStrength == 1)
                        {
                            if(roundedEstimate > handStrength)
                            {
                                //theirs is better and we dont have anything, we should fold
                                response = new PlayerAction(Name, lastAct.ActionPhase, "fold", 0); //fold in the same phase with 0 dollars bc folding
                            }
                            else
                            {
                                //how strong is our high card?
                                if(highCard.Value > 9)
                                {
                                    //a 10 or better - we'll check
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "check", 0);
                                }
                                else
                                {
                                    //we should fold
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "fold", 0);
                                }
                            }
                        }
                        else
                        {
                            //compare our hands
                            if(roundedEstimate > handStrength)
                            {
                                //are we willing to just bluff and try it?
                                if(roundedEstimate > handStrength + bluffWeight)
                                {
                                    //theirs is prolly too good - dont chance it
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "fold", 0);
                                }
                                else
                                {
                                    //how many times have we bet? OR are we too far from their strength to risk a bluff? - AND do we have money to use?
                                    if( (bettingCycleCount > 3 || Math.Abs(roundedEstimate - handStrength) > bluffWeight) && Money > 0)
                                    {
                                        //we've done it too many times, just check bud
                                        response = new PlayerAction(Name, lastAct.ActionPhase, "check", 0);
                                    }
                                    else
                                    {
                                        //bet- with bluffing enabled
                                        response = new PlayerAction(Name, lastAct.ActionPhase, "bet", CalcAmount(highCard.Value, true));
                                    }
                                
                                }
                            }
                            else
                            {
                                //how many times have we bet? and do we have money to bet?
                                if (bettingCycleCount > 3 && Money > 0)
                                {
                                    //we've done it too many times, just check bud
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "check", 0);
                                }
                                else
                                {
                                    //bet
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "bet", CalcAmount(highCard.Value, false));
                                }
                            }
                        }
                        break;
                    case "bet": //bet and raise should have same logic
                    case "raise": //raise, call, or fold

                        break;
                }
            else
                response = InitialBetting(lastAct, highCard, roundedEstimate); //Go through initial betting proceidures

            //we know what todo! - return our repsonse
            return response;
        }

        /// <summary>
        /// Calculates how much to bet/raise based on theirs and our own hand strength
        /// </summary>
        /// <param name="bluffing">Are we bluffing this bet? It'll change the actual amount used</param>
        /// <returns></returns>
        private int CalcAmount(int highCardValue, bool bluffing)
        {
            //start bet amount at 1 because 0 isnt a valid amount
            int amount = 1;

            //how good are our cards?
            switch(handStrength)
            {
                case 1: //5
                    amount = 5 / bettingCycleCount; //scale for betting cycle, so we dont drop 15 bucks on a crap hand
                    break;
                case 2:
                case 3:
                case 4:
                    //set aount
                    amount = (handStrength - 1) * 10;

                    //check for bluffing
                    if(bluffing)
                    {
                        //add a lil more ontop with scaling
                        amount += 10 / bettingCycleCount;
                    }
                    break;
                case 5: 
                case 6: 
                case 7:
                    //set amount based on hand strength, cut off a lil by betting cycle
                    amount = handStrength * 10 / bettingCycleCount;
                    break;
                case 8: 
                case 9:
                case 10:
                    
                    //account for inital bet
                    if(bettingCycleCount > 1)
                    {
                        //smaller bet and scaling
                        amount = (handStrength * 5) / bettingCycleCount;
                    }
                    else
                    {
                        //strong inital bet
                        amount = handStrength * 10;
                    }
                    break;
            }

            amount += (highCardValue * highCardValue); //Modify the bet by the highCardValue

            //do we have enough money?
            do
            {
                //cut amount down a bit
                amount -= amount / 10;

                //account for going below the anout of money we have
                if(amount < 1)
                {
                    //reset to 1
                    amount = 1;
                }

            } while (amount > Money);

            //we found the amount - give it back
            return amount;
        }

        /// <summary>
        /// Initial betting for when going first or after draw phase
        /// Can bet, check, and fold
        /// </summary>
        /// <param name="highCard"></param>
        /// <returns></returns>
        private PlayerAction InitialBetting(PlayerAction lastAct, Card highCard, int roundedEstimate)
        {
            string phase = null;

            //Get the right phase name
            if (lastAct == null)
                phase = "Bet1";
            else
                phase = "Bet2";

            switch (handStrength)
            {
                case 1: //Junk
                    if (roundedEstimate < handStrength) //Check if we feel good about this hand
                        return new PlayerAction(Name, phase, "check", 0);
                    else //Fold if we don't
                        return new PlayerAction(Name, phase, "fold", 0);

                case 2: //One pair
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                    {
                        for (int i = 2; i < 15; i++) //Loop for pair
                            if (Evaluate.ValueCount(i, Hand) == 2)
                            {
                                if (i > 12)
                                    return new PlayerAction(Name, phase, "bet", CalcAmount(i, true));
                                else
                                    return new PlayerAction(Name, phase, "bet", CalcAmount(i, false));
                            }
                    }
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 3: //Two pair
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                    {
                        for (int i = 15; i > 2; i++) //Loop for pair
                            if (Evaluate.ValueCount(i, Hand) == 2)
                                return new PlayerAction(Name, phase, "bet", CalcAmount(i, true));
                    }
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 4: //Three of a kind
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                    {
                        for (int i = 2; i < 15; i++) //Loop for pair
                            if (Evaluate.ValueCount(i, Hand) == 3)
                                return new PlayerAction(Name, phase, "bet", CalcAmount(i, true));
                    }
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 5: //Straight
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                        return new PlayerAction(Name, phase, "bet", CalcAmount(highCard.Value, false));
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 6: //Flush
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                        return new PlayerAction(Name, phase, "bet", CalcAmount(highCard.Value, false));
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 7: //Full house //Loop
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                    {
                        for (int i = 2; i < 15; i++) //Loop for pair
                            if (Evaluate.ValueCount(i, Hand) == 3)
                                return new PlayerAction(Name, phase, "bet", CalcAmount(i, true));
                    }
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 8: //Four of a kind
                    if (roundedEstimate < handStrength) //Bet if we feel good about this hand
                    {
                        for (int i = 2; i < 15; i++) //Loop for pair
                            if (Evaluate.ValueCount(i, Hand) == 4)
                                return new PlayerAction(Name, phase, "bet", CalcAmount(i, true));
                    }
                    else if (roundedEstimate == handStrength) //Check if we could win this hand
                        return new PlayerAction(Name, phase, "check", 0);

                    return new PlayerAction(Name, phase, "fold", 0); //Fold if we don't feel good about this hand

                case 9: //Straight flush
                    return new PlayerAction(Name, phase, "bet", CalcAmount(highCard.Value, false)); //Bet because who's gonna pull a royal flush

                case 10: //Royal flush
                    return new PlayerAction(Name, phase, "bet", CalcAmount(highCard.Value, false)); //Bet because we're UNSTOPPABLE
            }

            return new PlayerAction(Name, phase, "fold", 0); //Fold, but this should never happen
        }

        ///<summary>
        /// Resets all values because the round is over
        ///</summary>
        private void Reset()
        {
            //reset altered values
            theirHand = 1;
            bettingCycleCount = 1;
        }
        #endregion
    }
}