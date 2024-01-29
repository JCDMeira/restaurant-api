# restaurant-api - api para cadastros de restaurantes

<p align="center">
  <image
  src="https://img.shields.io/github/languages/count/JCDMeira/restaurant-api"
  />
  <image
  src="https://img.shields.io/github/languages/top/JCDMeira/restaurant-api"
  />
  <image
  src="https://img.shields.io/github/last-commit/JCDMeira/restaurant-api"
  />
  <image
  src="https://img.shields.io/github/watchers/JCDMeira/restaurant-api?style=social"
  />
</p>

# 📋 Indíce

- [Proposta](#id01)
  - [Conclusões](#id01.01)
- [Requisitos](#id02)
  - [Requisitos funcionais](#id02.1)
  - [Requisitos não funcionais](#id02.2)
  - [Requisitos não obrigatórios](#id02.3)
- [Aprendizados](#id03)
- [Feito com](#id04)
- [Pré-requisitos](#id05)
- [Procedimentos de instalação](#id06)
- [Autor](#id07)

# 🚀 Proposta <a name="id01"></a>

Este é o projeto tem como objetivo central a criação de uma api de cadastro e busca de restaurantes, também contendo categorias.

# 🎯 Requisitos <a name="id02"></a>

## 🎯 Requisitos funcionais <a name="id02.1"></a>

Sua aplicação deve ter:

- Um end-point para cada um dos métodos get, put, post e delete para um restaurante
- Deve ser possível criar um registro de restaurante
- Deve ser possível editar um registro de restaurante
- Deve ser possível deletar um registro de restaurante
- Deve ser possível buscar a lista de restaurantes já existentes
- Um end-point para cada um dos métodos get, put, post e delete para a categoria do restaurante
- Deve ser possível criar um registro de categoria
- Deve ser possível editar um registro de categoria
- Deve ser possível deletar um registro de categoria
- Deve ser possível buscar a lista de categorias já existentes
- Cada restaurante é vinculado a uma categoria de especialidade.
- Um restaurante deve ter
  - nome
  - horário de abrir
    - horário de fechar
- Uma categoria deve ter
  - nome
    - descrição

## 🎯 Requisitos não funcionais <a name="id02.2"></a>

É obrigatório a utilização de:

- .net
- mongoDB

## 🎯 Requisitos não obrigatórios <a name="id02.3"></a>

- uma busca por todos restaurantes de uma categoria específica

# Aprendizados <a name="id03"></a>

Como uma abstração para garantir a fonte da informação e escrever menos códigos repetidos foi usado uma tática de extensão de classe para as Models, sendo por exemplo abstraído coisas comuns do MongoDB.

```c#
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RestaurantApi.Models;

public class MongoBaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime? CreatedTime { get; } = DateTime.Now!;

    public DateTime? UpdatedTime { get; set; } = DateTime.Now!;
}
```

Então as classes de modelos do mongo extendem essa e já tem por padrão essasa três props.

No que trata a configuração do banco, inicialmente foi usada duas DatabaseName's diferentes, o que resultou em duas databases isoladas na cluster, mas por ser a mesma aplicação foi mudado para usar a mesma DatabaseName e ser duas collections dentro da mesma database.

```json
  "RestaurantDatabase": {
    "ConnectionString": "mongodb+srv://login:senha@cluster0.jhwjo5u.mongodb.net/",
    "DatabaseName": "RestaurantsResgiters",
    "RestaurantsCollectionName": "Restaurants"
  },
  "CategoriesDatabase": {
    "ConnectionString": "mongodb+srv://login:senha@cluster0.jhwjo5u.mongodb.net/",
    "DatabaseName": "RestaurantsResgiters",
    "CategoriesCollectionName": "Categories"
  },
```

Uma melhoria adicional que deve ser testada é o uso de CollectionName como chave para ambos os objetos, tentando assim reduzir o código da classe de configuração, que cairia de uma base class com duas classes extendidas para apenas uma classe.

Para validar o formato do dado da hora foi criado uma classe util, que foi abstraída para uso em mais lugares. O ato de abstrair tratamentos favorece responsabilidade única, desacoplamento e melhorar a leegibilidade e manutebilidade.

```c#
using System.Text.RegularExpressions;

namespace restaurant_api.Utils;
    public class ValidadeHourFormat
    {
      public bool IsValid(string hourAndMinutesString)
    {
        bool isValidOpenHour = Regex.IsMatch(hourAndMinutesString, "^[0-9]{2}:[0-9]{2}$");
        if (!isValidOpenHour) return false;

        var HourAndMinutes = hourAndMinutesString.Split(':');
        var hour = int.Parse(HourAndMinutes[0]);
        var minutes = int.Parse(HourAndMinutes[1]);
        bool isValidHourFormat = hour <= 24 && minutes < 60;
        if (!isValidHourFormat) return false;
        return true;
    }
    }

```

Como o dado de Restaurant é atrelado a uma Category e os campos de nomes dessas Models são únicas houve uma necessidadde maior de validações, que foram adicionadas no código do controller.

```c#
using RestaurantApi.Models;
using RestaurantApi.Services;
using Microsoft.AspNetCore.Mvc;
using restaurant_api.Utils;

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
```

E dddevido ao fator de ser adicionado uma categoria no restaurante resultaria numa busca que pegaria dados da collection de categoria e popularia nos dados de restaurante, como é feito com o populate() do mongoose no nodeJS. Mas o mongo sendo não relacional são é recomendado buscas desse tipo, e o .net até traz o uso do DBRef do MongoDB.Driver que é similar, mas diferente do populate traz muitos passos de construção maanual, sendo ainda muito desistimulado. Portanto foi trabalhado uma concepção que favorece a característica de validação de integridade de tipos da forte tipagem do c#.

Adotando Category como um campo da model de Restaurant, e buscando e validanddo o id da categoria na criação e edição do campo, isso evita a ref e a busca fazendo a relação dee tabelas, que seria muito custosa em casos de um getAll da collection de Restaurants.

Sendo ainda que armazenar texto na collection é relativamente barato, sendo melhor que o processo de busca com referências.

```c#
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.Models;

public class Restaurant : MongoBaseEntity
{
    // tem como caracterizar como unique pelo mongo ?
    [BsonElement("Name")]
    [MinLength(10)]
    public string Name { get; set; } = null!;

    public string OpenHour { get; set; } = null!;

    public string CloseHour { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string CategoryId { get; set; } = null!;

    public Category? Category { get; set; } = null!;
}
```

## Considerações

O mongoDB é um banco de dados Não-SQL, ou seja, ele não segue a ideia de tabelas e relações, sendo em princcípio formado de coleções e documentos, e um documento não guarda relações com outros documentos. Para isso é possível usar o processo de Refs, mas é fortemente avisado para não usar, e se for preciso manter esses tipos de busca se recomenda os bancos relacionais.

Uma maneira de trabalhar com esses tipos de buscas é manter mais texto escrito na collecion e salvar a entidade reelacionada dentro da entidade principal. Ou adotar modelagens que favoreçam isso.

Exemplos são salvar o userId dono de um item e ao buscar os itens do usuuário se pega todos que tem aquele id, ou ccomo foi feito no projeto salvar um CategoryId que vai chegar na requisição e então buscar a Category para adicionar ao Restaurant antes de salvar no banco, já que o restaurante se liga apenas a uma categoria.

Fora conceeitos do mongo também vale reforçar que a linguagem trabalha bem com herança, podendo extender as classes base para repetir menos código igual e isolar o princípio da fonte única da informação.

E de isolar processos possivelmente repetitivos que serão usados em mais lugares em classes utilitárias pode melhorar muito o processo do teu projeto.

# 🛠 Feito com <a name="id04"></a>

<br />

- C#
- .net 8
- visual studio
- mongoDB

<br />

# :sunglasses: Autor <a name="id07"></a>

<br />

- Linkedin - [jeanmeira](https://www.linkedin.com/in/jeanmeira/)
- Instagram - [@jean.meira10](https://www.instagram.com/jean.meira10/)
- GitHub - [JCDMeira](https://github.com/JCDMeira)
