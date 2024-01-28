using RestaurantApi.Models;
using RestaurantApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantApi.Controllers
{
    [Route("/api/categories")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly CategoriesService _categoriesController;

        public CategoriesController(CategoriesService categoriesService) =>
            _categoriesController = categoriesService;


        [HttpGet]
        public async Task<List<Category>> Get() =>
        await _categoriesController.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Category>> Get(string id)
        {
            var category = await _categoriesController.GetAsync(id);

            if (category is null)
                return NotFound();

            return category;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category newCategory)
        {
            var category = await _categoriesController.GetByNameAsync(newCategory.Name);

            if (category != null)
                return BadRequest();

            await _categoriesController.CreateAsync(newCategory);

            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Category updatedCategory)
        {
            var restaurant = await _categoriesController.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            updatedCategory.Id = restaurant.Id;
            updatedCategory.UpdatedTime = DateTime.Now;

            await _categoriesController.UpdateAsync(id, updatedCategory);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var restaurant = await _categoriesController.GetAsync(id);

            if (restaurant is null)
                return NotFound();

            await _categoriesController.RemoveAsync(id);

            return NoContent();
        }
    }
}
