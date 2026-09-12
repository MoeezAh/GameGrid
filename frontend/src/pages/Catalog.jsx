import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api, { API_HOST } from '../services/api';
import { useAuth } from '../context/AuthContext';

const Catalog = () => {
  const { hasPermission } = useAuth();
  const navigate = useNavigate();

  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(12);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  // Filters State
  const [search, setSearch] = useState('');
  const [selectedPlatforms, setSelectedPlatforms] = useState([]);
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [sortBy, setSortBy] = useState('title');
  const [sortOrder, setSortOrder] = useState('asc');

  // Metadata filter options list
  const [platforms, setPlatforms] = useState([]);
  const [genres, setGenres] = useState([]);

  // Add to Library Modal State
  const [showAddModal, setShowAddModal] = useState(false);
  const [targetGame, setTargetGame] = useState(null);
  const [userSelectedPlatformIds, setUserSelectedPlatformIds] = useState([]);
  const [userOwnershipStatus, setUserOwnershipStatus] = useState('own'); // 'own' | 'wishlist' | 'backlog'
  const [userPhysicalCopy, setUserPhysicalCopy] = useState(false);
  const [userDigitalCopy, setUserDigitalCopy] = useState(true);
  const [addingToLibrary, setAddingToLibrary] = useState(false);
  const [addModalError, setAddModalError] = useState(null);
  const [notificationMsg, setNotificationMsg] = useState(null);

  // Fetch Lookups for filter panels
  useEffect(() => {
    const loadLookups = async () => {
      try {
        const [pRes, gRes] = await Promise.all([
          api.get('/metadata/platforms/list'),
          api.get('/metadata/genres/list')
        ]);
        setPlatforms(pRes.data || []);
        setGenres(gRes.data || []);
      } catch (err) {
        console.error('Failed to load filter metadata', err);
      }
    };
    loadLookups();
  }, []);

  // Fetch Catalog Games
  const fetchCatalog = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams({
        pageNumber: page.toString(),
        pageSize: pageSize.toString(),
        sortBy,
        sortOrder
      });

      if (search) params.append('searchTerm', search);
      selectedPlatforms.forEach(id => params.append('platformIds', id.toString()));
      selectedGenres.forEach(id => params.append('genreIds', id.toString()));

      const response = await api.get(`/games?${params.toString()}`);
      const data = response.data;
      setGames(data.items || []);
      setTotalPages(data.totalPages || 0);
      setTotalCount(data.totalCount || 0);
    } catch (err) {
      console.error('Failed to load catalog games', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCatalog();
  }, [page, sortBy, sortOrder, selectedPlatforms, selectedGenres]);

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    setPage(1);
    fetchCatalog();
  };

  const handleTogglePlatformFilter = (id) => {
    setPage(1);
    setSelectedPlatforms(prev => 
      prev.includes(id) ? prev.filter(pId => pId !== id) : [...prev, id]
    );
  };

  const handleToggleGenreFilter = (id) => {
    setPage(1);
    setSelectedGenres(prev => 
      prev.includes(id) ? prev.filter(gId => gId !== id) : [...prev, id]
    );
  };

  const openAddToLibraryModal = (game) => {
    setTargetGame(game);
    // Preselect platforms that this game supports by finding their IDs if available
    setUserSelectedPlatformIds([]);
    setUserOwnershipStatus('own');
    setUserPhysicalCopy(false);
    setUserDigitalCopy(true);
    setAddModalError(null);
    setShowAddModal(true);
  };

  const handleToggleUserPlatform = (platformId) => {
    if (userSelectedPlatformIds.includes(platformId)) {
      setUserSelectedPlatformIds(userSelectedPlatformIds.filter(id => id !== platformId));
    } else {
      setUserSelectedPlatformIds([...userSelectedPlatformIds, platformId]);
    }
  };

  const handleSaveToLibrary = async (e) => {
    e.preventDefault();
    if (!targetGame) return;

    setAddingToLibrary(true);
    setAddModalError(null);

    const payload = {
      gameId: targetGame.id,
      platformIds: userSelectedPlatformIds,
      ownGame: userOwnershipStatus === 'own',
      wishlist: userOwnershipStatus === 'wishlist',
      backlog: userOwnershipStatus === 'backlog',
      physicalCopy: userPhysicalCopy,
      digitalCopy: userDigitalCopy
    };

    try {
      await api.post('/libraries', payload);
      setShowAddModal(false);
      setNotificationMsg(`'${targetGame.title}' was added to your personal library!`);
      fetchCatalog(); // Refresh catalog to show 'In Library'
      setTimeout(() => setNotificationMsg(null), 4000);
    } catch (err) {
      setAddModalError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to add game to library.');
    } finally {
      setAddingToLibrary(false);
    }
  };

  const getImageUrl = (url) => {
    if (!url) return 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80';
    if (url.startsWith('http')) return url;
    return `${API_HOST}${url}`;
  };

  return (
    <div className="container-fluid p-0">
      {/* Header Banner */}
      <div className="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
        <div>
          <h1 className="h3 mb-1 fw-bold text-gradient display-font">
            <i className="bi bi-compass-fill me-2 text-primary"></i>
            Central Game Catalog
          </h1>
          <p className="text-secondary mb-0">
            Browse the authoritative gaming catalog. Select existing games to add to your personal library.
          </p>
        </div>
        <div className="d-flex gap-2">
          <Link to="/game-requests" className="btn btn-outline-primary d-flex align-items-center gap-2">
            <i className="bi bi-send-plus"></i>
            <span>Request a Game</span>
          </Link>
          {hasPermission('Games.Create') && (
            <Link to="/games/add" className="btn btn-primary d-flex align-items-center gap-2">
              <i className="bi bi-plus-circle-fill"></i>
              <span>Add to Catalog</span>
            </Link>
          )}
        </div>
      </div>

      {/* Notifications */}
      {notificationMsg && (
        <div className="alert alert-success d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-check-circle-fill fs-5"></i>
          <div>{notificationMsg}</div>
        </div>
      )}

      {/* Search & Filter Bar */}
      <div className="card border-0 shadow-sm custom-card mb-4">
        <div className="card-body p-3">
          <form onSubmit={handleSearchSubmit} className="row g-2 align-items-center">
            <div className="col-md-5">
              <div className="input-group">
                <span className="input-group-text bg-body-tertiary border-secondary-subtle">
                  <i className="bi bi-search"></i>
                </span>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Search catalog games by title..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                />
              </div>
            </div>

            <div className="col-md-3">
              <select
                className="form-select"
                value={`${sortBy}_${sortOrder}`}
                onChange={(e) => {
                  const [sb, so] = e.target.value.split('_');
                  setSortBy(sb);
                  setSortOrder(so);
                }}
              >
                <option value="title_asc">Title (A-Z)</option>
                <option value="title_desc">Title (Z-A)</option>
                <option value="releasedate_desc">Release Date (Newest)</option>
                <option value="releasedate_asc">Release Date (Oldest)</option>
                <option value="criticrating_desc">Critic Rating (Highest)</option>
                <option value="metacritic_desc">Metacritic Score (Highest)</option>
              </select>
            </div>

            <div className="col-md-4 d-flex justify-content-md-end gap-2">
              <button type="submit" className="btn btn-primary px-4">
                Search
              </button>
              {(search || selectedPlatforms.length > 0 || selectedGenres.length > 0) && (
                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  onClick={() => {
                    setSearch('');
                    setSelectedPlatforms([]);
                    setSelectedGenres([]);
                    setPage(1);
                  }}
                >
                  Reset
                </button>
              )}
            </div>
          </form>

          {/* Quick Platform Filter Pills */}
          <div className="d-flex flex-wrap align-items-center gap-1 mt-3 pt-3 border-top border-secondary-subtle">
            <span className="small text-secondary fw-bold me-2">Platforms:</span>
            {platforms.map(p => (
              <button
                key={p.id}
                type="button"
                className={`btn btn-sm py-0 px-2 rounded-pill ${selectedPlatforms.includes(p.id) ? 'btn-primary' : 'btn-outline-secondary'}`}
                style={{ fontSize: '0.75rem' }}
                onClick={() => handleTogglePlatformFilter(p.id)}
              >
                {p.name}
              </button>
            ))}
          </div>

          {/* Quick Genre Filter Pills */}
          <div className="d-flex flex-wrap align-items-center gap-1 mt-2">
            <span className="small text-secondary fw-bold me-2">Genres:</span>
            {genres.slice(0, 8).map(g => (
              <button
                key={g.id}
                type="button"
                className={`btn btn-sm py-0 px-2 rounded-pill ${selectedGenres.includes(g.id) ? 'btn-info text-dark fw-bold' : 'btn-outline-secondary'}`}
                style={{ fontSize: '0.75rem' }}
                onClick={() => handleToggleGenreFilter(g.id)}
              >
                {g.name}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Catalog Grid */}
      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status"></div>
          <p className="text-secondary mt-2">Loading game catalog...</p>
        </div>
      ) : games.length === 0 ? (
        <div className="card custom-card border-0 p-5 text-center">
          <i className="bi bi-search fs-1 text-secondary mb-3"></i>
          <h4>No Games Found in Catalog</h4>
          <p className="text-secondary mb-4">
            Could not find any games matching your current search criteria.
          </p>
          <div className="d-flex justify-content-center gap-3">
            <Link to="/game-requests" className="btn btn-primary">
              <i className="bi bi-send-plus me-1"></i> Request This Game
            </Link>
          </div>
        </div>
      ) : (
        <>
          <div className="row g-4">
            {games.map((game) => (
              <div key={game.id} className="col-12 col-sm-6 col-md-4 col-xl-3">
                <div className="card h-100 border-0 shadow-sm game-card custom-card overflow-hidden">
                  {/* Game Cover */}
                  <div className="position-relative" style={{ height: '200px', backgroundColor: 'var(--bg-tertiary)' }}>
                    <img
                      src={getImageUrl(game.coverImage)}
                      alt={game.title}
                      className="w-100 h-100 object-fit-cover"
                      onError={(e) => {
                        e.target.src = 'https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=600&q=80';
                      }}
                    />
                    {game.metacriticScore && (
                      <div 
                        className="position-absolute top-0 end-0 m-2 badge bg-success fw-bold px-2 py-1 shadow"
                        style={{ fontSize: '0.75rem' }}
                      >
                        {game.metacriticScore}
                      </div>
                    )}
                    {game.isInUserLibrary && (
                      <div 
                        className="position-absolute top-0 start-0 m-2 badge bg-primary fw-bold px-2 py-1 shadow"
                        style={{ fontSize: '0.75rem' }}
                      >
                        <i className="bi bi-check-circle-fill me-1"></i> In Your Library
                      </div>
                    )}
                  </div>

                  {/* Card Content */}
                  <div className="card-body d-flex flex-column p-3">
                    <h5 className="card-title fw-bold text-truncate mb-1" title={game.title}>
                      <Link to={`/games/${game.id}`} className="text-decoration-none text-body">
                        {game.title}
                      </Link>
                    </h5>

                    <div className="d-flex align-items-center gap-2 text-secondary small mb-2">
                      {game.releaseDate && (
                        <span>{new Date(game.releaseDate).getFullYear()}</span>
                      )}
                      {game.genres && game.genres.length > 0 && (
                        <>
                          <span>•</span>
                          <span className="text-truncate">{game.genres.slice(0, 2).join(', ')}</span>
                        </>
                      )}
                    </div>

                    {/* Platforms Badges */}
                    <div className="d-flex flex-wrap gap-1 mb-3 flex-grow-1">
                      {game.platforms && game.platforms.map((plat, idx) => (
                        <span key={idx} className="badge bg-secondary-subtle text-secondary border" style={{ fontSize: '0.68rem' }}>
                          {plat}
                        </span>
                      ))}
                    </div>

                    {/* Action Buttons */}
                    <div className="d-flex gap-2 pt-2 border-top border-secondary-subtle">
                      <Link to={`/games/${game.id}`} className="btn btn-sm btn-outline-secondary flex-grow-1">
                        Details
                      </Link>
                      {game.isInUserLibrary ? (
                        <Link to={`/library`} className="btn btn-sm btn-outline-primary" title="View in My Library">
                          <i className="bi bi-collection-play-fill"></i>
                        </Link>
                      ) : (
                        <button
                          className="btn btn-sm btn-primary flex-grow-1 d-flex align-items-center justify-content-center gap-1"
                          onClick={() => openAddToLibraryModal(game)}
                        >
                          <i className="bi bi-plus-circle"></i>
                          <span>Add to Library</span>
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="d-flex justify-content-between align-items-center mt-4">
              <span className="text-secondary small">
                Showing {games.length} of {totalCount} games
              </span>
              <div className="btn-group">
                <button
                  className="btn btn-sm btn-outline-secondary"
                  disabled={page <= 1}
                  onClick={() => setPage(p => p - 1)}
                >
                  Previous
                </button>
                <button className="btn btn-sm btn-outline-secondary disabled">
                  {page} / {totalPages}
                </button>
                <button
                  className="btn btn-sm btn-outline-secondary"
                  disabled={page >= totalPages}
                  onClick={() => setPage(p => p + 1)}
                >
                  Next
                </button>
              </div>
            </div>
          )}
        </>
      )}

      {/* Add To Library Modal (Allows selecting multiple owned platforms) */}
      {showAddModal && targetGame && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className="bi bi-plus-circle me-2 text-primary"></i>
                  Add to Library: <span className="text-primary">{targetGame.title}</span>
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowAddModal(false)}></button>
              </div>

              <form onSubmit={handleSaveToLibrary}>
                <div className="modal-body p-4">
                  {addModalError && (
                    <div className="alert alert-danger py-2 mb-3">
                      <i className="bi bi-exclamation-circle me-1"></i> {addModalError}
                    </div>
                  )}

                  <p className="text-secondary small mb-3">
                    Select which platforms you own this game on. This creates your personal library entry without altering central game metadata.
                  </p>

                  {/* Multi-Platform Selection */}
                  <div className="mb-3">
                    <label className="form-label fw-bold small">
                      Select Your Owned Platform(s)
                    </label>
                    <div className="d-flex flex-wrap gap-2">
                      {platforms.map((plat) => (
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

                  {/* Ownership Status */}
                  <div className="mb-3">
                    <label className="form-label fw-bold small">Category / Status</label>
                    <div className="btn-group w-100" role="group">
                      <input
                        type="radio"
                        className="btn-check"
                        name="ownershipRadio"
                        id="status_own"
                        checked={userOwnershipStatus === 'own'}
                        onChange={() => setUserOwnershipStatus('own')}
                      />
                      <label className="btn btn-outline-primary btn-sm" htmlFor="status_own">
                        <i className="bi bi-check2-circle me-1"></i> Owned Game
                      </label>

                      <input
                        type="radio"
                        className="btn-check"
                        name="ownershipRadio"
                        id="status_backlog"
                        checked={userOwnershipStatus === 'backlog'}
                        onChange={() => setUserOwnershipStatus('backlog')}
                      />
                      <label className="btn btn-outline-warning btn-sm" htmlFor="status_backlog">
                        <i className="bi bi-inbox me-1"></i> Backlog
                      </label>

                      <input
                        type="radio"
                        className="btn-check"
                        name="ownershipRadio"
                        id="status_wishlist"
                        checked={userOwnershipStatus === 'wishlist'}
                        onChange={() => setUserOwnershipStatus('wishlist')}
                      />
                      <label className="btn btn-outline-info btn-sm" htmlFor="status_wishlist">
                        <i className="bi bi-heart me-1"></i> Wishlist
                      </label>
                    </div>
                  </div>

                  {/* Format Flags */}
                  <div className="row g-2 mb-2">
                    <div className="col-6">
                      <div className="form-check">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id="digitalCheck"
                          checked={userDigitalCopy}
                          onChange={(e) => setUserDigitalCopy(e.target.checked)}
                        />
                        <label className="form-check-label small" htmlFor="digitalCheck">
                          Digital Copy
                        </label>
                      </div>
                    </div>
                    <div className="col-6">
                      <div className="form-check">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id="physicalCheck"
                          checked={userPhysicalCopy}
                          onChange={(e) => setUserPhysicalCopy(e.target.checked)}
                        />
                        <label className="form-check-label small" htmlFor="physicalCheck">
                          Physical Copy
                        </label>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="modal-footer border-top border-secondary-subtle">
                  <button type="button" className="btn btn-secondary" onClick={() => setShowAddModal(false)} disabled={addingToLibrary}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary d-flex align-items-center gap-2" disabled={addingToLibrary}>
                    {addingToLibrary && <span className="spinner-border spinner-border-sm" role="status"></span>}
                    <span>Add to My Library</span>
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

export default Catalog;
