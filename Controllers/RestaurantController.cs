using RestaurantApi.Models;
using RestaurantApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using restaurant_api.Utils;

namespace RestaurantApi.Controllers
{
    [Route("/api/restaurants")]
    [ApiController]
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;

        public RestaurantController(RestaurantService restauranService) =>
            _restaurantService = restauranService;


        [HttpGet]
        public async Task<List<Restaurant>> Get() =>
        await _restaurantService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Restaurant>> Get(string id)
        {
            var restaurant = await _restaurantService.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            return restaurant;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Restaurant newRestaurant)
        {
            var validadeHourFormat = new ValidadeHourFormat();
            bool isValidOpenHour = validadeHourFormat.IsValid(newRestaurant.OpenHour);
            bool isValidCloseHour = validadeHourFormat.IsValid(newRestaurant.CloseHour);

            if (!isValidOpenHour || !isValidCloseHour) return BadRequest();

            await _restaurantService.CreateAsync(newRestaurant);

            return CreatedAtAction(nameof(Get), new { id = newRestaurant.Id }, newRestaurant);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Restaurant updatedRestaurant)
        {
            var restaurant = await _restaurantService.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            updatedRestaurant.Id = restaurant.Id;
            updatedRestaurant.UpdatedTime = DateTime.Now;

            await _restaurantService.UpdateAsync(id, updatedRestaurant);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var restaurant = await _restaurantService.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            await _restaurantService.RemoveAsync(id);

            return NoContent();
        }
    }
}
