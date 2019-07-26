namespace ParcerV1
{
    public enum FuspecErrorType
    {
        Unknown = 0,
        OpeningStringMissed = 1,
        NamedMissed = 10,
        EndingHeadMissed = 20,
        SeparatedStringMissed=21,
        ScriptMissed = 30,
        NoEndingTestCase = 40,
        ParamInMissed=50,
        ParamOutMissed=60,
        NFunError_ICantParseParamTypeString=70,
        WrongSetCheckKit=80,
        SetKitMissed=81,
        CheckKitMissed=82,
        ExpectedSeparatedLine = 83,
    }
}