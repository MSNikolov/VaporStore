using System.Collections.Generic;

namespace VaporStore.Data.Dtos.Export
{
    public class JsonExportGame
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Developer { get; set; }

        public string Tags { get; set; }

        public int Players { get; set; }
    }
}