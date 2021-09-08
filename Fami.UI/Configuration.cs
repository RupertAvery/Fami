using System.Collections.Generic;

namespace Fami.UI
{
    public class Configuration
    {
        public Configuration()
        {
            RecentItems = new List<string>();
        }

        public List<string> RecentItems { get; set; }
    }
}