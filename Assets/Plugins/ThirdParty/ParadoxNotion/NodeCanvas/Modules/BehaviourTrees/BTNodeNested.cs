using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{
    [Category("SubGraphs")]
    [Color("ffe4e1")]
    abstract public class BTNodeNested<T> : BTNode, IGraphAssignable<T> where T : Graph
    {
        [SerializeField] private List<BBMappingParameter> _variablesMap;

        public BBParameter<Component> subNodeAgent;


        abstract public T subGraph { get; set; }
        abstract public BBParameter subGraphParameter { get; }

        public T currentInstance { get; set; }
        public Dictionary<Graph, Graph> instances { get; set; }
        public List<BBMappingParameter> variablesMap { get { return _variablesMap; } set { _variablesMap = value; } }

        Graph IGraphAssignable.subGraph { get { return subGraph; } set { subGraph = (T)value; } }
        Graph IGraphAssignable.currentInstance { get { return currentInstance; } set { currentInstance = (T)value; } }
    }
}