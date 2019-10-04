namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Dtos.Export;
    using VaporStore.Data.Models;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var genres = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new JsonExportGenre
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(ga => ga.Purchases.Any())
                    .Select(ga => new JsonExportGame
                    {
                        Id = ga.Id,
                        Title = ga.Name,
                        Developer = ga.Developer.Name,
                        Tags = string.Join(", ", ga.GameTags.Select(t => t.Tag.Name)),
                        Players = ga.Purchases.Count()
                    })
                    .OrderByDescending(ga => ga.Players)
                    .ThenBy(ga => ga.Id)
                    .ToList(),
                    TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count())
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(genres, Newtonsoft.Json.Formatting.Indented);

            return json;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
            var users = context.Users
                .Where(u => u.Cards.Any(c => c.Purchases.Any(p => Enum.GetName(typeof(PurchaseType), p.Type) == storeType)))
                .Select(u => new XmlExportUser
                {
                    Username = u.Username,
                    Purchases = u.Cards.SelectMany(c => c.Purchases)
                    .Where(p => p.Type.ToString() == storeType)
                    .Select(p => new XmlExportPurchase
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new XmlExportGame
                        {
                            Title = p.Game.Name,
                            Genre = p.Game.Genre.Name,
                            Price = p.Game.Price
                        }
                    })
                    .OrderBy(p => p.Date)
                    .ToList(),
                    TotalSpent = u.Cards.SelectMany(c => c.Purchases).Where(p => Enum.GetName(typeof(PurchaseType), p.Type) == storeType).Sum(p => p.Game.Price)
                })
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToList();

            var ser = new XmlSerializer(typeof(List<XmlExportUser>), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            ser.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
		}
	}
}