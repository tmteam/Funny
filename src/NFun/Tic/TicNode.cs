﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NFun.Tic.SolvingStates;

namespace NFun.Tic
{
    public enum TicNodeType
    {
        /// <summary>
        /// input or output variable of expression
        /// TicNode's name equals to variable name
        /// </summary>
        Named = 2,
        /// <summary>
        /// Syntax node. TicNode's name equals to node's order number
        /// </summary>
        SyntaxNode = 4,
        /// <summary>
        /// Generic type from function/constant signature or created in process of solving. 
        /// </summary>
        TypeVariable = 8,
    }

    public class TicNode
    {
        internal int VisitMark = -1;
        internal bool Registrated = false;

        private ITicNodeState _state;
        public static TicNode CreateTypeVariableNode(ITypeState type) 
            => new TicNode(type.ToString(), type, TicNodeType.TypeVariable);

        private static int _interlockedId = 0;
        private readonly int _uid = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TicNode CreateSyntaxNode(int id, ITicNodeState state, bool registrated = false)
            => new TicNode(id, state, TicNodeType.SyntaxNode) {Registrated = registrated};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static  TicNode CreateNamedNode(object name, ITicNodeState state) 
            => new TicNode(name, state, TicNodeType.Named) {Registrated = true};


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TicNode CreateTypeVariableNode(string name, ITicNodeState state, bool registrated = false)
            => new TicNode(name, state, TicNodeType.TypeVariable) {Registrated = registrated};
        
        private TicNode(object name, ITicNodeState state, TicNodeType type)
        {
            _uid =  Interlocked.Increment(ref _interlockedId);
            Name = name;
            State = state;
            Type = type;
        }
        public TicNodeType Type { get; }
        public List<TicNode> Ancestors { get; } = new List<TicNode>();
        public bool IsMemberOfAnything { get; set; }
        public bool IsSolved => _state.IsSolved;

        public ITicNodeState State
        {
            get => _state;
            set
            {
                Debug.Assert(value != null);
                Debug.Assert(_state==null || !(IsSolved && !value.Equals(_state)),"Node is already solved");

                if (value is StateArray array)
                    array.ElementNode.IsMemberOfAnything = true;
                else
                {
                    Debug.Assert(!(value is StateRefTo refTo && refTo.Node== this),"Self referencing node");
                }
                _state = value;
            }
        }

        public object Name { get; }
        public override string ToString()
        {
            if (Name == _state.ToString())
                return Name.ToString();
            else 
                return $"{Name}:{_state}";
        }

        public void PrintToConsole()
        {
            if(!TraceLog.IsEnabled)
                return;
            
#if DEBUG
            if (TraceLog.IsEnabled)
            {
                TraceLog.Write($"{Name}:", ConsoleColor.Green);
                TraceLog.Write(State.Description);
                if (Ancestors.Any())
                    TraceLog.Write("  <=" + string.Join(",", Ancestors.Select(a => a.Name)));
                TraceLog.WriteLine();
            }
#endif
        }

        public bool TryBecomeConcrete(StatePrimitive primitiveState)
        {
            if (_state is StatePrimitive oldConcrete)
                return oldConcrete.Equals(primitiveState);
            if (_state is ConstrainsState constrains)
            {
                if (constrains.Fits(primitiveState))
                {
                    _state = primitiveState;
                    return true;
                }
            }
            return false;
        }
        public bool TrySetAncestor(StatePrimitive anc)
        {
            if (anc.Equals(StatePrimitive.Any))
                return true;
            var node = this;
            if (node.State is StateRefTo)
                node = node.GetNonReference();

            if (node.State is StatePrimitive oldConcrete)
            {
                return oldConcrete.CanBeImplicitlyConvertedTo(anc);
            }
            else if (node.State is ConstrainsState constrains)
            {
                if (!constrains.TryAddAncestor(anc))
                    return false;
                var optimized = constrains.GetOptimizedOrNull();
                if (optimized == null)
                    return false;
                State = optimized;
                return true;
            };
            return false;
        }
        public TicNode GetNonReference()
        {
            var result = this;
            if (result.State is StateRefTo referenceA)
            {
                result = referenceA.Node;
                if (result.State is StateRefTo)
                    return result.GetNonReference();
            }

            return result;
        }
        public override int GetHashCode() => _uid;
    }
}
