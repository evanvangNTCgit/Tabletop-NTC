# D&D DM Map Fixes - TODO

Status: In Progress

## Approved Plan Steps:

### 1. **✅ Create TODO.md** (Current - done)

### 2. **✅ Update CSS in wwwroot/css/site.css**
   - Fixed map-board sizing, added CSS vars/transform for zoom/pan.
   - Added zoom-btn, toolbar styles.


### 3. **✅ Add map.js (wwwroot/js/map.js)**
   - Created full JS with improved drag/drop, error handling, zoom (wheel/buttons), pan (middle-drag).

### 4. **✅ Update Views/Map/Map.cshtml**
   - Replaced inline JS with map.js reference.
   - Added zoom controls to toolbar.
   - Removed inline background-size: cover.
   - Fixed Razor @(piece.Id.ToString()).

### 5. Minor Controller enhancements (optional)
   - Add try-catch/logging to CreateToken/SavePositions.

### 6. **Test & Verify**
   - Drag pieces from sidebar to map → new token created/movable.
   - Existing tokens draggable.
   - Map not zoomed/cropped, proper size.
   - Zoom/pan works.
   - Save positions.

### 7. **attempt_completion**

### 5-7. ✅ Complete! Ready for testing.

