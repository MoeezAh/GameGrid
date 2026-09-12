import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api, { API_HOST } from '../services/api';
import { useAuth } from '../context/AuthContext';

const GameDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { hasPermission } = useAuth();

  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showAddModal, setShowAddModal] = useState(false);
  const [userSelectedPlatformIds, setUserSelectedPlatformIds] = useState([]);
  const [addingToLibrary, setAddingToLibrary] = useState(false);
  const [actionSuccess, setActionSuccess] = useState(null);

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

  useEffect(() => {
    fetchGame();
  }, [id]);

  const deleteGame = async () => {
    if (!window.confirm('Are you sure you want to delete this game from the central catalog?')) return;
    try {
      await api.delete(`/games/${id}`);
      navigate('/catalog');
    } catch (err) {
      alert('Delete failed: ' + (err.response?.data?.message || err.message));
    }
  };

  const handleToggleUserPlatform = (platformId) => {
    if (userSelectedPlatformIds.includes(platformId)) {
      setUserSelectedPlatformIds(userSelectedPlatformIds.filter(pid => pid !== platformId));
    } else {
      setUserSelectedPlatformIds([...userSelectedPlatformIds, platformId]);
    }
  };

  const handleAddToLibrary = async (e) => {
    e.preventDefault();
    setAddingToLibrary(true);
    try {
      await api.post('/libraries', {
        gameId: game.id,
        platformIds: userSelectedPlatformIds,
        ownGame: true
      });
      setShowAddModal(false);
      setActionSuccess(`'${game.title}' added to your library!`);
      fetchGame();
      setTimeout(() => setActionSuccess(null), 4000);
    } catch (err) {
      alert('Failed to add to library: ' + (err.response?.data?.message || err.message));
    } finally {
      setAddingToLibrary(false);
    }
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
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  if (error || !game) {
    return (
      <div className="alert alert-danger p-4 rounded-3">
        <h4 className="mb-2">Error loading details</h4>
        <p className="mb-3">{error || 'Game not found.'}</p>
        <Link to="/catalog" className="btn btn-outline-secondary btn-sm">Back to Catalog</Link>
      </div>
    );
  }

  const embedUrl = getYoutubeEmbedUrl(game.trailerUrl || game.gameplayUrl || game.youtubeLinks);

  return (
    <div className="container-fluid py-2 fade-in">
      {actionSuccess && (
        <div className="alert alert-success d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-check-circle-fill fs-5"></i>
          <div>{actionSuccess}</div>
        </div>
      )}

      {/* Top Banner Header block */}
      <div 
        className="position-relative rounded-4 overflow-hidden mb-4 d-flex align-items-end p-4 p-md-5"
        style={{
          minHeight: '350px',
          background: game.banner 
            ? `linear-gradient(to top, rgba(11, 14, 20, 0.95), rgba(11, 14, 20, 0.4)), url(${API_HOST}${game.banner}) center/cover no-repeat`
            : 'linear-gradient(135deg, var(--bg-tertiary) 0%, var(--bg-secondary) 100%)'
        }}
      >
        <div className="d-flex flex-column flex-md-row gap-4 align-items-md-end w-100 z-1">
          {/* Cover image thumbnail */}
          <div className="rounded-3 overflow-hidden shadow-lg flex-shrink-0" style={{ width: '135px', height: '180px', border: '3px solid rgba(255,255,255,0.2)' }}>
            <img 
              src={game.coverImage ? (game.coverImage.startsWith('http') ? game.coverImage : `${API_HOST}${game.coverImage}`) : 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80'} 
              alt={game.title} 
              className="w-100 h-100 object-fit-cover"
            />
          </div>
          
          <div className="flex-grow-1" style={{ color: '#ffffff' }}>
            <div className="d-flex flex-wrap align-items-center gap-2 mb-2">
              {game.isInUserLibrary ? (
                <span className="badge bg-primary px-3 py-2 fs-6">
                  <i className="bi bi-check-circle-fill me-1"></i> In Your Library
                </span>
              ) : (
                <span className="badge bg-secondary px-3 py-2 fs-6">Catalog Entry</span>
              )}
              {game.esrbRating && <span className="badge bg-dark-subtle text-body border px-2 py-1">ESRB: {game.esrbRating}</span>}
              {game.pegiRating && <span className="badge bg-dark-subtle text-body border px-2 py-1">PEGI: {game.pegiRating}</span>}
            </div>

            <h1 className="display-4 fw-bold display-font mb-2" style={{ color: '#ffffff' }}>{game.title}</h1>
            <p style={{ color: 'rgba(255,255,255,0.75)' }} className="mb-0">
              {game.platforms?.map(p => p.name).join(' | ') || 'No Platforms'} 
              {game.releaseDate && ` (Released: ${new Date(game.releaseDate).getFullYear()})`}
            </p>
          </div>

          <div className="d-flex gap-2 ms-md-auto align-self-start align-self-md-end">
            {!game.isInUserLibrary && (
              <button 
                className="btn btn-primary d-flex align-items-center gap-2"
                onClick={() => setShowAddModal(true)}
              >
                <i className="bi bi-plus-circle-fill"></i>
                <span>Add to My Library</span>
              </button>
            )}
            {hasPermission('Games.Update') && (
              <Link to={`/games/edit/${game.id}`} className="btn btn-outline-light d-flex align-items-center gap-2">
                <i className="bi bi-pencil-square"></i>
                <span>Edit Catalog</span>
              </Link>
            )}
            {hasPermission('Games.Delete') && (
              <button className="btn btn-outline-danger" onClick={deleteGame} title="Delete from Catalog">
                <i className="bi bi-trash3-fill"></i>
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="row g-4">
        {/* Left Column: Details, description, media */}
        <div className="col-12 col-lg-8">
          <div className="card custom-card border-0 p-4 mb-4">
            <h5 className="display-font pb-2 mb-3 border-bottom border-secondary-subtle">About the Game</h5>
            {game.originalTitle && <p className="text-secondary small"><strong>Original Title:</strong> {game.originalTitle}</p>}
            {game.alternateTitles && <p className="text-secondary small"><strong>Alternate Titles:</strong> {game.alternateTitles}</p>}
            <p className="leading-relaxed" style={{ whiteSpace: 'pre-wrap' }}>
              {game.description || 'No description provided.'}
            </p>
            
            {game.notes && (
              <>
                <h6 className="display-font mt-4 mb-2">Catalog Notes</h6>
                <div className="p-3 rounded bg-body-tertiary border small">
                  {game.notes}
                </div>
              </>
            )}
          </div>

          {/* YouTube Video Media */}
          {embedUrl && (
            <div className="card custom-card border-0 p-4 mb-4">
              <h5 className="display-font pb-2 mb-3 border-bottom border-secondary-subtle">Game Trailer &amp; Media</h5>
              <div className="ratio ratio-16x9 rounded overflow-hidden border">
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

        {/* Right Column: Ratings & Taxonomies */}
        <div className="col-12 col-lg-4">
          <div className="card custom-card border-0 p-4 mb-4">
            <h5 className="display-font pb-2 mb-3 border-bottom border-secondary-subtle">Ratings &amp; Scores</h5>
            <div className="row g-2 mb-3 text-center">
              <div className="col-6">
                <div className="p-2 rounded bg-body-tertiary border">
                  <div className="small text-secondary" style={{ fontSize: '0.7rem' }}>Community</div>
                  <div className="fw-bold fs-5 text-primary">
                    {game.communityRating ? <><i className="bi bi-people-fill me-1"></i>{game.communityRating}</> : '-'}
                  </div>
                </div>
              </div>
              <div className="col-6">
                <div className="p-2 rounded bg-body-tertiary border">
                  <div className="small text-secondary" style={{ fontSize: '0.7rem' }}>Critic Score</div>
                  <div className="fw-bold fs-5 text-success">
                    {game.criticRating ? <><i className="bi bi-award-fill me-1"></i>{game.criticRating}</> : '-'}
                  </div>
                </div>
              </div>
            </div>

            <div className="d-flex justify-content-between py-2 border-bottom border-secondary-subtle small">
              <span className="text-secondary">Metacritic:</span>
              <span className="fw-bold">{game.metacriticScore ? `${game.metacriticScore}/100` : '-'}</span>
            </div>
            <div className="d-flex justify-content-between py-2 border-bottom border-secondary-subtle small">
              <span className="text-secondary">OpenCritic:</span>
              <span className="fw-bold">{game.openCriticScore ? `${game.openCriticScore}/100` : '-'}</span>
            </div>
            <div className="d-flex justify-content-between py-2 border-bottom border-secondary-subtle small">
              <span className="text-secondary">Steam Deck:</span>
              <span className="fw-bold">{game.steamDeckCompatibility || 'Unknown'}</span>
            </div>
            <div className="d-flex justify-content-between py-2 small">
              <span className="text-secondary">Achievements:</span>
              <span className="fw-bold">{game.achievementCount || 0}</span>
            </div>
          </div>

          <div className="card custom-card border-0 p-4 mb-4">
            <h5 className="display-font pb-2 mb-3 border-bottom border-secondary-subtle">Metadata &amp; Taxonomy</h5>
            {[
              { label: 'Developers', items: game.developers },
              { label: 'Publishers', items: game.publishers },
              { label: 'Supported Platforms', items: game.platforms },
              { label: 'Digital Services', items: game.digitalServices },
              { label: 'Genres', items: game.genres },
              { label: 'Themes', items: game.themes },
              { label: 'Tags', items: game.tags },
            ].map(({ label, items }) => (
              <div key={label} className="mb-3">
                <div className="small text-secondary fw-bold mb-1" style={{ fontSize: '0.75rem' }}>{label}:</div>
                <div className="d-flex gap-1 flex-wrap">
                  {items?.length > 0 
                    ? items.map(item => <span key={item.id} className="badge bg-secondary-subtle text-secondary border">{item.name}</span>)
                    : <span className="text-secondary small fst-italic">None</span>
                  }
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Add To Library Modal */}
      {showAddModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className="bi bi-plus-circle me-2 text-primary"></i>
                  Add '{game.title}' to My Library
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowAddModal(false)}></button>
              </div>
              <form onSubmit={handleAddToLibrary}>
                <div className="modal-body p-4">
                  <p className="text-secondary small mb-3">
                    Select which platforms you own this game on:
                  </p>
                  <div className="d-flex flex-wrap gap-2 mb-3">
                    {game.platforms?.map((plat) => (
                      <button
                        key={plat.id}
                        type="button"
                        className={`btn btn-sm ${userSelectedPlatformIds.includes(plat.id) ? 'btn-primary' : 'btn-outline-secondary'}`}
                        onClick={() => handleToggleUserPlatform(plat.id)}
                      >
                        {userSelectedPlatformIds.includes(plat.id) && <i className="bi bi-check me-1"></i>}
                        {plat.name}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="modal-footer border-top border-secondary-subtle">
                  <button type="button" className="btn btn-secondary" onClick={() => setShowAddModal(false)} disabled={addingToLibrary}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary" disabled={addingToLibrary}>
                    {addingToLibrary ? 'Adding...' : 'Confirm Add to Library'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default GameDetail;
