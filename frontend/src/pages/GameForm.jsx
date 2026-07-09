import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import api, { API_HOST } from '../services/api';

const CompletionStatuses = [
  { value: 0, label: 'Not Started' },
  { value: 1, label: 'Playing' },
  { value: 2, label: 'On Hold' },
  { value: 3, label: 'Completed' },
  { value: 4, label: 'Dropped' },
  { value: 5, label: '100% Completed' },
  { value: 6, label: 'Replaying' }
];

const GameForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = !!id;

  const [activeTab, setActiveTab] = useState('general');
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  // Dropdown lists
  const [lookups, setLookups] = useState({
    platforms: [],
    services: [],
    developers: [],
    publishers: [],
    genres: [],
    tags: [],
    themes: [],
    franchises: [],
    series: []
  });

  // Main Form State
  const [form, setForm] = useState({
    title: '',
    alternateTitles: '',
    originalTitle: '',
    description: '',
    notes: '',
    personalNotes: '',

    ownGame: false,
    wishlist: false,
    backlog: false,
    physicalCopy: false,
    digitalCopy: false,
    collectorsEdition: false,
    specialEdition: false,

    purchaseDate: '',
    purchasePrice: '',
    currency: 'USD',
    storePurchasedFrom: '',
    purchaseRegion: '',
    receiptReference: '',
    gifted: false,

    startedPlayingDate: '',
    completedDate: '',
    lastPlayedDate: '',
    hoursPlayed: 0,
    completionStatus: 0,

    personalRating: '',
    communityRating: '',
    criticRating: '',

    releaseDate: '',
    originalReleaseDate: '',
    earlyAccessDate: '',

    coverImage: '',
    boxArt: '',
    banner: '',
    logo: '',
    screenshots: '',
    artwork: '',
    fanArt: '',
    trailerUrl: '',
    gameplayUrl: '',
    youtubeLinks: '',

    esrbRating: '',
    pegiRating: '',
    metacriticScore: '',
    openCriticScore: '',
    multiplayerSupport: false,
    coopSupport: false,
    vrSupport: false,
    crossplaySupport: false,
    cloudSaveSupport: false,
    controllerSupport: false,
    steamDeckCompatibility: 'Unknown',
    achievementCount: 0,
    dlcCount: 0,
    expansionCount: 0,

    franchiseId: '',
    seriesId: '',

    developerIds: [],
    publisherIds: [],
    genreIds: [],
    tagIds: [],
    themeIds: [],
    platformIds: [],
    serviceIds: []
  });

  // Load all select lists
  const fetchLookups = async () => {
    try {
      const keys = ['platforms', 'services', 'developers', 'publishers', 'genres', 'tags', 'themes', 'franchises', 'series'];
      const requests = keys.map(k => api.get(`/metadata/${k}/list`));
      const results = await Promise.all(requests);
      
      const newLookups = {};
      keys.forEach((k, idx) => {
        newLookups[k] = results[idx].data || [];
      });
      setLookups(newLookups);
    } catch (err) {
      console.error('Failed to load metadata lookups', err);
    }
  };

  // Load game details if editing
  const loadGameDetails = async () => {
    setLoading(true);
    try {
      const response = await api.get(`/games/${id}`);
      const data = response.data;
      
      // Map to form structure
      setForm({
        ...data,
        purchaseDate: data.purchaseDate ? data.purchaseDate.substring(0, 10) : '',
        startedPlayingDate: data.startedPlayingDate ? data.startedPlayingDate.substring(0, 10) : '',
        completedDate: data.completedDate ? data.completedDate.substring(0, 10) : '',
        lastPlayedDate: data.lastPlayedDate ? data.lastPlayedDate.substring(0, 10) : '',
        releaseDate: data.releaseDate ? data.releaseDate.substring(0, 10) : '',
        originalReleaseDate: data.originalReleaseDate ? data.originalReleaseDate.substring(0, 10) : '',
        earlyAccessDate: data.earlyAccessDate ? data.earlyAccessDate.substring(0, 10) : '',
        purchasePrice: data.purchasePrice || '',
        personalRating: data.personalRating || '',
        communityRating: data.communityRating || '',
        criticRating: data.criticRating || '',
        metacriticScore: data.metacriticScore || '',
        openCriticScore: data.openCriticScore || '',
        franchiseId: data.franchiseId || '',
        seriesId: data.seriesId || '',
        developerIds: data.developers?.map(d => d.id) || [],
        publisherIds: data.publishers?.map(p => p.id) || [],
        genreIds: data.genres?.map(g => g.id) || [],
        tagIds: data.tags?.map(t => t.id) || [],
        themeIds: data.themes?.map(t => t.id) || [],
        platformIds: data.platforms?.map(p => p.id) || [],
        serviceIds: data.digitalServices?.map(s => s.id) || []
      });
    } catch (err) {
      setError('Failed to retrieve game details.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const init = async () => {
      await fetchLookups();
      if (isEdit) {
        await loadGameDetails();
      }
    };
    init();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  const handleTextChange = (e) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  const handleCheckboxChange = (e) => {
    const { name, checked } = e.target;
    setForm(prev => ({ ...prev, [name]: checked }));
  };

  const handleListToggle = (name, itemId) => {
    setForm(prev => {
      const list = prev[name];
      if (list.includes(itemId)) {
        return { ...prev, [name]: list.filter(id => id !== itemId) };
      } else {
        return { ...prev, [name]: [...list, itemId] };
      }
    });
  };

  // Image Upload and preview handler
  const handleImageUpload = async (e, fieldName) => {
    const file = e.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);
    formData.append('folderName', fieldName === 'coverImage' ? 'covers' : fieldName === 'banner' ? 'banners' : 'screenshots');

    try {
      const response = await api.post('/games/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      setForm(prev => ({ ...prev, [fieldName]: response.data.url }));
    } catch (err) {
      alert('Upload failed: ' + (err.message || 'Server error'));
    }
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');

    // Prepare payload
    const payload = {
      ...form,
      purchasePrice: form.purchasePrice ? parseFloat(form.purchasePrice) : null,
      hoursPlayed: parseFloat(form.hoursPlayed) || 0,
      completionStatus: parseInt(form.completionStatus) || 0,
      personalRating: form.personalRating ? parseFloat(form.personalRating) : null,
      communityRating: form.communityRating ? parseFloat(form.communityRating) : null,
      criticRating: form.criticRating ? parseFloat(form.criticRating) : null,
      metacriticScore: form.metacriticScore ? parseInt(form.metacriticScore) : null,
      openCriticScore: form.openCriticScore ? parseInt(form.openCriticScore) : null,
      achievementCount: parseInt(form.achievementCount) || 0,
      dlcCount: parseInt(form.dlcCount) || 0,
      expansionCount: parseInt(form.expansionCount) || 0,
      franchiseId: form.franchiseId ? parseInt(form.franchiseId) : null,
      seriesId: form.seriesId ? parseInt(form.seriesId) : null,
      // Date validations
      purchaseDate: form.purchaseDate ? new Date(form.purchaseDate).toISOString() : null,
      startedPlayingDate: form.startedPlayingDate ? new Date(form.startedPlayingDate).toISOString() : null,
      completedDate: form.completedDate ? new Date(form.completedDate).toISOString() : null,
      lastPlayedDate: form.lastPlayedDate ? new Date(form.lastPlayedDate).toISOString() : null,
      releaseDate: form.releaseDate ? new Date(form.releaseDate).toISOString() : null,
      originalReleaseDate: form.originalReleaseDate ? new Date(form.originalReleaseDate).toISOString() : null,
      earlyAccessDate: form.earlyAccessDate ? new Date(form.earlyAccessDate).toISOString() : null
    };

    try {
      if (isEdit) {
        await api.put(`/games/${id}`, { id: parseInt(id), ...payload });
      } else {
        await api.post('/games', payload);
      }
      navigate('/library');
    } catch (err) {
      setError(err.message || 'An error occurred while saving.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  return (
    <div className="container-fluid py-2">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="display-font text-white mb-0">{isEdit ? 'Edit Game' : 'Add New Game'}</h3>
          <p className="text-muted small">Fill catalog attributes and details</p>
        </div>
        <Link to="/library" className="btn btn-premium-outline btn-sm">
          <i className="bi bi-chevron-left"></i> Cancel
        </Link>
      </div>

      {error && (
        <div className="alert alert-danger bg-danger bg-opacity-10 border-0 text-danger mb-4">
          {error}
        </div>
      )}

      <form onSubmit={handleFormSubmit}>
        <div className="row g-4">
          {/* Tabs navigation panel */}
          <div className="col-12 col-lg-3">
            <div className="glass-panel p-3 d-flex flex-row flex-lg-column gap-1 overflow-x-auto">
              <button
                type="button"
                className={`btn text-start w-100 px-3 py-2.5 rounded-3 border-0 display-font ${activeTab === 'general' ? 'bg-primary text-white' : 'text-muted hover-bg-tertiary'}`}
                onClick={() => setActiveTab('general')}
              >
                <i className="bi bi-info-circle-fill me-2"></i> General Info
              </button>
              <button
                type="button"
                className={`btn text-start w-100 px-3 py-2.5 rounded-3 border-0 display-font ${activeTab === 'ownership' ? 'bg-primary text-white' : 'text-muted hover-bg-tertiary'}`}
                onClick={() => setActiveTab('ownership')}
              >
                <i className="bi bi-shield-check me-2"></i> Ownership & Play
              </button>
              <button
                type="button"
                className={`btn text-start w-100 px-3 py-2.5 rounded-3 border-0 display-font ${activeTab === 'purchase' ? 'bg-primary text-white' : 'text-muted hover-bg-tertiary'}`}
                onClick={() => setActiveTab('purchase')}
              >
                <i className="bi bi-wallet2 me-2"></i> Purchase & Extras
              </button>
              <button
                type="button"
                className={`btn text-start w-100 px-3 py-2.5 rounded-3 border-0 display-font ${activeTab === 'media' ? 'bg-primary text-white' : 'text-muted hover-bg-tertiary'}`}
                onClick={() => setActiveTab('media')}
              >
                <i className="bi bi-images me-2"></i> Media & Links
              </button>
            </div>
          </div>

          {/* Form Tabs Content Area */}
          <div className="col-12 col-lg-9">
            <div className="glass-panel p-4 p-md-5">
              
              {/* Tab 1: General */}
              {activeTab === 'general' && (
                <div>
                  <h4 className="display-font text-white mb-4 border-bottom border-secondary border-opacity-10 pb-2">General Information</h4>
                  <div className="row g-3">
                    <div className="col-12">
                      <label className="form-label text-muted small fw-bold">GAME TITLE *</label>
                      <input type="text" name="title" className="form-control form-glass-control text-white" value={form.title} onChange={handleTextChange} required />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">ALTERNATE TITLES</label>
                      <input type="text" name="alternateTitles" className="form-control form-glass-control text-white" value={form.alternateTitles} onChange={handleTextChange} placeholder="e.g. Witcher 3, Wild Hunt" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">ORIGINAL TITLE</label>
                      <input type="text" name="originalTitle" className="form-control form-glass-control text-white" value={form.originalTitle} onChange={handleTextChange} placeholder="e.g. Wiedźmin 3" />
                    </div>
                    <div className="col-12">
                      <label className="form-label text-muted small fw-bold">DESCRIPTION</label>
                      <textarea name="description" className="form-control form-glass-control text-white" rows="4" value={form.description} onChange={handleTextChange}></textarea>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">NOTES</label>
                      <textarea name="notes" className="form-control form-glass-control text-white" rows="3" value={form.notes} onChange={handleTextChange}></textarea>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">PERSONAL NOTES</label>
                      <textarea name="personalNotes" className="form-control form-glass-control text-white" rows="3" value={form.personalNotes} onChange={handleTextChange}></textarea>
                    </div>
                    
                    {/* Release and Ratings */}
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">RELEASE DATE</label>
                      <input type="date" name="releaseDate" className="form-control form-glass-control text-white" value={form.releaseDate} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">ORIGINAL RELEASE DATE</label>
                      <input type="date" name="originalReleaseDate" className="form-control form-glass-control text-white" value={form.originalReleaseDate} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">EARLY ACCESS DATE</label>
                      <input type="date" name="earlyAccessDate" className="form-control form-glass-control text-white" value={form.earlyAccessDate} onChange={handleTextChange} />
                    </div>

                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">PERSONAL RATING (0-10)</label>
                      <input type="number" step="0.1" name="personalRating" className="form-control form-glass-control text-white" value={form.personalRating} onChange={handleTextChange} min="0" max="10" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">COMMUNITY RATING (0-10)</label>
                      <input type="number" step="0.1" name="communityRating" className="form-control form-glass-control text-white" value={form.communityRating} onChange={handleTextChange} min="0" max="10" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">CRITIC RATING (0-100)</label>
                      <input type="number" name="criticRating" className="form-control form-glass-control text-white" value={form.criticRating} onChange={handleTextChange} min="0" max="100" />
                    </div>

                    {/* Franchise, Series Dropdowns */}
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">FRANCHISE</label>
                      <select name="franchiseId" className="form-select form-glass-control text-white" value={form.franchiseId} onChange={handleTextChange}>
                        <option value="">-- Select Franchise --</option>
                        {lookups.franchises.map(f => <option key={f.id} value={f.id}>{f.name}</option>)}
                      </select>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">SERIES</label>
                      <select name="seriesId" className="form-select form-glass-control text-white" value={form.seriesId} onChange={handleTextChange}>
                        <option value="">-- Select Series --</option>
                        {lookups.series.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
                      </select>
                    </div>
                  </div>
                </div>
              )}

              {/* Tab 2: Ownership & Play */}
              {activeTab === 'ownership' && (
                <div>
                  <h4 className="display-font text-white mb-4 border-bottom border-secondary border-opacity-10 pb-2">Ownership & Play Progress</h4>
                  
                  <div className="mb-4">
                    <label className="form-label text-muted small fw-bold d-block mb-3">OWNERSHIP CHECKMARKS</label>
                    <div className="row g-3">
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="ownGame" id="ownGame" checked={form.ownGame} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="ownGame">Own Game</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="wishlist" id="wishlist" checked={form.wishlist} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="wishlist">Wishlist</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="backlog" id="backlog" checked={form.backlog} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="backlog">Backlog</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="physicalCopy" id="physicalCopy" checked={form.physicalCopy} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="physicalCopy">Physical Copy</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="digitalCopy" id="digitalCopy" checked={form.digitalCopy} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="digitalCopy">Digital Copy</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="collectorsEdition" id="collectorsEdition" checked={form.collectorsEdition} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="collectorsEdition">Collector's Edition</label>
                        </div>
                      </div>
                      <div className="col-6 col-md-4">
                        <div className="form-check form-switch">
                          <input className="form-check-input" type="checkbox" name="specialEdition" id="specialEdition" checked={form.specialEdition} onChange={handleCheckboxChange} />
                          <label className="form-check-label text-white small" htmlFor="specialEdition">Special Edition</label>
                        </div>
                      </div>
                    </div>
                  </div>

                  <hr className="my-4 border-secondary border-opacity-25" />

                  <div className="row g-3">
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">COMPLETION STATUS</label>
                      <select name="completionStatus" className="form-select form-glass-control text-white" value={form.completionStatus} onChange={handleTextChange}>
                        {CompletionStatuses.map(s => <option key={s.value} value={s.value}>{s.label}</option>)}
                      </select>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">HOURS PLAYED</label>
                      <input type="number" step="0.1" name="hoursPlayed" className="form-control form-glass-control text-white" value={form.hoursPlayed} onChange={handleTextChange} min="0" />
                    </div>

                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">STARTED PLAYING DATE</label>
                      <input type="date" name="startedPlayingDate" className="form-control form-glass-control text-white" value={form.startedPlayingDate} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">COMPLETED DATE</label>
                      <input type="date" name="completedDate" className="form-control form-glass-control text-white" value={form.completedDate} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">LAST PLAYED DATE</label>
                      <input type="date" name="lastPlayedDate" className="form-control form-glass-control text-white" value={form.lastPlayedDate} onChange={handleTextChange} />
                    </div>
                  </div>

                  <hr className="my-4 border-secondary border-opacity-25" />

                  {/* Platforms & Services selections Checkbox columns */}
                  <div className="row g-3">
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">PLATFORMS</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '150px' }}>
                        {lookups.platforms.map(p => (
                          <div key={p.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`plat-${p.id}`}
                              checked={form.platformIds.includes(p.id)}
                              onChange={() => handleListToggle('platformIds', p.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`plat-${p.id}`}>{p.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">DIGITAL SERVICES / STOREFRONTS</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '150px' }}>
                        {lookups.services.map(s => (
                          <div key={s.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`service-${s.id}`}
                              checked={form.serviceIds.includes(s.id)}
                              onChange={() => handleListToggle('serviceIds', s.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`service-${s.id}`}>{s.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              )}

              {/* Tab 3: Purchase & Extras */}
              {activeTab === 'purchase' && (
                <div>
                  <h4 className="display-font text-white mb-4 border-bottom border-secondary border-opacity-10 pb-2">Purchase Facts & Extra Data</h4>
                  <div className="row g-3">
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">PURCHASE DATE</label>
                      <input type="date" name="purchaseDate" className="form-control form-glass-control text-white" value={form.purchaseDate} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">PURCHASE PRICE</label>
                      <input type="number" step="0.01" name="purchasePrice" className="form-control form-glass-control text-white" value={form.purchasePrice} onChange={handleTextChange} min="0" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">CURRENCY</label>
                      <input type="text" name="currency" className="form-control form-glass-control text-white" value={form.currency} onChange={handleTextChange} placeholder="e.g. USD, EUR" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">STORE PURCHASED FROM</label>
                      <input type="text" name="storePurchasedFrom" className="form-control form-glass-control text-white" value={form.storePurchasedFrom} onChange={handleTextChange} placeholder="e.g. Steam, Amazon, Retail" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">PURCHASE REGION</label>
                      <input type="text" name="purchaseRegion" className="form-control form-glass-control text-white" value={form.purchaseRegion} onChange={handleTextChange} placeholder="e.g. US, EU, Global" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">RECEIPT REFERENCE</label>
                      <input type="text" name="receiptReference" className="form-control form-glass-control text-white" value={form.receiptReference} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-6">
                      <div className="form-check form-switch mt-md-4 pt-md-2">
                        <input className="form-check-input" type="checkbox" name="gifted" id="gifted" checked={form.gifted} onChange={handleCheckboxChange} />
                        <label className="form-check-label text-white small" htmlFor="gifted">This game was a gift</label>
                      </div>
                    </div>

                    <hr className="my-4 border-secondary border-opacity-25" />
                    
                    <h5 className="display-font text-white mb-3">Additional Stats & Feature Indicators</h5>
                    
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">ESRB RATING</label>
                      <input type="text" name="esrbRating" className="form-control form-glass-control text-white" value={form.esrbRating} onChange={handleTextChange} placeholder="e.g. E, T, M" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">PEGI RATING</label>
                      <input type="text" name="pegiRating" className="form-control form-glass-control text-white" value={form.pegiRating} onChange={handleTextChange} placeholder="e.g. 3, 12, 18" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">STEAM DECK COMPATIBILITY</label>
                      <select name="steamDeckCompatibility" className="form-select form-glass-control text-white" value={form.steamDeckCompatibility} onChange={handleTextChange}>
                        <option value="Unknown">Unknown</option>
                        <option value="Verified">Verified</option>
                        <option value="Playable">Playable</option>
                        <option value="Unsupported">Unsupported</option>
                      </select>
                    </div>

                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">METACRITIC SCORE (0-100)</label>
                      <input type="number" name="metacriticScore" className="form-control form-glass-control text-white" value={form.metacriticScore} onChange={handleTextChange} min="0" max="100" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">OPENCRITIC SCORE (0-100)</label>
                      <input type="number" name="openCriticScore" className="form-control form-glass-control text-white" value={form.openCriticScore} onChange={handleTextChange} min="0" max="100" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">ACHIEVEMENT COUNT</label>
                      <input type="number" name="achievementCount" className="form-control form-glass-control text-white" value={form.achievementCount} onChange={handleTextChange} min="0" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">DLC COUNT</label>
                      <input type="number" name="dlcCount" className="form-control form-glass-control text-white" value={form.dlcCount} onChange={handleTextChange} min="0" />
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">EXPANSION COUNT</label>
                      <input type="number" name="expansionCount" className="form-control form-glass-control text-white" value={form.expansionCount} onChange={handleTextChange} min="0" />
                    </div>

                    <div className="col-12">
                      <label className="form-label text-muted small fw-bold d-block mb-3">FEATURE CHECKBOXES</label>
                      <div className="row g-2">
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="multiplayerSupport" id="multiplayerSupport" checked={form.multiplayerSupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="multiplayerSupport">Multiplayer Support</label>
                          </div>
                        </div>
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="coopSupport" id="coopSupport" checked={form.coopSupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="coopSupport">Coop Support</label>
                          </div>
                        </div>
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="vrSupport" id="vrSupport" checked={form.vrSupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="vrSupport">VR Support</label>
                          </div>
                        </div>
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="crossplaySupport" id="crossplaySupport" checked={form.crossplaySupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="crossplaySupport">Crossplay Support</label>
                          </div>
                        </div>
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="cloudSaveSupport" id="cloudSaveSupport" checked={form.cloudSaveSupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="cloudSaveSupport">Cloud Save Support</label>
                          </div>
                        </div>
                        <div className="col-6 col-md-4">
                          <div className="form-check">
                            <input className="form-check-input" type="checkbox" name="controllerSupport" id="controllerSupport" checked={form.controllerSupport} onChange={handleCheckboxChange} />
                            <label className="form-check-label filter-check-label" htmlFor="controllerSupport">Controller Support</label>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              )}

              {/* Tab 4: Media & Lookups */}
              {activeTab === 'media' && (
                <div>
                  <h4 className="display-font text-white mb-4 border-bottom border-secondary border-opacity-10 pb-2">Media & Config Lookups</h4>
                  <div className="row g-3 mb-4">
                    {/* Media image inputs with uploads */}
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">COVER IMAGE URL</label>
                      <input type="text" name="coverImage" className="form-control form-glass-control mb-2" value={form.coverImage} onChange={handleTextChange} placeholder="/uploads/covers/..." />
                      <input type="file" className="form-control form-glass-control small" accept="image/*" onChange={(e) => handleImageUpload(e, 'coverImage')} />
                      {form.coverImage && (
                        <div className="mt-2 rounded overflow-hidden" style={{ maxWidth: '100px', height: '135px' }}>
                          <img src={`${API_HOST}${form.coverImage}`} alt="Cover Preview" className="w-100 h-100 object-fit-cover" onError={(e) => {e.target.src = form.coverImage}} />
                        </div>
                      )}
                    </div>
                    <div className="col-12 col-md-6">
                      <label className="form-label text-muted small fw-bold">BANNER IMAGE URL</label>
                      <input type="text" name="banner" className="form-control form-glass-control mb-2" value={form.banner} onChange={handleTextChange} placeholder="/uploads/banners/..." />
                      <input type="file" className="form-control form-glass-control small" accept="image/*" onChange={(e) => handleImageUpload(e, 'banner')} />
                      {form.banner && (
                        <div className="mt-2 rounded overflow-hidden" style={{ maxWidth: '240px', height: '90px' }}>
                          <img src={`${API_HOST}${form.banner}`} alt="Banner Preview" className="w-100 h-100 object-fit-cover" onError={(e) => {e.target.src = form.banner}} />
                        </div>
                      )}
                    </div>

                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">BOX ART URL</label>
                      <input type="text" name="boxArt" className="form-control form-glass-control text-white" value={form.boxArt} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">LOGO URL</label>
                      <input type="text" name="logo" className="form-control form-glass-control text-white" value={form.logo} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">SCREENSHOTS URL (COMMA SEPARATED)</label>
                      <input type="text" name="screenshots" className="form-control form-glass-control text-white" value={form.screenshots} onChange={handleTextChange} placeholder="url1, url2, url3" />
                    </div>

                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">TRAILER VIDEO URL</label>
                      <input type="url" name="trailerUrl" className="form-control form-glass-control text-white" value={form.trailerUrl} onChange={handleTextChange} placeholder="e.g. YouTube Video link" />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">GAMEPLAY VIDEO URL</label>
                      <input type="url" name="gameplayUrl" className="form-control form-glass-control text-white" value={form.gameplayUrl} onChange={handleTextChange} />
                    </div>
                    <div className="col-12 col-md-4">
                      <label className="form-label text-muted small fw-bold">YOUTUBE LINKS (COMMA SEPARATED)</label>
                      <input type="text" name="youtubeLinks" className="form-control form-glass-control text-white" value={form.youtubeLinks} onChange={handleTextChange} />
                    </div>
                  </div>

                  <hr className="my-4 border-secondary border-opacity-25" />

                  {/* Developers, Publishers, Genres, Tags, Themes checklists */}
                  <div className="row g-3">
                    <div className="col-12 col-md-6 col-xxl-4">
                      <label className="form-label text-muted small fw-bold">DEVELOPERS</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '130px' }}>
                        {lookups.developers.map(d => (
                          <div key={d.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`dev-${d.id}`}
                              checked={form.developerIds.includes(d.id)}
                              onChange={() => handleListToggle('developerIds', d.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`dev-${d.id}`}>{d.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="col-12 col-md-6 col-xxl-4">
                      <label className="form-label text-muted small fw-bold">PUBLISHERS</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '130px' }}>
                        {lookups.publishers.map(p => (
                          <div key={p.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`pub-${p.id}`}
                              checked={form.publisherIds.includes(p.id)}
                              onChange={() => handleListToggle('publisherIds', p.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`pub-${p.id}`}>{p.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="col-12 col-md-6 col-xxl-4">
                      <label className="form-label text-muted small fw-bold">GENRES</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '130px' }}>
                        {lookups.genres.map(g => (
                          <div key={g.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`genre-${g.id}`}
                              checked={form.genreIds.includes(g.id)}
                              onChange={() => handleListToggle('genreIds', g.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`genre-${g.id}`}>{g.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="col-12 col-md-6 col-xxl-4">
                      <label className="form-label text-muted small fw-bold">THEMES</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '130px' }}>
                        {lookups.themes.map(t => (
                          <div key={t.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`theme-${t.id}`}
                              checked={form.themeIds.includes(t.id)}
                              onChange={() => handleListToggle('themeIds', t.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`theme-${t.id}`}>{t.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div className="col-12 col-md-6 col-xxl-4">
                      <label className="form-label text-muted small fw-bold">TAGS</label>
                      <div className="filter-section-box overflow-y-auto" style={{ maxHeight: '130px' }}>
                        {lookups.tags.map(t => (
                          <div key={t.id} className="form-check mb-1">
                            <input
                              className="form-check-input"
                              type="checkbox"
                              id={`tag-${t.id}`}
                              checked={form.tagIds.includes(t.id)}
                              onChange={() => handleListToggle('tagIds', t.id)}
                            />
                            <label className="form-check-label filter-check-label" htmlFor={`tag-${t.id}`}>{t.name}</label>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              )}

              <hr className="my-4 border-secondary border-opacity-25" />

              {/* Submit Buttons */}
              <div className="d-flex justify-content-end gap-2">
                <Link to="/library" className="btn btn-premium-outline">
                  Cancel
                </Link>
                <button type="submit" className="btn btn-premium-purple px-5" disabled={saving}>
                  {saving ? <span className="spinner-border spinner-border-sm me-2"></span> : <i className="bi bi-check-circle me-1"></i>}
                  Save Game
                </button>
              </div>

            </div>
          </div>
        </div>
      </form>
    </div>
  );
};

export default GameForm;
