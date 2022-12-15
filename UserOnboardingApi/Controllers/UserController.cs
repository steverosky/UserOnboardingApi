
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using UserOnboardingApi.EFCore;
using UserOnboardingApi.Model;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserOnboardingApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly EF_DataContext _context;
        private readonly DbHelper _db;
        public IConfiguration _configuration;
        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityUser> _roleManager;

        public UserController(EF_DataContext eF_DataContext, IConfiguration config, DbHelper db /*UserManager<IdentityUser> userManager, RoleManager<IdentityUser> roleManager*/)
        {
            _context = eF_DataContext;
            _db = db;
            _configuration = config;
            //_userManager = userManager;
            //_roleManager = roleManager;
        }


        // GET: api/<UserController>
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult Get()
        {
            ResponseType type = ResponseType.Success;
            try
            {
                // Load the JWT token from the request header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decode the JWT token and extract the payload
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

                if (HasPermission(userRole, "GetUsers"))
                {

                    IEnumerable<userModel> data = _db.GetUsers();

                    if (!data.Any())
                    {
                        type = ResponseType.NotFound;
                    }
                    return Ok(ResponseHandler.GetAppResponse(type, data));
                }
                else
                {
                    type = ResponseType.Unauthorized;
                    return BadRequest(ResponseHandler.GetAppResponse(type, null));
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            };
        }

        // GET api/<UserController>/5
        [HttpGet]
        [Route("GetUserById")]
        public IActionResult Get(int id)
        {
            ResponseType type = ResponseType.Success;
            try
            {
                // Load the JWT token from the request header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Decode the JWT token and extract the payload
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

                if (HasPermission(userRole, "GetUserById"))
                {userModel data = _db.GetUserById(id);
                    if (data == null)
                    {
                        type = ResponseType.NotFound;
                    }
                    return Ok(ResponseHandler.GetAppResponse(type, data));
                }
                else
                {
                    type = ResponseType.Unauthorized;
                    return BadRequest(ResponseHandler.GetAppResponse(type, null));
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ResponseHandler.GetExceptionResponse(ex));
            };
        }

        // POST api/<UserController>
        [HttpPost]
        [Route("AddUser")]
        public IActionResult Post([FromBody] userModel model)
        {

            // Load the JWT token from the request header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the JWT token and extract the payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            if (HasPermission(userRole, "AddUser"))
            {
                //// Set the default role for the user
                //model.Role = "user";

                // Validate the model and the default role
                if (!ModelState.IsValid /*|| !IsValidRole(model.Role*/)
                {
                    return BadRequest(ModelState);
                }
                try
                {
                    ResponseType type = ResponseType.Success;
                    _db.AddUser(model);

                    return Ok(ResponseHandler.GetAppResponse(type, model));

                }
                catch (Exception ex)
                {

                    return BadRequest(ResponseHandler.GetExceptionResponse(ex));
                }
            }
            else
            {
                ResponseType type = ResponseType.Unauthorized;
                return BadRequest(ResponseHandler.GetAppResponse(type, null));
            }
        }



        // PUT api/<UserController>/5
        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult Put(int id, [FromBody] userModel model)
        {
            // Load the JWT token from the request header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the JWT token and extract the payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            if (HasPermission(userRole, "UpdateUser"))
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
            else
            {
                ResponseType type = ResponseType.Unauthorized;
                return BadRequest(ResponseHandler.GetAppResponse(type, null));
            }
        }

        // PUT api/<UserController>/6
        [HttpPut]
        [Route("ChangePass")]
        public IActionResult Change([FromBody] userModel email)
        {
            // Load the JWT token from the request header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the JWT token and extract the payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            if (HasPermission(userRole, "ChangePass"))
            {
                try
                {
                    ResponseType type = ResponseType.Success;
                    _db.ChangePass(email);
                    return Ok(ResponseHandler.GetAppResponse(type, email));
                }
                catch (Exception ex)
                {

                    return BadRequest(ResponseHandler.GetExceptionResponse(ex));
                }
            }
            else
            {
                ResponseType type = ResponseType.Unauthorized;
                return BadRequest(ResponseHandler.GetAppResponse(type, null));
            }
        }

        // login  api/<UserController>/6
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(userModel model)
        {
            //// Load the JWT token from the request header
            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //// Decode the JWT token and extract the payload
            //var handler = new JwtSecurityTokenHandler();
            //var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            //var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            //if (HasPermission(userRole, "Login"))
            //{
                try
                {
                    ResponseType type = ResponseType.Success;
                    var test = await _db.CreateTokenAsync(model.Email, model.Password);

                    return Ok(ResponseHandler.GetAppResponse(type, test));
                }
                catch (Exception ex)
                {

                    return BadRequest(ResponseHandler.GetExceptionResponse(ex));
                }
            //}
            //else
            //{
            //    ResponseType type = ResponseType.Unauthorized;
            //    return BadRequest(ResponseHandler.GetAppResponse(type, null));
            //}
        }



        // DELETE api/<UserController>/5
        [HttpDelete]
        [Route("DeleteUser")]
        public IActionResult Delete(int id)
        {
            // Load the JWT token from the request header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the JWT token and extract the payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            if (HasPermission(userRole, "DeleteUser"))
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
            else
            {
                ResponseType type = ResponseType.Unauthorized;
                return BadRequest(ResponseHandler.GetAppResponse(type, null));
            }
        }


        // ASSIGN ROLES api/<UserController>/5
        [HttpPost]
        [Route("AssignRoles")]
        public IActionResult AssignRoles([FromBody] userModel model)
        {
            // Load the JWT token from the request header
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the JWT token and extract the payload
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userRole = jsonToken.Claims.First(claim => claim.Type == "Role").Value;

            if (HasPermission(userRole, "AssignRoles"))
            {
                try
                {
                    ResponseType type = ResponseType.Success;

                    _db.AssignRoles(model);
                    return Ok(ResponseHandler.GetAppResponse(type, "Role Assigned Successfully"));
                }
                catch (Exception ex)
                {

                    return BadRequest(ResponseHandler.GetExceptionResponse(ex));
                }
            }
            else
            {
                ResponseType type = ResponseType.Unauthorized;
                return BadRequest(ResponseHandler.GetAppResponse(type, null));
            }
        }

        // Method to check if the specified role is valid and supported by the application
        //private bool IsValidRole(string role)
        //{
        //    // Check if the role is supported by the application
        //    // and return the result
        //    return (role == "user" || role == "admin" || role == "moderator");
        //}


        // Dictionary to map roles to their corresponding permissions
        private Dictionary<string, List<string>> rolePermissions = new Dictionary<string, List<string>>()
        {
            { "user", new List<string>() { "ChangePass", "UpdateUser", "Login" } },
            { "admin", new List<string>() { "ChangePass", "UpdateUser", "Login", "AddUser", "DeleteUser", "GetUsers", "GetUserById", "AssignRoles" } }
        };

        // Method to check if the user has the specified permission
        private bool HasPermission(string role, string permission)
        {
            // Check if the role exists in the dictionary and if it has the specified permission
            if (rolePermissions.ContainsKey(role) && rolePermissions[role].Contains(permission))
            {
                return true;
            }

            return false;
        }

    }
}


//[HttpPost]
//[Route("AddAdmin")]
//public async Task<IActionResult> AddAdmin([FromBody] userModel model)
//{
//    // Validate the model
//    if (!ModelState.IsValid)
//    {
//        return BadRequest(ModelState);
//    }

//    IdentityUser user = new()
//    {
//        Email = model.Email,
//        SecurityStamp = Guid.NewGuid().ToString(),

//    };
//    try
//    {
//        ResponseType type = ResponseType.Success;

//        //if (!await _roleManager.RoleExistsAsync(UserRoles.Admin)) 
//        //    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
//        //if (!await _roleManager.RoleExistsAsync(UserRoles.User))
//        //    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

//        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
//        {
//            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
//        }
//        if (await _roleManager.RoleExistsAsync(UserRoles.User))
//        {
//            await _userManager.AddToRoleAsync(user, UserRoles.User);
//        }


//        return Ok(ResponseHandler.GetAppResponse(type, model));

//    }
//    catch (Exception ex)
//    {

//        return BadRequest(ResponseHandler.GetExceptionResponse(ex));
//    }
//}
