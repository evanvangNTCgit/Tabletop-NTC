using Microsoft.EntityFrameworkCore;
using SAGroupAlphaSpring26.ApiServices;
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
            return this._dataContext.Pieces
                .Where(P => P.IsArchived == false)
                .ToList();
        }

        // Returns a piece with the 
        public Piece GetPiece(int id)
        {
            try
            {
                var piece = _dataContext.Pieces
                    .FirstOrDefault(p => p.Id == id);

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

        public Set GetSet(int id)
        {
            try
            {
                var set = _dataContext.Sets
                    .Include(p => p.PiecesList!)
                    .ThenInclude(pl => pl.Piece)
                    .FirstOrDefault(s => s.Id == id);

                if (set == null)
                {
                    throw new Exception();
                }

                return set;
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
                .Include(s => s.Scenes)
                .Include(s => s.Tokens)
                    .ThenInclude(t => t.Piece)
                        .ThenInclude(p => p!.PieceType)
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

        // Gets scenes by session id.
        public List<Scene> GetScenes(int sessionId)
        {
            return _dataContext.Scenes
                .Where(s => s.SessionId == sessionId)
                .ToList();
        }

        // Adds a scene to the session.
        public Scene AddScene(Scene scene)
        {
            _dataContext.Scenes.Add(scene);
            _dataContext.SaveChanges();
            return scene;
        }

        // Clones a list of tokens to the new scene.
        public void CloneTokensToScene(List<int> tokenIds, int targetSceneId)
        {
            var tokens = _dataContext.Tokens
                .AsNoTracking()
                .Where(t => tokenIds.Contains(t.Id))
                .ToList();

            foreach (var t in tokens)
            {
                var newToken = new Token
                {
                    Name = t.Name,
                    PieceID = t.PieceID,
                    SessionId = t.SessionId,
                    SceneId = targetSceneId,
                    X = t.X,
                    Y = t.Y,
                    ZIndex = t.ZIndex,
                    Visibility = t.Visibility,
                    Notes = t.Notes
                };
                _dataContext.Tokens.Add(newToken);
            }
            _dataContext.SaveChanges();
        }

        // Deletes a scene from the session, including all of its tokens.
        public void DeleteScene(int sceneId)
        {
            var scene = _dataContext.Scenes
                .Include(s => s.Tokens)
                .FirstOrDefault(s => s.Id == sceneId);
            if (scene != null)
            {
                _dataContext.Tokens.RemoveRange(scene.Tokens);
                _dataContext.Scenes.Remove(scene);
                _dataContext.SaveChanges();
            }
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

        public Token AddToken(Token t)
        {
            try
            {
                _dataContext.Add(t);
                _dataContext.SaveChanges();
                return t;
            }
            catch (Exception e)
            {
                throw new Exception($"Error in adding Tpken: {e.Message}");
            }
        }

        public void UpdateTokenPositions(List<TokenUpdateModel> updates)
        {
            foreach (var update in updates)
            {
                if (int.TryParse(update.Id, out int realTokenId))
                {
                    var token = this._dataContext.Tokens.Find(realTokenId);
                    if (token != null)
                    {
                        token.X = update.X;
                        token.Y = update.Y;
                        token.ZIndex = update.zIndex;
                        token.Visibility = update.Visibility;
                        token.Name = update.Name;
                        token.Notes = update.Notes;

                        // Only update SceneId if a valid id is provided.
                        if (update.SceneId.HasValue && update.SceneId.Value > 0)
                        {
                            token.SceneId = update.SceneId.Value;
                        }

                        var session = this.GetSession(token.SessionId);
                        if (session != null) session.LastUpdated = DateTime.Now;
                    }
                }
                else if (update.Id != null && update.Id.StartsWith("temp-"))
                {
                    var piece = this.GetPiece(update.PieceId);
                    if (piece != null)
                    {
                        // Ensure we have a valid SceneId. Fallback sends us to first scene.
                        int? targetSceneId = (update.SceneId.HasValue && update.SceneId.Value > 0)
                                            ? update.SceneId.Value
                                            : this.GetScenes(update.SessionID).FirstOrDefault()?.Id;

                        this._dataContext.Tokens.Add(new Token
                        {
                            SessionId = update.SessionID,
                            SceneId = targetSceneId,
                            PieceID = update.PieceId,
                            Name = piece.Name,
                            X = update.X,
                            Y = update.Y,
                            ZIndex = update.zIndex,
                            Visibility = update.Visibility,
                            Notes = update.Notes
                        });

                        var session = this.GetSession(update.SessionID);
                        if (session != null) session.LastUpdated = DateTime.Now;
                    }
                }
            }
            this._dataContext.SaveChanges();
        }

        public int GetMaxZIndexForSession(int sessionId)
        {
            return this._dataContext.Tokens
                .Where(t => t.SessionId == sessionId)
                .Max(t => (int?)t.ZIndex) ?? 0;
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
            return this._dataContext.Pieces
                .Where(p => p.IsArchived == false)
                .Include(p => p.PieceType).ToList();
        }

        public Set CreateSet(Set set, List<int> pieceIds)
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

                return set;
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
            //return _dataContext.Sets
            //    .Include(s => s.PiecesList)
            //        .ThenInclude(ps => ps.Piece).Where(ps => ps.isarvhiced == false)
            //    .ToList();
            return this._dataContext.Sets
                .Include(s => s.PiecesList!.Where(ps => ps.Piece.IsArchived == false))
                .ThenInclude(ps => ps.Piece)
                .ToList();
        }

        /// <summary>
        /// Gets the sets with proper price conversions.
        /// AsNoTracking() is added on this query make sure to use updateSet() for set updates.
        /// </summary>
        /// <param name="currency">Currency to convert with.</param>
        /// <returns>Sets converted to users currency.</returns>
        public List<Set> GetAllSetsWithConvertedCurrency(string currency)
        {
            var sets = this._dataContext.Sets
                    .AsNoTracking()
                    .Include(s => s.PiecesList!.Where(ps => ps.Piece.IsArchived == false))
                    .ThenInclude(ps => ps.Piece)
                    .ToList();

            // Clone so that it does not affect the original sets.
            //var json = JsonSerializer.Serialize(sets, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve });
            //var clonedSets = JsonSerializer.Deserialize<List<Set>>(json, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve });
            //clonedSets = CurrencyConverter.GetStoreItemsPriceConverted(clonedSets, currency);
            sets = CurrencyConverter.GetStoreItemsPriceConverted(sets, currency);

            return sets;
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

        // Updates an existing user's information
        public void UpdateUser(User user)
        {
            try
            {
                this._dataContext.Users.Update(user);
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to update user {user.FirstName}, {e.Message}");
            }
        }

        // Updates a user's password
        public void UpdateUserPassword(User user, string newPasswordHash)
        {
            try
            {
                user.PasswordHash = newPasswordHash;
                this._dataContext.Users.Update(user);
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to update user password {user.FirstName}, {e.Message}");
            }
        }

        // Gets the user's id.
        public int GetUserId(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdString, out int userId) ? userId : 0;
        }

        // used to update a piece, specifically it's price and description, as name should stay the same.
        public Piece UpdatePiece(Piece piece)
        {
            try
            {
                this._dataContext.Pieces.Update(piece);
                this._dataContext.SaveChanges();
                return piece;
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating piece {piece.Id}: {e.Message}");
            }
        }

        public Set UpdateSet(Set set)
        {
            try
            {
                // First get the old piecesets for that set and delete them.
                List<PieceSets> oldPieceSets = this._dataContext.PieceSets
                    .Where(ps => ps.SetId == set.Id)
                    .ToList();

                foreach (PieceSets oldPieceSet in oldPieceSets)
                {
                    this._dataContext.Remove(oldPieceSet);
                }

                // Now we can add the new piecesets.
                foreach (PieceSets newPieceSets in set.PiecesList)
                {
                    this._dataContext.PieceSets.Add(newPieceSets);
                }

                this._dataContext.Update(set);

                this._dataContext.SaveChanges();

                return set;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update set\n{ex.Message}");
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
                return this._dataContext.UserPieces
                    .Where(up => up.UserId == id)
                    .Include(up => up.Piece)
                    .ThenInclude(p => p.PieceType)
                    .Select(up => up.Piece)
                    .ToList();
                // We do not run the not archived check here since user bought piece before archival.
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured getting user pieces User likely does not exist, {ex.Message}");
            }
        }

        public List<Piece> GetDeletedPieces()
        {
            return this._dataContext.Pieces
                .Where(p => p.IsArchived == true)
                .ToList();
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

        public void ClearSessionTokens(int sessionId, int sceneId)
        {
            var sessionTokens = this._dataContext.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p!.PieceType)
                .Where(t => t.SessionId == sessionId && t.SceneId == sceneId && t.Piece!.PieceType!.Name != "Map")
                .ToList();

            this._dataContext.Tokens.RemoveRange(sessionTokens);

            var session = this.GetSession(sessionId);
            if (session != null) session.LastUpdated = DateTime.Now;

            this._dataContext.SaveChanges();
        }

        // Updates the session's map. (adding a map piece to the session or updating the session's current scene.)
        public string UpdateSessionMap(int sessionId, int pieceId, int? sceneId = null)
        {
            var piece = this._dataContext.Pieces.Include(p => p.PieceType).FirstOrDefault(p => p.Id == pieceId);
            if (piece == null || piece.PieceType!.Name != "Map") throw new Exception("Invalid piece or piece is not a map");

            if (!sceneId.HasValue)
            {
                var firstScene = this._dataContext.Scenes.FirstOrDefault(s => s.SessionId == sessionId);
                if (firstScene == null)
                {
                    firstScene = new Scene { SessionId = sessionId, Name = "Default Scene" };
                    this._dataContext.Scenes.Add(firstScene);
                    this._dataContext.SaveChanges();
                }
                sceneId = firstScene.Id;
            }

            var mapToken = this._dataContext.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p!.PieceType)
                .FirstOrDefault(t => t.SessionId == sessionId && t.SceneId == sceneId && t.Piece!.PieceType!.Name == "Map");

            if (mapToken != null)
            {
                mapToken.PieceID = pieceId;
                mapToken.Name = piece.Name;
            }
            else
            {
                this._dataContext.Tokens.Add(new Token
                {
                    SessionId = sessionId,
                    SceneId = sceneId,
                    PieceID = pieceId,
                    Name = piece.Name,
                    X = 0,
                    Y = 0,
                    ZIndex = 0,
                    Visibility = true
                });
            }

            var session = this.GetSession(sessionId);
            if (session != null) session.LastUpdated = DateTime.Now;

            this._dataContext.SaveChanges();

            return piece.ImagePath;
        }

        // Deletes a piece by Id.
        public void DeletePiece(int id)
        {
            try
            {
                var piece = this._dataContext.Pieces.FirstOrDefault(p => p.Id == id);

                piece!.IsArchived = true;

                this.UpdatePiece(piece);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to archive piece\n${ex.Message}");
            }
        }

        // Restores a piece by Id.
        public void RestorePiece(int id)
        {
            try
            {
                var piece = this._dataContext.Pieces.FirstOrDefault(p => p.Id == id);

                piece!.IsArchived = false;

                this.UpdatePiece(piece);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to restore piece {id} Likely does not exist\n{ex.Message}");
            }

        }

        public CartItem GetCartItem(int userId, int pieceId)
        {
            try
            {
                return this._dataContext.CartItems
                    .Where(ci => ci.UserId == userId)
                    .Where(ci => ci.PieceId == pieceId)
                    .Where(ci => ci.IsArchived == false)
                    .Where(ci => ci.Piece!.IsArchived == false)
                    .Include(ci => ci.Piece)
                    .FirstOrDefault()!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get piece\n{ex.Message}");
            }
        }

        public CartItemSet GetCartItemSet(int userId, int setId)
        {
            try
            {
                return this._dataContext.CartItemSets
                    .Where(cis => cis.UserId == userId)
                    .Where(cis => cis.SetId == setId)
                    .Where(cis => cis.IsArchived == false)
                    .Where(cis => cis.Set!.IsArchived == false)
                    .Include(cis => cis.Set)
                    .FirstOrDefault()!;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get set\n{ex.Message}");
            }
        }

        public List<CartItem> GetCartItemsNoTracking(int userId)
        {
            return this._dataContext.CartItems
                .AsNoTracking()
                .Where(ci => ci.UserId == userId)
                .Where(ci => ci.IsArchived == false)
                .Include(ci => ci.Piece)
                .ToList();
        }

        public List<CartItem> GetCartItems(int userId)
        {
            return this._dataContext.CartItems
                .Where(ci => ci.UserId == userId)
                .Where(ci => ci.IsArchived == false)
                .Include(ci => ci.Piece)
                .ToList();
        }

        public List<CartItemSet> GetCartItemsSetNoTracking(int userId)
        {
            return this._dataContext.CartItemSets
                .AsNoTracking()
                .Where(cis => cis.UserId == userId)
                .Where(cis => cis.IsArchived == false)
                .Include(cis => cis.Set)
                .ThenInclude(s => s.PiecesList)
                .ThenInclude(pl => pl.Piece)
                .ToList();
        }

        public List<CartItemSet> GetCartItemSets(int userId)
        {
            return this._dataContext.CartItemSets
                .Where(cis => cis.UserId == userId)
                .Where(cis => cis.IsArchived == false)
                .Include(cis => cis.Set)
                .ThenInclude(s => s.PiecesList)
                .ThenInclude(pl => pl.Piece)
                .ToList();
        }

        public void AddCartItem(int userId, int pieceId)
        {
            // Check if the user has an archived cartItem of same pieceId
            CartItem userCartItem = this._dataContext.CartItems
                .Where(ci => ci.UserId == userId)
                .Where(ci => ci.IsArchived == true)
                .Where(ci => ci.PieceId == pieceId)
                .FirstOrDefault()!;

            // Also check if user has cartItem in cart since in our app. you can only buy things once.
            CartItem alreadyInCart = this._dataContext.CartItems
                .Where(ci => ci.UserId == userId)
                .Where(ci => ci.IsArchived == false)
                .Where(ci => ci.PieceId == pieceId)
                .FirstOrDefault()!;
            if (alreadyInCart != null)
            {
                return;
            }

            if (userCartItem != null)
            {
                userCartItem.IsArchived = false;
                this._dataContext.Update(userCartItem);
                this._dataContext.SaveChanges();
            }
            else
            {
                User user = this._dataContext.Users.FirstOrDefault(u => u.Id == userId)!;
                if (user != null)
                {
                    CartItem newCartItem = new()
                    {
                        PieceId = pieceId,
                        UserId = userId,
                        IsArchived = false
                    };

                    this._dataContext.Add(newCartItem);
                    this._dataContext.SaveChanges();
                }
            }
        }

        public void AddSetCartItem(int userId, int setId)
        {
            // Check if the user has an archived cartItem of same pieceId
            CartItemSet userSetItem = this._dataContext.CartItemSets
                .Where(cis => cis.UserId == userId)
                .Where(cis => cis.IsArchived == true)
                .Where(cis => cis.SetId == setId)
                .FirstOrDefault()!;

            // Also check if user already has this set in the cart they can only buy things once.
            CartItemSet alreadyInCart = this._dataContext.CartItemSets
                .Where(cis => cis.UserId == userId)
                .Where(cis => cis.IsArchived == false)
                .Where(cis => cis.SetId == setId)
                .FirstOrDefault()!;
            if (alreadyInCart != null)
            {
                return;
            }


            if (userSetItem != null)
            {
                userSetItem.IsArchived = false;
                this._dataContext.Update(userSetItem);
                this._dataContext.SaveChanges();
            }
            else
            {
                User user = this._dataContext.Users.FirstOrDefault(u => u.Id == userId)!;
                if (user != null)
                {
                    CartItemSet newCartItemSet = new()
                    {
                        SetId = setId,
                        UserId = userId,
                        IsArchived = false
                    };

                    this._dataContext.Add(newCartItemSet);
                    this._dataContext.SaveChanges();
                }
            }
        }

        public void DeleteCartItem(int userId, int pieceId)
        {
            var userCartItem = this.GetCartItem(userId, pieceId);
            userCartItem.IsArchived = true;
            this._dataContext.Update(userCartItem);
            this._dataContext.SaveChanges();
        }

        public void DeleteSetCartItem(int userId, int setId)
        {
            var userCartSetItem = this.GetCartItemSet(userId, setId);
            userCartSetItem.IsArchived = true;
            this._dataContext.Update(userCartSetItem);
            this._dataContext.SaveChanges();
        }

        // Checks out the user's cart, adding all pieces to their collection and archiving the cart items.
        public void CheckoutCart(int userId)
        {
            try
            {
                // First get the users cart items and cart item sets.
                var cartItems = this.GetCartItems(userId);
                var cartSets = this.GetCartItemSets(userId);

                // We can iterate over a list of pieces, then add it to user cart.
                List<Piece> pieces = new();

                foreach (CartItem p in cartItems)
                {
                    if (p.Piece != null)
                    {
                        if (pieces.Contains(p.Piece))
                            continue;

                        pieces.Add(p.Piece);
                    }

                    // Now archive/remove the cart item.
                    p.IsArchived = true;
                }
                foreach (CartItemSet cis in cartSets)
                {
                    // If the set or pieces list is null for some reason just go to next iteration.
                    if (cis.Set == null || cis.Set.PiecesList == null)
                        continue;

                    // Now for each set in the cart iterate over the pieces list and add to the pieces list.
                    foreach (PieceSets p in cis.Set.PiecesList)
                    {
                        if (p.Piece != null)
                        {
                            // If the pieces already contains the piece, continue.
                            if (pieces.Contains(p.Piece))
                                continue;

                            pieces.Add(p.Piece);
                        }
                    }

                    // Now archive/remove the cart item.
                    cis.IsArchived = true;
                }

                // Now get the users current pieces...
                List<Piece> usersCurrentPieces = this.GetUserPieces(userId);

                // Now check to see what pieces user already owns and remove that off of the piece list made from reading user cart.
                pieces = pieces.Where(p => !usersCurrentPieces.Any(ucp => ucp.Id == p.Id)).ToList();

                // NOW we can add to the UserPieces table.
                foreach (Piece p in pieces)
                {
                    this._dataContext.UserPieces.Add(new UserPieces { PieceId = p.Id, UserId = userId });
                }

                this._dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to checkout cart for user {userId}: {ex.Message}");
            }
        }

        public void AddSale(Sale sale)
        {
            foreach (SaleLine sl in sale.SaleLines)
            {
                // So EF does not freak out.
                sl.Piece = null;
                sl.Set = null;
            }

            this._dataContext.Sales.Add(sale);
            this._dataContext.SaveChanges();
        }

        public List<Sale> GetUserSales(int userId)
        {
            return this._dataContext.Sales
                .AsNoTracking()
                .Where(sa => sa.UserID == userId)
                .Include(sa => sa.SaleLines)
                .ThenInclude(sl => sl.Piece)
                .ToList();
        }

        public List<PurchaseStatsViewModel> GetPiecePurchaseStats()
        {
            return _dataContext.SaleLines
                .Include(sl => sl.Piece)
                .ThenInclude(p => p.PieceType)
                .Where(pi => pi.SetID == null) // I would like the ones that are not in a set.
                .GroupBy(sl => sl.PieceID)
                .Select(g => new PurchaseStatsViewModel
                {
                    Piece = g.First().Piece,
                    TotalPurchased = g.Count(),
                    PurchasedAmountTotal = g.Sum(g => g.Price)
                })
                .OrderByDescending(x => x.TotalPurchased)
                .ToList();
        }

        // Returns most purchased pieces (not in sets) for the given user, excluding pieces the user already owns.
        // Important: the returned Piece list is based on the Pieces table (not SaleLines) and uses purchase counts from SaleLines.
        public List<PurchaseStatsViewModel> GetTopUnownedPiecePurchaseStats(int userId)
        {
            var ownedPieceIds = _dataContext.UserPieces
                .Where(up => up.UserId == userId)
                .Select(up => up.PieceId)
                .ToList();

            // Purchase counts come from SaleLines (pieces not in sets).
            var purchaseStats = _dataContext.SaleLines
                .Where(sl => sl.SetID == null)
                .Where(sl => sl.PieceID.HasValue)
                .GroupBy(sl => sl.PieceID!.Value)
                .Select(g => new
                {
                    PieceId = g.Key,
                    TotalPurchased = g.Count(),
                    PurchasedAmountTotal = g.Sum(x => x.Price)
                });

            // Pieces come from Pieces table; we left-join purchaseStats so unpurchased (0) pieces can still show.
            var query = _dataContext.Pieces
                .Where(p => p.IsArchived == false)
                .Where(p => !ownedPieceIds.Contains(p.Id))
                .GroupJoin(
                    purchaseStats,
                    p => p.Id,
                    s => s.PieceId,
                    (p, stats) => new { Piece = p, Stats = stats.FirstOrDefault() })
                .Select(x => new PurchaseStatsViewModel
                {
                    Piece = x.Piece,
                    TotalPurchased = x.Stats == null ? 0 : x.Stats.TotalPurchased,
                    PurchasedAmountTotal = x.Stats == null ? 0 : x.Stats.PurchasedAmountTotal
                })
                .OrderByDescending(x => x.TotalPurchased);

            return query.ToList();
        }



        public List<SetStatsViewModel> GetSetPurchaseStats()
        {
            return _dataContext.SaleLines
                .Where(sl => sl.SetID != null || sl.SetID > 0)
                .Include(sl => sl.Set)
                .ThenInclude(s => s.PiecesList)
                .ThenInclude(pl => pl.Piece)
                .GroupBy(sl => sl.SetID)
                .Select(g => new SetStatsViewModel
                {
                    Set = g.First().Set,
                    // Count unique SaleIDs associated with this SetID
                    TotalPurchased = g.Select(sl => sl.SaleID).Distinct().Count(),

                    PurchasedAmountTotal = g.First().Set.Price * g.Select(sl => sl.SaleID).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalPurchased)
                .ToList();
        }

        public List<SaleLine> GetAllSaleLines()
        {
            return this._dataContext.SaleLines
                .Include(sl => sl.Piece)
                .Include(sl => sl.Set)
                .ToList();
        }

        public bool UserOwnsPiece(int userId, int PieceId)
        {
            var piece = this._dataContext.UserPieces
                .Where(up => up.PieceId == PieceId)
                .Where(up => up.UserId == userId)
                .FirstOrDefault();

            return piece != null;
        }

        /// <summary>
        /// Gets piece usage statistics, optionally filtered by piece type.
        /// Counts total tokens per piece across non-archived sessions.
        /// </summary>
        /// <param name="pieceTypeId">Optional PieceType ID to filter; null for all.</param>
        /// <returns>Ordered list of PieceUsageStatsViewModel (Piece, TotalUsed).</returns>
        public List<PieceUsageStatDto> GetPieceUsageStats(int? pieceTypeId = null)
        {
            return _dataContext.Tokens
                .Include(t => t.Piece)
                .ThenInclude(p => p.PieceType)
                .Include(t => t.Session)
                .Where(t => t.Session != null && !t.Session!.IsArchived && (!pieceTypeId.HasValue || t.Piece!.PieceTypeID == pieceTypeId.Value))
                .GroupBy(t => t.PieceID)
                .Select(g => new PieceUsageStatDto
                {
                    PieceId = g.Key,
                    PieceName = g.First().Piece.Name ?? "",
                    ImagePath = g.First().Piece.ImagePath ?? "/images/default.png",
                    PieceTypeName = g.First().Piece.PieceType.Name ?? "",
                    TotalUsed = g.Count()
                })
                .OrderByDescending(x => x.TotalUsed)
                .ToList();
        }

        public int GetTotalTokens()
        {
            return _dataContext.Tokens
                .Include(t => t.Session)
                .Where(t => t.Session != null && !t.Session!.IsArchived)
                .Count();
        }
    }
}


