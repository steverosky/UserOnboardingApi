
using Microsoft.AspNetCore.Mvc;
using UserOnboardingApi.EFCore;
using UserOnboardingApi.Model;
using static UserOnboardingApi.Model.DbHelper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserOnboardingApi.Controllers
{
   
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbHelper _db;
        public UserController(EF_DataContext eF_DataContext)
        {
            _db = new DbHelper(eF_DataContext);
        }
        // GET: api/<UserController>
        [HttpGet]
        [Route("api/[controller]/GetUsers")]
        public IActionResult Get()
        {
            ResponseType type = ResponseType.Success;
            try
            {
                IEnumerable<userModel> data = _db.GetUsers();
                
                if (!data.Any())
                {
                    type = ResponseType.NotFound;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            };
        }

        // GET api/<UserController>/5
        [HttpGet]
        [Route("api/[controller]/GetUserById/{id}")]
        public IActionResult Get(int id)
        {
            ResponseType type = ResponseType.Success;
            try
            {
                userModel data = _db.GetUserById(id);

                if (data == null)
                {
                    type = ResponseType.Failure;
                }
                return Ok(ResponseHandler.GetAppResponse(type, data));

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            };
        }

        // POST api/<UserController>
        [HttpPost]
        [Route("api/[controller]/AddUser")] 
        public IActionResult PosAt([FromBody] userModel model)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                _db.AddUser(model);
                return Ok(ResponseHandler.GetAppResponse(type, model));


                //var message = new Message(new string[] { model.Email }, "Welcome to User Onboarding", "Hello " + model.Name + ", Your account has been created successfully.\n Your password is " + model.Password + "\n Please change your password after login.");
                //emailSender.SendEmail(message);


                
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

        // PUT api/<UserController>/5
        [HttpPut]
        [Route("api/[controller]/UpdateUser")]
        public IActionResult Put(int id, [FromBody] userModel model)
        {
            try
            {
                ResponseType type = ResponseType.Success;
                _db.SaveUser(model);
                return Ok(ResponseHandler.GetAppResponse(type, model));
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }

       
        // DELETE api/<UserController>/5
        [HttpDelete]
        [Route("api/[controller]/DeleteUser/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                ResponseType type = ResponseType.Success;

                  _db.DeleteUser(id);
                return Ok(ResponseHandler.GetAppResponse(type, "Delete Success"));
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            }
        }
    }
}
