import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
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

const Library = () => {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(12);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  // Layout View Mode: 'grid' | 'list' | 'card' | 'gallery'
  const [viewMode, setViewMode] = useState('grid');
  
  // Collapsible filters status
  const [showFilters, setShowFilters] = useState(true);

  // Filters State
  const [search, setSearch] = useState('');
  const [selectedPlatforms, setSelectedPlatforms] = useState([]);
  const [selectedServices, setSelectedServices] = useState([]);
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [selectedStatuses, setSelectedStatuses] = useState([]);
  const [wishlistFilter, setWishlistFilter] = useState(null);
  const [backlogFilter, setBacklogFilter] = useState(null);
  const [ownGameFilter, setOwnGameFilter] = useState(null);
  
  // Sort State
  const [sortBy, setSortBy] = useState('title');
  const [sortOrder, setSortOrder] = useState('asc');

  // Metadata filter options list
  const [platforms, setPlatforms] = useState([]);
  const [services, setServices] = useState([]);
  const [genres, setGenres] = useState([]);

  // Fetch Lookups for filter panels
  useEffect(() => {
    const loadFiltersLookups = async () => {
      try {
        const [pRes, sRes, gRes] = await Promise.all([
          api.get('/metadata/platforms/list'),
          api.get('/metadata/services/list'),
          api.get('/metadata/genres/list')
        ]);
        setPlatforms(pRes.data || []);
        setServices(sRes.data || []);
        setGenres(gRes.data || []);
      } catch (err) {
        console.error('Failed to load filter metadata lists', err);
      }
    };
    loadFiltersLookups();
  }, []);

  // Fetch Library Games
  const fetchLibrary = async () => {
    setLoading(true);
    try {
      const params = {
        pageNumber: page,
        pageSize: viewMode === 'gallery' ? 18 : pageSize,
        searchTerm: search || null,
        sortBy,
        sortOrder,
        platformIds: selectedPlatforms.length > 0 ? selectedPlatforms : null,
        serviceIds: selectedServices.length > 0 ? selectedServices : null,
        genreIds: selectedGenres.length > 0 ? selectedGenres : null,
        completionStatuses: selectedStatuses.length > 0 ? selectedStatuses : null,
        wishlist: wishlistFilter,
        backlog: backlogFilter,
        ownGame: ownGameFilter
      };

      const response = await api.get('/libraries', { params });
      setGames(response.data.items || []);
      setTotalPages(response.data.totalPages || 0);
      setTotalCount(response.data.totalCount || 0);
    } catch (err) {
      console.error('Failed to retrieve personal library collection', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setPage(1);
    fetchLibrary();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search, selectedPlatforms, selectedServices, selectedGenres, selectedStatuses, wishlistFilter, backlogFilter, ownGameFilter, sortBy, sortOrder, viewMode]);

  useEffect(() => {
    fetchLibrary();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  // Toggle checklist filters helper
  const handleFilterToggle = (list, setList, itemId) => {
    if (list.includes(itemId)) {
      setList(list.filter(id => id !== itemId));
    } else {
      setList([...list, itemId]);
    }
  };

  const handleResetFilters = () => {
    setSearch('');
    setSelectedPlatforms([]);
    setSelectedServices([]);
    setSelectedGenres([]);
    setSelectedStatuses([]);
    setWishlistFilter(null);
    setBacklogFilter(null);
    setOwnGameFilter(null);
    setSortBy('title');
    setSortOrder('asc');
  };

  const deleteGame = async (id) => {
    if (!window.confirm('Are you sure you want to remove this game from your personal library?')) return;
    try {
      await api.delete(`/libraries/${id}`);
      fetchLibrary();
    } catch (err) {
      alert('Failed to remove game from library: ' + (err.message || 'Server error'));
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

  return (
    <div className="container-fluid py-2">
      {/* Top action bar */}
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
        <div>
          <h3 className="display-font mb-1">My Game Library</h3>
          <p className="text-muted small mb-0">{totalCount} games cataloged in your collection</p>
        </div>
        <div className="d-flex align-items-center gap-2">
          <button 
            className={`btn btn-sm ${showFilters ? 'btn-premium-purple' : 'btn-premium-outline'}`}
            onClick={() => setShowFilters(!showFilters)}
            title="Toggle Filter Panel"
          >
            <i className="bi bi-funnel-fill me-1"></i> {showFilters ? 'Hide Filters' : 'Show Filters'}
          </button>
          
          <Link to="/catalog" className="btn btn-sm btn-premium-purple d-flex align-items-center gap-2">
            <i className="bi bi-compass"></i>
            <span>Browse Catalog</span>
          </Link>
        </div>
      </div>

      <div className="row g-4">
        {/* Left filter side column */}
        {showFilters && (
          <div className="col-12 col-lg-3 fade-in">
            <div className="glass-panel p-3 d-flex flex-column gap-3 mb-4" style={{ maxHeight: '80vh', overflowY: 'auto' }}>
              <div className="d-flex justify-content-between align-items-center pb-2" style={{ borderBottom: '1px solid var(--border-subtle)' }}>
                <span className="fw-bold small display-font uppercase" style={{ color: 'var(--text-primary)' }}>Filters Panel</span>
                <button className="btn btn-link p-0 small text-decoration-none" style={{ color: 'var(--accent-purple)' }} onClick={handleResetFilters}>
                  Reset All
                </button>
              </div>

              {/* Toggles */}
              <div>
                <label className="form-label small fw-bold uppercase mb-2">Category Toggles</label>
                <div className="d-flex flex-column gap-2">
                  <div className="form-check form-switch">
                    <input 
                      className="form-check-input" 
                      type="checkbox" 
                      id="toggle-own"
                      checked={ownGameFilter === true}
                      onChange={(e) => setOwnGameFilter(e.target.checked ? true : null)}
                    />
                    <label className="form-check-label filter-check-label" htmlFor="toggle-own">Owned Games</label>
                  </div>
                  <div className="form-check form-switch">
                    <input 
                      className="form-check-input" 
                      type="checkbox" 
                      id="toggle-wish"
                      checked={wishlistFilter === true}
                      onChange={(e) => setWishlistFilter(e.target.checked ? true : null)}
                    />
                    <label className="form-check-label filter-check-label" htmlFor="toggle-wish">Wishlisted</label>
                  </div>
                  <div className="form-check form-switch">
                    <input 
                      className="form-check-input" 
                      type="checkbox" 
                      id="toggle-back"
                      checked={backlogFilter === true}
                      onChange={(e) => setBacklogFilter(e.target.checked ? true : null)}
                    />
                    <label className="form-check-label filter-check-label" htmlFor="toggle-back">Backlog Queue</label>
                  </div>
                </div>
              </div>

              {/* Status checkboxes */}
              <div>
                <label className="form-label small fw-bold uppercase mb-2">Play Status</label>
                <div className="filter-section-box" style={{ maxHeight: '120px', overflowY: 'auto' }}>
                  {CompletionStatuses.map(s => (
                    <div key={s.value} className="form-check mb-1">
                      <input 
                        className="form-check-input" 
                        type="checkbox" 
                        id={`status-${s.value}`}
                        checked={selectedStatuses.includes(s.value)}
                        onChange={() => handleFilterToggle(selectedStatuses, setSelectedStatuses, s.value)}
                      />
                      <label className="form-check-label filter-check-label" htmlFor={`status-${s.value}`}>{s.label}</label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Platforms */}
              <div>
                <label className="form-label small fw-bold uppercase mb-2">Platforms</label>
                <div className="filter-section-box" style={{ maxHeight: '140px', overflowY: 'auto' }}>
                  {platforms.map(p => (
                    <div key={p.id} className="form-check mb-1">
                      <input 
                        className="form-check-input" 
                        type="checkbox" 
                        id={`platform-${p.id}`}
                        checked={selectedPlatforms.includes(p.id)}
                        onChange={() => handleFilterToggle(selectedPlatforms, setSelectedPlatforms, p.id)}
                      />
                      <label className="form-check-label filter-check-label" htmlFor={`platform-${p.id}`}>{p.name}</label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Digital Services */}
              <div>
                <label className="form-label small fw-bold uppercase mb-2">Storefronts/Services</label>
                <div className="filter-section-box" style={{ maxHeight: '140px', overflowY: 'auto' }}>
                  {services.map(s => (
                    <div key={s.id} className="form-check mb-1">
                      <input 
                        className="form-check-input" 
                        type="checkbox" 
                        id={`service-${s.id}`}
                        checked={selectedServices.includes(s.id)}
                        onChange={() => handleFilterToggle(selectedServices, setSelectedServices, s.id)}
                      />
                      <label className="form-check-label filter-check-label" htmlFor={`service-${s.id}`}>{s.name}</label>
                    </div>
                  ))}
                </div>
              </div>

              {/* Genres */}
              <div>
                <label className="form-label small fw-bold uppercase mb-2">Genres</label>
                <div className="filter-section-box" style={{ maxHeight: '140px', overflowY: 'auto' }}>
                  {genres.map(g => (
                    <div key={g.id} className="form-check mb-1">
                      <input 
                        className="form-check-input" 
                        type="checkbox" 
                        id={`genre-${g.id}`}
                        checked={selectedGenres.includes(g.id)}
                        onChange={() => handleFilterToggle(selectedGenres, setSelectedGenres, g.id)}
                      />
                      <label className="form-check-label filter-check-label" htmlFor={`genre-${g.id}`}>{g.name}</label>
                    </div>
                  ))}
                </div>
              </div>

            </div>
          </div>
        )}

        {/* Catalog List columns */}
        <div className={showFilters ? 'col-12 col-lg-9' : 'col-12'}>
          {/* Toolbar settings */}
          <div className="glass-panel p-3 mb-4 d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3">
            <div className="flex-grow-1" style={{ maxWidth: '350px' }}>
              <div className="input-group">
                <span className="input-group-addon"><i className="bi bi-search"></i></span>
                <input 
                  type="text" 
                  className="form-control form-glass-control"
                  placeholder="Search titles..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                />
              </div>
            </div>

            <div className="d-flex align-items-center gap-3 flex-wrap">
              {/* Sorting options */}
              <div className="d-flex align-items-center gap-2">
                <span className="text-muted small">Sort:</span>
                <select 
                  className="form-select form-glass-control py-1 px-3 small" 
                  style={{ width: '130px' }}
                  value={sortBy}
                  onChange={(e) => setSortBy(e.target.value)}
                >
                  <option value="title">Title</option>
                  <option value="releasedate">Release Date</option>
                  <option value="purchasedate">Purchase Date</option>
                  <option value="rating">Rating</option>
                  <option value="hoursplayed">Time Played</option>
                </select>
                <button 
                  className="sort-dir-btn"
                  onClick={() => setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc')}
                >
                  {sortOrder === 'asc' ? <i className="bi bi-sort-up"></i> : <i className="bi bi-sort-down"></i>}
                </button>
              </div>

              {/* View mode toggle */}
              <div className="view-switcher">
                <button className={`view-btn ${viewMode === 'grid' ? 'active' : ''}`} onClick={() => setViewMode('grid')} title="Grid View">
                  <i className="bi bi-grid-fill"></i>
                </button>
                <button className={`view-btn ${viewMode === 'card' ? 'active' : ''}`} onClick={() => setViewMode('card')} title="Card View">
                  <i className="bi bi-card-heading"></i>
                </button>
                <button className={`view-btn ${viewMode === 'gallery' ? 'active' : ''}`} onClick={() => setViewMode('gallery')} title="Gallery Wall">
                  <i className="bi bi-images"></i>
                </button>
                <button className={`view-btn ${viewMode === 'list' ? 'active' : ''}`} onClick={() => setViewMode('list')} title="List View">
                  <i className="bi bi-list-task"></i>
                </button>
              </div>
            </div>
          </div>

          {/* Catalog Renders */}
          {loading ? (
            <div className="row g-3">
              {[...Array(6)].map((_, i) => (
                <div key={i} className="col-12 col-md-6 col-xl-4">
                  <div className="glass-panel p-3" style={{ height: '320px' }}>
                    <div className="skeleton rounded-3 mb-3" style={{ height: '180px' }}></div>
                    <div className="skeleton mb-2" style={{ height: '20px', width: '70%' }}></div>
                    <div className="skeleton" style={{ height: '15px', width: '40%' }}></div>
                  </div>
                </div>
              ))}
            </div>
          ) : games.length === 0 ? (
            <div className="glass-panel p-5 text-center">
              <i className="bi bi-controller fs-1 d-block mb-3" style={{ color: 'var(--text-muted)' }}></i>
              <h4 className="mb-2">No Games Found</h4>
              <p className="text-muted small mb-0">Your catalog is currently empty. Reset filters or add new games.</p>
            </div>
          ) : (
            <>
              {/* Grid View */}
              {viewMode === 'grid' && (
                <div className="row g-3">
                  {games.map(game => (
                    <div key={game.id} className="col-12 col-sm-6 col-md-6 col-xl-4 fade-in">
                      <div className="game-card">
                        <div className="game-card-img-wrapper">
                          <Link to={`/games/${game.id}`}>
                            <img 
                              src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                              alt={game.title} 
                              className="game-card-img"
                              onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                            />
                          </Link>
                          <span className={`game-card-badge ${getStatusBadgeClass(game.completionStatus)}`}>
                            {getStatusLabel(game.completionStatus)}
                          </span>
                        </div>
                        <div className="game-card-body">
                          <div>
                            <Link to={`/games/${game.id}`} className="text-decoration-none">
                              <h5 className="game-card-title display-font">{game.title}</h5>
                            </Link>
                            <p className="text-muted small mb-2 text-truncate">
                              {game.platforms.join(', ') || 'No Platform'}
                            </p>
                          </div>
                          
                          <div className="d-flex justify-content-between align-items-center pt-2 mt-2" style={{ borderTop: '1px solid var(--border-subtle)' }}>
                            <span className="small text-muted">
                              <i className="bi bi-clock me-1" style={{ color: 'var(--accent-purple)' }}></i>
                              {game.hoursPlayed} hrs
                            </span>
                            <div className="d-flex align-items-center gap-2">
                              {game.personalRating && (
                                <span className="small" style={{ color: '#ca8a04' }}>
                                  <i className="bi bi-star-fill me-1"></i>
                                  {game.personalRating}/10
                                </span>
                              )}
                              <Link to={`/games/edit/${game.id}`} className="btn btn-sm btn-link p-1" style={{ color: 'var(--text-muted)' }} title="Edit Game">
                                <i className="bi bi-pencil-fill"></i>
                              </Link>
                              <button className="btn btn-sm btn-link text-danger p-1" onClick={() => deleteGame(game.id)} title="Delete Game">
                                <i className="bi bi-trash-fill"></i>
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}

              {/* Card View */}
              {viewMode === 'card' && (
                <div className="row g-3">
                  {games.map(game => (
                    <div key={game.id} className="col-12 fade-in">
                      <div className="glass-panel p-3 d-flex flex-column flex-sm-row gap-3 align-items-start align-items-sm-center">
                        <Link to={`/games/${game.id}`} className="rounded overflow-hidden flex-shrink-0" style={{ width: '80px', height: '110px' }}>
                          <img 
                            src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                            alt={game.title} 
                            className="w-100 h-100 object-fit-cover hover-scale"
                            onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                          />
                        </Link>
                        <div className="flex-grow-1 min-w-0">
                          <div className="d-flex flex-wrap align-items-center gap-2 mb-1">
                            <Link to={`/games/${game.id}`} className="text-decoration-none">
                              <h4 className="mb-0 display-font text-truncate h5">{game.title}</h4>
                            </Link>
                            <span className={`badge ${getStatusBadgeClass(game.completionStatus)}`}>
                              {getStatusLabel(game.completionStatus)}
                            </span>
                          </div>
                          <p className="text-muted small mb-2">
                            {game.platforms.join(', ') || 'No Platform'} | Genres: {game.genres.join(', ') || '-'}
                          </p>
                          <div className="d-flex gap-3 text-muted small">
                            <span><i className="bi bi-clock me-1"></i> {game.hoursPlayed} hrs</span>
                            {game.personalRating && <span><i className="bi bi-star-fill me-1" style={{ color: '#ca8a04' }}></i> {game.personalRating}/10</span>}
                            {game.purchasePrice && <span><i className="bi bi-cash-stack me-1"></i> ${game.purchasePrice}</span>}
                          </div>
                        </div>
                        <div className="d-flex gap-1 align-self-end align-self-sm-center ms-auto">
                          <Link to={`/games/edit/${game.id}`} className="btn btn-sm btn-outline-info border-0 p-2">
                            <i className="bi bi-pencil-fill"></i>
                          </Link>
                          <button className="btn btn-sm btn-outline-danger border-0 p-2" onClick={() => deleteGame(game.id)}>
                            <i className="bi bi-trash-fill"></i>
                          </button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}

              {/* Gallery View */}
              {viewMode === 'gallery' && (
                <div className="row g-2">
                  {games.map(game => (
                    <div key={game.id} className="col-4 col-sm-3 col-md-3 col-xl-2 fade-in">
                      <div className="position-relative overflow-hidden rounded-3 game-card" style={{ border: 'none', aspectRatio: '3/4' }}>
                        <Link to={`/games/${game.id}`} className="d-block w-100 h-100">
                          <img 
                            src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                            alt={game.title} 
                            className="w-100 h-100 object-fit-cover game-card-img"
                            onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                          />
                        </Link>
                        <div className="position-absolute bottom-0 start-0 end-0 p-2 text-center text-truncate small" 
                          style={{ background: 'rgba(0,0,0,0.75)', color: '#ffffff', fontSize: '0.75rem' }}>
                          {game.title}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}

              {/* List View */}
              {viewMode === 'list' && (
                <div className="glass-panel p-0 overflow-hidden">
                  <div className="table-responsive">
                    <table className="table table-theme mb-0 align-middle">
                      <thead>
                        <tr>
                          <th className="p-3">Title</th>
                          <th className="p-3">Platforms</th>
                          <th className="p-3">Status</th>
                          <th className="p-3">Hours</th>
                          <th className="p-3">Rating</th>
                          <th className="p-3">Release</th>
                          <th className="p-3 text-end">Actions</th>
                        </tr>
                      </thead>
                      <tbody>
                        {games.map(game => (
                          <tr key={game.id}>
                            <td className="p-3 fw-bold">
                              <Link to={`/games/${game.id}`} className="text-decoration-none" style={{ color: 'var(--text-primary)' }}>
                                {game.title}
                              </Link>
                            </td>
                            <td className="p-3 text-muted small">{game.platforms.join(', ') || '-'}</td>
                            <td className="p-3">
                              <span className={`badge ${getStatusBadgeClass(game.completionStatus)}`}>
                                {getStatusLabel(game.completionStatus)}
                              </span>
                            </td>
                            <td className="p-3 text-muted">{game.hoursPlayed} hrs</td>
                            <td className="p-3" style={{ color: '#ca8a04' }}>
                              {game.personalRating ? <><i className="bi bi-star-fill me-1"></i>{game.personalRating}</> : '-'}
                            </td>
                            <td className="p-3 text-muted small">
                              {game.releaseDate ? new Date(game.releaseDate).getFullYear() : '-'}
                            </td>
                            <td className="p-3 text-end">
                              <div className="d-flex justify-content-end gap-1">
                                <Link to={`/games/edit/${game.id}`} className="btn btn-sm btn-outline-info border-0 p-1">
                                  <i className="bi bi-pencil-fill"></i>
                                </Link>
                                <button className="btn btn-sm btn-outline-danger border-0 p-1" onClick={() => deleteGame(game.id)}>
                                  <i className="bi bi-trash-fill"></i>
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {/* Pagination bar */}
              {totalPages > 1 && (
                <div className="d-flex justify-content-between align-items-center mt-4">
                  <span className="text-muted small">
                    Showing Page {page} of {totalPages} ({totalCount} total games)
                  </span>
                  <nav>
                    <ul className="pagination pagination-sm mb-0 gap-1">
                      <li className={`page-item ${page === 1 ? 'disabled' : ''}`}>
                        <button className="page-btn" onClick={() => setPage(page - 1)} disabled={page === 1}>
                          Previous
                        </button>
                      </li>
                      <li className={`page-item ${page === totalPages ? 'disabled' : ''}`}>
                        <button className="page-btn" onClick={() => setPage(page + 1)} disabled={page === totalPages}>
                          Next
                        </button>
                      </li>
                    </ul>
                  </nav>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default Library;
