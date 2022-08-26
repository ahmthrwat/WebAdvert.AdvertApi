using AdvertApi.Model;
using AdvertApi.Services;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertApi.Controllers
{

   
    [Route("adverts/v1")]
    [ApiController]
    public class Advert : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly IAdvertStorageService _advertStorageService;
        public Advert(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            Configuration = configuration;
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type=typeof(CreateAdvertResponse))]
        public async Task<IActionResult> CreateAsync(AdvertModel model)
        {
            string recordId;
            try
            {
                  recordId = await _advertStorageService.Add(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }
            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }


        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.Confirm(model);
              //  await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return new OkResult();
        }

        //private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        //{
        //    var topicArn = Configuration.GetValue<string>("TopicArn");
        //    var dbModel = await _advertStorageService.GetByIdAsync(model.Id);

        //    using (var client = new AmazonSimpleNotificationServiceClient())
        //    {
        //        var message = new AdvertConfirmedMessage
        //        {
        //            Id = model.Id,
        //            Title = dbModel.Title
        //        };

        //        var messageJson = JsonConvert.SerializeObject(message);
        //        await client.PublishAsync(topicArn, messageJson);
        //    }
        //}

    }
}
