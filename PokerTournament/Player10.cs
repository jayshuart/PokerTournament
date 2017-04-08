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
            throw new NotImplementedException();
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
                        if (act.ActionName == "stand pat") //not taking any cards
                        {
                            //prolly has solid hand if they dont want to change out cards
                            theirHand += 3; //atleast rank 5 uses all 5 cards
                        }
                        else //draw
                        {
                            //switch based on how many cards are to be drawn
                            switch (act.Amount)
                            {
                                case 1:  //1, 2, 3 cards measn they prolly have somthing- average
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
                                    theirHand = 1; // set to 1 bc they dont have anythin
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
                                break;
                            case "call":
                                //player is calling we can reset bet cycle
                                bettingCycleCount = 0;
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
        /// used to chose what action todo in response to the last act of the other player and our weights
        /// </summary>
        /// <param name="lastAct">last PlayerAction done by the other player</param>
        /// <returns></returns>
        private PlayerAction ResponseAction(PlayerAction lastAct, Card highCard)
        {
            //how much wiggle room are we giving our estimatedHand weights?
            float wiggleRoom = -1; //negative for downward wiggle

            //round the estimated hand stregnth, also accounts for wiggle room
            int roundedEstimate = (int) Math.Round(theirHand + wiggleRoom, MidpointRounding.AwayFromZero);

            //PlayerAction to be returned and done by our AI
            PlayerAction response = null;

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
                                //how many times have we bet? OR are we too far from their strength to risk a bluff?
                                if(bettingCycleCount > 3 || Math.Abs(roundedEstimate - handStrength) > bluffWeight)
                                {
                                    //we've done it too many times, just check bud
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "check", 0);
                                }
                                else
                                {
                                    //bet- with bluffing enabled
                                    response = new PlayerAction(Name, lastAct.ActionPhase, "bet", CalcAmount(true));
                                }
                                
                            }
                        }
                        else
                        {
                            //how many times have we bet? 
                            if (bettingCycleCount > 3)
                            {
                                //we've done it too many times, just check bud
                                response = new PlayerAction(Name, lastAct.ActionPhase, "check", 0);
                            }
                            else
                            {
                                //bet
                                response = new PlayerAction(Name, lastAct.ActionPhase, "bet", CalcAmount(false));
                            }
                        }
                    }
                    break;
                case "bet": //bet and raise should have same logic
                case "raise": //raise, call, or fold

                    break;
            }


            //we know what todo! - return our repsonse
            return response;
        }

        /// <summary>
        /// Calculates how much to bet/raise based on theirs and our own hand strength
        /// </summary>
        /// <param name="bluffing">Are we bluffing this bet? itll change the actual amount used</param>
        /// <returns></returns>
        private int CalcAmount(bool bluffing)
        {
            //start bet amount at 1 because 0 isnt a valid amount
            int amount = 1;

            return amount;
        }
        #endregion
    }
}
