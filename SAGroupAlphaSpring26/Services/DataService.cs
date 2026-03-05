using SAGroupAlphaSpring26.Data;

namespace SAGroupAlphaSpring26.Services
{
    public class DataService
    {
        private DataContext _dataContext;

        public List<Piece> pieces;

        public List<Session> sessions;

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
                return pieces.FirstOrDefault(p => p.PieceId == id)!;
            }
            catch
            {
                throw new Exception($"Piece of {id} not found");
            }
        }

        // Gets a session. by session id.
        public Session GetSession(int sessionId)
        {
            try
            {
                return sessions.FirstOrDefault(s => s.SessionId == sessionId)!;
            }
            catch
            {
                throw new Exception($"Session of {sessionId} not found.");
            }
        }

        // Get multiple sessions by user id.
        public List<Session> GetSessions(int userId)
        {
            try
            {
                return sessions.Where(s => s.UserId == userId).ToList();
            }
            catch
            {
                throw new Exception($"Failed to get session for User: {userId}");
            }
        }
    }
}
