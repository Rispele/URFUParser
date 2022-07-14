using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

struct PersonInfo
{
	int id;
	int snils;
	string competitionType;
	bool confirmApplication;
	bool originalDocuments;
	string qualification;
	string programm;
	string educationType;
	string moneyQuest;
	int sumScore;

	PersonInfo(int id0, int sn, string ct, bool ca, bool od,
				string q, string p, string et, string mq, int ss)
	{
		id = id0;
		snils = sn;
		competitionType = ct;
		confirmApplication = ca;
		originalDocuments = od;
		qualification = q;
		programm = p;
		educationType = et;
		moneyQuest = mq;
		sumScore = ss;
	}

}
struct ShortenPersonInfo
{
	public short highN;
	public short midN;
	public short lowN;
	public short totalScore;
}

namespace URFUParser
{
	internal class URFUParser
	{
		HTMLHandler hh { get; set; }
		public List<ShortenPersonInfo> arr { get; set; }
		
		public URFUParser(string html)
		{
			hh = new HTMLHandler(html);
			arr = new List<ShortenPersonInfo>();
		}
		public void parse()
		{
			string utf8ToWin1251(string s)
			{
				Encoding utf8 = Encoding.GetEncoding("UTF-8");
				Encoding win1251 = Encoding.GetEncoding("Windows-1251");

				byte[] utf8Bytes = win1251.GetBytes(s);
				byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

				return win1251.GetString(win1251Bytes);
			}
			
			hh.MoveTo(@"table class=""alpha");
			hh.MoveTo("tr");
			try
			{
				int span = 0;
				int span1 = 0;
				while (hh.MoveTo("tr"))
				{
					// Переход на нового чела
					hh.MoveTo("td");
					int tspan = 1;
					if (hh.CheckProperty("rowspan"))
						tspan = int.Parse(hh.GetPropertyValue("rowspan"));

					List<string> tarr = new List<string>();
					tarr.Add(hh.GetValue());

					int border = 11;
					if (span > 0)
						border = 9;
					if (span1-- > 0)
						border -= 1;
					int brcount = 0;

					// Парсинг данных
					for (int i = 0; i < border; i++)
					{
						hh.MoveTo("td");
						if (hh.CheckProperty("rowspan") && i > 4)
							span1 = int.Parse(hh.GetPropertyValue("rowspan")) - 1;
						tarr.Add(hh.GetValue());

						if (tarr[i].Contains("br") && i > 4)
						{
							border += 1;
							brcount += 1;
						}
						if(i == border - 1 && brcount == 0)
						{
							border += 1;
							brcount += 1;
						}
					}

					// Обработка инфы
					ShortenPersonInfo inf = new ShortenPersonInfo();

					string[] sspl = span > 0 ? tarr[4].Split('.') : tarr[6].Split('.');

					short.TryParse(sspl[0], out inf.highN);
					short.TryParse(sspl[1], out inf.midN);
					if(sspl.Length > 2)
						short.TryParse(sspl[2].Substring(0, sspl[2].IndexOf(" ")), out inf.lowN);

					// Поправить кодировку
					string totalScore = utf8ToWin1251(tarr.Last());
					string BVI = utf8ToWin1251(tarr[tarr.ToArray().Length - 3]);
					string moneyQ = utf8ToWin1251(tarr[span > 0 ? 7 : 9]);

					span = span == 0 ? tspan - 1 : span - 1;

					if (moneyQ == "контрактная основа")
					{
						continue;
					}
					else if (totalScore == "&nbsp;" || totalScore == "Выбыл из конкурса" || totalScore == "Забрал документы")
					{
						continue;
					}
					else if (BVI == "Без вступительных испытаний")
					{
						Console.WriteLine("BVI");
						inf.totalScore = 300;
						arr.Add(inf);
						continue;
					}
					else
					{
						inf.totalScore = short.Parse(tarr.Last());
					}

					//Отабрасываем всех у кого не 3 экзамена
					if (brcount < 3)
						continue;
					//Отбросили

					arr.Add(inf);
				}
			}
			catch
			{

			}
			
		}

	}
}
