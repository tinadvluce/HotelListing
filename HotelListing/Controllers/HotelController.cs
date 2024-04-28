using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitofWork _unitofWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitofWork unitofWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            //try
            //{
                var hotels = await _unitofWork.Hotels.GetAll();
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(results);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
            //    return StatusCode(500, "Internal server error. Please try again later.");
            //}
        }
        [Authorize]
        [HttpGet("{id:int}", Name = "GetHotel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var hotel = await _unitofWork.Hotels.Get(q => q.Id == id, new List<string> { "Country" });
                var result = _mapper.Map<HotelDTO>(hotel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetHotel)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                await _unitofWork.Hotels.Insert(hotel);
                await _unitofWork.Save();

                return CreatedAtRoute("GetHotel", new { id = hotel.Id}, hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateHotel)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitofWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Invalid update attempt in {nameof(UpdateHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                _mapper.Map(hotelDTO, hotel);
                _unitofWork.Hotels.Update(hotel);
                await _unitofWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if(id < 1)
            {
                _logger.LogError($"Invalid delete attemp in {nameof(DeleteHotel)}");
                return BadRequest("Data not available");
            }
            try
            {
                var hotel = _unitofWork.Hotels.Get(q => q.Id == id);
                if(hotel == null)
                {
                    _logger.LogError($"Invalid update attempt in {nameof(DeleteHotel)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitofWork.Hotels.Delete(id);
                await _unitofWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}
