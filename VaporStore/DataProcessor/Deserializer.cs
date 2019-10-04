namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Dtos.Import;
    using VaporStore.Data.Models;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public static class Deserializer
	{
        private const string ErrorMessage = "Invalid Data";
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            var gamesDto = JsonConvert.DeserializeObject<List<JsonImportGame>>(jsonString);

            var games = new List<Game>();

            var devs = new List<Developer>();

            var genres = new List<Genre>();

            var tags = new List<Tag>();

            var res = new StringBuilder();

            foreach (var gameDto in gamesDto)
            {
                var validGame = IsValid(gameDto);

                var validTags = gameDto.TagNames.Count > 0;

                if (!validGame || !validTags)
                {
                    res.AppendLine(ErrorMessage);

                    continue;
                }

                var game = Mapper.Map<Game>(gameDto);

                if (devs.Any(d => d.Name == gameDto.DeveloperName))
                {
                    var dev = devs.First(d => d.Name == gameDto.DeveloperName);

                    game.Developer = dev;
                }

                else
                {
                    var dev = new Developer
                    {
                        Name = gameDto.DeveloperName
                    };

                    game.Developer = dev;

                    devs.Add(dev);
                }

                if (genres.Any(g => g.Name == gameDto.GenreName))
                {
                    game.Genre = genres.First(g => g.Name == gameDto.GenreName);
                }

                else
                {
                    var genre = new Genre
                    {
                        Name = gameDto.GenreName
                    };

                    game.Genre = genre;

                    genres.Add(genre);
                }

                foreach (var tagDto in gameDto.TagNames)
                {
                    if (tags.Any(t => t.Name == tagDto))
                    {
                        var tag = tags.First(t => t.Name == tagDto);

                        game.GameTags.Add(new GameTag
                        {
                            Game = game,
                            Tag = tag
                        });
                    }

                    else
                    {
                        var tag = new Tag
                        {
                            Name = tagDto
                        };

                        game.GameTags.Add(new GameTag
                        {
                            Game = game,
                            Tag = tag
                        });

                        tags.Add(tag);
                    }
                }

                games.Add(game);

                res.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(games);

            context.SaveChanges();

            return res.ToString().Trim();
    }

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
            var usersDto = JsonConvert.DeserializeObject<List<JsonImportUser>>(jsonString);

            var users = new List<User>();

            var res = new StringBuilder();

            foreach (var userDto in usersDto)
            {
                var validUser = IsValid(userDto);

                var validCard = true;

                var validCardType = true;

                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        validCard = false;
                    }

                    var validType = Enum.TryParse(typeof(CardType), cardDto.Type, out object cardType);

                    if (!validType)
                    {
                        validCardType = false;
                    }
                }

                if (!validUser || !validCard || !validCardType)
                {
                    res.AppendLine(ErrorMessage);

                    continue;
                }

                var user = Mapper.Map<User>(userDto);

                users.Add(user);

                res.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(users);

            context.SaveChanges();

            return res.ToString().Trim();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
            var ser = new XmlSerializer(typeof(List<XmlImportPurchase>), new XmlRootAttribute("Purchases"));

            var purchasesDto = (List<XmlImportPurchase>)ser.Deserialize(new StringReader(xmlString));

            var res = new StringBuilder();

            var purchases = new List<Purchase>();

            foreach (var purchaseDto in purchasesDto)
            {
                var validPurch = IsValid(purchaseDto);

                var validCard = context.Cards.Any(c => c.Number == purchaseDto.CreditCardNumber);

                var validGame = context.Games.Any(g => g.Name == purchaseDto.Title);

                if (!validPurch || !validCard || !validGame)
                {
                    res.AppendLine(ErrorMessage);
                }

                var purchase = new Purchase
                {
                    Card = context.Cards.First(c => c.Number == purchaseDto.CreditCardNumber),

                    Game = context.Games.First(g => g.Name == purchaseDto.Title),

                    Type = Enum.Parse<PurchaseType>(purchaseDto.Type),

                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),

                    ProductKey = purchaseDto.ProductKey
                };

                purchases.Add(purchase);

                res.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }
            context.Purchases.AddRange(purchases);

            context.SaveChanges();

            return res.ToString().Trim();
		}

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}