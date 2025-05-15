using System;
using System.Collections.Generic;
using System.Text;

namespace MASTERMIND
{
   public  class saveGame
    {
        public int rowround;
        public int[] goalFrame;
        public int[,] boardFrame;
        public int[,] mindsFrame;

        public saveGame()
        {
            goalFrame = new int[4];
            boardFrame = new int[8, 4];
            mindsFrame = new int[8, 2];

            for(int x = 0; x < boardFrame.GetLength(0); x++)
            {
                for (int y = 0; y <boardFrame.GetLength(1); y++)
                {
                    boardFrame[x, y] = -1;
                }
            }
        }
    }
}
