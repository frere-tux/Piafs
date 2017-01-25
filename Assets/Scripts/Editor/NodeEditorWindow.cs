using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

namespace Piafs
{
	public class NodeEditorWindow : EditorWindow
	{
		public class Node
		{
			public Modulator modulator;
			public Rect rect;
			public float maxY;
			public Color color;

			public Node(Rect r, Modulator m, Color c)
			{
				modulator = m;
				rect = r;
				color = c;
			}
		}

		static NodeEditorWindow window;

		public Rect window1, window2, _handleArea;
		private List<Node> _nodeWindows = new List<Node>();
		private Dictionary<Modulator, Node> _nodeFinder = new Dictionary<Modulator, Node>();
		private LevelDescriptor _levelDescriptor;
		private Vector2 _scrollPos = Vector2.zero;
		private Vector2 _dragStartPos = Vector2.zero;
		private Vector2 _scrollStartPos = Vector2.zero;
		private bool _isDragging;
		private int _treeDepth = 0;
		private bool _nodeOption, _refresh, _handleActive, _action;
		private Texture2D _resizeHandle, _aaLine;
		private GUIContent _icon;
		private float _winMinX, _winMinY;
		private int _mainwindowID;

		[MenuItem("Window/Node Editor")]
		static void Init()
		{
			window = (NodeEditorWindow)EditorWindow.GetWindow(typeof(NodeEditorWindow));
			window.titleContent = new GUIContent("Node Editor");
			window.ShowNodes();
		}

		void OnFocus()
		{
			SetLevelDescriptor(_levelDescriptor, true);
		}

		private void SetLevelDescriptor(LevelDescriptor _levelDescriptor, bool forceRefresh = false)
		{
			if(_levelDescriptor != this._levelDescriptor || forceRefresh)
			{
				this._levelDescriptor = _levelDescriptor;
				_nodeFinder.Clear();
				_nodeWindows.Clear();
				if(_levelDescriptor != null)BuildNodeTree();
			}
		}

		void BuildNodeTree()
		{
			_treeDepth = 0;
			float _maxY = 50f;
			BuildNodeTreeRecursive(_levelDescriptor.outputModulator, Vector2.one * 150f, ref _treeDepth, ref _maxY);
			_nodeWindows[0].color = Color.yellow;
			Debug.Log("Node windows : "+_nodeWindows.Count);
			Debug.Log("Tree depth : " + _treeDepth);
		}

		void BuildNodeTreeRecursive(Modulator modulator, Vector2 pos, ref int depth, ref float maxY)
		{
			if (!_nodeFinder.ContainsKey(modulator))
			{
				_nodeWindows.Add(new Node(new Rect(pos.x, pos.y, _winMinX, _winMinY), modulator,Color.white));
				_nodeFinder.Add(modulator,_nodeWindows[_nodeWindows.Count-1]);
			}
			_nodeWindows[_nodeWindows.Count - 1].maxY = maxY;
			depth++;
			BuildNodeTreeForModulatorList(modulator.ampModulators,pos, ref depth, ref maxY);
			BuildNodeTreeForModulatorList(modulator.freqModulators,pos, ref depth, ref maxY);
			BuildNodeTreeForModulatorList(modulator.phaseModulators,pos, ref depth, ref maxY);
		}

		void BuildNodeTreeForModulatorList(List<Modulator> list, Vector2 pos, ref int depth, ref float maxY)
		{
			float maximumY = maxY;
			pos.x += 150f;
			for (int i = 0; i < list.Count; i++)
			{
				pos.y = maxY + (i +1) * 100f;
				BuildNodeTreeRecursive(list[i], pos, ref depth, ref maxY);
				maximumY = Mathf.Max(pos.y, maxY);
			}
			maxY = maximumY;
		}

		private void ShowNodes()
		{
			_isDragging = false;
			_scrollPos = Vector2.zero;
			_winMinX = 120f;
			_winMinY = 50f;
			_resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Editor/Images/ResizeHandle.png", typeof(Texture2D)) as Texture2D;
			_aaLine = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Editor/Images/AA1x5.png", typeof(Texture2D)) as Texture2D;
			_icon = new GUIContent(_resizeHandle);
			_mainwindowID = GUIUtility.GetControlID(FocusType.Passive); //grab primary editor window controlID
		}

		void OnGUI()
		{
			SetLevelDescriptor(EditorGUILayout.ObjectField("Edited level : ",_levelDescriptor, typeof(LevelDescriptor), true) as LevelDescriptor);
			BeginWindows();
			//window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
			//window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
			for (int i = 0; i< _nodeWindows.Count; i++)
			{
				Rect scrolledRect = new Rect(_nodeWindows[i].rect.position + _scrollPos, _nodeWindows[i].rect.size);
				GUI.color = _nodeWindows[i].color;
				if (!_isDragging)
				{
					_nodeWindows[i].rect = GUI.Window(i, scrolledRect, DrawNodeWindow, _nodeWindows[i].modulator.GetType().Name);
					_nodeWindows[i].rect.position -= _scrollPos;
				}
				else
				{
					GUI.Window(i, scrolledRect, DrawNodeWindow, _nodeWindows[i].modulator.GetType().Name);
				}
			}
			GUI.color = Color.white;
			EndWindows();

			for (int i = 0; i < _nodeWindows.Count; i++)
			{
				foreach(Modulator m in _nodeWindows[i].modulator.ampModulators)
				{
					DrawNodeCurve(_nodeWindows[i].rect, _nodeFinder[m].rect);
				}
				foreach (Modulator m in _nodeWindows[i].modulator.freqModulators)
				{
					DrawNodeCurve(_nodeWindows[i].rect, _nodeFinder[m].rect);
				}
				foreach (Modulator m in _nodeWindows[i].modulator.phaseModulators)
				{
					DrawNodeCurve(_nodeWindows[i].rect, _nodeFinder[m].rect);
				}
			}	

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			if(GUILayout.Button("Refresh", EditorStyles.toolbarButton))
			{
				SetLevelDescriptor(_levelDescriptor, true);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			//if drag extends inner window bounds _handleActive remains true as event gets lost to parent window
			if ((Event.current.rawType == EventType.MouseUp) && (GUIUtility.hotControl != _mainwindowID))
			{
				GUIUtility.hotControl = 0;
			}
			if ((Event.current.rawType == EventType.MouseDown) && Event.current.button == 2 && !_isDragging)
			{
				_dragStartPos = Event.current.mousePosition;
				_scrollStartPos = _scrollPos;
				_isDragging = true;
				Debug.Log("start drag");
			}
			if((Event.current.rawType == EventType.MouseUp)&& Event.current.button == 2)
			{
				_isDragging = false;
				_scrollStartPos = _scrollPos;
				Debug.Log("end drag");
			}
			if(_isDragging)
			{
				//Debug.Log(_scrollPos);
				_scrollPos = _scrollStartPos + (Event.current.mousePosition - _dragStartPos) * 1f;

				Repaint();
			}
		}

		private void DrawNodeWindow(int id)
		{
			if (GUIUtility.hotControl == 0)  //mouseup event outside parent window?
			{
				_handleActive = false; //make sure handle is deactivated
			}

			float _cornerX = 0f;
			float _cornerY = 0f;

			_cornerX = _nodeWindows[id].rect.width;
			_cornerY = _nodeWindows[id].rect.height;


			//begin layout of contents
			//GUILayout.BeginArea(new Rect(1, 16, _cornerX - 3, _cornerY - 1));
			//GUILayout.BeginHorizontal(EditorStyles.toolbar);
			//_nodeOption = GUILayout.Toggle(_nodeOption, "Node Toggle", EditorStyles.toolbarButton);
			//GUILayout.FlexibleSpace();
			//GUILayout.EndHorizontal();
			//GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(1, _cornerY - 16, _cornerX - 3, 14));
			GUILayout.BeginHorizontal(EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();

			//grab corner area based on content reference
			_handleArea = GUILayoutUtility.GetRect(_icon, GUIStyle.none);
			GUI.DrawTexture(new Rect(_handleArea.xMin + 6, _handleArea.yMin - 3, 20, 20), _resizeHandle); //hacky placement
			_action = (Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag);
			if (!_handleActive && _action)
			{
				if (_handleArea.Contains(Event.current.mousePosition, true))
				{
					_handleActive = true; //active when cursor is in contact area
					GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive); //set handle hot
				}
			}

			EditorGUIUtility.AddCursorRect(_handleArea, MouseCursor.ResizeUpLeft);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			//resize window
			if (_handleActive && (Event.current.type == EventType.MouseDrag))
			{
				ResizeNode(id, Event.current.delta.x, Event.current.delta.y);
				Repaint();
				Event.current.Use();
			}

			//enable drag for node
			if (!_handleActive && Event.current.type == EventType.mouseDown)
			{
				Selection.activeGameObject = _nodeWindows[id].modulator.gameObject;
				
			}
			GUI.DragWindow();
		}

		private void ResizeNode(int id, float deltaX, float deltaY)
		{
			Rect r = _nodeWindows[id].rect;
			if ((r.width + deltaX) > _winMinX) { r.xMax += deltaX; }
			if ((r.height + deltaY) > _winMinY) {r.yMax += deltaY; }
			_nodeWindows[id].rect = r;
		}

		void DrawNodeCurve(Rect start, Rect end)
		{
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0) + (Vector3)_scrollPos;
			Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0) + (Vector3)_scrollPos;
			Vector3 startTan = startPos + Vector3.right * 50;
			Vector3 endTan = endPos + Vector3.left * 50;
			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, _aaLine, 1.5f);
		}
	}

}
