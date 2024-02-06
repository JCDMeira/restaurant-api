using RestaurantApi.Models;
using RestaurantApi.Services;
using Microsoft.AspNetCore.Mvc;
using restaurant_api.Utils;
using System.Text.RegularExpressions;

namespace RestaurantApi.Controllers
{
    [Route("/api/restaurants")]
    [ApiController]
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly CategoriesService _categoriesController;
     

        public RestaurantController(RestaurantService restauranService, CategoriesService categoriesService)
        {
            _restaurantService = restauranService;
            _categoriesController = categoriesService;
        }

        [HttpGet]
        public async Task<List<Restaurant>> Get([FromQuery] string? name, [FromQuery] string? categoryId) {
            List < Restaurant > restaurants = await _restaurantService.GetAsync();


            return restaurants
                .Where(r => 
                    RestaurantFilters.hasSameCategory( r.CategoryId, categoryId) && 
                    RestaurantFilters.hasSearchString(r.Name, name))
                .ToList();
        }

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

            var category = await _categoriesController.GetAsync(newRestaurant.CategoryId);

            if (category is null)
                return BadRequest();

            var restaurant = await _restaurantService.GetByNameAsync(newRestaurant.Name);

            if (restaurant != null)
                return BadRequest();

            newRestaurant.Category = category;

            await _restaurantService.CreateAsync(newRestaurant);

            return CreatedAtAction(nameof(Get), new { id = newRestaurant.Id }, newRestaurant);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Restaurant updatedRestaurant)
        {
            var restaurant = await _restaurantService.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            var category = await _categoriesController.GetAsync(updatedRestaurant.CategoryId);

            if (category is null)
                return BadRequest();

            var restaurantByName = await _restaurantService.GetByNameAsync(updatedRestaurant.Name);

            if (restaurantByName != null)
                return BadRequest();

            updatedRestaurant.Id = restaurant.Id;
            updatedRestaurant.UpdatedTime = DateTime.Now;
            updatedRestaurant.Category = category;

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
