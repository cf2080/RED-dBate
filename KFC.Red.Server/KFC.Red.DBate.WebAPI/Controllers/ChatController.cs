﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using KFC.Red.DataAccessLayer.Models;
using KFC.Red.ManagerLayer.ChatroomManager;
using KFC.Red.ManagerLayer.QuestionManagement;
using KFC.Red.ServiceLayer.ChatRoom;
using KFC.Red.DataAccessLayer.DTOs;
using KFC.Red.ManagerLayer.UserManagement;
using KFC.Red.ManagerLayer.SessionManagement;
using System.Web.Http.Cors;
using KFC.RED.DataAccessLayer.DTOs;

namespace KFC.Red.DBate.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ChatController : ApiController
    {
        private GameSessionManager _GameSessionManager;
        private UserGameStorageManager _UserGameStoreManager;
        private HubService _ChatHub;
        private SessionManager _SessionManager;

        public ChatController()
        {
            _UserGameStoreManager = new UserGameStorageManager();
            _GameSessionManager = new GameSessionManager();
            _ChatHub = new HubService();
            _SessionManager = new SessionManager();
        }

        [HttpPost]
        [Route("api/chat/postmessage")]
        public IHttpActionResult PostMessage([FromBody] ChatMessageDTO chatMsg)
        {
            _ChatHub.Send(chatMsg.Username,chatMsg.Message);
            return Ok(chatMsg);
        }


        [HttpGet]
        [Route("api/chat/getusers")]
        public List<User> GetUsers(int gid)
        {
            //_ChatHub.SendUserList(_UserGameStoreManager.GetGameUsers(gid));
            return _UserGameStoreManager.GetGameUsers(gid);
        }

        [HttpGet]
        [Route("api/chat/getusers")]
        public List<User> GetUsers(string gameSessionToken)
        {
            var gameSession = _GameSessionManager.GetGameSession(gameSessionToken);
            var users = _UserGameStoreManager.GetGameUsers(gameSession.Id);
            //_ChatHub.SendUserList(_UserGameStoreManager.GetGameUsers(gid));
            return users;
        }

        [HttpGet]
        [Route("api/chat/createchat")]
        public IHttpActionResult CreateChat(string token)
        {
                var userManager = new UserManager();
                var gameSession = new GameSession();
                var sessionManager = new SessionManager();
                var questionManager = new QuestionManager();
                

                try
                {
                    var question = questionManager.RandomizeQuestion();

                    gameSession = _GameSessionManager.CreateGameSession(question);
                    var session = sessionManager.GetSession(token);
                    var user = userManager.GetUser(session.UId);

                    var userGameStorage = new UserGameStorage()
                    {
                        UId = user.ID,
                        GId = gameSession.Id
                    };

                    //questionManager.DeleteQuestion();
                    var storage = _UserGameStoreManager.CreateUGS(userGameStorage);

                    var gameSessionDTO = new GameSessionDTO()
                    {
                        Token = gameSession.Token,
                        Question = questionManager.GetQuestion(gameSession.QuestionID).QuestionString,
                        IsSessionUsed = gameSession.isSessionUsed,
                        PlayerCount = gameSession.PlayerCount
                    };

                    return Ok(gameSessionDTO);
                }
                catch (ArgumentException)
                {
                    return Conflict();
                }
                catch (Exception e)
                {
                    return Content(HttpStatusCode.BadRequest, e.ToString());
                }
        }

        //When joining random room another game session is created MUST BE FIXED!!!!!
        [HttpGet]
        [Route("api/chat/joinrandomchat")]
        public IHttpActionResult JoinRandomChat(string token)
        {
                var userManager = new UserManager();
                var gameSession = new GameSession();
                var sessionManager = new SessionManager();
                var questionManager = new QuestionManager();

                try
                {
                    gameSession = _GameSessionManager.GetRandomGameSession();
                    ++gameSession.PlayerCount;
                    _GameSessionManager.UpdateGameSession(gameSession);
                    var session = sessionManager.GetSession(token);
                    var user = userManager.GetUser(session.UId);

                    var userGameStorage = new UserGameStorage()
                    {
                        UId = user.ID,
                        GId = gameSession.Id
                    };

                    var storage = _UserGameStoreManager.CreateUGS(userGameStorage);

                    var gameSessionDTO = new GameSessionDTO()
                    {
                        Token = gameSession.Token,
                        Question = questionManager.GetQuestion(gameSession.QuestionID).QuestionString,
                        IsSessionUsed = gameSession.isSessionUsed,
                        PlayerCount = gameSession.PlayerCount
                    };

                    return Ok(gameSessionDTO);
                }
                catch (Exception e)
                    {
                        return Content(HttpStatusCode.BadRequest, e.ToString());
                    }
        }

        [HttpDelete]
        [Route("api/chat/deletegame")]
        public IHttpActionResult DeleteGameSession(string gameSessionToken)
        {
            var gameSession = _GameSessionManager.GetGameSession(gameSessionToken);
            _UserGameStoreManager.DeleteGameSessionUsers(gameSessionToken);
            _GameSessionManager.DeleteGameSession(gameSession);
            return Ok();
        }

        [HttpDelete]
        [Route("api/chat/leavegame")]
        public IHttpActionResult LeaveGame(string sessionToken)
        {
            try
            {
                var userSession = _SessionManager.GetSession(sessionToken);
                var userID = userSession.UId;
                var currentUserGameStore = _UserGameStoreManager.GetUserGameStorage(userID);
                var gameSession = _GameSessionManager.GetGameSession(currentUserGameStore.GId);
                --gameSession.PlayerCount;
                _GameSessionManager.UpdateGameSession(gameSession);
                _UserGameStoreManager.DeleteGameUser(userID);
                return Ok();
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.Conflict, e.Message);
            }
        }

        [HttpGet]
        [Route("api/chat/playercount")]
        public IHttpActionResult GetPlayerCount(string gameSessionToken)
        {
            var gameSession = _GameSessionManager.GetGameSession(gameSessionToken);

            return Ok(gameSession.PlayerCount);
        }
    }
}