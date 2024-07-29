using AmadeusAI.Parsers.Models;
using System.Collections.Generic;

namespace AmadeusAI.Parsers
{
    /// <summary>
    /// Implement this interface to make it easier to switch between different types of response parsers.
    /// </summary>
    interface IntelligenceParser
    {
        string GetData(string csvFileName);
        List<amareresponse> ParseData(string csv);
    }
}