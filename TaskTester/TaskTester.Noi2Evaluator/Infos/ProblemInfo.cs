using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TaskTester.Noi2Evaluator.Infos
{
    [DataContract]
    public class ProblemInfo
    {
        [DataMember(Name="name")]
        public string Name { get; set; }


        [DataMember(Name="timeLimit")]
        public double TimeLimit { get; set; }

        [DataMember(Name ="checkerBindings")]
        public List<CheckerBindingInfo> CheckerBindings { get; set; }

        [DataMember(Name ="testGroups")]
        public int[] TestGroups { get; set; }
    }
}