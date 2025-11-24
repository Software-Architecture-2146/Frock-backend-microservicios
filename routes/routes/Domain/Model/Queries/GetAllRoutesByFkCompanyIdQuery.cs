namespace Frock_backend.routes.Domain.Model.Queries
{
    public record GetAllRoutesByFkCompanyIdQuery
    (
        Guid FkCompanyId
    );
}
