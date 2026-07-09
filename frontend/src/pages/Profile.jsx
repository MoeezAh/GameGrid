import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const Profile = () => {
  const { user, logout } = useAuth();
  
  const [email, setEmail] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (user) {
      setEmail(user.email);
    }
  }, [user]);

  const handleUpdate = async (e) => {
    e.preventDefault();
    setMessage('');
    setError('');

    if (newPassword && newPassword !== confirmPassword) {
      setError('New passwords do not match.');
      return;
    }

    setLoading(true);
    try {
      await api.put('/auth/profile', {
        email,
        currentPassword: currentPassword || null,
        newPassword: newPassword || null
      });
      setMessage('Profile updated successfully.');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (err) {
      setError(err.message || 'Failed to update profile.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container-fluid py-4 fade-in">
      <div className="row justify-content-center">
        <div className="col-12 col-md-8 col-lg-6">
          <div className="glass-panel p-4 p-md-5">
            <h3 className="display-font mb-4">Account Settings</h3>
            
            {message && (
              <div className="alert-theme-success d-flex align-items-center gap-2 mb-4" role="alert">
                <i className="bi bi-check-circle-fill flex-shrink-0"></i>
                <span>{message}</span>
              </div>
            )}

            {error && (
              <div className="alert-theme-danger d-flex align-items-center gap-2 mb-4" role="alert">
                <i className="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
                <span>{error}</span>
              </div>
            )}

            <form onSubmit={handleUpdate}>
              <div className="mb-3">
                <label className="form-label small fw-bold uppercase">Username</label>
                <input 
                  type="text" 
                  className="form-control form-glass-control"
                  value={user?.username || ''} 
                  disabled 
                  readOnly 
                />
              </div>

              <div className="mb-3">
                <label className="form-label small fw-bold uppercase">Email Address</label>
                <input 
                  type="email" 
                  className="form-control form-glass-control"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>

              <div className="mb-4">
                <label className="form-label small fw-bold uppercase">User Role(s)</label>
                <div className="d-flex gap-2 flex-wrap">
                  {user?.roles?.map((role, index) => (
                    <span key={index} className="meta-tag">
                      <i className="bi bi-shield-check me-1" style={{ color: 'var(--accent-purple)' }}></i>
                      {role}
                    </span>
                  ))}
                </div>
              </div>

              <hr className="my-4" style={{ borderColor: 'var(--border-color)' }} />
              
              <h5 className="display-font mb-3">Change Password</h5>
              
              <div className="mb-3">
                <label className="form-label small fw-bold uppercase">Current Password</label>
                <input 
                  type="password" 
                  className="form-control form-glass-control"
                  placeholder="Enter current password"
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                />
              </div>

              <div className="mb-3">
                <label className="form-label small fw-bold uppercase">New Password</label>
                <input 
                  type="password" 
                  className="form-control form-glass-control"
                  placeholder="Enter new password"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                />
              </div>

              <div className="mb-4">
                <label className="form-label small fw-bold uppercase">Confirm New Password</label>
                <input 
                  type="password" 
                  className="form-control form-glass-control"
                  placeholder="Confirm new password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>

              <button 
                type="submit" 
                className="btn btn-premium-purple w-100 py-2 fw-bold d-flex align-items-center justify-content-center gap-2"
                disabled={loading}
              >
                {loading ? (
                  <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                ) : (
                  <>
                    <i className="bi bi-save-fill"></i>
                    <span>Save Changes</span>
                  </>
                )}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Profile;
