using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.json
{
    public class MusicItem
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string thumbnailURL { get; set; }
        public string URL { get; set; }
        public DateTime DateAdded { get; set; }
        public float Rating { get; set; }
        public int Energy { get; set; }

    }
}
