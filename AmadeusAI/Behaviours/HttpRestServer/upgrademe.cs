using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusAI.Behaviours.HttpRestServer
{
    internal class newunused
    {
        /* NOTE:
            
              * The quick solution to checking if an entry exists would be to just iterate through every pair and then check every trigger in each key
              * which is being done in the eventarrived method.
              * Alternatively, implementing a custom comparer can be done which is what I've done here (see TriggerComparer.cs)
              * I think it would be better to just have an individual process name as a key because duplicating values is more performant than duplicating keys
              * Big O O(1) time complexity is one of the main strengths of using a dictionary in the first place but that is lost when storing an array as a key.
              * Also unrelated, but it might be better to just create a public dictionary that gets populated from the parser class so that this method can be moved to keep this class cleaner.
              */

        // If key already exists in the table, append the new response chain
    }
}
