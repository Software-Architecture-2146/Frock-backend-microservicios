using Frock_backend.transport_Company.Domain.Model.Commands;

namespace Frock_backend.transport_Company.Domain.Model.Aggregates
{
    public class Companies
    {
        public int Id { get; protected set; }
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public int FkIdUser { get; set; }

        protected Companies()
        {
            Name = string.Empty;
            LogoUrl = string.Empty;
            FkIdUser = 0;
        }

        public Companies(CreateCompanyCommand command)
        {
            Name = command.Name;
            LogoUrl = command.LogoUrl;
            FkIdUser = command.FkIdUser;
        }

        public Companies(UpdateCompanyCommand command)
        {
            Id = command.Id;
            Name = command.Name;
            LogoUrl = command.LogoUrl;
            FkIdUser = command.FkIdUser;
        }

        public Companies(DeleteCompanyCommand command)
        {
            Id = command.Id;
            Name = "";
            LogoUrl = "";
            FkIdUser = 0;
        }
    }
}
