using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.json
{
    public class BlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostDate { get; set; }
        public string HeaderImage { get; set; }
        public string BlogPostURL { get; set; }
        public List<string> Tags { get; set; }
    }
}
