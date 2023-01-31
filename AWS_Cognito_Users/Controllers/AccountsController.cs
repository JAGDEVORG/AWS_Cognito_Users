using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using AWS_Cognito_Users.Models;
using AWS_Cognito_Users.Models.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AWS_Cognito_Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly CognitoUserPool _pool;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly SignInManager<CognitoUser> _signInManager;
        private ResponseModel responseModel;


        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            responseModel = new ResponseModel();
            var user = _pool.GetUser(model.Email);
            if (user.Status != null)
            {
                responseModel.IsSuccess = false;
                responseModel.Content = "UserExists : User with this email already exists";
                return BadRequest(responseModel);
            }

            user.Attributes.Add("name", model.Email);
            user.Attributes.Add("gender", model.Gender);
            user.Attributes.Add("address", model.Address);
            var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
            responseModel.IsSuccess = createdUser.Succeeded;
            responseModel.Content = JsonConvert.SerializeObject(createdUser);
            return Ok(responseModel);
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmModel model)
        {
            responseModel = new ResponseModel();
            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
            if (user == null)
            {
                responseModel.IsSuccess = false;
                responseModel.Content = "NotFound: A user with the given email address was not found";
                return BadRequest(responseModel);
            }

            var result = await ((CognitoUserManager<CognitoUser>)_userManager)
                .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
            responseModel.IsSuccess = result.Succeeded;
            responseModel.Content = JsonConvert.SerializeObject(result);
            return Ok(responseModel);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            responseModel = new ResponseModel();
            var result = await _signInManager.PasswordSignInAsync(model.Email,
                model.Password, model.RememberMe, false).ConfigureAwait(false);
            if (result.Succeeded)
            {
                responseModel.IsSuccess = true;
                responseModel.Content = JsonConvert.SerializeObject(result);
                return Ok(responseModel);
            }

            responseModel.IsSuccess = false;
            responseModel.Content = "LoginError: Email and password do not match";
            return BadRequest(responseModel);
        }
    }
}
