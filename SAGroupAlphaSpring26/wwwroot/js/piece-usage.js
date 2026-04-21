async function filterPieces(typeId) {
    const tableBody = document.getElementById('statsTableBody');
    const title = document.getElementById('filterTitle');
    tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Loading...</td></tr>';
    
    try {
        const response = await fetch(`/Admin/PieceUsageJson?pieceTypeId=${typeId || ''}`, { credentials: 'same-origin' });
        const data = await response.json();
        console.log('JSON data:', data);
        
        // Update title
        const filterName = typeId ? data.types.find(t => t.Id === parseInt(typeId))?.Name || 'Unknown' : 'All Types';
        title.textContent = `Piece Usage Statistics ${typeId ? '- ' + filterName : ''}`;
        
        // Update table
        let html = '';
        if (data.stats && data.stats.length > 0) {
            data.stats.forEach(stat => {
                const percent = data.totalTokens > 0 ? (stat.totalUsed / data.totalTokens * 100).toFixed(1) : 0;
                html += `
                    <tr class="saBackground">
                        <td><img src="${stat.imagePath || '/images/default.png'}" alt="${stat.pieceName}" style="width: 50px; height: 50px; object-fit: cover;" /></td>
                        <td><a href="/Home/Piece?id=${stat.pieceId}">${stat.pieceName || 'Unknown'}</a></td>
                        <td>${stat.pieceTypeName || ''}</td>
                        <td><strong>${stat.totalUsed}</strong></td>
                        <td><strong>${percent}%</strong></td>
                    </tr>`;
            });
        } else {
            html = '<tr><td colspan="5" class="text-center">No piece usage found.</td></tr>';
        }
        tableBody.innerHTML = html;
    } catch (error) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Error loading data: ${error.message}</td></tr>';
        console.error('Filter error:', error);
    }
}
