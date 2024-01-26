using RestaurantApi.Models;
using RestaurantApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace books_api.Controllers
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
        public async Task<IActionResult> Post(Restaurant newBook)
        {
            await _restaurantService.CreateAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
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
