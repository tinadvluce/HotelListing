using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Marvin.Cache.Headers;
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
    public class CountryController : ControllerBase
    {
        private readonly IUnitofWork _unitofWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitofWork unitofWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitofWork = unitofWork;
            _logger = logger;
            _mapper = mapper;
        }

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetCountries()
        //{
        //    try
        //    {
        //        var countries = await _unitofWork.Countries.GetAll();
        //        var results = _mapper.Map<IList<CountryDTO>>(countries);
        //        return Ok(results);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
        //        return StatusCode(500, "Internal Server Error. Please try again later.");
        //    }
        //}
        [HttpGet]
        //[ResponseCache(CacheProfileName = "120SecondsDuration")]
        //can be used to override global settings
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
        {
            try
            {
                var countries = await _unitofWork.Countries.GetPagedList(requestParams);
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetCountry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var country = await _unitofWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels"});
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post attempt in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }
            
            try
            {
                var country = _mapper.Map<Country>(countryDTO);
                await _unitofWork.Countries.Insert(country);
                await _unitofWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(CreateCountry)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        //[Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogError($"Invalid update attempt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitofWork.Countries.Get(q => q.Id == id);
                if(country == null || id < 0)
                {
                    _logger.LogError($"Invalid update attempt in {nameof(UpdateCountry)}");
                    return BadRequest("Submitted data is invalid.");
                }

                _mapper.Map(countryDTO, country);
                _unitofWork.Countries.Update(country);
                await _unitofWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(UpdateCountry)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if(id < 1)
            {
                _logger.LogError($"Invalid delete attemp in {nameof(DeleteCountry)}");
                return BadRequest("Data not available");
            }

            try
            {
                var country = _unitofWork.Countries.Get(q => q.Id == id);
                if (country == null)
                {
                    _logger.LogError($"Invalid update attempt in {nameof(DeleteCountry)}");
                    return BadRequest("Submitted data is invalid");
                }

                await _unitofWork.Countries.Delete(id);
                await _unitofWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(DeleteCountry)}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
            
        }
    }
}
