import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api, { API_HOST } from '../services/api';

const CompletionStatuses = [
  { value: 0, label: 'Not Started', badgeClass: 'badge-status-notstarted' },
  { value: 1, label: 'Playing', badgeClass: 'badge-status-playing' },
  { value: 2, label: 'On Hold', badgeClass: 'badge-status-onhold' },
  { value: 3, label: 'Completed', badgeClass: 'badge-status-completed' },
  { value: 4, label: 'Dropped', badgeClass: 'badge-status-dropped' },
  { value: 5, label: '100% Completed', badgeClass: 'badge-status-completed100' },
  { value: 6, label: 'Replaying', badgeClass: 'badge-status-replaying' }
];

const GameDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchGame = async () => {
      setLoading(true);
      try {
        const response = await api.get(`/games/${id}`);
        setGame(response.data);
      } catch (err) {
        setError(err.message || 'Failed to retrieve game details.');
      } finally {
        setLoading(false);
      }
    };
    fetchGame();
  }, [id]);

  const deleteGame = async () => {
    if (!window.confirm('Are you sure you want to delete this game from your library?')) return;
    try {
      await api.delete(`/games/${id}`);
      navigate('/library');
    } catch (err) {
      alert('Delete failed: ' + err.message);
    }
  };

  const getStatusLabel = (statusValue) => {
    const statusObj = CompletionStatuses.find(s => s.value === statusValue);
    return statusObj ? statusObj.label : 'Unknown';
  };

  const getStatusBadgeClass = (statusValue) => {
    const statusObj = CompletionStatuses.find(s => s.value === statusValue);
    return statusObj ? statusObj.badgeClass : 'badge-status-notstarted';
  };

  // Extract YouTube ID for trailer embedding
  const getYoutubeEmbedUrl = (url) => {
    if (!url) return null;
    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
    const match = url.match(regExp);
    if (match && match[2].length === 11) return `https://www.youtube.com/embed/${match[2]}`;
    return null;
  };

  if (loading) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border" style={{ color: 'var(--accent-purple)' }} role="status"></div>
      </div>
    );
  }

  if (error || !game) {
    return (
      <div className="alert-theme-danger p-4 rounded-3">
        <h4 className="mb-2">Error loading details</h4>
        <p className="mb-3">{error || 'Game not found.'}</p>
        <Link to="/library" className="btn btn-premium-outline btn-sm">Back to Library</Link>
      </div>
    );
  }

  const embedUrl = getYoutubeEmbedUrl(game.trailerUrl || game.gameplayUrl || game.youtubeLinks);

  return (
    <div className="container-fluid py-2 fade-in">
      {/* Top Banner Header block */}
      <div 
        className="position-relative rounded-4 overflow-hidden mb-4 d-flex align-items-end p-4 p-md-5"
        style={{
          height: '350px',
          background: game.banner 
            ? `linear-gradient(to top, rgba(11, 14, 20, 0.95), rgba(11, 14, 20, 0.3)), url(${API_HOST}${game.banner}) center/cover no-repeat`
            : 'linear-gradient(135deg, var(--bg-tertiary) 0%, var(--bg-secondary) 100%)'
        }}
      >
        <div className="d-flex flex-column flex-md-row gap-4 align-items-md-end w-100 z-1">
          {/* Cover image thumbnail */}
          <div className="rounded-3 overflow-hidden shadow-lg flex-shrink-0" style={{ width: '135px', height: '180px', border: '3px solid rgba(255,255,255,0.2)' }}>
            <img 
              src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
              alt={game.title} 
              className="w-100 h-100 object-fit-cover"
              onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
            />
          </div>
          
          {/* Always white text over the dark banner overlay */}
          <div className="flex-grow-1" style={{ color: '#ffffff' }}>
            <div className="d-flex flex-wrap align-items-center gap-2 mb-2">
              <span className={`badge ${getStatusBadgeClass(game.completionStatus)} px-3 py-2 fs-6`}>
                {getStatusLabel(game.completionStatus)}
              </span>
              {game.ownGame && <span className="badge px-3 py-2" style={{ background: 'rgba(22,163,74,0.25)', color: '#4ade80', border: '1px solid rgba(74,222,128,0.4)' }}>Owned</span>}
              {game.wishlist && <span className="badge px-3 py-2" style={{ background: 'rgba(59,130,246,0.25)', color: '#60a5fa', border: '1px solid rgba(96,165,250,0.4)' }}>Wishlisted</span>}
              {game.backlog && <span className="badge px-3 py-2" style={{ background: 'rgba(234,179,8,0.25)', color: '#fbbf24', border: '1px solid rgba(251,191,36,0.4)' }}>Backlog</span>}
            </div>

            <h1 className="display-4 fw-bold display-font mb-2" style={{ color: '#ffffff' }}>{game.title}</h1>
            <p style={{ color: 'rgba(255,255,255,0.65)' }} className="mb-0">
              {game.platforms.map(p => p.name).join(' | ') || 'No Platform'} 
              {game.releaseDate && ` (Released: ${new Date(game.releaseDate).getFullYear()})`}
            </p>
          </div>

          <div className="d-flex gap-2 ms-md-auto align-self-start align-self-md-end">
            <Link to={`/games/edit/${game.id}`} className="btn btn-premium-purple d-flex align-items-center gap-2">
              <i className="bi bi-pencil-square"></i>
              <span>Edit Game</span>
            </Link>
            <button className="btn btn-sm" style={{ background: 'rgba(220,38,38,0.2)', border: '1px solid rgba(220,38,38,0.5)', color: '#f87171' }} onClick={deleteGame} title="Delete Game">
              <i className="bi bi-trash3-fill"></i>
            </button>
          </div>
        </div>
      </div>

      <div className="row g-4">
        {/* Left Column: Details, description, reviews */}
        <div className="col-12 col-lg-8">
          {/* Basic Info panel */}
          <div className="glass-panel p-4 mb-4">
            <h5 className="display-font pb-2 mb-3" style={{ borderBottom: '1px solid var(--border-subtle)' }}>About the Game</h5>
            {game.originalTitle && <p className="text-muted small"><strong style={{ color: 'var(--text-secondary)' }}>Original Title:</strong> {game.originalTitle}</p>}
            {game.alternateTitles && <p className="text-muted small"><strong style={{ color: 'var(--text-secondary)' }}>Alternate Titles:</strong> {game.alternateTitles}</p>}
            <p className="leading-relaxed" style={{ color: 'var(--text-secondary)', whiteSpace: 'pre-wrap' }}>
              {game.description || 'No description provided.'}
            </p>
            
            {game.notes && (
              <>
                <h6 className="display-font mt-4 mb-2">General Notes</h6>
                <div className="p-3 rounded-3 small" style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-color)', color: 'var(--text-secondary)' }}>
                  {game.notes}
                </div>
              </>
            )}

            {game.personalNotes && (
              <>
                <h6 className="display-font mt-4 mb-2">Personal Comments</h6>
                <div className="p-3 rounded-3 small" style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-color)', color: 'var(--text-secondary)' }}>
                  {game.personalNotes}
                </div>
              </>
            )}
          </div>

          {/* YouTube video panel */}
          {embedUrl && (
            <div className="glass-panel p-4 mb-4">
              <h5 className="display-font pb-2 mb-3" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Game Trailer &amp; Media</h5>
              <div className="ratio ratio-16x9 rounded-3 overflow-hidden" style={{ border: '1px solid var(--border-color)' }}>
                <iframe 
                  src={embedUrl} 
                  title="Game Video Trailer" 
                  allowFullScreen
                  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                ></iframe>
              </div>
            </div>
          )}
        </div>

        {/* Right Column: Ratings, Purchase, Metadata lookups */}
        <div className="col-12 col-lg-4">
          
          {/* Game Stats & Ratings */}
          <div className="glass-panel p-4 mb-4">
            <h5 className="display-font pb-2 mb-3" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Time &amp; Ratings</h5>
            <div className="d-flex align-items-center gap-3 mb-4">
              <div className="kpi-icon-wrapper kpi-purple flex-shrink-0">
                <i className="bi bi-clock-history"></i>
              </div>
              <div>
                <div className="kpi-card-label">Time Played</div>
                <h4 className="mb-0 kpi-card-value display-font">{game.hoursPlayed} Hours</h4>
              </div>
            </div>

            <div className="row g-2 mb-3 text-center">
              <div className="col-4">
                <div className="p-2 rounded-3" style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-subtle)' }}>
                  <div className="kpi-card-label" style={{ fontSize: '0.7rem' }}>Personal</div>
                  <div className="fw-bold display-font" style={{ color: '#ca8a04' }}>
                    {game.personalRating ? <><i className="bi bi-star-fill me-1"></i>{game.personalRating}</> : '-'}
                  </div>
                </div>
              </div>
              <div className="col-4">
                <div className="p-2 rounded-3" style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-subtle)' }}>
                  <div className="kpi-card-label" style={{ fontSize: '0.7rem' }}>Community</div>
                  <div className="fw-bold display-font" style={{ color: '#3b82f6' }}>
                    {game.communityRating ? <><i className="bi bi-people-fill me-1"></i>{game.communityRating}</> : '-'}
                  </div>
                </div>
              </div>
              <div className="col-4">
                <div className="p-2 rounded-3" style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-subtle)' }}>
                  <div className="kpi-card-label" style={{ fontSize: '0.7rem' }}>Critic Score</div>
                  <div className="fw-bold display-font" style={{ color: 'var(--accent-green)' }}>
                    {game.criticRating ? <><i className="bi bi-award-fill me-1"></i>{game.criticRating}</> : '-'}
                  </div>
                </div>
              </div>
            </div>

            <div className="spec-row"><span className="spec-label">Metacritic Score:</span><span className="spec-value">{game.metacriticScore ? `${game.metacriticScore}/100` : '-'}</span></div>
            <div className="spec-row"><span className="spec-label">OpenCritic Score:</span><span className="spec-value">{game.openCriticScore ? `${game.openCriticScore}/100` : '-'}</span></div>
            <div className="spec-row"><span className="spec-label">Steam Deck Status:</span><span className="spec-value">{game.steamDeckCompatibility || 'Unknown'}</span></div>
            <div className="spec-row"><span className="spec-label">Achievements:</span><span className="spec-value">{game.achievementCount || 0} unlocked</span></div>
            <div className="spec-row" style={{ borderBottom: 'none' }}><span className="spec-label">DLCs &amp; Expansions:</span><span className="spec-value">{game.dlcCount + game.expansionCount} owned</span></div>
          </div>

          {/* Ownership & Purchase Info */}
          <div className="glass-panel p-4 mb-4">
            <h5 className="display-font pb-2 mb-3" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Purchase Details</h5>
            <div className="spec-row">
              <span className="spec-label">Format Copy:</span>
              <span className="spec-value" style={{ fontWeight: 400, color: 'var(--text-secondary)' }}>
                {game.physicalCopy ? 'Physical' : ''}
                {game.physicalCopy && game.digitalCopy ? ' & ' : ''}
                {game.digitalCopy ? 'Digital' : ''}
                {!game.physicalCopy && !game.digitalCopy ? '-' : ''}
              </span>
            </div>
            <div className="spec-row"><span className="spec-label">Purchase Date:</span><span className="spec-value" style={{ fontWeight: 400, color: 'var(--text-secondary)' }}>{game.purchaseDate ? new Date(game.purchaseDate).toLocaleDateString() : '-'}</span></div>
            <div className="spec-row"><span className="spec-label">Paid Price:</span><span className="spec-value" style={{ fontWeight: 400, color: 'var(--text-secondary)' }}>{game.purchasePrice ? `${game.purchasePrice} ${game.currency || 'USD'}` : '-'}</span></div>
            <div className="spec-row"><span className="spec-label">Storefront:</span><span className="spec-value" style={{ fontWeight: 400, color: 'var(--text-secondary)' }}>{game.storePurchasedFrom || '-'}</span></div>
            <div className="spec-row" style={{ borderBottom: 'none' }}><span className="spec-label">Purchase Region:</span><span className="spec-value" style={{ fontWeight: 400, color: 'var(--text-secondary)' }}>{game.purchaseRegion || '-'}</span></div>
          </div>

          {/* Game Taxonomies metadata */}
          <div className="glass-panel p-4 mb-4">
            <h5 className="display-font pb-2 mb-3" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Relations &amp; Metadata</h5>
            
            {[
              { label: 'Developers', items: game.developers },
              { label: 'Publishers', items: game.publishers },
              { label: 'Storefront Services', items: game.digitalServices },
              { label: 'Genres', items: game.genres },
              { label: 'Themes', items: game.themes },
              { label: 'User Tags', items: game.tags },
            ].map(({ label, items }) => (
              <div key={label} className="mb-3">
                <div className="kpi-card-label mb-1" style={{ fontSize: '0.75rem' }}>{label}:</div>
                <div className="d-flex gap-1 flex-wrap">
                  {items?.length > 0 
                    ? items.map(item => <span key={item.id} className="meta-tag">{item.name}</span>)
                    : <span className="text-muted small">-</span>
                  }
                </div>
              </div>
            ))}
          </div>

        </div>
      </div>
    </div>
  );
};

export default GameDetail;
