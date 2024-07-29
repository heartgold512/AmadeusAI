using System.Collections.Generic;

namespace AmadeusAI.Parsers.Models
{
    /// <summary>
    /// TanResponses are objects populated from a parser that are used to populate behavior 
    //response tables ultimately retrieving the data
    /// </summary>
    class TanResponse
    {
        public List<string> ResponseTriggers { get; set; }
        public List<Expression> ResponseChain { get; set; }

        public TanResponse()
        {
            ResponseTriggers = new List<string>();
            ResponseChain = new List<Expression>();
        }
    }
}