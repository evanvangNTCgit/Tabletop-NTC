using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.Data;
using System.Security.Claims;

namespace SAGroupAlphaSpring26.Services
{
    public class DataService
    {
        private DataContext _dataContext;

        // public List<Piece> pieces;
        // public List<Session> sessions;

        public DataService(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        // Returns a list of all pieces in the data service.
        public List<Piece> GetPieces()
        {
            return this._dataContext.Pieces.ToList();
        }

        // Returns a piece with the 
        public Piece GetPiece(int id)
        {
            try
            {
                var piece = _dataContext.Pieces.FirstOrDefault(p => p.Id == id);

                if (piece == null)
                {
                    throw new Exception();
                }

                return piece;
            }
            catch
            {
                throw new Exception($"Piece of {id} not found");
            }
        }

        public List<PieceType> GetPieceTypes()
        {
            try
            {
                return this._dataContext.PieceTypes.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get piece types {e.Message}");
            }
        }

        // Gets a session. by session id.
        public Session GetSession(int sessionId)
        {
            try
            {
                var session = _dataContext.Sessions
                .Include(s => s.Tokens)
                    .ThenInclude(t => t.Piece)
                .FirstOrDefault(s => s.Id == sessionId);

                if (session == null)
                {
                    throw new Exception();
                }

                return session;
            }
            catch
            {
                throw new Exception($"Session of {sessionId} not found.");
            }
        }

        // Get multiple sessions by user id.
        public List<Session> GetSessions(int userId)
        {
                return _dataContext.Sessions
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.LastUpdated)
                    .ToList();
        }

        // Update sessions.
        public void UpdateSession(Session session)
        {
            // Tracks if sessions are updated in the data context.
            _dataContext.Sessions.Update(session);

            _dataContext.SaveChanges();
        }

        public Piece AddPiece(Piece p)
        {
            try
            {
                _dataContext.Add(p);
                _dataContext.SaveChanges();
                return p;
            }
            catch (Exception e)
            {
                throw new Exception($"Error in adding piece {e.Message}");
            }
        }

        public PieceType AddPieceType(PieceType pt)
        {
            try
            {
                this._dataContext.Add(pt);
                this._dataContext.SaveChanges();
                return pt;
            }
            catch (Exception e)
            {
                throw new Exception($"Error in adding piece type {e.Message}");
            }
        }

        public Session AddSession(Session s)
        {
            _dataContext.Sessions.Add(s);
            _dataContext.SaveChanges();
            return s;
        }

        // Get a user by their email address.
        public User GetUser(string email)
        {
            try
            {

                var user = this._dataContext.Users.FirstOrDefault(u => u.Email == email);
                return user!;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not find user of email {email}, {e.Message}");
            }
        }

        public User AddUser(User user)
        {
            try
            {
                this._dataContext.Add(user);
                this._dataContext.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to add user {user.FirstName}, {e.Message}");
            }
        }

        // Gets the user's id.
        public int GetUserId(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdString, out int userId) ? userId : 0;
        }

        // Gets the piece type based on id on parameter.
        public PieceType GetPieceType(int id) 
        {
            try
            {
                var pt = this._dataContext.PieceTypes.FirstOrDefault(pt => pt.Id == id);
                return pt!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured trying to get piece type\n{ex.Message}");
            }
        }

        public PieceType UpdatePieceType(PieceType pt) 
        {
            this._dataContext.PieceTypes.Update(pt);
            this._dataContext.SaveChanges();
            return pt;
        }
    }
}


