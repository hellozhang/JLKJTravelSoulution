using Castle.Components.DictionaryAdapter;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestClass();
            test.TestProperty = "测试属性";//这里会调用属性的setter方法

            Console.Read();

            DictionaryAdapterFactory factory = new DictionaryAdapterFactory();
            var dictionary = new Dictionary<string, object>
            {
                { "FirstName", "Giants" },
                { "LastName", "ZZZ" }
            };

            var player = factory.GetAdapter<IPlayer>(dictionary);
           



            ILogger Logger = NullLogger.Instance;
            XmlConfigurator.ConfigureAndWatch(GetConfigFile("log4net.config"));
            string logMessage = "testingiiiiam for iiconfiguration";
            var log = LogManager.GetLogger(typeof(Program));
            Logger.Debug(logMessage);
            Console.ReadLine();


            // 加载xml
            XmlDocument xml = new XmlDocument();
            xml.Load("cfg.xml");
            // 获取操作xml的代理对象
            Config config = factory.GetAdapter<Config>(xml);
            Console.WriteLine("UserName : " + config.UserName);
            Console.WriteLine("IPAddress : " + config.IPAddress);
            Console.WriteLine("Numbers : " + string.Join(", ", config.Numbers));
            Console.WriteLine("Sum : " + config.Sum);
            // 修改数据
            config.UserName = "张三";
            config.Numbers.Add(666);
            // 保存数据
            xml.Save("output.xml");

            var foo = factory.GetAdapter<IRoot>(Xml("<Root> <X> <A X='1'/> <B X='2'/> </X> </Root>"));

            var array = foo.Items;


            Console.WriteLine(((IDerived1)array[0]).X);


            var minAge = 30;
            var name = "Soccer Adult Winter II 2010";
            var division = Division.Coed;
            var startsOn = new DateTime(2010, 2, 21);
            var endsOn = new DateTime(2010, 4, 18);
            var line1 = "2922 South Highway 205";
            var city = "Rockwall";
            var state = "TX";
            var zipCode = "75032";
            var team1Name = "Fairly Oddparents";
            var team1Balance = 450.00M;
            var team1Player1FirstName = "Mike";
            var team1Player1LastName = "Etheridge";
            var team1Player2FirstName = "Susan";
            var team1Player2LastName = "Houston";
            var team2Name = "Team Punishment";
            var team2Balance = 655.50M;
            var team2Player1FirstName = "Stephen";
            var team2Player1LastName = "Gray";
            var licenseNo = "1234";
            var tags = new[] { "Soccer", "Skills", "Fun" };

            var xml2 = string.Format(
                @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>{0}</Name>
					 <MinimumAge>{1}</MinimumAge>
					 <Division>{2}</Division>
					 <StartsOn>{3}</StartsOn>
					 <EndsOn>{4}</EndsOn>
					 <Address xmlns='Common'>
						<Line1>{5}</Line1>
						<City>{6}</City>
						<State>{7}</State>
						<ZipCode>{8}</ZipCode>
					 </Address>
					 <League>
						<Team Name='{9}'>
						   <AmountDue>{10}</AmountDue>
						   <Roster>
							  <Participant FirstName='{11}' lastName='{12}'>
							  </Participant>
							  <Participant FirstName='{13}' lastName='{14}'>
							  </Participant>
						   </Roster>
						</Team>
						<Team Name='{15}'>
						   <AmountDue>{16}</AmountDue>
						   <Roster>
							  <Participant FirstName='{17}' lastName='{18}'>
							  </Participant>
						   </Roster>
						</Team>
					 </League>
					 <Documentation xmlns=''>
					  <Notes>notes</Notes>
					 </Documentation>
					 <Tag>{19}</Tag>
					 <Tag>{20}</Tag>
					 <Tag>{21}</Tag>
					 <ExtraStuff>
						<LicenseNo>{22}</LicenseNo>
					 </ExtraStuff>
				  </Season>",
                name, minAge, division,
                XmlConvert.ToString(startsOn, XmlDateTimeSerializationMode.Local),
                XmlConvert.ToString(endsOn, XmlDateTimeSerializationMode.Local),
                line1, city, state, zipCode,
                team1Name, XmlConvert.ToString(team1Balance),
                team1Player1FirstName, team1Player1LastName,
                team1Player2FirstName, team1Player2LastName,
                team2Name, XmlConvert.ToString(team2Balance),
                team2Player1FirstName, team2Player1LastName,
                tags[0], tags[1], tags[2],
                licenseNo);
            var season1 = factory.GetAdapter<IRoot>(Xml(xml2));
            ////Assert.AreEqual(name, season.Name);
            ////Assert.AreEqual(minAge, season.MinimumAge);
            ////Assert.AreEqual(division, season.Division);
            ////Assert.AreEqual(startsOn.Date, season.StartsOn.Date);
            ////Assert.AreEqual(endsOn.Date, season.EndsOn.Date);
            ////Assert.AreEqual(line1, season.Location.Line1);
            ////Assert.AreEqual(city, season.Location.City);
            ////Assert.AreEqual(state, season.Location.State);
            ////Assert.AreEqual(zipCode, season.Location.ZipCode);
            ////Assert.AreEqual(2, season.Teams.Count);

            ////var team = season.Teams[0];
            ////var n = team.Name;

            ////Assert.AreEqual(team1Name, season.Teams[0].Name);
            ////Assert.AreEqual(team1Balance, season.Teams[0].Balance);
            ////Assert.IsNull(season.Teams[0].GamesPlayed);
            ////Assert.AreEqual(2, season.Teams[0].Players.Count);
            ////Assert.AreEqual(team1Player1FirstName, season.Teams[0].Players[0].FirstName);
            ////Assert.AreEqual(team1Player1LastName, season.Teams[0].Players[0].LastName);
            ////Assert.AreEqual(team1Player2FirstName, season.Teams[0].Players[1].FirstName);
            ////Assert.AreEqual(team1Player2LastName, season.Teams[0].Players[1].LastName);
            ////Assert.AreEqual(team2Name, season.Teams[1].Name);
            ////Assert.AreEqual(team2Balance, season.Teams[1].Balance);
            ////Assert.AreEqual(1, season.Teams[1].Players.Count);
            ////Assert.AreEqual(team2Player1FirstName, season.Teams[1].Players[0].FirstName);
            ////Assert.AreEqual(team2Player1LastName, season.Teams[1].Players[0].LastName);
            ////Assert.AreEqual(2, season.TeamsArray.Length);
            ////Assert.AreEqual(team1Name, season.TeamsArray[0].Name);
            ////Assert.AreEqual(team1Balance, season.TeamsArray[0].Balance);
            ////Assert.AreEqual(team2Name, season.TeamsArray[1].Name);
            ////Assert.AreEqual(team2Balance, season.TeamsArray[1].Balance);
            ////Assert.AreEqual(team1Balance + team2Balance, season.Balance);
            ////Assert.AreEqual(team1Name, season.FirstTeamName);
            ////Assert.AreEqual("notes", season.Notes); // TODO: Improve
            ////Assert.AreEqual(3, season.Tags.Length);
            ////Assert.Contains(tags[0], season.Tags);
            ////Assert.Contains(tags[1], season.Tags);
            ////Assert.Contains(tags[2], season.Tags);
            ////Assert.IsNotNull(season.ExtraStuff);
            ////Assert.AreEqual(licenseNo, season.ExtraStuff["LicenseNo", "RISE"].InnerText);












            Console.ReadLine();

           

        }
        protected static XmlDocument Xml(params string[] xml)
        {
            var document = new XmlDocument();
            var text = string.Concat(xml)
                .Replace("$xsd", "xmlns:xsd='http://www.w3.org/2001/XMLSchema'")
                .Replace("$xsi", "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'")
                .Replace("$x", "xmlns:x='urn:schemas-castle-org:xml-reference'");
            document.LoadXml(text);
            return document;
        }
        private static Stream StringToStream(string s)
        {
            return new MemoryStream(Encoding.Default.GetBytes(s));
        }
        protected static FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;

            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {
#if FEATURE_APPDOMAIN
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#else
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
                result = new FileInfo(Path.Combine(baseDirectory, fileName));
            }

            return result;
        }
    }
}
