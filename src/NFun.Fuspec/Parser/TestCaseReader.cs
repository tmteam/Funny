using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Nfun.Fuspec.Parser.FuspecParserErrors;
using Nfun.Fuspec.Parser.Model;
using ParcerV1;
using static Nfun.Fuspec.Parser.FuspecParserHelper;

namespace Nfun.Fuspec.Parser
{
    public class TestCasesReader
    {
        private List<string> _listOfString = new List<string>();
        private TestCaseBuilder _testCaseBuilder;
        private List<FuspecTestCase> _fuspecTestCases;
        private int _index = 0;
        private bool _isReading = false;
        private List<string> _script= new List<string>();
        private List<FuspecParserError> _errors=new List<FuspecParserError>();
        
        public TestCasesReader(StreamReader streamReader)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
                _listOfString.Add(line);
            _testCaseBuilder = new TestCaseBuilder();
            _fuspecTestCases = new List<FuspecTestCase>();
        }

        public FuspecTestCases Read()
        {
            var state = TestCaseParseState.FindingOpeningString;
            foreach (var lineStr in _listOfString)
            {
                state = ReadNext(lineStr, state);
                _index++;
            }

            if (_isReading)
                AddLast(state); 
              
              if (_errors.Any())
                  return new FuspecTestCases(_errors.ToArray());
            return new FuspecTestCases(_fuspecTestCases.ToArray());
        }
        
        private TestCaseParseState ReadNext(string str, TestCaseParseState testCaseParseState)
        {
            switch (testCaseParseState)
            {   case TestCaseParseState.FindingOpeningString:
                    return FindOpeningString(str);
                case TestCaseParseState.ReadingName :
                    return FindName(str);
                case TestCaseParseState.ReadingTags:
                    return FindTags(str);
                case TestCaseParseState.ReadingBody:
                    return ReadBody(str);
                case TestCaseParseState.ReadingParamsIn:
                    return ReadParamsIn(str);
                case TestCaseParseState.ReadingParamsOut:
                    return ReadParamsOut(str);
                case TestCaseParseState.ReadingValues:
                    return ReadValues(str);
                default:
                    return WriteError(new FuspecParserError(FuspecErrorType.Unknown, _index));
            }
        }

        private TestCaseParseState FindOpeningString(string str)
        {
            if (str.Trim() == "")
                return TestCaseParseState.FindingOpeningString;
            _isReading = false;
            
            _testCaseBuilder = new TestCaseBuilder();
            if (IsSeparatingLine(str, '*'))
            {
                _isReading = true;
                _script=new List<string>();
                return TestCaseParseState.ReadingName;
            }
            return WriteError(new FuspecParserError(FuspecErrorType.OpeningStringMissed, _index));
        }
        
        private TestCaseParseState FindName(string str)
        {
            if (str.Trim() == "")
                return TestCaseParseState.ReadingName;
            var name = FindKeyWord("| TEST", str);
            if (name == null || name.Trim()=="")
                return WriteError(new FuspecParserError(FuspecErrorType.NamedMissed, _index));
            _testCaseBuilder.Name =name.Trim();
            return TestCaseParseState.ReadingTags;
        }

        private TestCaseParseState FindTags(string str)
        {
            if (str.Trim()== "")
                return TestCaseParseState.ReadingTags;
            if (IsSeparatingLine(str, '*'))
                return TestCaseParseState.ReadingParamsIn;
            var tags = FindKeyWord("| TAGS", str);
            if (tags == null )
                return WriteError(new FuspecParserError(FuspecErrorType.EndingHeadMissed,_index));
            if (tags.Trim() != "")
            {
                var splittedTags = tags.Split(',');
                foreach (var tag in splittedTags)
                    if (tag.Trim() != "")
                        _testCaseBuilder.Tags.Add(tag.Trim());
            }
            return TestCaseParseState.ReadingTags;
        }
/*
        private TestCaseParseState ReadParamsIn(string str)
        {
            return ReadBody(str);
        }

        private TestCaseParseState ReadParamsOut(string str)
        {
            return ReadBody(str);
        }*/
        
        private TestCaseParseState ReadParamsIn(string str)
        {
            if (str.Trim() == "")
            {
                _script.Add(str);
                return TestCaseParseState.ReadingParamsIn;
            }
            var paramIn = FindKeyWord("| in", str);
            if (paramIn == null)
                return ReadParamsOut(str);
            if (paramIn.Trim() == "")
                return WriteError(new FuspecParserError(FuspecErrorType.ParamInMissed, _index));
            var splittedParamsIn = paramIn.Split(',');
            foreach (var tag in splittedParamsIn)
                if (tag.Trim() != "")
                {
                    _testCaseBuilder.ParamsIm.Add(tag.Trim());
                }

            if (!_testCaseBuilder.ParamsIm.Any())
                return WriteError(new FuspecParserError(FuspecErrorType.ParamInMissed, _index));
            _script=new List<string>();
            return TestCaseParseState.ReadingParamsOut;
        }

        private TestCaseParseState ReadParamsOut(string str)
        {
            if (str.Trim() == "")
            {
                _script.Add(str);
                return TestCaseParseState.ReadingParamsOut;
            }
            var paramOut = FindKeyWord("| out", str);
            if (paramOut == null)
                return ReadBody(str);
            if (paramOut=="" || paramOut.Substring(0,1)!=" ")
                return WriteError(new FuspecParserError(FuspecErrorType.ParamOutMissed, _index));
            var splittedParamsOut = paramOut.Split(',');
            foreach (var param in splittedParamsOut)
            {
                if (param.Trim()=="")
                    return WriteError(new FuspecParserError(FuspecErrorType.ParamOutMissed, _index));
                    _testCaseBuilder.ParamsOut.Add(param.Trim());
            }
            _script = new List<string>();
            
            return TestCaseParseState.ReadingBody;
        }

        private TestCaseParseState ReadBody(string str)
        {
            if (str.Trim() == "")
            {
                _script.Add(str);
                return TestCaseParseState.ReadingBody;
            }

            if (FindKeyWord("|***", str) != null || FindKeyWord("|---",str)!=null)
            {
                if (IsSeparatingLine(str, '-') || IsSeparatingLine(str, '*'))
                    return ReadValues(str);
                return WriteError(new FuspecParserError(FuspecErrorType.OpeningStringMissed, _index));
            }
            _script.Add(str);
            return TestCaseParseState.ReadingBody;
        }

        private TestCaseParseState ReadValues(string str)
        {
            if (str.Trim() == "")
                return TestCaseParseState.ReadingValues;
            if (IsSeparatingLine(str,'*'))
                return AddTestCase(str);
            return TestCaseParseState.ReadingValues;
        }

        private TestCaseParseState AddTestCase(string str)
        {
            if (!_script.Any())
                return WriteError(new FuspecParserError(FuspecErrorType.ScriptMissed, _index));
            BuildScript();
            _fuspecTestCases.Add(_testCaseBuilder.Build());
            return FindOpeningString(str);
        }

        private void AddLast(TestCaseParseState state)
        {
            if ((state == TestCaseParseState.ReadingName) || (state == TestCaseParseState.ReadingTags))
                WriteError(new FuspecParserError(FuspecErrorType.NoEndingTestCase, _index));
            if (!_script.Any() && ((state == TestCaseParseState.ReadingParamsOut ||
                                   state == TestCaseParseState.ReadingParamsIn)))
                WriteError(new FuspecParserError(FuspecErrorType.NoEndingTestCase, _index));
            else
            {
                if (!_script.Any())
                    WriteError(new FuspecParserError(FuspecErrorType.NoEndingTestCase, _index));
                else
                {
                    BuildScript();
                    _fuspecTestCases.Add(_testCaseBuilder.Build());
                }
            }
        }

        private void BuildScript()
        {
            _testCaseBuilder.Script = "";
            foreach (var strScript in _script)
                _testCaseBuilder.Script = _testCaseBuilder.Script + strScript + "\r";
            _testCaseBuilder.Script =
                _testCaseBuilder.Script.Substring(0, _testCaseBuilder.Script.Length - 1);
        }

        private TestCaseParseState WriteError(FuspecParserError fuspecParserError)
        {
            _errors.Add(fuspecParserError);
            _isReading = false;
            return TestCaseParseState.FindingOpeningString;
        }
    }
}