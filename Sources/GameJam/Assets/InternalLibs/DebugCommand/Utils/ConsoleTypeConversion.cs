/// <summary>
/// OneP GM Console version 0.6
/// Strong D
/// </summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OneP.GMConsole
{
    public class ConsoleTypeConversion
    {
        private static Dictionary<System.Type, MethodInfo> methods = new Dictionary<System.Type, MethodInfo>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleTypeConversion"/> class.
		/// </summary>
        static ConsoleTypeConversion()
        {
			foreach (MethodInfo info in typeof(ConsoleTypeConversion).GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (info.Name == "convert")
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    if (parameters.Length == 3)
                    {
                        ParameterInfo info2 = parameters[2];
                        if (info2.ParameterType.IsByRef)
                        {
                            methods[info2.ParameterType.GetElementType()] = info;
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Determines whether this instance [can convert to] the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if this instance [can convert to] the specified type; otherwise, <c>false</c>.
		/// </returns>
        public static bool CanConvertTo(System.Type type)
        {
			if (type.IsArray)
            {
                type = type.GetElementType();
            }
			try
			{
	            if (TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string)))
	            {
	                return true;
	            }
			}
			catch(System.Exception ex) {
				Debug.LogError ("Console CanConvertTo exception:"+type.ToString()+"," + ex.Message);
			}
            MethodInfo info = null;
            return methods.TryGetValue(type, out info);
        }

		/// <summary>
		/// Converts the specified params.
		/// </summary>
		/// <param name="Params">The params.</param>
		/// <param name="ParamIndex">Index of the param.</param>
		/// <param name="boolVal">if set to <c>true</c> [bool val].</param>
		/// <returns></returns>
		private static bool Convert(object[] Params, ref int ParamIndex, out bool boolVal)
        {
            if ((ParamIndex < Params.Length) && (Params[ParamIndex] is string))
            {
                if (((string.Compare("1", (string) Params[ParamIndex], true) == 0) || (string.Compare("yes", (string) Params[ParamIndex], true) == 0)) || ((string.Compare("on", (string) Params[ParamIndex], true) == 0) || (string.Compare("true", (string) Params[ParamIndex], true) == 0)))
                {
                    boolVal = true;
                    ParamIndex++;
                    return true;
                }
                if (((string.Compare("0", (string) Params[ParamIndex], true) == 0) || (string.Compare("no", (string) Params[ParamIndex], true) == 0)) || ((string.Compare("off", (string) Params[ParamIndex], true) == 0) || (string.Compare("false", (string) Params[ParamIndex], true) == 0)))
                {
                    boolVal = false;
                    ParamIndex++;
                    return true;
                }
            }
            boolVal = false;
            return false;
        }

		/// <summary>
		/// Converts the specified params.
		/// </summary>
		/// <param name="Params">The params.</param>
		/// <param name="ParamIndex">Index of the param.</param>
		/// <param name="pt">The pt.</param>
		/// <returns></returns>
		private static bool Convert(object[] Params, ref int ParamIndex, out Vector2 pt)
        {
            object[] destinationArray = null;
            int num = 0;
            if (((Params[ParamIndex] is object[]) && (((object[]) Params[ParamIndex]).Length == 2)) && ((((object[]) Params[ParamIndex])[0] is string) && (((object[]) Params[ParamIndex])[1] is string)))
            {
                destinationArray = (object[]) Params[ParamIndex];
                num = 1;
            }
            else if (((ParamIndex < (Params.Length - 1)) && (Params[ParamIndex] is string)) && (Params[ParamIndex + 1] is string))
            {
                destinationArray = new object[2];
                Array.Copy(Params, ParamIndex, destinationArray, 0, 2);
                num = 2;
            }
            if (destinationArray != null)
            {
                float x = float.Parse((string) destinationArray[0]);
                float y = float.Parse((string) destinationArray[1]);
                pt = new Vector2(x, y);
                ParamIndex += num;
                return true;
            }
            pt = Vector2.zero;
            return false;
        }

		/// <summary>
		/// Converts to type.
		/// </summary>
		/// <param name="Params">The params.</param>
		/// <param name="ParamIndex">Index of the param.</param>
		/// <param name="toType">To type.</param>
		/// <param name="ConvertedParam">The converted param.</param>
		/// <returns></returns>
		public static bool ConvertToType(object[] Params, ref int ParamIndex, System.Type toType, out object ConvertedParam)
        {
            ConvertedParam = null;
            if (ParamIndex < Params.Length)
            {
                if (Params[ParamIndex] is string)
                {
                    bool flag2 = false;
                    try
                    {
                        ConvertedParam = TypeDescriptor.GetConverter(toType).ConvertFromString((string) Params[ParamIndex]);
                        ParamIndex++;
                        flag2 = true;
                    }
                    catch (Exception)
                    {
                    }
                    return flag2;
                }
                if (toType.IsArray && (Params[ParamIndex] is object[]))
                {
                    object[] @params = (object[]) Params[ParamIndex];
                    object[] objArray2 = (object[]) Array.CreateInstance(toType.GetElementType(), @params.Length);
                    int paramIndex = 0;
                    while (paramIndex < @params.Length)
                    {
                        int index = paramIndex;
                        object convertedParam = null;
                        if (!ConvertToType(@params, ref paramIndex, toType.GetElementType(), out convertedParam))
                        {
                            break;
                        }
                        objArray2[index] = convertedParam;
                    }
                    if (paramIndex == @params.Length)
                    {
                        ConvertedParam = objArray2;
                        ParamIndex++;
                        return true;
                    }
                }
                try
                {
                    MethodInfo info = null;
                    if (methods.TryGetValue(toType, out info))
                    {
                        ConvertedParam = null;
                        object[] parameters = new object[] { Params, (int) ParamIndex, ConvertedParam };
                        if ((bool) info.Invoke(null, parameters))
                        {
                            ParamIndex = (int) parameters[1];
                            ConvertedParam = parameters[2];
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return false;
        }
    }
}

