using System;
using System.Linq;
using System.Reflection;
using NFun.Interpretation;
using NFun.ParseErrors;
using NFun.Runtime;
using NFun.SyntaxParsing;
using NFun.Types;

namespace NFun
{
    internal static class FluentApiTools
    {
        public static TOutput CreateOutputValueFromResults<TOutput>(
            FunnyRuntime runtime,
            Memory<(string, IOutputFunnyConverter, PropertyInfo)> outputs
        )
            where TOutput : new()
        {
            var span = outputs.Span;
            var answer = new TOutput();
            int settedCount = 0;
            for (int i = 0; i < outputs.Length; i++)
            {
                var (outName, outConverter, outProperty) = span[i];
                if (runtime.TryGetVariable(outName, out var actualOutput))
                {
                    outProperty.SetValue(answer, outConverter.ToClrObject(actualOutput.FunnyValue));
                    settedCount++;
                }
            }

            if (settedCount == 0)
                throw ErrorFactory.NoOutputVariablesSetted(outputs);
            return answer;
        }


        public static Memory<(string, IOutputFunnyConverter, PropertyInfo)> SetupManyAprioriOutputs<TOutput>(
            AprioriTypesMap aprioriTypesMap) where TOutput : new()
        {
            var outputProperties = typeof(TOutput).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var outputVarVals = new (string, IOutputFunnyConverter, PropertyInfo)[outputProperties.Length];
            int actualOutputsCount = 0;
            for (var i = 0; i < outputProperties.Length; i++)
            {
                var outputProperty = outputProperties[i];
                if (!outputProperty.CanWrite)
                    continue;
                var converter = FunnyTypeConverters.GetOutputConverter(outputProperty.PropertyType);
                var outputName = outputProperty.Name.ToLower();

                aprioriTypesMap.Add(outputName, converter.FunnyType);
                outputVarVals[actualOutputsCount] = new ValueTuple<string, IOutputFunnyConverter, PropertyInfo>(
                    outputName,
                    converter,
                    outputProperty);

                actualOutputsCount++;
            }

            return outputVarVals.AsMemory(0, actualOutputsCount);
        }

        internal static object GetClrOut(FunnyRuntime runtime) => GetOut(runtime).Value;

        private static IFunnyVar GetOut(FunnyRuntime runtime)
        {
            if (!runtime.TryGetVariable(Parser.AnonymousEquationId, out var outResult))
                throw ErrorFactory.OutputIsUnset();
            return outResult;
        }

        public static TOutput CalcSingleOutput<TOutput>(string expression)
        {
            var outputConverter = FunnyTypeConverters.GetOutputConverter(typeof(TOutput));
            var apriories = AprioriTypesMap.Empty;
            apriories.Add(Parser.AnonymousEquationId, outputConverter.FunnyType);

            var runtime = RuntimeBuilder.Build(
                expression, 
                BaseFunctions.DefaultFunctions, 
                Dialects.Classic,
                EmptyConstantList.Instance,
                apriories
                );

            if (runtime.Variables.Any(v => !v.IsOutput))
                throw ErrorFactory.UnknownInputs(runtime.GetInputVariableUsages());

            runtime.Run();

            return (TOutput)outputConverter.ToClrObject(GetOut(runtime).FunnyValue);
        }
        
        internal static void SetInputValues<TInput>(FunnyRuntime runtime,
            Memory<(string, IinputFunnyConverter, PropertyInfo)> inputMap, TInput value)
        {
            var span = inputMap.Span;

            foreach (var (name, converter, propertyInfo) in span)
            {
                if (!runtime.TryGetVariable(name, out var variable))
                    continue;

                if (variable is VariableSource source)
                    source.InternalFunnyValue = converter.ToFunObject(propertyInfo.GetValue(value));
            }
        }

        public static Memory<(string, IinputFunnyConverter, PropertyInfo)>
            SetupAprioriInputs<TInput>(AprioriTypesMap apriories)
        {
            var inputProperties = typeof(TInput).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var inputTypes = new (string, IinputFunnyConverter, PropertyInfo)[inputProperties.Length];
            int actualInputsCount = 0;

            for (var i = 0; i < inputProperties.Length; i++)
            {
                var inputProperty = inputProperties[i];
                if (!inputProperty.CanRead)
                    continue;
                var converter = FunnyTypeConverters.GetInputConverter(inputProperty.PropertyType);
                var inputName = inputProperty.Name.ToLower();

                apriories.Add(inputName, converter.FunnyType);
                inputTypes[i] = new ValueTuple<string, IinputFunnyConverter, PropertyInfo>(
                    inputName,
                    converter,
                    inputProperty
                );
                actualInputsCount++;
            }

            return inputTypes.AsMemory(0, actualInputsCount);
        }

        internal static void ThrowIfHasUnknownInputs(FunnyRuntime runtime,
            Memory<(string, IinputFunnyConverter, PropertyInfo)> expectedInputs)
        {
            var span = expectedInputs.Span;
            foreach (var actualInput in runtime.Variables)
            {
                if (actualInput.IsOutput)
                    continue;

                bool known = false;
                for (var i = 0; i < span.Length; i++)
                {
                    if (span[i].Item1.Equals(actualInput.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        known = true;
                        break;
                    }
                }

                if (!known)
                {
                    throw ErrorFactory.UnknownInputs(runtime.GetInputVariableUsages());
                }
            }
        }
    }
}