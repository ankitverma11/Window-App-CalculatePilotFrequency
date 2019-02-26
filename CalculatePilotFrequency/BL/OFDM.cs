namespace CalculatePilotFrequency
{
    /// <summary>
    /// OFDM properties
    /// </summary>
    public class OFDM
    {
        public string BandName { get; set; }
        public int OFDMID { get; set; }
        public int CustomerID { get; set; }
        public double SubcarrierFrequency { get; set; }
        public int CyclicPrefixLength { get; set; }
        public int FFTSize { get; set; }
        public string PilotIndices { get; set; }
        public int CenterFrequency { get; set; }
        public int MarkerSelection { get; set; }
        public decimal Pilot1Frequency { get; set; }
        public decimal Pilot2Frequency { get; set; }
        public decimal Pilot1RelativeLevelAdjustment { get; set; }
        public decimal Pilot2RelativeLevelAdjustment { get; set; }
        public decimal Pilot1TotalLevelAdjustment { get; set; }
        public decimal Pilot2TotalLevelAdjustment { get; set; }
    }
}