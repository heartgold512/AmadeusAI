using System;
using System.Collections.Generic;
using System.Linq;

namespace AmadeusAI.Tetris
{
    public class TetQueue
    {
        private const string ImageFolderPath = "pack://application:,,,/AmadeusAI;component/Tetris/Tetrisgui/";

      public readonly Dictionary<string, string> defaultBlockTypes = new Dictionary<string, string>
        {
            {"IBlock", "Block-I.png"},
            {"JBlock", "Block-J.png"},
            {"TBlock", "Block-T.png"},
            {"LBlock", "Block-L.png"},
            {"SBlock", "Block-S.png"},
            {"ZBlock", "Block-Z.png"},
            {"OBlock", "Block-O.png"}
        };

        private readonly Dictionary<string, string> externalBlockTypes;
        private readonly Random random = new Random();

        public TetQueue(Dictionary<string, string> blockTypes)
        {
            externalBlockTypes = blockTypes;
        }

        public Block LoadRandomBlock()
        {
            string randomBlockType = externalBlockTypes.Keys.ElementAt(random.Next(externalBlockTypes.Count));

            if (externalBlockTypes.TryGetValue(randomBlockType, out string imageName))
            {
                string imagePath = ImageFolderPath + imageName;
                return new Block(randomBlockType, imagePath);
            }

            return null;
        }
    }
}


/*
public Block GetAndUpdate()
{
    Block block = NextBlock;

    do
    {
        NextBlock = RandomBlock();
    }
    //while (block.Id == NextBlock.Id);

    return block;
}
}
}
*/
