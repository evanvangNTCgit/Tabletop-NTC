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
                    .Where(s => s.IsArchived == false)
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
                .Where(s => s.IsArchived == false)
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

        public List<Piece> GetAllPieces()
        {
            return this._dataContext.Pieces.Include(p => p.PieceType).ToList();
        }

        public void CreateSet(Set set, List<int> pieceIds)
        {
            try
            {
                _dataContext.Sets.Add(set);
                _dataContext.SaveChanges();

                foreach (int pieceId in pieceIds)
                {
                    _dataContext.PieceSets.Add(new PieceSets { PieceId = pieceId, SetId = set.Id });
                }

                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating set: {e.Message}");
            }
        }

        public Session AddSession(Session s)
        {
            _dataContext.Sessions.Add(s);
            _dataContext.SaveChanges();
            return s;
        }

        public List<Set> GetAllSets()
        {
            return _dataContext.Sets
                .Include(s => s.PiecesList)
                    .ThenInclude(ps => ps.Piece)
                .ToList();
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

        // An overload to get the user by an ID
        public User GetUser(int id)
        {
            try
            {

                var user = this._dataContext.Users.FirstOrDefault(u => u.Id == id);
                return user!;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not find user of Id {id}, {e.Message}");
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

        // used to update a piece, specifically it's price and description, as name should stay the same.
        public void UpdatePiece(Piece piece)
        {
            try
            {
                this._dataContext.Pieces.Update(piece);
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating piece {piece.Id}: {e.Message}");
            }
        }


        // used to delete a session by it's ID
        public void DeleteSession(int id)
        {
            try
            {
                var session = this._dataContext.Sessions.FirstOrDefault(s => s.Id == id);
                if (session != null)
                {
                    // Archived instead now!
                    // this._dataContext.Sessions.Remove(session);
                    // this._dataContext.SaveChanges();
                    session.IsArchived = true;
                    this.UpdateSession(session);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting session {id}: {e.Message}");
            }
        }

        public List<Session> GetDeletedSessions(int id)
        {
            return this._dataContext.Sessions
                .Where(s => s.IsArchived == true)
                .Where(s => s.UserId == id)
                .ToList();
        }

        public Session RestoreSession(int id)
        {
            try
            {
                Session s = this._dataContext.Sessions.FirstOrDefault(s => s.Id == id)!;
                s.IsArchived = false;
                this.UpdateSession(s);
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in recovering session likely does not exist\n{ex.Message}");
            }
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

        // Gets the Users Pieces based on ID
        public List<Piece> GetUserPieces(int id)
        {
            try
            {
                // return this._dataContext.UserPieces.Where(up => up.UserId == id).Select(up => up.Piece).Include(up => up.PieceType).ToList();
                return this._dataContext.UserPieces.Where(up => up.UserId == id).Include(up => up.Piece).ThenInclude(p => p.PieceType).Select(up => up.Piece).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured getting user pieces User likely does not exist, {ex.Message}");
            }
        }

        // Deletes a list of tokens by ID. The Javascript saves each deleted token's ID, then sends it to the mapcontroller, then calls this function to delete them from the database.
        public void DeleteTokens(List<int> tokenIds)
        {
            try
            {
                var tokensToDelete = _dataContext.Tokens.Where(t => tokenIds.Contains(t.Id)).ToList();
                if (tokensToDelete.Any())
                {
                    this._dataContext.Tokens.RemoveRange(tokensToDelete);
                    this._dataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"DataService Exception! Error deleting tokens: {e.Message}");
            }
        }
    }
}


