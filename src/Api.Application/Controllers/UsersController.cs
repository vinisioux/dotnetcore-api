using System;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private IUserService _service;
    public UsersController(IUserService service)
    {
      _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        return Ok(await _service.GetAll());
      }
      catch (ArgumentException err)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
      }
    }

    // localhost:5000/api/users/331231-312312-31231
    [HttpGet]
    [Route("{id}", Name = "GetWithId")]
    public async Task<ActionResult> GetById(Guid id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        return Ok(await _service.Get(id));
      }
      catch (ArgumentException err)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
      }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] UserEntity user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {

        var userCreated = await _service.Post(user);

        if (userCreated != null)
        {
          return Created(new Uri(Url.Link("GetWithId", new { id = userCreated.Id })), userCreated);
        }
        else
        {
          return BadRequest();
        }

      }
      catch (ArgumentException err)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
      }
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UserEntity user)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {

        var userUpdated = await _service.Put(user);

        if (userUpdated != null)
        {
          return Ok(userUpdated);
        }
        else
        {
          return BadRequest();
        }

      }
      catch (ArgumentException err)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
      }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        return Ok(await _service.Delete(id));
      }
      catch (ArgumentException err)
      {
        return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
      }
    }
  }
}