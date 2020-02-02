/// <summary>
/// OneP GM Console version 0.6
/// Create by Strong D
/// </summary>
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OnePStudio.Console
{
    public static class Console
    {
        private const int HistoryCapacity = 128;
        public class DebugObject
        {
            public DateTime lastTime;

            public Color color = Color.white;

            public List<DebugObject> history = new List<DebugObject>();
            public Vector2 scroll;

            public object Value
            {
                get { return _value; }
                set
                {
                    lastTime = DateTime.Now; _value = value;
                    history.Insert(0, new DebugObject(value.ToString()));
                    if (history.Count > HistoryCapacity) history.RemoveAt(HistoryCapacity);
                }
            }
            private object _value;
			public bool open = false;

            public DebugObject(object value, Color color)
            {
                lastTime = DateTime.Now;
                history.Add(new DebugObject(value.ToString()));
                _value = value;
                this.color = color;
            }

            public DebugObject(object value)
            {
                lastTime = DateTime.Now;
                _value = value;
            }

            public Dictionary<string, DebugObject> GetMessageHolder()
            {
                return Value as Dictionary<string, DebugObject>;
            }

            public void RemoveEntry(string entryName)
            {
                Dictionary<string, DebugObject> messages = _value as Dictionary<string, DebugObject>;
                if (messages == null || !messages.ContainsKey(entryName)) return;
                messages.Remove(entryName);
                _value = messages;
            }

            public GameObject owner;
            public bool noMonobeh = false;
        }

        private const uint Capacity = 128;
        public static Dictionary<object, DebugObject> entries = new Dictionary<object, DebugObject>();
		private static int uniqueID = 0;

        public static void Log(object sender, string description, object value, Color color)
        {
            if (value == null) return;
            DebugObject obj;
            if (entries.TryGetValue(sender, out obj))
            {
				if (string.IsNullOrEmpty(description))
					description = string.Format("#{0}", uniqueID++);

                Dictionary<string, DebugObject> member = obj.GetMessageHolder();
                switch (member.ContainsKey(description))
                {
                    case true:
                        member[description].Value = value;
                        break;
                    case false:
                        member.Add(description, new DebugObject(value, color));
                        break;
                }
                obj.color = color;
                obj.lastTime = DateTime.Now;
                return;
            }
            Dictionary<string, DebugObject> d = new Dictionary<string, DebugObject>();
            d.Add(description, new DebugObject(value, color));
            entries.Add(sender, new DebugObject(d, color));
            MonoBehaviour mb = sender as MonoBehaviour;
            if (mb == null)
            {
                switch (sender.GetType().ToString())
                {
                    case "UnityEngine.GameObject":
                        entries[sender].owner = (GameObject)sender;
                        return;
                }
                entries[sender].noMonobeh = true;
                return;
            }
            entries[sender].owner = mb.gameObject;
            if (entries.Count > Capacity)
            {
                object[] keys = new object[entries.Count];
                entries.Keys.CopyTo(keys, 0);
                entries.Remove(keys[keys.Length - 1]);
            }
        }

        public static void Log(object sender, string description, object value)
        {
            Log(sender, description, value, Color.white);
        }

        public static void ClearAll()
        {
            entries.Clear();
        }
    }

    public static class ColorUtil
    {
        
        #region ========================== COLOR ==========================================
        public static Color A(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
			
        public static Color R(this Color color, float red)
        {
            return new Color(red, color.g, color.b, color.a);
        }
			
        public static Color G(this Color color, float green)
        {
            return new Color(color.r, green, color.b, color.a);
        }
			
        public static Color B(this Color color, float blue)
        {
            return new Color(color.r, color.g, blue, color.a);
        }
        #endregion
    }
}