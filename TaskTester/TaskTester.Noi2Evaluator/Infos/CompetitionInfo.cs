using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TaskTester.Noi2Evaluator.Infos
{
    [DataContract]
    class CompetitionInfo
    {
        [DataMember(Name ="commands")]
        public List<string> CommandLines { get; set; }

        [DataMember(Name ="problems")]
        public List<ProblemInfo> Problems { get; set; }

        [DataMember(Name ="folderCriteria")]
        public string FolderCriteria { get; set; }
    }
}
