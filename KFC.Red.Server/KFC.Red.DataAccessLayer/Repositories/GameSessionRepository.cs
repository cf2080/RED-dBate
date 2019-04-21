﻿using KFC.Red.DataAccessLayer.Data;
using KFC.Red.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFC.Red.DataAccessLayer.Repositories
{
    public class GameSessionRepository
    {
        public GameSession CreateNewGameSession(ApplicationDbContext _db, GameSession gamesession)
        {
            _db.Entry(gamesession).State = EntityState.Added;
            return gamesession;
        }

        public GameSession DeleteGameSession(ApplicationDbContext _db, int Id)
        {
            var gamesession = _db.GameSessions
                .Where(c => c.Id == Id)
                .FirstOrDefault<GameSession>();
            if (gamesession == null)
                return null;
            _db.Entry(gamesession).State = EntityState.Deleted;
            return gamesession;
        }

        public GameSession DeleteGameSession(ApplicationDbContext _db, GameSession gamesession)
        {
            if (gamesession == null)
                return null;
            _db.Entry(gamesession).State = EntityState.Deleted;
            return gamesession;
        }

        public GameSession GetGameSession(ApplicationDbContext _db, int Id)
        {
            return _db.GameSessions.Find(Id);
        }

        public GameSession GetGameSession(ApplicationDbContext _db, string token)
        {
            return _db.GameSessions.Find(token);
        }

        public UserGameStorage GetUserGameStorage(ApplicationDbContext _db, int id)
        {
            return _db.UserGameStorages.Find(id);
        }

        public bool ExistingGameSession(ApplicationDbContext _db, GameSession gamesession)
        {
            var result = GetGameSession(_db, gamesession.Id);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public bool ExistingGameSession(ApplicationDbContext _db, int id)
        {
            var result = GetGameSession(_db, id);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public List<UserGameStorage> GetGameStorages(ApplicationDbContext _db, int gid)
        {
            var ugs = _db.UserGameStorages
                .Where(s => s.GId == gid)
                .ToList();
            return ugs;
        }
    }
}