﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    /*
     * This program will run a five card draw poker tournament
     * for the Game AI course. This program will use separate PlayerN
     * classes (with N being the team number) for each team whose methods 
     * will be called for decisions.
     * Kevin Bierre  Spring 2017
     */
    class Program
    {
        static void Main(string[] args)
        {
            // create two players
            Human h0 = new Human(0, "Joe", 1000);
            Human h1 = new Human(1, "Sue", 1000);
            Player10 player10 = new Player10(-1, "AI10", 1000);
            Player10 player10_2 = new Player10(10, "AI10_2", 1000);

            // create the Game
            //Game myGame = new Game(h0, player10);
            Game myGame = new Game(player10, h0);
            //Game myGame = new Game(player10, player10_2); //AI testing

            myGame.Tournament(); // run the game
        }
    }
}
