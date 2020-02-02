/// <summary>
/// OneP GM Console version 0.6
/// Strong D
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OneP.GMConsole
{
	public class Layout
    {
        public class Scroll : IDisposable
        {
            bool _disposed = false;

            private Scroll()
            {

            }

            public static Scroll ScrollView(ref Vector2 scrollPosition)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                return scrl;
            }

			public static Scroll ScrollView(ref Vector2 scrollPosition, GUIStyle style)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, style);
                return scrl;
            }

			public static Scroll ScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
                return scrl;
            }

			public static Scroll ScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
                return scrl;
            }

			public static Scroll ScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, options);
                return scrl;
            }

			public static Scroll ScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
            {
                Scroll scrl = new Scroll();
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
                return scrl;
            }

            public void Dispose()
            {
                if (_disposed) return;
                GUILayout.EndScrollView();
                _disposed = true;
            }
        }

        public class SetColor : IDisposable
        {
            bool _disposed = false;
            private readonly Color _color;
            public SetColor(Color color)
            {
                _color = GUI.color;
                GUI.color = color;
            }

            public void Dispose()
            {
                if (_disposed) return;
                GUI.color = _color;
                _disposed = true;
            }
        }

        public class GUIEnabled : IDisposable
        {
            bool _disposed = false;
            private readonly bool _enabled;
            public GUIEnabled(bool enabled)
            {
                _enabled = GUI.enabled;
                GUI.enabled = enabled;
            }

            public void Dispose()
            {
                if (_disposed) return;
                GUI.enabled = _enabled;
                _disposed = true;
            }
        }

        public class Horizontal : IDisposable
        {
            bool _disposed = false;
            public Horizontal()
            {
                GUILayout.BeginHorizontal();
            }

			public Horizontal(params GUILayoutOption[] options)
			{
				GUILayout.BeginHorizontal(options);
			}

			public Horizontal(GUIStyle style, params GUILayoutOption[] options)
			{
				GUILayout.BeginHorizontal(style, options);
			}

            public void Dispose()
            {
                if (_disposed) return;
                GUILayout.EndHorizontal();
                _disposed = true;
            }
        }

        public class AlignToCenter : IDisposable
        {
            bool _disposed = false;
            public AlignToCenter()
            {
                GUILayout.FlexibleSpace();
            }

            public void Dispose()
            {
                if (_disposed) return;
                GUILayout.FlexibleSpace();
                _disposed = true;
            }
        }

        public class Vertical : IDisposable
        {
            bool _disposed = false;
            public Vertical()
            {
                GUILayout.BeginVertical();
            }

			public Vertical(params GUILayoutOption[] options)
			{
				GUILayout.BeginHorizontal(options);
			}

			public Vertical(GUIStyle style, params GUILayoutOption[] options)
			{
				GUILayout.BeginVertical(style, options);
			}

            public void Dispose()
            {
                if (_disposed) return;
                GUILayout.EndVertical();
                _disposed = true;
            }
        }

        public class Area : IDisposable
        {
            bool _disposed = false;
            public Area(Rect rect)
            {
                GUILayout.BeginArea(rect);
            }

            public void Dispose()
            {
                if (_disposed) return;
                GUILayout.EndArea();
                _disposed = true;
            }
        }

#if UNITY_EDITOR
		/// <summary>
		/// Display a session header
		/// </summary>
		public static void EditorSessionHeader(string name)
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
				GUILayout.Box("", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			}
			GUILayout.EndHorizontal();
		}

		/// <summary>
		/// Display a toogle session header
		/// </summary>
		public static bool EditorSessionHeaderToogle(string name, bool toogle)
		{
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					toogle = !toogle;
				GUILayout.Box("", EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			}
			GUILayout.EndHorizontal();

			return toogle;
		}
#endif
    }
}
