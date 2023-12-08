namespace CustomDataTypes
{
    public enum QuestionnaireOrderType
    {
        VVIQthenOSIQthenSAM = 1,
        VVIQthenSAMthenOSIQ = 2,
        OSIQthenVVIQthenSAM = 3,
        OSIQthenSAMthenVVIQ = 4,
        SAMthenVVIQthenOSIQ = 5,
        SAMthenOSIQthenVVIQ = 6
    }
}