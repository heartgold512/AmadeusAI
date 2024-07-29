using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusAI.Tetris
{
    public class Block
    {
        public string BlockType { get; }
        public string ImagePath { get; }

        public Block(string blockType, string imagePath)
        {
            BlockType = blockType;
            ImagePath = imagePath;
        }
    }
}
