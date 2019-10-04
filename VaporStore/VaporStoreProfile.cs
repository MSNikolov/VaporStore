namespace VaporStore
{
	using AutoMapper;
    using VaporStore.Data.Dtos.Export;
    using VaporStore.Data.Dtos.Import;
    using VaporStore.Data.Models;

    public class VaporStoreProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public VaporStoreProfile()
		{
            this.CreateMap<JsonImportGame, Game>();

            this.CreateMap<JsonImportUser, User>();

            this.CreateMap<JsonImportCard, Card>();

            this.CreateMap<Game, JsonExportGame>();

            this.CreateMap<Genre, JsonExportGenre>();

            this.CreateMap<Game, XmlExportGame>();

            this.CreateMap<Purchase, XmlExportPurchase>();

            this.CreateMap<User, XmlExportUser>();
		}
	}
}