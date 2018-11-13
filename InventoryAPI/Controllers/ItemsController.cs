using AutoMapper;
using Castle.DynamicProxy;
using ChangeTracking;
using InventoryAPI.Models;
using InventoryAPI.Repository;
using InventoryAPI.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(string id, [FromBody] JsonPatchDocument<ItemPatchViewModel> patch)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }
            if (patch == null)
            {
                return BadRequest();
            }

            var item = await _itemsRepository.GetOne(id);
            if (item == null)
            {
                return NotFound();
            }

            var patchViewModel = _mapper.Map<ItemPatchViewModel>(item);
            patch.ApplyTo(patchViewModel, ModelState);
            TryValidateModel(patchViewModel);
            
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var trackedItem = item.AsTrackable();

            _mapper.Map(patchViewModel, trackedItem);
            
            // CHANGED PROPERTIES CHECKER

            // var trackable = trackedItem.CastToIChangeTrackable();
            // var changes = trackable.ChangedProperties;
            // var subTrackable = trackedItem.Fetal.CastToIChangeTrackable();
            // var subChanges = subTrackable.ChangedProperties;

            // THE CODE

            // var operations = new List<MongoOperation>();

            // var trackable = trackedItem.CastToIChangeTrackable();
            // foreach (var changedProperty in trackable.ChangedProperties)
            // {
            //     var propertyInfo = trackedItem.GetType().GetProperty(changedProperty);
            //     var value = propertyInfo.GetValue(trackedItem);
            //     operations.Add(new MongoOperation
            //     {
            //         Path = changedProperty,
            //         Value = value,
            //         Type = propertyInfo.PropertyType
            //     });
            // }
            
            // var itemProperties = trackedItem.GetType().GetProperties();
            // foreach (var property in itemProperties)
            // {
            //     var propValue = trackedItem.GetType().GetProperty(property.Name).GetValue(trackedItem);
            //     if (propValue != null)
            //     {
            //         if (propValue.GetType().Namespace == "Castle.Proxies")
            //         {
            //             var changedInfo = propValue.GetType().GetProperties().Where(p => p.Name == "ChangedProperties")?.FirstOrDefault();
            //             if (changedInfo != null)
            //             {
            //                 var changedObject = changedInfo.GetValue(propValue);
            //                 var changedProperties = (IEnumerable<string>)changedObject;
            //                 foreach(var changedProperty in changedProperties)
            //                 {
            //                     var path = $"{property.Name}.{changedProperty}";
            //                     var propertyInfo = property.PropertyType.GetProperty(changedProperty);
            //                     var value = propertyInfo.GetValue(propValue);
            //                     operations.Add(new MongoOperation
            //                     {
            //                         Path = path,
            //                         Value = value,
            //                         Type = propertyInfo.PropertyType

            //                     });
            //                 }
            //             }
            //         }
            //     }
            // }

            // var check = "";

            // foreach (var operation in operations)
            // {
            //     if (operation.Value != null)
            //     {
            //         dynamic changed = Convert.ChangeType(operation.Value, operation.Type);
            //         var check2 = "";
            //     }
            // }

            // TODO: Remove this
            // var patchModel = new ItemPatchViewModel
            // {
            //     Cost = patchViewModel.Cost == item.Cost ? null : patchViewModel.Cost,
            //     Notes = patchViewModel.Notes == item.Notes ? null : patchViewModel.Notes
            // };

            // var funch = await _itemsRepository.UpdatePartial(id, _mapper.Map<PartialItem>(patchModel));

            // var FUNCH = await _itemsRepository.UpdatePartial(id, GetChangeDocument(patch));

            // var itemUpdate = new ItemUpdate {
            //     Replace = patch.Operations.Where(o => o.OperationType == OperationType.Replace).ToDictionary(o => o.path, o => o.value),
            //     Remove = patch.Operations.Where(o => o.OperationType == OperationType.Remove).ToDictionary(o => o.path, o => o.value)
            // };

            // var SUPERFUNCH = await _itemsRepository.UpdatePartial(id, itemUpdate.ToBsonDocument());

            // var itemUpdate = new ItemUpdate
            // {
            //     Operations = _mapper.Map<List<ItemUpdateOperation>>(patch.Operations)
            // };

            // var ULTRAMEGAFUNCH = await _itemsRepository.UpdatePartial(id, itemUpdate.ToBsonDocument());

            var result = await _itemsRepository.UpdatePartial(id, trackedItem);

            return NoContent();
        }

        // TODO: Remove this
        // private BsonDocument GetChangeDocument(JsonPatchDocument<ItemPatchViewModel> patch)
        // {
        //     var changeDoc = new BsonDocument();
        //     foreach (var operation in patch.Operations)
        //     {
        //         var fieldChange = new BsonDocument(new Dictionary<string, object> { [operation.path] = operation.value });
        //         switch (operation.OperationType)
        //         {
        //             case OperationType.Replace:
        //                 changeDoc["$set"] = fieldChange;
        //                 break;
        //             case OperationType.Remove:
        //                 changeDoc["$unset"] = fieldChange;
        //                 break;
        //             default:
        //                 throw new Exception();
        //         }
        //     }

        //     return changeDoc;
        // }

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

    public class MongoOperation
    {
        public string Path { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}