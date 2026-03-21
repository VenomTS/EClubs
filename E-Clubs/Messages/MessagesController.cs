using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Messages;

[ApiController]
[Route("api/clubs/{clubId:guid}/[controller]")]
public class MessagesController(MessageService messageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAllMessagesByClubIdResponse>>> GetAll([FromRoute] Guid clubId)
    {
        var result = await messageService.GetAllMessagesByClubIdAsync(clubId);

        if (result.IsT0)
            return Ok(result.Value);
        return NotFound("Club not found");
    }
    
    [HttpPost]
    public async Task<ActionResult<CreateMessageResponse>> Create([FromRoute] Guid clubId,
        [FromBody] CreateMessageRequest request)
    {
        var result = await messageService.CreateMessageAsync(clubId, request);

        if (result.IsT0)
            return Ok(result.Value);
        return NotFound("Club not found");
    }
}