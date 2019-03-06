using Castle.Components.DictionaryAdapter;
using Castle.Components.DictionaryAdapter.Xml;
using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApp
{
    public interface Config
    {
        string UserName { get; set; }

        [XPath("ServerIPAddress")]
        string IPAddress { get; set; }

        IList<int> Numbers { get; }// 数组、BindingList都支持

        [XPath("sum(Numbers)")]// xpath函数
        int Sum { get; }
    }


    public interface IRoot
    {
        [XmlArray("X")]
        [XmlArrayItem("A", typeof(IDerived1))]
        [XmlArrayItem("B", typeof(IDerived2))]
        IBase[] Items { get; set; }
    }

    public interface IBase { }
    public interface IDerived1 : IBase { int X { get; set; } }
    public interface IDerived2 : IBase { string X { get; set; } }
    [XmlRoot("Season", Namespace = "RISE"),
 XmlNamespace("RISE", "rise", Default = true),
 XmlNamespace("Common", "c", Root = true)]
    public interface ISeason : IDictionaryAdapter
    {
        [RemoveIf("")]
        string Name { get; set; }
        int MinimumAge { get; set; }
        Division Division { get; set; }
        DateTime StartsOn { get; set; }
        DateTime EndsOn { get; set; }
        [XPath("sum(rise:League/rise:Team/rise:AmountDue)")]
        decimal Balance { get; }
        [XmlElement("Address", Namespace = "Common")]
        IAddress Location { get; set; }
        [Key("League"), XmlArrayItem("Team")]
        IBindingList<ITeam> Teams { get; set; }
        [XPath("rise:League/rise:Team")]
        ITeam[] TeamsArray { get; }
        [XPath("rise:League/rise:Team[position()=1]/@Name")]
        string FirstTeamName { get; }
        [XPath("Documentation/Notes")]
        string Notes { get; set; }
        [XmlElement("Tag")]
        string[] Tags { get; set; }
        XmlElement ExtraStuff { get; set; }
    }
    public enum Division
    {
        Male,
        Female,
        Coed
    }

    [XmlNamespace("Common", "c", Root = true),
     XmlNamespace("RISE", "r"),
     XPath("r:Season/c:Address")]
    public interface IAddress
    {
        [Volatile]
        string Line1 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string ZipCode { get; set; }
    }

    [XmlType("Player", Namespace = "People")]
    public interface IPlayer : IDictionaryAdapter, IEditableObject, IDictionaryNotify
#if FEATURE_IDATAERRORINFO
		, IDataErrorInfo
#endif
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }

    [XmlType("Goalie", Namespace = "People")]
    public interface IGoalie : IPlayer
    {
        int GoalAllowed { get; set; }
    }

    public interface ITeam
    {
        [XmlAttribute]
        string Name { get; set; }
        [XmlAttribute]
        int? GamesPlayed { get; set; }
        int? MaxPlayers { get; set; }
        [XmlElement("AmountDue")]
        decimal Balance { get; set; }
        [XmlArray("Roster", IsNullable = true), XmlArrayItem("Participant"), RemoveIfEmpty]
        IBindingList<IPlayer> Players { get; }
    }
    public class TestClass
    {
        public string TestProperty
        {
            get;
            [MyMethodAspect]
            set;//在一个属性的setter上使用方法拦截切面
        }
    }

    [Serializable]
    public class MyMethodAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            Console.WriteLine("这条语句来自自定义方法拦截切面");
            args.Proceed();
        }
    }

}
