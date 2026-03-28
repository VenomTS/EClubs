using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Messages;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class ClubMessagesController(MessageService messageService) : ControllerBase
{
    
    [HttpGet(Name = "GetMessagesForClub")]
    [ProducesResponseType<IEnumerable<GetAllMessagesByClubIdResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll([FromRoute] Guid clubId)
    {
        var result = await messageService.GetAllMessagesByClubIdAsync(clubId);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }
    
    [HttpPost(Name = "CreateMessageForClub")]
    [ProducesResponseType<CreateMessageResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromRoute] Guid clubId, [FromBody] CreateMessageRequest request)
    {
        // Validate sender and stuff
        
        var result = await messageService.CreateMessageAsync(clubId, request);

        return result.Match<IActionResult>(
            message => CreatedAtRoute("GetMessageById",  new { id = message.Id }, message),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The club with ID {clubId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }
}