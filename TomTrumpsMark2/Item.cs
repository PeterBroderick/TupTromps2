using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TomTrumpsMark2
{
    class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }

       

        [JsonProperty(PropertyName = "movesT")]
        public int Moves { get; set; }
    }


}
