using System.Collections.Generic;

namespace ContentSandbox.Models
{
    class Project
    {
        public string Name { get; set; }
        public List<Area> Areas { get; set; }
    }

    class Area
    {
        public string Name { get; set; }
        public List<Zone> Zones { get; set; }
    }

    class Zone
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Layer DefaultLayer { get; set; }
    }
}
