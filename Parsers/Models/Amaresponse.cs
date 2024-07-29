using System.Collections.Generic;

namespace AmadeusAI.Parsers.Models
{
    /// <summary>
    /// Amaresponse are objects populated from a parser that are used to populate behavior response tables.
    /// </summary>
    class amareresponse
    {
        public List<string> ResponseTriggers { get; set; } //gathering and setting the responses 
        public List<Expression> ResponseChain { get; set; }

        public amareresponse()
        {
            ResponseTriggers = new List<string>();
            ResponseChain = new List<Expression>();
        }
    }
}