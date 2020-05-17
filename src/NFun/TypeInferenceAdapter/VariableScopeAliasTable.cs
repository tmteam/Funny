using System;
using System.Collections.Generic;

namespace NFun.TypeInferenceAdapter
{
    /// <summary>
    /// Variable table. It needs to give special names to variable during TI-setup process
    /// </summary>
    public class VariableScopeAliasTable
    {
        public VariableScopeAliasTable()
        {
            _variableAliasesStack = new List<Dictionary<string, string>>();
            _variableAliasesStack.Add(new Dictionary<string, string>());
        }
        
        private readonly List<Dictionary<string, string>> _variableAliasesStack;
        
        public bool HasVariable(string variableName)
        {
            for (int i = _variableAliasesStack.Count  - 1; i >= 0; i--)
            {
                if (_variableAliasesStack[i].ContainsKey(variableName))
                    return true;
            }
            return false;
        }

        public bool AddVariableAlias(string originName, string alias)
        {
            var currentFrame = _variableAliasesStack[_variableAliasesStack.Count - 1];
            if (currentFrame.ContainsKey(originName))
                return false;
            currentFrame.Add(originName, alias);
            return true;
        }
        public string AddVariableAlias(int node, string variableName)
        {
            var currentFrame = _variableAliasesStack[_variableAliasesStack.Count - 1];
            var alias = MakeAlias(node, variableName);
            if (currentFrame.ContainsKey(variableName))
            {
                throw new InvalidOperationException("varable name already exist");
            }
            currentFrame.Add(variableName, alias);
            return alias;
        }
        public string GetVariableAlias(string origin)
        {
            for (int i = _variableAliasesStack.Count  - 1; i >= 0; i--)
            {
                if (_variableAliasesStack[i].ContainsKey(origin))
                    return _variableAliasesStack[i][origin];
            }
            return origin;
        }

        public void EnterScope(int nodeNumber, IList<string> scopeVariables = null)
        {
            var dictionary = new Dictionary<string,string>();
            if(scopeVariables!=null)
                foreach (var scopeVariable in scopeVariables)
                    dictionary.Add(scopeVariable, MakeAlias(nodeNumber, scopeVariable));

            _variableAliasesStack.Add(dictionary);
        }
        public void ExitScope()
        {
            _variableAliasesStack.RemoveAt(_variableAliasesStack.Count-1);
        }
        private string MakeAlias(int nodeLayerId, string varName) => nodeLayerId + "::" + varName;

    }
}