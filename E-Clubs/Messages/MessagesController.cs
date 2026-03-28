using E_Clubs.Messages.DTO;
using E_Clubs.Messages.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Clubs.Messages;

[ApiController]
[Route("/api/[controller]")]
public class MessagesController(MessageService messageService) : ControllerBase
{
    [HttpGet("{messageId:guid}", Name = "GetMessageById")]
    [ProducesResponseType<GetMessageByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessageById([FromRoute] Guid messageId)
    {
        var result = await messageService.GetMessageByIdAsync(messageId);

        return result.Match<IActionResult>(
            Ok,
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The message with ID {messageId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }

    [HttpDelete(Name = "DeleteMessageById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessageById([FromQuery] Guid messageId)
    {
        var result = await messageService.DeleteMessageByIdAsync(messageId);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The message with ID {messageId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }
    
    [HttpPut("{messageId:guid}", Name = "UpdateMessageById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMessageById([FromRoute] Guid messageId, [FromBody] UpdateMessageRequest request)
    {
        var result = await messageService.UpdateMessageByIdAsync(messageId, request);

        return result.Match<IActionResult>(
            _ => NoContent(),
            _ => NotFound(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"The message with ID {messageId} was not found",
                Instance = HttpContext.Request.Path,
            })
        );
    }
    
}