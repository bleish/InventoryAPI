using AutoMapper;
using InventoryAPI.Models;
using InventoryAPI.Repository;
using InventoryAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ItemsListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _itemsRepository;
        private readonly IMapper _mapper;

        public ItemsController(IItemsRepository itemsRepository, IMapper mapper)
        {
            _itemsRepository = itemsRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of all inventory items.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemReadViewModel>), 200)]
        public async Task<ActionResult> Get()
        {
            var items = await _itemsRepository.GetMany();
            return Ok(_mapper.Map<List<ItemReadViewModel>>(items));
        }

        /// <summary>
        /// Retrieves a single inventory item.
        /// </summary>
        /// <param name="id">Item ID (ObjectId format).</param>
        [HttpGet("{id}", Name = "GetItem")]
        [ProducesResponseType(typeof(ItemReadViewModel), 200)]
        public async Task<ActionResult> Get(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }
            var item = await _itemsRepository.GetOne(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ItemReadViewModel>(item));
        }

        /// <summary>
        /// Creates a single inventory item.
        /// </summary>
        /// <param name="itemViewModel">See the model for details.</param>
        [HttpPost]
        public async Task<ActionResult> Create(ItemViewModel itemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newItem = await _itemsRepository.Add(_mapper.Map<Item>(itemViewModel));
            return CreatedAtRoute("GetItem", new { id = newItem.Id }, null);
        }

        /// <summary>
        /// Updates a single inventory item.
        /// </summary>
        /// <param name="id">Item ID (ObjectId format).</param>
        /// <param name="itemViewModel">See the model for details.</param>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ItemViewModel itemViewModel)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }
            var item = await _itemsRepository.GetOne(id);
            if (item == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _itemsRepository.Update(_mapper.Map(itemViewModel, item));
            return NoContent();
        }

        /// <summary>
        /// Removes a single inventory item.
        /// </summary>
        /// <param name="id">Item ID (ObjectId format).</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }
            var oldItem = await _itemsRepository.GetOne(id);
            if (oldItem == null)
            {
                return NotFound();
            }
            await _itemsRepository.Remove(id);
            return NoContent();
        }
    }
}