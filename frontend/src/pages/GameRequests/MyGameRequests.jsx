import React, { useState, useEffect } from 'react';
import api from '../../services/api';

const MyGameRequests = () => {
  const [requests, setRequests] = useState([]);
  const [platformsList, setPlatformsList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  // Modal State
  const [showModal, setShowModal] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [modalError, setModalError] = useState(null);

  // Form State
  const [gameTitle, setGameTitle] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
  const [selectedPlatforms, setSelectedPlatforms] = useState([]);
  const [customPlatforms, setCustomPlatforms] = useState('');
  const [links, setLinks] = useState(['']);
  const [additionalInfo, setAdditionalInfo] = useState('');

  const fetchMyRequests = async () => {
    setLoading(true);
    setError(null);
    try {
      const [reqRes, platRes] = await Promise.all([
        api.get('/gamerequests/my'),
        api.get('/metadata/platforms/list')
      ]);
      setRequests(reqRes.data || []);
      setPlatformsList(platRes.data || []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load your game requests.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyRequests();
  }, []);

  const openRequestModal = () => {
    setGameTitle('');
    setReleaseYear('');
    setSelectedPlatforms([]);
    setCustomPlatforms('');
    setLinks(['']);
    setAdditionalInfo('');
    setModalError(null);
    setShowModal(true);
  };

  const handleAddLinkField = () => {
    setLinks([...links, '']);
  };

  const handleLinkChange = (index, value) => {
    const updated = [...links];
    updated[index] = value;
    setLinks(updated);
  };

  const handleRemoveLinkField = (index) => {
    if (links.length === 1) {
      setLinks(['']);
      return;
    }
    setLinks(links.filter((_, i) => i !== index));
  };

  const handleTogglePlatform = (platformName) => {
    if (selectedPlatforms.includes(platformName)) {
      setSelectedPlatforms(selectedPlatforms.filter(p => p !== platformName));
    } else {
      setSelectedPlatforms([...selectedPlatforms, platformName]);
    }
  };

  const handleSubmitRequest = async (e) => {
    e.preventDefault();
    if (!gameTitle.trim()) {
      setModalError('Game title is required.');
      return;
    }

    // Combine selected platforms + custom platform input
    const allPlatforms = [...selectedPlatforms];
    if (customPlatforms.trim()) {
      allPlatforms.push(customPlatforms.trim());
    }

    const validLinks = links.filter(l => l.trim().length > 0);

    const payload = {
      gameTitle: gameTitle.trim(),
      approximateReleaseYear: releaseYear.trim() || null,
      platforms: allPlatforms.join(', ') || null,
      links: validLinks,
      additionalInformation: additionalInfo.trim() || null
    };

    setSubmitting(true);
    setModalError(null);

    try {
      await api.post('/gamerequests', payload);
      setSuccessMsg(`Request for '${payload.gameTitle}' submitted successfully! Our curators will review it.`);
      setShowModal(false);
      fetchMyRequests();
      setTimeout(() => setSuccessMsg(null), 5000);
    } catch (err) {
      setModalError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to submit request.');
    } finally {
      setSubmitting(false);
    }
  };

  const getStatusBadge = (status) => {
    switch (status) {
      case 1: // Approved
        return <span className="badge bg-success-subtle text-success border border-success-subtle"><i className="bi bi-check-circle me-1"></i>Approved</span>;
      case 2: // Rejected
        return <span className="badge bg-danger-subtle text-danger border border-danger-subtle"><i className="bi bi-x-circle me-1"></i>Rejected</span>;
      case 3: // Cancelled
        return <span className="badge bg-secondary"><i className="bi bi-slash-circle me-1"></i>Cancelled</span>;
      default: // Pending
        return <span className="badge bg-warning-subtle text-warning border border-warning-subtle"><i className="bi bi-hourglass-split me-1"></i>Pending Review</span>;
    }
  };

  return (
    <div className="container-fluid p-0">
      {/* Header */}
      <div className="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
        <div>
          <h1 className="h3 mb-1 fw-bold text-gradient display-font">
            <i className="bi bi-send-fill me-2 text-primary"></i>
            My Game Requests
          </h1>
          <p className="text-secondary mb-0">
            Can't find a game in the central catalog? Submit a request with approximate details and references.
          </p>
        </div>
        <button className="btn btn-primary d-flex align-items-center gap-2" onClick={openRequestModal}>
          <i className="bi bi-plus-circle-fill"></i>
          <span>Request New Game</span>
        </button>
      </div>

      {/* Notifications */}
      {successMsg && (
        <div className="alert alert-success d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-check-circle-fill fs-5"></i>
          <div>{successMsg}</div>
        </div>
      )}
      {error && (
        <div className="alert alert-danger d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-exclamation-triangle-fill fs-5"></i>
          <div>{error}</div>
        </div>
      )}

      {/* Requests History List */}
      <div className="card border-0 shadow-sm custom-card">
        <div className="card-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-primary" role="status"></div>
              <p className="text-secondary mt-2">Loading your requests...</p>
            </div>
          ) : requests.length === 0 ? (
            <div className="text-center py-5 text-secondary">
              <i className="bi bi-inbox fs-1 d-block mb-2"></i>
              You haven't submitted any game requests yet.
              <div className="mt-3">
                <button className="btn btn-sm btn-outline-primary" onClick={openRequestModal}>
                  <i className="bi bi-plus-circle me-1"></i> Submit First Request
                </button>
              </div>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0 custom-table">
                <thead>
                  <tr>
                    <th style={{ width: '220px' }}>Game Title</th>
                    <th style={{ width: '100px' }}>Approx. Year</th>
                    <th>Platforms</th>
                    <th>Reference Links</th>
                    <th style={{ width: '140px' }} className="text-center">Status</th>
                    <th>Feedback / Notes</th>
                  </tr>
                </thead>
                <tbody>
                  {requests.map((req) => (
                    <tr key={req.id}>
                      <td>
                        <div className="fw-bold">{req.gameTitle}</div>
                        <div className="text-secondary" style={{ fontSize: '0.72rem' }}>
                          Submitted: {new Date(req.createdDate).toLocaleDateString()}
                        </div>
                      </td>
                      <td>
                        <span className="badge bg-dark-subtle text-body border">
                          {req.approximateReleaseYear || 'N/A'}
                        </span>
                      </td>
                      <td>
                        <span className="small text-secondary">{req.platforms || 'Not specified'}</span>
                      </td>
                      <td>
                        {req.linkList && req.linkList.length > 0 ? (
                          <div className="d-flex flex-column gap-1">
                            {req.linkList.map((link, idx) => (
                              <a
                                key={idx}
                                href={link.startsWith('http') ? link : `https://${link}`}
                                target="_blank"
                                rel="noreferrer"
                                className="small text-decoration-none text-truncate d-inline-block"
                                style={{ maxWidth: '200px' }}
                              >
                                <i className="bi bi-link-45deg me-1"></i>
                                {link}
                              </a>
                            ))}
                          </div>
                        ) : (
                          <span className="text-secondary small fst-italic">None</span>
                        )}
                      </td>
                      <td className="text-center">
                        {getStatusBadge(req.status)}
                      </td>
                      <td>
                        {req.reviewNotes ? (
                          <span className="small text-body">{req.reviewNotes}</span>
                        ) : (
                          <span className="text-secondary small fst-italic opacity-50">Pending review</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Submit Request Modal */}
      {showModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className="bi bi-plus-circle me-2 text-primary"></i>
                  Request a New Game
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>

              <form onSubmit={handleSubmitRequest}>
                <div className="modal-body p-4">
                  {modalError && (
                    <div className="alert alert-danger py-2 mb-3">
                      <i className="bi bi-exclamation-circle me-1"></i> {modalError}
                    </div>
                  )}

                  <div className="row g-3">
                    <div className="col-md-8">
                      <label className="form-label fw-bold small">Game Title *</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="e.g. Hades II, Grand Theft Auto VI"
                        value={gameTitle}
                        onChange={(e) => setGameTitle(e.target.value)}
                        required
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-bold small">Approx. Release Year / Date</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="e.g. 2024, Q4 2025"
                        value={releaseYear}
                        onChange={(e) => setReleaseYear(e.target.value)}
                      />
                    </div>

                    {/* Platform Selection */}
                    <div className="col-12">
                      <label className="form-label fw-bold small mb-1">Platforms</label>
                      <div className="d-flex flex-wrap gap-2 mb-2">
                        {platformsList.map((plat) => (
                          <button
                            key={plat.id}
                            type="button"
                            className={`btn btn-sm ${selectedPlatforms.includes(plat.name) ? 'btn-primary' : 'btn-outline-secondary'}`}
                            onClick={() => handleTogglePlatform(plat.name)}
                          >
                            {selectedPlatforms.includes(plat.name) && <i className="bi bi-check me-1"></i>}
                            {plat.name}
                          </button>
                        ))}
                      </div>
                      <input
                        type="text"
                        className="form-control form-control-sm"
                        placeholder="Other platforms (e.g. RetroArch, macOS)..."
                        value={customPlatforms}
                        onChange={(e) => setCustomPlatforms(e.target.value)}
                      />
                    </div>

                    {/* Reference Links */}
                    <div className="col-12">
                      <div className="d-flex align-items-center justify-content-between mb-1">
                        <label className="form-label fw-bold small mb-0">Reference Links</label>
                        <button
                          type="button"
                          className="btn btn-sm btn-link p-0 text-decoration-none"
                          onClick={handleAddLinkField}
                        >
                          <i className="bi bi-plus-lg me-1"></i> Add Another Link
                        </button>
                      </div>
                      <p className="text-secondary small mb-2" style={{ fontSize: '0.75rem' }}>
                        Provide links to Steam, IGDB, official website, or news announcements.
                      </p>

                      <div className="d-flex flex-column gap-2">
                        {links.map((link, idx) => (
                          <div key={idx} className="input-group input-group-sm">
                            <span className="input-group-text"><i className="bi bi-link-45deg"></i></span>
                            <input
                              type="url"
                              className="form-control"
                              placeholder="https://store.steampowered.com/app/..."
                              value={link}
                              onChange={(e) => handleLinkChange(idx, e.target.value)}
                            />
                            {links.length > 1 && (
                              <button
                                type="button"
                                className="btn btn-outline-danger"
                                onClick={() => handleRemoveLinkField(idx)}
                              >
                                <i className="bi bi-x-lg"></i>
                              </button>
                            )}
                          </div>
                        ))}
                      </div>
                    </div>

                    {/* Additional Notes */}
                    <div className="col-12">
                      <label className="form-label fw-bold small">Additional Information / Description</label>
                      <textarea
                        className="form-control"
                        rows="3"
                        placeholder="Any extra details, developer/publisher names, or edition notes..."
                        value={additionalInfo}
                        onChange={(e) => setAdditionalInfo(e.target.value)}
                      ></textarea>
                    </div>
                  </div>
                </div>

                <div className="modal-footer border-top border-secondary-subtle">
                  <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)} disabled={submitting}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary d-flex align-items-center gap-2" disabled={submitting}>
                    {submitting && <span className="spinner-border spinner-border-sm" role="status"></span>}
                    <span>Submit Game Request</span>
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

export default MyGameRequests;
