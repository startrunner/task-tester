using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.Noi2Evaluator.Infos
{
    [DataContract]
    public class CheckerBindingInfo
    {
        [DataMember(Name ="text")]
        public string SearchText { get; set; }

        [DataMember(Name ="points")]
        public int Points { get; set; }
    }
}
