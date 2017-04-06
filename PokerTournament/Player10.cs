using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            estimatedHandStrength = 0.0f;
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
                //how much did they bet in round 1?

                //draw? - how many?
                    //1
                    //2
                    //3
                    //4+
                        //they dont have shit

                //call?

                //bet/raise?
                    //how much?

                    //how many times has this happened?
                        //twice means

            //EVAL HAND
            //save hand strength
            Card highCard = null;
            handStrength = Evaluate.RateAHand(hand, out highCard); //(0 weak - 10 strong af)

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
        private void ReviewActions(List<PlayerAction> actions)
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
                            estimatedHandStrength = 5; //atleast rank 5 uses all 5 cards
                        }
                        else //draw
                        {
                            //switch based on how many cards are to be drawn
                            switch (act.Amount)
                            {
                                case 1:  //1, 2, 3 cards measn they prolly have somthing- average
                                    break;
                                case 2:
                                    break;
                                case 3: //most likely atleast have a pair
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
                        //how much was bet?

                        //how many times in a row have they bet? if its 2+ might mean good hand

                    }

                }
            }
        }
        #endregion
    }
}
