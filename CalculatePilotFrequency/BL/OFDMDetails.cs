using System.Collections.Generic;
namespace CalculatePilotFrequency
{
    /// <summary>
    /// Ofdm details
    /// </summary>
    public class OFDMDetails
    {
        public OFDMCredential OFDMCredentials { get; set; }
        public List<OFDM> OFDMDataList { get; set; }
    }
}