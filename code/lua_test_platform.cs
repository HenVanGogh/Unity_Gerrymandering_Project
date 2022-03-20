using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class lua_test_platform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Debug.Log(MoonSharpFactorial());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	double MoonSharpFactorial()
	{
		string script = @"    
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end

		return fact(5)";

		DynValue res = Script.RunString(script);
		
		return res.Number;
	}
}
