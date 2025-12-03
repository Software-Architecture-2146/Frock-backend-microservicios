using Frock_backend.routes.Domain.Model.Commands;
using Frock_backend.routes.Domain.Model.Queries;
using Frock_backend.routes.Domain.Service;
using Frock_backend.routes.Interface.REST.Resources;
using Frock_backend.routes.Interface.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;

namespace Frock_backend.routes.Interface.REST
{
    /// <summary>
    /// Routes controller.
    /// </summary>
    /// <param name="routeCommandService">The Route Command Service</param>
    /// <param name="">The Route Query Service</param>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Routes")]
    public class RoutesController(IRouteCommandService routeCommandService, IRouteQueryService routeQueryService) : ControllerBase
    {
        /// <summary>
        /// Creates a new route.
        /// </summary>
        /// <param name="resource">The CreateStopResource resource</param>
        /// <returns>
        /// A response as an action result containing the created stop, or bad request if the stop was not created.
        /// </returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new route.",
            Description = "Creates a new route with a given parameters",
            OperationId = "Create route"
            )]
        [SwaggerResponse(201, "The route was created", typeof(RouteAggregateResource))] // Ajusté el tipo de retorno
        [SwaggerResponse(400, "The route was not created")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateFullRouteResource resource)
        {
            
            if (resource == null) return BadRequest("Resource cannot be null.");

            // --- 1. SEGURIDAD: EXTRAER ID DEL TOKEN ---
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);
  
            var command = CreateFullRouteCommandFromResourceAssembler.toCommandFromResource(resource, userId);
            var result = await routeCommandService.Handle(command);            
            if (result is null) return BadRequest("Could not create route. Check Company ownership or Stop existence.");
            
            // Retornamos el recurso creado
            var routeResource = RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity(result);
            return CreatedAtAction(nameof(GetRouteById), new { id = result.Id }, routeResource);
            
        }

        //Get all routes
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all routes",
            Description = "Get all routes in the system",
            OperationId = "GetAllRoutes"
            )]
        [SwaggerResponse(200, "The routes were retrieved", typeof(IEnumerable<RouteAggregateResource>))]
        [SwaggerResponse(404, "No routes found")]
        public async Task<ActionResult<IEnumerable<RouteAggregateResource>>> GetAllRoutes()
        {
            GetAllRoutesQuery query = new GetAllRoutesQuery();
            var routes = await routeQueryService.Handle(query);
            if (routes == null) return NotFound(); // Cambié a solo check de null para simplificar
            var resources = routes.Select(RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity);
            return Ok(resources);
        }

        /// <summary>
        /// Gets all routes by FkIdCompany.
        /// </summary>
        /// <param name="resource">The GetStopsBy resource</param>
        /// <returns>
        /// A response as an action result containing the Get, or bad request if the stop was not created.
        /// </returns>
        [HttpGet("company/{FkIdCompany}")]
        [SwaggerOperation(
            Summary = "Get Routes By Company Id",
            Description = "Get routes by Company Id",
            OperationId = "GetRoutesByCompanyId"
            )]
        [SwaggerResponse(200, "The routes were retrieved", typeof(IEnumerable<RouteAggregateResource>))]
        [SwaggerResponse(404, "No routes found")]
        public async Task<ActionResult<IEnumerable<RouteAggregateResource>>> GetAllRoutes(int FkIdCompany)
        {
            GetAllRoutesByFkCompanyIdQuery query = new GetAllRoutesByFkCompanyIdQuery(FkIdCompany);
            var routes = await routeQueryService.Handle(query);
            if (routes == null) return NotFound();
            var resources = routes.Select(RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        /// <summary>
        /// Gets all routes by FkIdDistrict.
        /// 
        [HttpGet("district/{FkIdDistrict}")]
        [SwaggerOperation(
            Summary = "Get Routes By District Id",
            Description = "Get routes by District Id",
            OperationId = "GetRoutesByDistrictId"
            )]
        [SwaggerResponse(200, "The routes were retrieved", typeof(IEnumerable<RouteAggregateResource>))]
        [SwaggerResponse(404, "No routes found")]
        public async Task<ActionResult<IEnumerable<RouteAggregateResource>>> GetAllRoutesByDistrict(int FkIdDistrict)
        {
            GetAllRoutesByFkDistrictIdQuery query = new GetAllRoutesByFkDistrictIdQuery(FkIdDistrict);
            var routes = await routeQueryService.Handle(query);
            if (routes == null) return NotFound();
            var resources = routes.Select(RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        //Get route by id
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get Route By Id",
            Description = "Get route by Id",
            OperationId = "GetRouteById"
            )]
        [SwaggerResponse(200, "The route was retrieved", typeof(RouteAggregateResource))]
        [SwaggerResponse(404, "Route not found")]
        public async Task<ActionResult<RouteAggregateResource>> GetRouteById(int id)
        {
            GetRouteByIdQuery query = new GetRouteByIdQuery(id);
            var route = await routeQueryService.Handle(query);
            if (route == null) return NotFound("Route not found.");
            var resource = RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity(route);
            return Ok(resource);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update Route",
            Description = "Update a route by Id",
            OperationId = "UpdateRoute"
            )]
        [SwaggerResponse(200, "The route was updated", typeof(RouteAggregateResource))]
        [SwaggerResponse(404, "Route not found")]
        public async Task<ActionResult<RouteAggregateResource>> UpdateRoute(int id, [FromBody] UpdateRouteResource resource)
        {
            if (resource == null) return BadRequest();
            var updateRouteCommand = UpdateRouteCommandFromResourceAssembler.toCommandFromResource(resource);
            var result = await routeCommandService.Handle(id, updateRouteCommand);
            if (result is null) return NotFound("Route not found.");
            var updatedResource = RouteAggregateResourceFromResourceAssembler.ToResourceFromEntity(result);
            return Ok(updatedResource);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete Route",
            Description = "Delete a route by Id",
            OperationId = "DeleteRoute"
            )]
        [SwaggerResponse(204, "The route was deleted")]
        [SwaggerResponse(404, "Route not found")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var command = new DeleteRouteCommand(id);
            await routeCommandService.Handle(command);
            return NoContent();
        }
        
    }
}
