using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace ShaderGraphEnhancer
{
#if SGE_INSTALLED
	using UnityEditor.ShaderGraph;
	using UnityEditor.ShaderGraph.Drawing;
#else
	public class MaterialGraphView {}
	public class GraphData { }

	public class AbstractMaterialNode { }
#endif

	internal class KeyboardShortcutHelper
    {
        private MaterialGraphView GraphView { get; }
        private GraphData GraphData { get; }
 
        public KeyboardShortcutHelper(MaterialGraphView graphView, GraphData graph)
        {
#if SGE_INSTALLED
            GraphView = graphView;
            GraphData = graph;
 
            GraphView.RegisterCallback<KeyDownEvent>(OnKeyDown);
#endif
        }
 
        void OnKeyDown(KeyDownEvent evt) {
#if SGE_INSTALLED
	        
	        
            if (GraphData == null) return;
 
            Vector2 pos = evt.originalMousePosition;
            
            switch (evt.keyCode) {
                case KeyCode.O:
                    CreateNode(() => new OneMinusNode(), pos);
                    break;
                case KeyCode.M:
                    CreateNode(() => new MultiplyNode(), pos);
                    break;
                case KeyCode.L:
                    CreateNode(() => new LerpNode(), pos);
                    break;
                case KeyCode.D:
                    CreateNode(() => new DivideNode(), pos);
                    break;
                case KeyCode.S:
                    CreateNode(() => new SubtractNode(), pos);
                    break;
                case KeyCode.A:
                    CreateNode(() => new AddNode(), pos);
                    break;
                case KeyCode.F:
                    CreateNode(() => new FractionNode(), pos);
                    break;
                case KeyCode.Alpha1:
                    CreateNode(() => new Vector1Node(), pos);
                    break;
                case KeyCode.Alpha2:
                    CreateNode(() => new Vector2Node(), pos);
                    break;
                case KeyCode.Alpha3:
                    CreateNode(() => new Vector3Node(), pos);
                    break;
                case KeyCode.Alpha4:
                    CreateNode(() => new Vector4Node(), pos);
                    break;
                case KeyCode.Alpha5:
                    CreateNode(() => new ColorNode(), pos);
                    break;
            }
#endif
        }
 
        private void CreateNode(Func<AbstractMaterialNode> createNode, Vector2 pos) {
#if SGE_INSTALLED
			AbstractMaterialNode multiplyNode = createNode();
 
            var drawState = multiplyNode.drawState;
            Vector2 p = GraphView.contentViewContainer.WorldToLocal(pos);
 
            drawState.position = new Rect(p.x, p.y, drawState.position.width, drawState.position.height);
            multiplyNode.drawState = drawState;
            GraphData.AddNode(multiplyNode);
#endif
        }
    }
}