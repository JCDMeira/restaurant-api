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

        [HttpPost]
        public async Task<IActionResult> Post(Restaurant newBook)
        {
            await _restaurantService.CreateAsync(newBook);

            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

    }
}
