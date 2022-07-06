using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace URFUParser
{
	internal class HTMLHandler
	{
		string htmlp = "";
		int pos = 0;
		string curh = "";

		public HTMLHandler(string htmlp)
		{
			this.htmlp = htmlp;
		}
		
		public bool MoveTo(string h)
		{
			int t = htmlp.IndexOf($"<{h}", pos);
			if(t == -1)
				return false;
			pos = t + 1;
			curh = h;
			return true;
		}
		public string GetValue()
		{
			int t = htmlp.IndexOf(">", pos);
			if(t == -1)
				return $"No value";

			int bpos = t + 1,
				epos = htmlp.IndexOf($"</{curh}", bpos);

			return htmlp.Substring(bpos, epos - bpos);
		}
		public bool CheckProperty(string h)
		{
			int p = htmlp.IndexOf(">", pos);
			int ph = htmlp.IndexOf(h, pos);
			if (ph >= p || ph <= pos)
				return false;
			return true;
		}
		public string GetPropertyValue(string h)
		{
			int pb = htmlp.IndexOf(h, pos);
			pb = htmlp.IndexOf("\"", pb) + 1;
			int pe = htmlp.IndexOf("\"", pb);
			return htmlp.Substring(pb, pe - pb);
		}
	}

	
}
