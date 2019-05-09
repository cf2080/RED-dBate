﻿using KFC.Red.DataAccessLayer.Data;
using KFC.Red.DataAccessLayer.DTOs;
using KFC.Red.ManagerLayer.SessionManagement;
using KFC.Red.ManagerLayer.SSO;
using KFC.Red.ManagerLayer.UserManagement;
using KFC.RED.DataAccessLayer.DTOs;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KFC.Red.DBate.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SSOController : ApiController
    {
        [HttpPost]
        [Route("api/sso/login")]
        public IHttpActionResult SsoLogin([FromBody] LoginDTO request)
        {
                try
                {
                    var ssoLoginManager = new SSO_Manager();
                    var ssoId = new Guid(request.SSOUserId);

                    var loginSession = ssoLoginManager.LoginFromSSO(
                        request.Email,
                        ssoId);
                    var redirectURL = "http://localhost:8080/#/login/?token=" + loginSession.Token;
                    return Redirect(redirectURL);
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, "An Error Occured: " + e.Message + e.TargetSite + e.Source);
                }
        }
        
        [HttpPost]
        [Route("api/sso/logout")]
        public IHttpActionResult Logout([FromBody] LogoutDTO req)
        {   
                var sessionManager = new SessionManager();
                try
                {
                    sessionManager.DeleteSession(req.Token);

                    return Ok();
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.Conflict, e.Message);
                }
        }

        //NEED TO FIX
        [HttpDelete]
        [Route("api/user/delete")]
        public async Task<IHttpActionResult> DeleteFromSSO(string token)
        {

            try
            {
                var sessionManager = new SessionManager();
                var ssoManager = new SSO_Manager();
                var userManager = new UserManager();

                var session = sessionManager.GetSession(token);
                var userId = session.UId;
                var user = userManager.GetUser(userId);

                if (user == null)
                {
                    return Content(HttpStatusCode.NotFound, "User does not exist: " + session + "/n" + userId + "/n" + user);
                }

                var deleteResult = await ssoManager.DeleteAccountSSO(user);

                if (deleteResult)
                {
                    var sessionDeleted = sessionManager.DeleteSession(token);
                    var userDeleted = userManager.DeleteUser(user);
                    return Ok();
                }

                return Content(HttpStatusCode.NotImplemented,"User not able to delete");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NotImplemented, e.Message);
            }
        }

        [HttpGet]
        [Route("api/sso/health")]
        public IHttpActionResult HealthCheck()
        {
            using(var _db = new ApplicationDbContext())
            {
                try
                {
                    if (_db.Database.Exists())
                    {
                        return Ok("DBate is working properly");
                    }

                    return Content(HttpStatusCode.InternalServerError, "An Error has been encountered");

                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.InternalServerError, e.Message);
                }
            }
        }
    }
}