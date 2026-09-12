import React, { useState, useEffect } from 'react';
import api from '../../services/api';

const AdminGameRequests = () => {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState(null); // null (All) | 0 (Pending) | 1 (Approved) | 2 (Rejected)
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  // Review Modal State
  const [selectedRequest, setSelectedRequest] = useState(null);
  const [duplicateResult, setDuplicateResult] = useState(null);
  const [checkingDuplicates, setCheckingDuplicates] = useState(false);
  const [reviewNotes, setReviewNotes] = useState('');
  const [linkToExistingGameId, setLinkToExistingGameId] = useState(null);
  const [actionInProgress, setActionInProgress] = useState(false);
  const [actionError, setActionError] = useState(null);

  const fetchRequests = async () => {
    setLoading(true);
    setError(null);
    try {
      const url = statusFilter !== null ? `/gamerequests?status=${statusFilter}` : '/gamerequests';
      const res = await api.get(url);
      setRequests(res.data || []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load game requests.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
  }, [statusFilter]);

  const openReviewModal = async (req) => {
    setSelectedRequest(req);
    setReviewNotes('');
    setLinkToExistingGameId(null);
    setActionError(null);
    setDuplicateResult(null);

    // Run instant duplicate check
    setCheckingDuplicates(true);
    try {
      const dupRes = await api.get(`/gamerequests/check-duplicate?title=${encodeURIComponent(req.gameTitle)}`);
      setDuplicateResult(dupRes.data);
      if (dupRes.data?.candidates?.length > 0) {
        // If there's an exact or close match, default link option to first candidate
        setLinkToExistingGameId(dupRes.data.candidates[0].id);
      }
    } catch (err) {
      console.error('Duplicate check failed', err);
    } finally {
      setCheckingDuplicates(false);
    }
  };

  const handleApprove = async () => {
    if (!selectedRequest) return;
    setActionInProgress(true);
    setActionError(null);

    const payload = {
      reviewNotes: reviewNotes.trim() || null,
      existingGameId: linkToExistingGameId ? parseInt(linkToExistingGameId, 10) : null
    };

    try {
      await api.post(`/gamerequests/${selectedRequest.id}/approve`, payload);
      setSuccessMsg(`Request for '${selectedRequest.gameTitle}' approved successfully!`);
      setSelectedRequest(null);
      fetchRequests();
      setTimeout(() => setSuccessMsg(null), 4000);
    } catch (err) {
      setActionError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to approve request.');
    } finally {
      setActionInProgress(false);
    }
  };

  const handleReject = async () => {
    if (!selectedRequest) return;
    if (!reviewNotes.trim()) {
      setActionError('Please provide a reason or notes for rejecting this request.');
      return;
    }

    setActionInProgress(true);
    setActionError(null);

    const payload = {
      rejectionReason: reviewNotes.trim()
    };

    try {
      await api.post(`/gamerequests/${selectedRequest.id}/reject`, payload);
      setSuccessMsg(`Request for '${selectedRequest.gameTitle}' was rejected.`);
      setSelectedRequest(null);
      fetchRequests();
      setTimeout(() => setSuccessMsg(null), 4000);
    } catch (err) {
      setActionError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to reject request.');
    } finally {
      setActionInProgress(false);
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
            <i className="bi bi-patch-check-fill me-2 text-primary"></i>
            Game Requests Review Queue
          </h1>
          <p className="text-secondary mb-0">
            Review user-submitted requests, verify against duplicates, and approve games into the central catalog.
          </p>
        </div>
      </div>

      {/* Filter Tabs */}
      <div className="d-flex gap-2 mb-3">
        <button
          className={`btn btn-sm ${statusFilter === null ? 'btn-primary' : 'btn-outline-secondary'}`}
          onClick={() => setStatusFilter(null)}
        >
          All Requests
        </button>
        <button
          className={`btn btn-sm ${statusFilter === 0 ? 'btn-warning text-dark fw-bold' : 'btn-outline-secondary'}`}
          onClick={() => setStatusFilter(0)}
        >
          <i className="bi bi-hourglass-split me-1"></i>
          Pending Review
        </button>
        <button
          className={`btn btn-sm ${statusFilter === 1 ? 'btn-success' : 'btn-outline-secondary'}`}
          onClick={() => setStatusFilter(1)}
        >
          <i className="bi bi-check-circle me-1"></i>
          Approved
        </button>
        <button
          className={`btn btn-sm ${statusFilter === 2 ? 'btn-danger' : 'btn-outline-secondary'}`}
          onClick={() => setStatusFilter(2)}
        >
          <i className="bi bi-x-circle me-1"></i>
          Rejected
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

      {/* Requests Table */}
      <div className="card border-0 shadow-sm custom-card">
        <div className="card-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-primary" role="status"></div>
              <p className="text-secondary mt-2">Loading game requests...</p>
            </div>
          ) : requests.length === 0 ? (
            <div className="text-center py-5 text-secondary">
              <i className="bi bi-check-all fs-1 d-block mb-2 text-success"></i>
              No game requests found in this view.
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
                    <th style={{ width: '140px' }} className="text-end">Action</th>
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
                                style={{ maxWidth: '220px' }}
                              >
                                <i className="bi bi-box-arrow-up-right me-1"></i>
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
                      <td className="text-end">
                        <button
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => openReviewModal(req)}
                        >
                          <i className="bi bi-clipboard-check me-1"></i>
                          Review
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Review Modal */}
      {selectedRequest && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className="bi bi-clipboard-data me-2 text-primary"></i>
                  Review Game Request: <span className="text-primary">{selectedRequest.gameTitle}</span>
                </h5>
                <button type="button" className="btn-close" onClick={() => setSelectedRequest(null)}></button>
              </div>

              <div className="modal-body p-4">
                {actionError && (
                  <div className="alert alert-danger py-2 mb-3">
                    <i className="bi bi-exclamation-circle me-1"></i> {actionError}
                  </div>
                )}

                {/* Submitted Details Card */}
                <div className="card bg-body-tertiary border p-3 mb-4">
                  <div className="row g-2 small">
                    <div className="col-md-6">
                      <span className="text-secondary d-block">Requested Title:</span>
                      <strong className="fs-6">{selectedRequest.gameTitle}</strong>
                    </div>
                    <div className="col-md-6">
                      <span className="text-secondary d-block">Approx. Release Year:</span>
                      <strong>{selectedRequest.approximateReleaseYear || 'N/A'}</strong>
                    </div>
                    <div className="col-md-6">
                      <span className="text-secondary d-block">Requested Platforms:</span>
                      <strong>{selectedRequest.platforms || 'None specified'}</strong>
                    </div>
                    <div className="col-md-6">
                      <span className="text-secondary d-block">Current Status:</span>
                      <div>{getStatusBadge(selectedRequest.status)}</div>
                    </div>
                    {selectedRequest.additionalInformation && (
                      <div className="col-12 mt-2">
                        <span className="text-secondary d-block">User Description / Notes:</span>
                        <div className="p-2 rounded bg-body border mt-1">{selectedRequest.additionalInformation}</div>
                      </div>
                    )}
                    {selectedRequest.linkList && selectedRequest.linkList.length > 0 && (
                      <div className="col-12 mt-2">
                        <span className="text-secondary d-block">Reference Links:</span>
                        <div className="d-flex flex-wrap gap-2 mt-1">
                          {selectedRequest.linkList.map((l, i) => (
                            <a
                              key={i}
                              href={l.startsWith('http') ? l : `https://${l}`}
                              target="_blank"
                              rel="noreferrer"
                              className="badge bg-primary-subtle text-primary border text-decoration-none p-2"
                            >
                              <i className="bi bi-box-arrow-up-right me-1"></i> {l}
                            </a>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                </div>

                {/* Duplicate Detection Alert */}
                {checkingDuplicates ? (
                  <div className="text-center py-2 text-secondary">
                    <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                    Checking central catalog for duplicates...
                  </div>
                ) : duplicateResult?.hasPotentialDuplicate ? (
                  <div className="alert alert-warning mb-4">
                    <h6 className="fw-bold mb-1">
                      <i className="bi bi-exclamation-triangle-fill me-1"></i>
                      Potential Catalog Duplicates Found
                    </h6>
                    <p className="small mb-2">
                      The catalog already contains games with similar names. You can link this request to an existing game to prevent duplicate catalog entries:
                    </p>
                    <div className="d-flex flex-column gap-2 mt-2">
                      {duplicateResult.candidates.map((cand) => (
                        <div key={cand.id} className="d-flex align-items-center justify-content-between p-2 rounded bg-body border">
                          <div>
                            <strong>{cand.title}</strong>
                            {cand.releaseDate && (
                              <span className="text-secondary small ms-2">
                                ({new Date(cand.releaseDate).getFullYear()})
                              </span>
                            )}
                          </div>
                          <div className="form-check mb-0">
                            <input
                              className="form-check-input"
                              type="radio"
                              name="linkExisting"
                              id={`dup_${cand.id}`}
                              checked={linkToExistingGameId === cand.id}
                              onChange={() => setLinkToExistingGameId(cand.id)}
                            />
                            <label className="form-check-label small" htmlFor={`dup_${cand.id}`}>
                              Link to this Game
                            </label>
                          </div>
                        </div>
                      ))}
                      <div className="form-check mt-1">
                        <input
                          className="form-check-input"
                          type="radio"
                          name="linkExisting"
                          id="create_new_override"
                          checked={linkToExistingGameId === null}
                          onChange={() => setLinkToExistingGameId(null)}
                        />
                        <label className="form-check-label small fw-semibold" htmlFor="create_new_override">
                          Create as brand new catalog game anyway
                        </label>
                      </div>
                    </div>
                  </div>
                ) : (
                  <div className="alert alert-success py-2 mb-4 small">
                    <i className="bi bi-check-circle-fill me-1"></i>
                    No conflicting duplicate games found in the central catalog.
                  </div>
                )}

                {/* Review Notes / Rejection Reason Input */}
                <div className="mb-3">
                  <label className="form-label fw-bold small">
                    Review Notes / Feedback (Sent to user)
                  </label>
                  <textarea
                    className="form-control"
                    rows="2"
                    placeholder="Enter approval notes or rejection reason..."
                    value={reviewNotes}
                    onChange={(e) => setReviewNotes(e.target.value)}
                  ></textarea>
                </div>
              </div>

              <div className="modal-footer border-top border-secondary-subtle justify-content-between">
                <button
                  type="button"
                  className="btn btn-outline-danger d-flex align-items-center gap-1"
                  onClick={handleReject}
                  disabled={actionInProgress || selectedRequest.status !== 0}
                >
                  <i className="bi bi-x-circle"></i>
                  <span>Reject Request</span>
                </button>

                <div className="d-flex gap-2">
                  <button type="button" className="btn btn-secondary" onClick={() => setSelectedRequest(null)} disabled={actionInProgress}>
                    Close
                  </button>
                  <button
                    type="button"
                    className="btn btn-success d-flex align-items-center gap-2"
                    onClick={handleApprove}
                    disabled={actionInProgress || selectedRequest.status !== 0}
                  >
                    {actionInProgress && <span className="spinner-border spinner-border-sm" role="status"></span>}
                    <i className="bi bi-check-circle-fill"></i>
                    <span>{linkToExistingGameId ? 'Approve & Link to Existing Game' : 'Approve & Create Catalog Game'}</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminGameRequests;
