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

            sessions = new List<Session>();
            sessions.Add(new Session { Id = 1, UserId = 1, Notes = "This is a session note.", LastUpdated = DateTime.Now });

            pieces = new List<Piece>();
            pieces.Add(new Piece { Id = 1, PieceTypeID = 1, SetID = 1, Name = "Evan", Description = "Evan is fast.", Price = 1.00m, });
        }

        // Returns a list of all pieces in the data service.
        public List<Piece> GetPieces()
        {
            return this._dataContext.Pieces.ToList();
        }

        // Returns a piece with the 
        public Piece GetPiece(int id)
        {
            return pieces.FirstOrDefault(p => p.Id == id);
        }

        // Gets a session. by session id.
        public Session GetSession(int sessionId)
        {
            return sessions.FirstOrDefault(s => s.Id == sessionId);
        }

        // Get multiple sessions by user id.
        public List<Session> GetSessions(int userId) { 

            return sessions.Where(s => s.UserId == userId).ToList();
        }

    }
}
