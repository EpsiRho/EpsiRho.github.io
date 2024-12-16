using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.json
{
    public class ProjectPost
    {
        public string Name { get; set; }
        public DateTime AnnounceDate { get; set; }
        public string ImageURL { get; set; }
        public string PostURL { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public List<ProjectLink> Links { get; set; }
    }
    public class ProjectLink
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
