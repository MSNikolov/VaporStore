using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.Data.Dtos.Export
{
    public class JsonExportGenre
    {
        public int Id { get; set; }

        public string Genre { get; set; }

        public List<JsonExportGame> Games { get; set; } = new List<JsonExportGame>();

        public int TotalPlayers { get; set; }
    }
}
