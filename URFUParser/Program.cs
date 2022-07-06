using System.Windows.Forms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace URFUParser
{
    internal class Program
    {
       
        static async Task<int> Main(string[] args)
        {
			WEBHandler wh = new WEBHandler();

			List<int>[,,] arr = new List<int>[100, 100, 100];
			for(int i = 0; i < 100; i++)
				for (int j = 0; j < 100; j++)
					for (int k = 0; k < 100; k++)
					{
						arr[i, j, k] = new List<int>();
					}

			for (int i = 1; i < 34; i++)
			{
				if (i == 28 || i == 30)
					continue;
				string source = await wh.GetHtmlPage($@"https://urfu.ru/api/ratings/info/27/{i}/");
				HTMLHandler hh = new HTMLHandler(source);

				hh.MoveTo("pre");
				string url = hh.GetValue();
				url = url.Substring(url.IndexOf("/"), url.IndexOf("html") + 4 - url.IndexOf("/"));
				url = $@"https://urfu.ru{url}";

				source = await wh.GetHtmlPage(url);
				URFUParser p = new URFUParser(source);

				p.parse();

				if(p.arr.ToArray().Length > 0)
					foreach (ShortenPersonInfo inf in p.arr)
						arr[inf.highN, inf.midN, inf.lowN].Add(inf.totalScore);
				Console.WriteLine($"=>{i} page parsed");
			}
			Console.Clear();
			Console.WriteLine("=>Completed...");
			Thread.Sleep(500);
			while (true)
			{
				Console.Clear();
				int hn = 0, mn = 0, ln = 0;
				Console.Write("Enter this number XX.xx.xx: "); hn = int.Parse(Console.ReadLine());
				Console.Write("Enter this number xx.XX.xx: "); mn = int.Parse(Console.ReadLine());
				Console.Write("Enter this number xx.xx.XX: "); ln = int.Parse(Console.ReadLine());

				List<int> spi = arr[hn, mn, ln];
				if (spi.ToArray().Length != 0)
				{

					spi.Sort(delegate(int a, int b)
					{
						if (a > b) return -1;
						else if (a == b) return 0;
						else return 1;
					});
					int av = 0;
					Console.WriteLine($"=>{spi.ToArray().Length} applicants this moment");
					Console.WriteLine("=>Scores: ");
					Console.ForegroundColor = System.ConsoleColor.Red;
					int counter = 0;
					foreach (short i in spi)
					{
						if(counter == 90)
						{
							Console.ResetColor();
						}

						av += i;
						Console.WriteLine($"=>{i}");
						counter++;
					}
					Console.WriteLine($"=>Average score: {av / spi.ToArray().Length}");
				}
				else
					Console.WriteLine("No one aplied this program");

				Console.WriteLine("Press any key");
				Console.ReadLine();
			}
			return 0;
        }
     
    }
}