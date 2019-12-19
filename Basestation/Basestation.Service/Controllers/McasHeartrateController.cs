using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basestation.MobileAppCommunication.Relay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basestation.MobileAppCommunication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class McasHeartrateController : ControllerBase
    {
        private readonly IRelayManager _relays;

        public McasHeartrateController(IRelayManager relays)
        {
            _relays = relays;
        }
        // GET: api/Heartrate
        [HttpGet]
        public double Get()
        {
            var channel = _relays.Heartrate.AddSubscriber();
            //channel.Reader.next();
            var msg = channel.Reader.ReadAsync().Result;
            //var val = channel.current;
            return msg.Heartrate;
        }


        // GET: api/Heartrate/
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Heartrate
         [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Heartrate/5
         [HttpPut("{id}")]
         public void Put(int id, [FromBody] string value)
         {
        }

        // DELETE: api/ApiWithActions/5
         [HttpDelete("{id}")]
         public void Delete(int id)
          {
         }

    }
}
