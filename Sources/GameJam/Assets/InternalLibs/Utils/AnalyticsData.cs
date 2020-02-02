using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Gamejam.Utils
{
    public class CustomEvent {
        public string eventName;            // event name
        public Dictionary<string,object>  eventParams;  // event param
        public CustomEvent(string eventName) {
            this.eventName = eventName;
            this.eventParams = new Dictionary<string, object>();
        }
        public CustomEvent(string eventName,string key, string value)
        {
            this.eventName = eventName;
            this.eventParams = new Dictionary<string, object>();
            this.eventParams[key] = value;
        }
        public CustomEvent(string eventName, Dictionary<string, object> eventParams)
        {
            this.eventName = eventName;
            this.eventParams = eventParams;
            if (this.eventParams == null) {
                this.eventParams = new Dictionary<string, object>();
            }
        }
    }

    public class CustomEventInt
    {
        public string eventName;            // event name
        public Dictionary<string, int> eventParams;  // event param
        public CustomEventInt(string eventName)
        {
            this.eventName = eventName;
            this.eventParams = new Dictionary<string, int>();
        }
        public CustomEventInt(string eventName, string key, int value)
        {
            this.eventName = eventName;
            this.eventParams = new Dictionary<string, int>();
            this.eventParams[key] = value;
        }
        public CustomEventInt(string eventName, Dictionary<string, int> eventParams)
        {
            this.eventName = eventName;
            this.eventParams = eventParams;
            if (this.eventParams == null)
            {
                this.eventParams = new Dictionary<string, int>();
            }
        }
    }
}