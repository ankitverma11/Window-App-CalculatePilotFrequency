namespace CalculatePilotFrequency
{
    /// <summary>
    /// Status after saving 
    /// </summary>
    public enum CMTSStatus
    {
        EmptyUserNamePassword = 0,
        DataSaved,
        ProblemOnSave,
        CredentialsNotValid,
        SuccessOnGet,
        FailedOnGet,
        RangeNotValidForSubcarrierFrequency,
        NotValidValueForCyclicPrefixLength,
        NotValidValueForFFTSize,
        NotValidValueForCenterFrequency,
        NotValidValueForMarkerSelection,
        NotValidValueForPilot1Frequency,
        NotValidValueForPilot2Frequency,
        NotValidValueForPilot1RelativeLevelAdjustment,
        NotValidValueForPilot2RelativeLevelAdjustment
    };
   
}