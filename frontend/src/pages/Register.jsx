import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Register = () => {
  const { register } = useAuth();
  const navigate = useNavigate();
  
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!username || !email || !password || !confirmPassword) {
      setError('Please fill in all fields.');
      return;
    }
    
    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    
    setError('');
    setSubmitting(true);
    
    const result = await register(username, email, password);
    setSubmitting(false);
    
    if (result.success) {
      navigate('/');
    } else {
      setError(result.message);
    }
  };

  return (
    <div className="auth-page-bg d-flex align-items-center justify-content-center p-3">
      <div className="glass-panel p-4 p-sm-5 w-100" style={{ maxWidth: '480px' }}>
        <div className="text-center mb-4">
          <i 
            className="bi bi-controller fs-1 mb-2 d-inline-block" 
            style={{ background: 'linear-gradient(135deg, var(--accent-purple), var(--accent-teal))', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}
          ></i>
          <h2 className="display-font fw-bold" style={{ color: 'var(--text-primary)' }}>Create Account</h2>
          <p className="text-muted small">Join the game archive community</p>
        </div>

        {error && (
          <div className="alert-theme-danger d-flex align-items-center gap-2 mb-3" role="alert">
            <i className="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label small fw-bold uppercase">Username</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-person-fill"></i></span>
              <input 
                type="text" 
                className="form-control form-glass-control"
                placeholder="Choose a username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="mb-3">
            <label className="form-label small fw-bold uppercase">Email Address</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-envelope-fill"></i></span>
              <input 
                type="email" 
                className="form-control form-glass-control"
                placeholder="Enter email address"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="mb-3">
            <label className="form-label small fw-bold uppercase">Password</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-shield-lock-fill"></i></span>
              <input 
                type="password" 
                className="form-control form-glass-control"
                placeholder="Create password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="mb-4">
            <label className="form-label small fw-bold uppercase">Confirm Password</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-shield-lock-check-fill"></i></span>
              <input 
                type="password" 
                className="form-control form-glass-control"
                placeholder="Repeat password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <button 
            type="submit" 
            className="btn btn-premium-purple w-100 py-2 fw-bold d-flex align-items-center justify-content-center gap-2"
            disabled={submitting}
          >
            {submitting ? (
              <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            ) : (
              <>
                <span>Sign Up</span>
                <i className="bi bi-arrow-right-short fs-5"></i>
              </>
            )}
          </button>
        </form>

        <div className="text-center mt-4">
          <p className="text-muted small mb-0">
            Already have an account?{' '}
            <Link to="/login" className="text-theme-accent fw-bold text-decoration-none">Sign In</Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
