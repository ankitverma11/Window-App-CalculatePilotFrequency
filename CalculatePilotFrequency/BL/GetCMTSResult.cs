using System.Collections.Generic;

namespace CalculatePilotFrequency
{
    /// <summary>
    /// Result with status and data
    /// </summary>
    public class GetCMTSResult
    {
        public CMTSStatus status { get; set; }
        public IEnumerable<OFDM> Data { get; set; }
    }
}